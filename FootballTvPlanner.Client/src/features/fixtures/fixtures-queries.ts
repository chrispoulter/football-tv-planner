import { keepPreviousData, useQuery } from '@tanstack/react-query';
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
    competition: { id: string; name: string };
    homeTeam: { id: string; name: string };
    awayTeam: { id: string; name: string };
    channels: { id: string; name: string }[];
    isBookmarked?: boolean;
}

interface GetFixturesRequest {
    from: string;
    to: string;
    competitionId?: string;
    channelId?: string;
    bookmarked?: boolean;
}

export interface GetFixturesResponse {
    items: FixtureSummary[];
}

export interface GetCompetitionsResponse {
    items: { id: string; name: string }[];
}

export interface GetChannelsResponse {
    items: { id: string; name: string }[];
}

export const useGetFixtures = (
    request: GetFixturesRequest,
    { enabled }: { enabled?: boolean } = {}
) => {
    const searchParams = Object.fromEntries(
        Object.entries(request).filter(([, value]) => !!value)
    );

    return useQuery({
        enabled,
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
            apiClient
                .get('competitions', { signal })
                .json<GetCompetitionsResponse>(),
        staleTime: 1000 * 60 * 60,
    });

export const useGetChannels = () =>
    useQuery({
        queryKey: fixtureKeys.channels,
        queryFn: ({ signal }) =>
            apiClient.get('channels', { signal }).json<GetChannelsResponse>(),
        staleTime: 1000 * 60 * 60,
    });
