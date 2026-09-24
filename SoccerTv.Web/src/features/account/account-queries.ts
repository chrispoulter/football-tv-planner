import { useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/lib/api-client';
import { profileKeys } from '../profile/profile-queries';

interface LoginRequest {
    emailAddress: string;
    password: string;
}

interface LoginResponse {
    accessToken: string;
}

export const useLogin = () =>
    useMutation({
        mutationFn: (request: LoginRequest) =>
            apiClient
                .post('account/login', { json: request })
                .json<LoginResponse>(),
    });

interface RegisterRequest {
    emailAddress: string;
    password: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
}

interface RegisterResponse {
    id: string;
}

export const useRegister = () =>
    useMutation({
        mutationFn: (request: RegisterRequest) =>
            apiClient
                .post('account/register', { json: request })
                .json<RegisterResponse>(),
    });

interface ForgotPasswordRequest {
    emailAddress: string;
}

export const useForgotPassword = () =>
    useMutation({
        mutationFn: (request: ForgotPasswordRequest) =>
            apiClient
                .put('account/forgot-password', { json: request })
                .then(() => undefined),
    });

interface ResetPasswordRequest {
    token: string;
    emailAddress: string;
    newPassword: string;
}

interface ResetPasswordResponse {
    id: string;
}

export const useResetPassword = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (request: ResetPasswordRequest) =>
            apiClient
                .put('account/reset-password', { json: request })
                .json<ResetPasswordResponse>(),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: profileKeys.all }),
    });
};
