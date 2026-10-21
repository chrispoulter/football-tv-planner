import ky, { HTTPError } from 'ky';
import { sessionKeys } from '@/components/auth-provider';
import { queryClient } from './query-client';

export interface ProblemDetails {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
}

export const apiClient = ky.create({
    prefix: '/api',
    // Allow for the API cold starting
    timeout: 30_000,
    hooks: {
        beforeError: [
            async ({ error }) => {
                if (error instanceof HTTPError) {
                    const body = error.data as ProblemDetails | undefined;
                    const [firstError] = Object.values(
                        body?.errors ?? {}
                    ).flat();

                    error.message = firstError || body?.title || error.message;
                }
                return error;
            },
        ],
        afterResponse: [
            ({ response }) => {
                // The cookie is missing or expired, so forget the user
                if (response.status === 401) {
                    queryClient.setQueryData(sessionKeys.all, null);
                }
                return response;
            },
        ],
    },
});
