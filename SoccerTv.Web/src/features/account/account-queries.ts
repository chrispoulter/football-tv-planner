import { useMutation, useQuery } from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';

export interface LoginRequest {
    email: string;
    password: string;
    rememberMe: boolean;
    twoFactorCode?: string;
    twoFactorRecoveryCode?: string;
}

export const useLogin = () => {
    const { refreshAuth } = useAuth();

    return useMutation({
        mutationFn: ({ rememberMe, ...request }: LoginRequest) =>
            apiClient
                .post('account/login', {
                    json: request,
                    searchParams: {
                        useCookies: true,
                        useSessionCookies: !rememberMe,
                    },
                })
                .then(() => undefined),
        onSuccess: refreshAuth,
    });
};

export const useLogout = () => {
    const { refreshAuth } = useAuth();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('account/logout', { json: {} })
                .then(() => undefined),
        onSettled: refreshAuth,
    });
};

interface RegisterRequest {
    email: string;
    password: string;
}

export const useRegister = () =>
    useMutation({
        mutationFn: (request: RegisterRequest) =>
            apiClient
                .post('account/register', { json: request })
                .then(() => undefined),
    });

interface ConfirmEmailRequest {
    userId: string;
    code: string;
    changedEmail?: string;
}

// A query rather than a mutation so the request is only sent once
export const useConfirmEmail = (request: ConfirmEmailRequest) =>
    useQuery({
        queryKey: ['confirm-email', request],
        queryFn: ({ signal }) =>
            apiClient
                .get('account/confirmEmail', {
                    searchParams: { ...request },
                    signal,
                })
                .then(() => true),
        staleTime: Infinity,
    });

interface EmailRequest {
    email: string;
}

export const useResendConfirmationEmail = () =>
    useMutation({
        mutationFn: (request: EmailRequest) =>
            apiClient
                .post('account/resendConfirmationEmail', { json: request })
                .then(() => undefined),
    });

export const useForgotPassword = () =>
    useMutation({
        mutationFn: (request: EmailRequest) =>
            apiClient
                .post('account/forgotPassword', { json: request })
                .then(() => undefined),
    });

interface ResetPasswordRequest {
    email: string;
    resetCode: string;
    newPassword: string;
}

export const useResetPassword = () =>
    useMutation({
        mutationFn: (request: ResetPasswordRequest) =>
            apiClient
                .post('account/resetPassword', { json: request })
                .then(() => undefined),
    });

export function googleLoginUrl(returnUrl = '/') {
    const searchParams = new URLSearchParams({ provider: 'Google', returnUrl });

    return `/api/account/external-login?${searchParams}`;
}
