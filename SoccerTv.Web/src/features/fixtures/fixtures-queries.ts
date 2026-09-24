import { keepPreviousData, useMutation, useQuery } from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';

export const fixtureKeys = {
    all: ['fixtures'] as const,
    list: (request: GetFixturesRequest) => ['fixtures', request] as const,
    competitions: ['competitions'] as const,
    channels: ['channels'] as const,
};

export type ChannelType = 'Tv' | 'Streaming';

export type FixtureStatus = 'Scheduled' | 'Postponed' | 'Cancelled';

export interface ChannelSummary {
    id: string;
    name: string;
    provider: string;
    type?: ChannelType;
}

export interface FixtureSummary {
    id: string;
    kickoffUtc: string;
    status?: FixtureStatus;
    competition: {
        id: string;
        name: string;
        shortName: string;
    };
    homeTeam: {
        id: string;
        name: string;
        shortName: string;
        badgeUrl?: string;
    };
    awayTeam: {
        id: string;
        name: string;
        shortName: string;
        badgeUrl?: string;
    };
    channels: ChannelSummary[];
    isBookmarked?: boolean;
}

interface GetFixturesRequest {
    date: string;
    competitionId?: string;
    provider?: string;
    bookmarked?: boolean;
}

export interface GetFixturesResponse {
    date: string;
    items: FixtureSummary[];
}

export const useGetFixtures = (request: GetFixturesRequest) => {
    const { accessToken } = useAuth();

    const searchParams = Object.fromEntries(
        Object.entries(request).filter(([, value]) => !!value)
    );

    return useQuery({
        queryKey: fixtureKeys.list(request),
        queryFn: ({ signal }) =>
            apiClient
                .get('fixtures', {
                    searchParams,
                    context: {
                        accessToken,
                    },
                    signal,
                })
                .json<GetFixturesResponse>(),
        placeholderData: keepPreviousData,
    });
};

export interface Competition {
    id: string;
    name: string;
    shortName: string;
    country: string;
}

export const useGetCompetitions = () =>
    useQuery({
        queryKey: fixtureKeys.competitions,
        queryFn: ({ signal }) =>
            apiClient.get('competitions', { signal }).json<Competition[]>(),
        staleTime: 1000 * 60 * 60,
    });

export const useGetChannels = () =>
    useQuery({
        queryKey: fixtureKeys.channels,
        queryFn: ({ signal }) =>
            apiClient.get('channels', { signal }).json<ChannelSummary[]>(),
        staleTime: 1000 * 60 * 60,
    });

export const useDownloadFixtureCalendar = () => {
    const { accessToken } = useAuth();

    return useMutation({
        mutationFn: async (fixture: FixtureSummary) => {
            const blob = await apiClient
                .get(`fixtures/${fixture.id}/calendar.ics`, {
                    context: {
                        accessToken,
                    },
                })
                .blob();

            const url = URL.createObjectURL(blob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `${fixture.homeTeam.name} v ${fixture.awayTeam.name}.ics`;
            link.click();
            URL.revokeObjectURL(url);
        },
    });
};
