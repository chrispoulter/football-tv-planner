import { useState } from 'react';
import { Navigate, useLocation, useNavigate, Link } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import {
    type LoginTwoFactorRequest,
    useLoginTwoFactor,
} from '../account-queries';
import { AccountLayout } from '../account-layout';
import { TwoFactorForm, type TwoFactorFormValues } from './two-factor-form';
import {
    RecoveryCodeForm,
    type RecoveryCodeFormValues,
} from './recovery-code-form';
import type { TwoFactorState } from './two-factor-state';

export function TwoFactorPage() {
    const navigate = useNavigate();

    const location = useLocation();

    const state = location.state as TwoFactorState | null;

    const [useRecoveryCode, setUseRecoveryCode] = useState(false);

    const { mutate: loginTwoFactor, isPending } = useLoginTwoFactor();

    if (!state) {
        return <Navigate to="/login" replace />;
    }

    const { rememberMe, from } = state;

    function submit(request: Omit<LoginTwoFactorRequest, 'rememberMe'>) {
        loginTwoFactor(
            { ...request, rememberMe },
            {
                onSuccess: () => navigate(from, { replace: true }),
                onError: (error) => toast.error(error.message),
            }
        );
    }

    function onAuthenticatorSubmit(values: TwoFactorFormValues) {
        submit(values);
    }

    function onRecoveryCodeSubmit(values: RecoveryCodeFormValues) {
        submit(values);
    }

    return (
        <AccountLayout
            title="Two-Factor Authentication"
            description={
                useRecoveryCode
                    ? 'Enter one of your backup recovery codes'
                    : 'Enter the 6-digit code from your authenticator app'
            }
            footer={
                <Link
                    className="underline underline-offset-4 hover:text-foreground"
                    to="/login"
                >
                    Back to sign in
                </Link>
            }
        >
            <Metadata title="Two-Factor Authentication" />

            {useRecoveryCode ? (
                <RecoveryCodeForm
                    loading={isPending}
                    onSubmit={onRecoveryCodeSubmit}
                    onUseAuthenticator={() => setUseRecoveryCode(false)}
                />
            ) : (
                <TwoFactorForm
                    loading={isPending}
                    onSubmit={onAuthenticatorSubmit}
                    onUseRecoveryCode={() => setUseRecoveryCode(true)}
                />
            )}
        </AccountLayout>
    );
}
