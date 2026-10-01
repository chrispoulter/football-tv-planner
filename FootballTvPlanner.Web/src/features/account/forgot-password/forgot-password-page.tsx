import { useState } from 'react';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { useForgotPassword } from '../account-queries';
import { AccountLayout, AccountLink } from '../account-layout';
import {
    ForgotPasswordForm,
    type ForgotPasswordFormValues,
} from './forgot-password-form';

export function ForgotPasswordPage() {
    const [isSent, setIsSent] = useState(false);

    const { mutate: forgotPassword, isPending } = useForgotPassword();

    function onSubmit(values: ForgotPasswordFormValues) {
        forgotPassword(
            { email: values.emailAddress },
            {
                onSuccess: () => setIsSent(true),
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <AccountLayout
            title="Forgot Your Password?"
            description="Enter your email and we'll send you a reset link"
            footer={<AccountLink to="/login">Back to sign in</AccountLink>}
        >
            <Metadata title="Forgot Password" />

            {isSent ? (
                <Alert>
                    <AlertDescription>
                        <p>
                            If an account with that email exists, we&apos;ve
                            sent a password reset link.
                        </p>
                        <p>Please check your inbox.</p>
                    </AlertDescription>
                </Alert>
            ) : (
                <ForgotPasswordForm loading={isPending} onSubmit={onSubmit} />
            )}
        </AccountLayout>
    );
}
