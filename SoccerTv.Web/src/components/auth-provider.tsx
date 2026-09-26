import { useCallback } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { HTTPError } from 'ky';
import { apiClient } from '@/lib/api-client';

export const sessionKeys = {
    all: ['session'] as const,
};

export interface Session {
    email: string;
    isEmailConfirmed?: boolean;
}

export function useAuth() {
    const queryClient = useQueryClient();

    const { data: user, isPending } = useQuery({
        queryKey: sessionKeys.all,
        queryFn: async ({ signal }) => {
            try {
                return await apiClient
                    .get('account/manage/info', { signal })
                    .json<Session>();
            } catch (error) {
                if (
                    error instanceof HTTPError &&
                    error.response.status === 401
                ) {
                    return null;
                }
                throw error;
            }
        },
        staleTime: Infinity,
    });

    // Refetch everything after signing in or out, as results depend on the user
    const refreshAuth = useCallback(
        () => queryClient.resetQueries(),
        [queryClient]
    );

    // The cookie is already gone or expired, so just forget the user
    const clearAuth = useCallback(
        () => queryClient.setQueryData(sessionKeys.all, null),
        [queryClient]
    );

    return {
        user: user ?? undefined,
        isLoading: isPending,
        refreshAuth,
        clearAuth,
    };
}
