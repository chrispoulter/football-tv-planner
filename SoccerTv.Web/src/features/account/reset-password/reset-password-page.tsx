import { useNavigate, useParams } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { useResetPassword } from '../account-queries';
import {
    ResetPasswordForm,
    type ResetPasswordFormValues,
} from './reset-password-form';
import { Card, CardContent } from '@/components/ui/card';

type ResetPasswordPageParams = { token: string };

export function ResetPasswordPage() {
    const { token } = useParams() as ResetPasswordPageParams;

    const navigate = useNavigate();

    const { mutate: resetPassword, isPending: isSaving } = useResetPassword();

    function onSubmit(values: ResetPasswordFormValues) {
        resetPassword(
            {
                token,
                ...values,
            },
            {
                onSuccess: () => {
                    toast.success('Your password has been reset.');
                    navigate('/account/login');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <div className="flex flex-1 items-center justify-center">
            <Card className="w-full max-w-md">
                <CardContent className="space-y-6">
                    <Metadata title="Reset Password" />

                    <div className="space-y-1">
                        <h1 className="text-2xl font-bold tracking-tight">
                            Reset Password
                        </h1>
                        <p className="text-sm text-muted-foreground">
                            Reset your password below. Choose a strong password
                            and don&apos;t reuse it for other accounts. For
                            security reasons, change your password on a regular
                            basis.
                        </p>
                    </div>

                    <ResetPasswordForm loading={isSaving} onSubmit={onSubmit} />
                </CardContent>
            </Card>
        </div>
    );
}
