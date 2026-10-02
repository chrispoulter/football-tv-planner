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
    competition: string;
    homeTeam: string;
    awayTeam: string;
    channels: string[];
    isBookmarked?: boolean;
}

export interface CompetitionResponse {
    id: string;
    name: string;
}

export interface ChannelResponse {
    id: string;
    name: string;
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
                .json<CompetitionResponse[]>(),
        staleTime: 1000 * 60 * 60,
    });

export const useGetChannels = () =>
    useQuery({
        queryKey: fixtureKeys.channels,
        queryFn: ({ signal }) =>
            apiClient.get('channels', { signal }).json<ChannelResponse[]>(),
        staleTime: 1000 * 60 * 60,
    });
