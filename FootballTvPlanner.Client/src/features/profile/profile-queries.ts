import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { sessionKeys } from '@/components/auth-provider';
import { apiClient } from '@/lib/api-client';

export const profileKeys = {
    all: ['profile'] as const,
    twoFactor: ['profile', 'two-factor'] as const,
    linkedAccounts: ['profile', 'linked-accounts'] as const,
};

export const personalDataUrl = '/api/profile/personal-data';

function useInvalidateSession() {
    const queryClient = useQueryClient();

    return () => queryClient.invalidateQueries({ queryKey: sessionKeys.all });
}

interface UpdateProfileRequest {
    name: string;
}

export function useUpdateProfile() {
    const invalidateSession = useInvalidateSession();

    return useMutation({
        mutationFn: (request: UpdateProfileRequest) =>
            apiClient.put('profile', { json: request }).then(() => undefined),
        onSuccess: invalidateSession,
    });
}

interface ChangeEmailRequest {
    newEmail: string;
}

export function useChangeEmail() {
    return useMutation({
        mutationFn: (request: ChangeEmailRequest) =>
            apiClient
                .post('profile/change-email', { json: request })
                .then(() => undefined),
    });
}

export function useSendConfirmationEmail() {
    return useMutation({
        mutationFn: () =>
            apiClient
                .post('profile/send-confirmation-email', { json: {} })
                .then(() => undefined),
    });
}

interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
}

export function useChangePassword() {
    return useMutation({
        mutationFn: (request: ChangePasswordRequest) =>
            apiClient
                .post('profile/change-password', { json: request })
                .then(() => undefined),
    });
}

interface SetPasswordRequest {
    newPassword: string;
}

export function useSetPassword() {
    const invalidateSession = useInvalidateSession();

    return useMutation({
        mutationFn: (request: SetPasswordRequest) =>
            apiClient
                .post('profile/set-password', { json: request })
                .then(() => undefined),
        onSuccess: invalidateSession,
    });
}

export interface TwoFactorResponse {
    isEnabled?: boolean;
    recoveryCodesLeft?: number;
}

export function useGetTwoFactor() {
    return useQuery({
        queryKey: profileKeys.twoFactor,
        queryFn: ({ signal }) =>
            apiClient
                .get('profile/two-factor', { signal })
                .json<TwoFactorResponse>(),
    });
}

interface SetupTwoFactorResponse {
    sharedKey: string;
    authenticatorUri: string;
}

export function useSetupTwoFactor() {
    return useMutation({
        mutationFn: () =>
            apiClient
                .post('profile/two-factor/setup', { json: {} })
                .json<SetupTwoFactorResponse>(),
    });
}

interface RecoveryCodesResponse {
    recoveryCodes: string[];
}

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

export function useEnableTwoFactor() {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: (request: EnableTwoFactorRequest) =>
            apiClient
                .post('profile/two-factor/enable', { json: request })
                .json<RecoveryCodesResponse>(),
        onSuccess: invalidate,
    });
}

export function useDisableTwoFactor() {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('profile/two-factor/disable', { json: {} })
                .then(() => undefined),
        onSuccess: invalidate,
    });
}

export function useGenerateRecoveryCodes() {
    const invalidate = useInvalidateTwoFactor();

    return useMutation({
        mutationFn: () =>
            apiClient
                .post('profile/two-factor/recovery-codes', { json: {} })
                .json<RecoveryCodesResponse>(),
        onSuccess: invalidate,
    });
}

interface LinkedAccountsResponse {
    providers: string[];
}

export function useGetLinkedAccounts() {
    return useQuery({
        queryKey: profileKeys.linkedAccounts,
        queryFn: ({ signal }) =>
            apiClient
                .get('profile/linked-accounts', { signal })
                .json<LinkedAccountsResponse>(),
    });
}

export function useRemoveLinkedAccount() {
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
}

export function linkAccountUrl(provider: string, returnUrl: string) {
    const searchParams = new URLSearchParams({ provider, returnUrl });
    return `/api/profile/linked-accounts/link?${searchParams}`;
}

interface DeleteAccountResponse {
    id: string;
}

export function useDeleteAccount() {
    return useMutation({
        mutationFn: () =>
            apiClient.delete('account').json<DeleteAccountResponse>(),
    });
}
