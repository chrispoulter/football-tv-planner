import { keepPreviousData, useMutation, useQuery } from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';

export const fixtureKeys = {
    all: ['fixtures'] as const,
    list: (request: GetFixturesRequest) => ['fixtures', request] as const,
    competitions: ['competitions'] as const,
    providers: ['providers'] as const,
};

export type ChannelType = 'Tv' | 'Streaming';

export type FixtureStatus = 'Scheduled' | 'Postponed' | 'Cancelled';

export interface ChannelSummary {
    name: string;
    provider: string;
    type?: ChannelType;
}

export interface FixtureSummary {
    id: string;
    kickoffUtc: string;
    status?: FixtureStatus;
    competition: string;
    homeTeam: string;
    awayTeam: string;
    channels: ChannelSummary[];
    isBookmarked?: boolean;
}

interface GetFixturesRequest {
    from: string;
    to: string;
    competition?: string;
    provider?: string;
    bookmarked?: boolean;
}

export interface GetFixturesResponse {
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
    name: string;
}

export const useGetCompetitions = () =>
    useQuery({
        queryKey: fixtureKeys.competitions,
        queryFn: ({ signal }) =>
            apiClient.get('competitions', { signal }).json<Competition[]>(),
        staleTime: 1000 * 60 * 60,
    });

export const useGetProviders = () =>
    useQuery({
        queryKey: fixtureKeys.providers,
        queryFn: ({ signal }) =>
            apiClient.get('providers', { signal }).json<string[]>(),
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
            link.download = `${fixture.homeTeam} v ${fixture.awayTeam}.ics`;
            link.click();
            URL.revokeObjectURL(url);
        },
    });
};
