import {
    useMutation,
    useQueryClient,
    type QueryClient,
} from '@tanstack/react-query';
import { apiClient } from '@/lib/api-client';
import {
    fixtureKeys,
    type GetFixturesResponse,
} from '../fixtures/fixtures-queries';

export const scheduleKeys = {
    all: ['schedule'] as const,
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
