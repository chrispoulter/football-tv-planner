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
                if (response.status === 401) {
                    queryClient.setQueryData(sessionKeys.all, null);
                }
                return response;
            },
        ],
    },
});
