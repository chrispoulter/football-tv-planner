import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { apiClient } from '@/lib/api-client';

export const profileKeys = {
    all: ['profile'] as const,
    twoFactor: ['profile', 'two-factor'] as const,
};

interface ChangePasswordRequest {
    oldPassword: string;
    newPassword: string;
}

export const useChangePassword = () =>
    useMutation({
        mutationFn: (request: ChangePasswordRequest) =>
            apiClient
                .post('account/manage/info', { json: request })
                .then(() => undefined),
    });

interface ChangeEmailRequest {
    newEmail: string;
}

// The email only changes once the link sent to the new address is followed
export const useChangeEmail = () =>
    useMutation({
        mutationFn: (request: ChangeEmailRequest) =>
            apiClient
                .post('account/manage/info', { json: request })
                .then(() => undefined),
    });

// The API leaves out false and zero values
export interface TwoFactorResponse {
    sharedKey: string;
    recoveryCodesLeft?: number;
    recoveryCodes?: string[];
    isTwoFactorEnabled?: boolean;
    isMachineRemembered?: boolean;
}

interface TwoFactorRequest {
    enable?: boolean;
    twoFactorCode?: string;
    resetRecoveryCodes?: boolean;
    forgetMachine?: boolean;
}

const postTwoFactor = (request: TwoFactorRequest) =>
    apiClient
        .post('account/manage/2fa', { json: request })
        .json<TwoFactorResponse>();

// Posting an empty request just returns the current status
export const useGetTwoFactor = () =>
    useQuery({
        queryKey: profileKeys.twoFactor,
        queryFn: () => postTwoFactor({}),
    });

export const useUpdateTwoFactor = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: postTwoFactor,
        // Refetch rather than use the response, as after forgetting the browser
        // it still reports the browser as remembered
        onSuccess: () =>
            queryClient.invalidateQueries({ queryKey: profileKeys.twoFactor }),
    });
};

interface DeleteAccountResponse {
    id: string;
}

export const useDeleteAccount = () =>
    useMutation({
        mutationFn: () =>
            apiClient.delete('profile').json<DeleteAccountResponse>(),
    });
