import { useMutation, useQuery } from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';

export interface LoginRequest {
    email: string;
    password: string;
    rememberMe: boolean;
}

// The API leaves out false values
interface LoginResponse {
    requiresTwoFactor?: boolean;
}

export const useLogin = () => {
    const { refreshAuth } = useAuth();

    return useMutation({
        mutationFn: (request: LoginRequest) =>
            apiClient
                .post('account/login', { json: request })
                .json<LoginResponse>(),
        // Not logged in yet when a two-factor code is still needed
        onSuccess: (response) => {
            if (!response.requiresTwoFactor) {
                return refreshAuth();
            }
        },
    });
};

// Either a code from the authenticator app or a recovery code
export interface LoginTwoFactorRequest {
    code?: string;
    recoveryCode?: string;
    rememberMe: boolean;
}

export const useLoginTwoFactor = () => {
    const { refreshAuth } = useAuth();

    return useMutation({
        mutationFn: (request: LoginTwoFactorRequest) =>
            apiClient
                .post('account/login/two-factor', { json: request })
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
    name: string;
    email: string;
    password: string;
}

// Also logs the new user in
export const useRegister = () => {
    const { refreshAuth } = useAuth();

    return useMutation({
        mutationFn: (request: RegisterRequest) =>
            apiClient
                .post('account/register', { json: request })
                .then(() => undefined),
        onSuccess: refreshAuth,
    });
};

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
                .post('account/confirm-email', { json: request, signal })
                .then(() => true),
        staleTime: Infinity,
    });

interface EmailRequest {
    email: string;
}

export const useForgotPassword = () =>
    useMutation({
        mutationFn: (request: EmailRequest) =>
            apiClient
                .post('account/forgot-password', { json: request })
                .then(() => undefined),
    });

interface ResetPasswordRequest {
    email: string;
    code: string;
    newPassword: string;
}

export const useResetPassword = () =>
    useMutation({
        mutationFn: (request: ResetPasswordRequest) =>
            apiClient
                .post('account/reset-password', { json: request })
                .then(() => undefined),
    });

export function googleLoginUrl(returnUrl = '/') {
    const searchParams = new URLSearchParams({ provider: 'Google', returnUrl });

    return `/api/account/external-login?${searchParams}`;
}
