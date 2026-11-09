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

interface AddToScheduleResponse {
    fixtureId: string;
}

export function useAddToSchedule() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (fixtureId: string) =>
            apiClient
                .put(`schedule/${fixtureId}`)
                .json<AddToScheduleResponse>(),
        onMutate: (fixtureId) => setBookmarked(queryClient, fixtureId, true),
        onError: (_error, _fixtureId, rollback) => rollback?.(),
        onSettled: () =>
            queryClient.invalidateQueries({ queryKey: fixtureKeys.all }),
    });
}

interface RemoveFromScheduleResponse {
    fixtureId: string;
}

export function useRemoveFromSchedule() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (fixtureId: string) =>
            apiClient
                .delete(`schedule/${fixtureId}`)
                .json<RemoveFromScheduleResponse>(),
        onMutate: (fixtureId) => setBookmarked(queryClient, fixtureId, false),
        onError: (_error, _fixtureId, rollback) => rollback?.(),
        onSettled: () =>
            queryClient.invalidateQueries({ queryKey: fixtureKeys.all }),
    });
}

export interface CalendarFeedResponse {
    httpsUrl: string;
    webcalUrl: string;
}

export function useGetCalendarFeed({ enabled }: { enabled?: boolean } = {}) {
    return useQuery({
        enabled,
        queryKey: scheduleKeys.calendarFeed,
        queryFn: ({ signal }) =>
            apiClient
                .get('schedule/calendar-feed', { signal })
                .json<CalendarFeedResponse>(),
    });
}

export function useResetCalendarFeed() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('schedule/calendar-feed/reset')
                .json<CalendarFeedResponse>(),
        onSuccess: (data) =>
            queryClient.setQueryData(scheduleKeys.calendarFeed, data),
    });
}
