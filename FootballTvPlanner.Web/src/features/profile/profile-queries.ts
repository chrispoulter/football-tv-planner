import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { sessionKeys } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';

export const profileKeys = {
    all: ['profile'] as const,
    twoFactor: ['profile', 'two-factor'] as const,
    linkedAccounts: ['profile', 'linked-accounts'] as const,
};

// Most profile changes are also reflected in the session
function useInvalidateSession() {
    const queryClient = useQueryClient();

    return () => queryClient.invalidateQueries({ queryKey: sessionKeys.all });
}

interface UpdateProfileRequest {
    name: string;
}

export const useUpdateProfile = () => {
    const invalidateSession = useInvalidateSession();

    return useMutation({
        mutationFn: (request: UpdateProfileRequest) =>
            apiClient.put('profile', { json: request }).then(() => undefined),
        onSuccess: invalidateSession,
    });
};

interface ChangeEmailRequest {
    newEmail: string;
}

// The email only changes once the link sent to the new address is followed
export const useChangeEmail = () =>
    useMutation({
        mutationFn: (request: ChangeEmailRequest) =>
            apiClient
                .post('profile/change-email', { json: request })
                .then(() => undefined),
    });

interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
}

export const useChangePassword = () =>
    useMutation({
        mutationFn: (request: ChangePasswordRequest) =>
            apiClient
                .post('profile/change-password', { json: request })
                .then(() => undefined),
    });

interface SetPasswordRequest {
    newPassword: string;
}

// For accounts that only sign in with Google
export const useSetPassword = () => {
    const invalidateSession = useInvalidateSession();

    return useMutation({
        mutationFn: (request: SetPasswordRequest) =>
            apiClient
                .post('profile/set-password', { json: request })
                .then(() => undefined),
        onSuccess: invalidateSession,
    });
};

// The API leaves out false and zero values
export interface TwoFactorResponse {
    isEnabled?: boolean;
    recoveryCodesLeft?: number;
}

export const useGetTwoFactor = () =>
    useQuery({
        queryKey: profileKeys.twoFactor,
        queryFn: ({ signal }) =>
            apiClient
                .get('profile/two-factor', { signal })
                .json<TwoFactorResponse>(),
    });

interface SetupTwoFactorResponse {
    sharedKey: string;
    authenticatorUri: string;
}

// The key stays the same until 2FA is enabled, so setup can be restarted
export const useSetupTwoFactor = () =>
    useMutation({
        mutationFn: () =>
            apiClient
                .post('profile/two-factor/setup', { json: {} })
                .json<SetupTwoFactorResponse>(),
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
                .post('profile/two-factor/enable', { json: request })
                .json<RecoveryCodesResponse>(),
        onSuccess: invalidate,
    });
};

export const useDisableTwoFactor = () => {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('profile/two-factor/disable', { json: {} })
                .then(() => undefined),
        onSuccess: invalidate,
    });
};

export const useGenerateRecoveryCodes = () => {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('profile/two-factor/recovery-codes', { json: {} })
                .json<RecoveryCodesResponse>(),
        onSuccess: invalidate,
    });
};

interface LinkedAccountsResponse {
    providers: string[];
}

export const useGetLinkedAccounts = () =>
    useQuery({
        queryKey: profileKeys.linkedAccounts,
        queryFn: ({ signal }) =>
            apiClient
                .get('profile/linked-accounts', { signal })
                .json<LinkedAccountsResponse>(),
    });

export const useRemoveLinkedAccount = () => {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (provider: string) =>
            apiClient
                .delete(
                    `profile/linked-accounts/${encodeURIComponent(provider)}`
                )
                .then(() => undefined),
        onSuccess: () =>
            queryClient.invalidateQueries({
                queryKey: profileKeys.linkedAccounts,
            }),
    });
};

// A full page navigation, as the provider redirects back to the API
export function linkAccountUrl(provider: string, returnUrl: string) {
    const searchParams = new URLSearchParams({ provider, returnUrl });

    return `/api/profile/linked-accounts/link?${searchParams}`;
}

interface DeleteAccountResponse {
    id: string;
}

export const useDeleteAccount = () =>
    useMutation({
        mutationFn: () =>
            apiClient.delete('account').json<DeleteAccountResponse>(),
    });
