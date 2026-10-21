import { useCallback } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { HTTPError } from 'ky';
import { apiClient } from '@/lib/api-client';

export const sessionKeys = {
    all: ['session'] as const,
};

// The API leaves out false and null values
export interface Session {
    id: string;
    email: string;
    name?: string;
    isEmailConfirmed?: boolean;
    hasPassword?: boolean;
    isTwoFactorEnabled?: boolean;
}

export function useAuth() {
    const queryClient = useQueryClient();

    const { data: user, isPending } = useQuery({
        queryKey: sessionKeys.all,
        queryFn: async ({ signal }) => {
            try {
                return await apiClient
                    .get('account/me', { signal })
                    .json<Session>();
            } catch (error) {
                // 404 means the cookie is valid but its user no longer exists
                if (
                    error instanceof HTTPError &&
                    (error.response.status === 401 ||
                        error.response.status === 404)
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
