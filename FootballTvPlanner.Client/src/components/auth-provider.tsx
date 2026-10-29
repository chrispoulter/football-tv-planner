import { useCallback } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/lib/api-client';

export const sessionKeys = {
    all: ['session'] as const,
};

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
            const response = await apiClient.get('account/me', { signal });

            if (response.status === 204) {
                return null;
            }

            return response.json<Session>();
        },
        staleTime: Infinity,
    });

    const refreshAuth = useCallback(
        () => queryClient.resetQueries(),
        [queryClient]
    );

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
