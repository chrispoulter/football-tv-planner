import { useNavigate, useSearchParams, Link } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { useResetPassword } from '../account-queries';
import { AccountLayout } from '../account-layout';
import {
    ResetPasswordForm,
    type ResetPasswordFormValues,
} from './reset-password-form';

export function ResetPasswordPage() {
    const [searchParams] = useSearchParams();

    const navigate = useNavigate();

    const { mutate: resetPassword, isPending } = useResetPassword();

    const email = searchParams.get('email');
    const code = searchParams.get('code');

    function onSubmit(values: ResetPasswordFormValues) {
        if (!email || !code) {
            return;
        }

        resetPassword(
            { email, code, newPassword: values.newPassword },
            {
                onSuccess: () => {
                    toast.success(
                        'Password reset successfully. Please sign in.'
                    );
                    navigate('/login');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <AccountLayout
            title="Reset your password"
            description="Enter your new password below"
            footer={
                <Link
                    className="underline underline-offset-4 hover:text-foreground"
                    to="/login"
                >
                    Back to sign in
                </Link>
            }
        >
            <Metadata title="Reset Password" />

            {email && code ? (
                <ResetPasswordForm loading={isPending} onSubmit={onSubmit} />
            ) : (
                <Alert variant="destructive">
                    <AlertDescription>
                        This link is invalid or has expired. Please{' '}
                        <Link
                            className="underline underline-offset-4 hover:text-foreground"
                            to="/forgot-password"
                        >
                            request a new one
                        </Link>
                        .
                    </AlertDescription>
                </Alert>
            )}
        </AccountLayout>
    );
}
