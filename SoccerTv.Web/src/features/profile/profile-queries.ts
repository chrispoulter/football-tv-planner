import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useAuth } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';

export const profileKeys = {
    all: ['profile'] as const,
};

export interface GetProfileResponse {
    id: string;
    emailAddress: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
}

export const useGetProfile = () => {
    const { accessToken } = useAuth();

    return useQuery({
        queryKey: profileKeys.all,
        queryFn: ({ signal }) =>
            apiClient
                .get('profile', {
                    context: {
                        accessToken,
                    },
                    signal,
                })
                .json<GetProfileResponse>(),
    });
};

interface UpdateProfileRequest {
    emailAddress: string;
    firstName: string;
    lastName: string;
    dateOfBirth: string;
}

interface UpdateProfileResponse {
    id: string;
}

export const useUpdateProfile = () => {
    const { accessToken } = useAuth();

    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (request: UpdateProfileRequest) =>
            apiClient
                .put('profile', {
                    json: request,
                    context: {
                        accessToken,
                    },
                })
                .json<UpdateProfileResponse>(),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: profileKeys.all }),
    });
};

interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
}

interface ChangePasswordResponse {
    id: string;
}

export const useChangePassword = () => {
    const { accessToken } = useAuth();

    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (request: ChangePasswordRequest) =>
            apiClient
                .put('profile/change-password', {
                    json: request,
                    context: {
                        accessToken,
                    },
                })
                .json<ChangePasswordResponse>(),
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: profileKeys.all }),
    });
};

interface DeleteAccountResponse {
    id: string;
}

export const useDeleteAccount = () => {
    const { accessToken } = useAuth();

    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () =>
            apiClient
                .delete('profile', {
                    context: {
                        accessToken,
                    },
                })
                .json<DeleteAccountResponse>(),
        onSuccess: () =>
            queryClient.invalidateQueries({
                queryKey: profileKeys.all,
                refetchType: 'none',
            }),
    });
};
