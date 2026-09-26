import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { sessionKeys } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';

export const profileKeys = {
    all: ['profile'] as const,
    twoFactor: ['profile', 'two-factor'] as const,
    twoFactorSetup: ['profile', 'two-factor', 'setup'] as const,
};

interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
}

export const useChangePassword = () =>
    useMutation({
        mutationFn: (request: ChangePasswordRequest) =>
            apiClient
                .post('account/change-password', { json: request })
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
                .post('account/change-email', { json: request })
                .then(() => undefined),
    });

// The API leaves out false and zero values
export interface TwoFactorResponse {
    isEnabled?: boolean;
    recoveryCodesLeft?: number;
    isMachineRemembered?: boolean;
}

export const useGetTwoFactor = () =>
    useQuery({
        queryKey: profileKeys.twoFactor,
        queryFn: ({ signal }) =>
            apiClient
                .get('account/two-factor', { signal })
                .json<TwoFactorResponse>(),
    });

interface SetupTwoFactorResponse {
    sharedKey: string;
    authenticatorUri: string;
}

// A query rather than a mutation, as the key stays the same until 2FA is enabled
export const useSetupTwoFactor = (enabled: boolean) =>
    useQuery({
        queryKey: profileKeys.twoFactorSetup,
        queryFn: ({ signal }) =>
            apiClient
                .post('account/two-factor/setup', { json: {}, signal })
                .json<SetupTwoFactorResponse>(),
        enabled,
        staleTime: Infinity,
    });

interface RecoveryCodesResponse {
    recoveryCodes: string[];
}

// Changes to 2FA are also reflected in the session
function useInvalidateTwoFactor() {
    const queryClient = useQueryClient();

    return () =>
        Promise.all([
            queryClient.invalidateQueries({ queryKey: profileKeys.twoFactor }),
            queryClient.invalidateQueries({ queryKey: sessionKeys.all }),
        ]);
}

interface EnableTwoFactorRequest {
    code: string;
}

export const useEnableTwoFactor = () => {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: (request: EnableTwoFactorRequest) =>
            apiClient
                .post('account/two-factor/enable', { json: request })
                .json<RecoveryCodesResponse>(),
        onSuccess: invalidate,
    });
};

export const useDisableTwoFactor = () => {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('account/two-factor/disable', { json: {} })
                .then(() => undefined),
        onSuccess: invalidate,
    });
};

export const useGenerateRecoveryCodes = () => {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('account/two-factor/recovery-codes', { json: {} })
                .json<RecoveryCodesResponse>(),
        onSuccess: invalidate,
    });
};

export const useForgetTwoFactorMachine = () => {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('account/two-factor/forget-machine', { json: {} })
                .then(() => undefined),
        onSuccess: invalidate,
    });
};

interface DeleteAccountResponse {
    id: string;
}

export const useDeleteAccount = () =>
    useMutation({
        mutationFn: () =>
            apiClient.delete('account').json<DeleteAccountResponse>(),
    });
