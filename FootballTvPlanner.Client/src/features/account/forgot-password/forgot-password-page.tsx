import { useState } from 'react';
import { Link } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { useForgotPassword } from '../account-queries';
import { AccountLayout } from '../account-layout';
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
            footer={
                <Link
                    className="underline underline-offset-4 hover:text-foreground"
                    to="/login"
                >
                    Back to sign in
                </Link>
            }
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
