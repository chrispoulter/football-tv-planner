import {
    useMutation,
    useQuery,
    useQueryClient,
    type QueryClient,
    type QueryFilters,
} from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';
import {
    fixtureKeys,
    type FixtureSummary,
    type GetFixturesResponse,
} from '../fixtures/fixtures-queries';

export const scheduleKeys = {
    all: ['schedule'] as const,
    calendarFeed: ['schedule', 'calendar-feed'] as const,
};

export interface GetScheduleResponse {
    items: FixtureSummary[];
}

export const useGetSchedule = () => {
    const { accessToken } = useAuth();

    return useQuery({
        queryKey: scheduleKeys.all,
        queryFn: ({ signal }) =>
            apiClient
                .get('schedule', {
                    context: {
                        accessToken,
                    },
                    signal,
                })
                .json<GetScheduleResponse>(),
    });
};

interface ScheduleItemResponse {
    fixtureId: string;
}

/**
 * Flips the bookmark flag on every cached fixture list so the star responds instantly,
 * returning a function that restores the previous state.
 */
async function setBookmarked(
    queryClient: QueryClient,
    fixtureId: string,
    isBookmarked: boolean
) {
    const filters: QueryFilters[] = [
        { queryKey: fixtureKeys.all },
        { queryKey: scheduleKeys.all, exact: true },
    ];

    await Promise.all(filters.map((f) => queryClient.cancelQueries(f)));

    const previous = filters.flatMap((f) =>
        queryClient.getQueriesData<GetFixturesResponse | GetScheduleResponse>(f)
    );

    for (const f of filters) {
        queryClient.setQueriesData<GetFixturesResponse | GetScheduleResponse>(
            f,
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
    }

    return () => {
        for (const [queryKey, data] of previous) {
            queryClient.setQueryData(queryKey, data);
        }
    };
}

export const useToggleSchedule = () => {
    const { accessToken } = useAuth();

    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: ({
            fixtureId,
            isBookmarked,
        }: {
            fixtureId: string;
            isBookmarked: boolean;
        }) => {
            const options = { context: { accessToken } };

            const request = isBookmarked
                ? apiClient.put(`schedule/${fixtureId}`, options)
                : apiClient.delete(`schedule/${fixtureId}`, options);

            return request.json<ScheduleItemResponse>();
        },
        onMutate: ({ fixtureId, isBookmarked }) =>
            setBookmarked(queryClient, fixtureId, isBookmarked),
        onError: (_error, _variables, rollback) => rollback?.(),
        onSettled: () => {
            queryClient.invalidateQueries({ queryKey: fixtureKeys.all });
            queryClient.invalidateQueries({
                queryKey: scheduleKeys.all,
                exact: true,
            });
        },
    });
};

export interface CalendarFeedResponse {
    httpsUrl: string;
    webcalUrl: string;
}

export const useGetCalendarFeed = () => {
    const { accessToken } = useAuth();

    return useQuery({
        queryKey: scheduleKeys.calendarFeed,
        queryFn: ({ signal }) =>
            apiClient
                .get('schedule/calendar-feed', {
                    context: {
                        accessToken,
                    },
                    signal,
                })
                .json<CalendarFeedResponse>(),
    });
};

export const useResetCalendarFeed = () => {
    const { accessToken } = useAuth();

    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('schedule/calendar-feed/reset', {
                    context: {
                        accessToken,
                    },
                })
                .json<CalendarFeedResponse>(),
        onSuccess: (data) =>
            queryClient.setQueryData(scheduleKeys.calendarFeed, data),
    });
};
