import {
    useMutation,
    useQuery,
    useQueryClient,
    type QueryClient,
} from '@tanstack/react-query';
import {
    fixtureKeys,
    type GetFixturesResponse,
} from '@/features/fixtures/fixtures-queries';
import { apiClient } from '@/lib/api-client';

export const scheduleKeys = {
    all: ['schedule'] as const,
    calendarFeed: ['schedule', 'calendar-feed'] as const,
};

interface ScheduleItemResponse {
    fixtureId: string;
}

async function setBookmarked(
    queryClient: QueryClient,
    fixtureId: string,
    isBookmarked: boolean
) {
    await queryClient.cancelQueries({ queryKey: fixtureKeys.all });

    const previous = queryClient.getQueriesData<GetFixturesResponse>({
        queryKey: fixtureKeys.all,
    });

    queryClient.setQueriesData<GetFixturesResponse>(
        { queryKey: fixtureKeys.all },
        (data) =>
            data && {
                ...data,
                items: data.items.map((fixture) =>
                    fixture.id === fixtureId
                        ? { ...fixture, isBookmarked }
                        : fixture
                ),
            }
    );

    return () => {
        for (const [queryKey, data] of previous) {
            queryClient.setQueryData(queryKey, data);
        }
    };
}

export const useToggleSchedule = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({
            fixtureId,
            isBookmarked,
        }: {
            fixtureId: string;
            isBookmarked: boolean;
        }) => {
            const request = isBookmarked
                ? apiClient.put(`schedule/${fixtureId}`)
                : apiClient.delete(`schedule/${fixtureId}`);

            return request.json<ScheduleItemResponse>();
        },
        onMutate: ({ fixtureId, isBookmarked }) =>
            setBookmarked(queryClient, fixtureId, isBookmarked),
        onError: (_error, _variables, rollback) => rollback?.(),
        onSettled: () =>
            queryClient.invalidateQueries({ queryKey: fixtureKeys.all }),
    });
};

export interface CalendarFeedResponse {
    httpsUrl: string;
    webcalUrl: string;
}

export const useGetCalendarFeed = ({ enabled }: { enabled?: boolean } = {}) =>
    useQuery({
        enabled,
        queryKey: scheduleKeys.calendarFeed,
        queryFn: ({ signal }) =>
            apiClient
                .get('schedule/calendar-feed', { signal })
                .json<CalendarFeedResponse>(),
    });

export const useResetCalendarFeed = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('schedule/calendar-feed/reset')
                .json<CalendarFeedResponse>(),
        onSuccess: (data) =>
            queryClient.setQueryData(scheduleKeys.calendarFeed, data),
    });
};
