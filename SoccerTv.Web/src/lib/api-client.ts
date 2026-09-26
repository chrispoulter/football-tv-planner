import ky, { HTTPError } from 'ky';

export interface ProblemDetails {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
}

// The API is proxied through this origin so the auth cookie is first-party
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
    },
});

export function getProblemDetail(error: unknown) {
    if (error instanceof HTTPError) {
        return (error.data as ProblemDetails | undefined)?.detail;
    }
}
