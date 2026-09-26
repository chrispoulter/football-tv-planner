import { keepPreviousData, useMutation, useQuery } from '@tanstack/react-query';
import { apiClient } from '@/lib/api-client';

export const fixtureKeys = {
    all: ['fixtures'] as const,
    list: (request: GetFixturesRequest) => ['fixtures', request] as const,
    competitions: ['competitions'] as const,
    channels: ['channels'] as const,
};

export interface FixtureSummary {
    id: string;
    kickoffUtc: string;
    competition: string;
    homeTeam: string;
    awayTeam: string;
    channels: string[];
    isBookmarked?: boolean;
}

interface GetFixturesRequest {
    from: string;
    to: string;
    competition?: string;
    channel?: string;
    bookmarked?: boolean;
}

export interface GetFixturesResponse {
    items: FixtureSummary[];
}

export const useGetFixtures = (request: GetFixturesRequest) => {
    const searchParams = Object.fromEntries(
        Object.entries(request).filter(([, value]) => !!value)
    );

    return useQuery({
        queryKey: fixtureKeys.list(request),
        queryFn: ({ signal }) =>
            apiClient
                .get('fixtures', {
                    searchParams,
                    signal,
                })
                .json<GetFixturesResponse>(),
        placeholderData: keepPreviousData,
    });
};

export const useGetCompetitions = () =>
    useQuery({
        queryKey: fixtureKeys.competitions,
        queryFn: ({ signal }) =>
            apiClient.get('competitions', { signal }).json<string[]>(),
        staleTime: 1000 * 60 * 60,
    });

export const useGetChannels = () =>
    useQuery({
        queryKey: fixtureKeys.channels,
        queryFn: ({ signal }) =>
            apiClient.get('channels', { signal }).json<string[]>(),
        staleTime: 1000 * 60 * 60,
    });

export const useDownloadFixtureCalendar = () => {
    return useMutation({
        mutationFn: async (fixture: FixtureSummary) => {
            const blob = await apiClient
                .get(`fixtures/${fixture.id}/calendar.ics`, {})
                .blob();

            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `${fixture.homeTeam} v ${fixture.awayTeam}.ics`;
            link.click();
            URL.revokeObjectURL(url);
        },
    });
};
