import { useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { useForgotPassword } from '../account-queries';
import {
    ForgotPasswordForm,
    type ForgotPasswordFormValues,
} from './forgot-password-form';
import { Card, CardContent } from '@/components/ui/card';

export function ForgotPasswordPage() {
    const navigate = useNavigate();

    const { mutate: forgotPassword, isPending: isSaving } = useForgotPassword();

    function onSubmit(values: ForgotPasswordFormValues) {
        forgotPassword(
            { email: values.emailAddress },
            {
                onSuccess: () => {
                    toast.success(
                        'Instructions as to how to reset your password have been sent to you via email.'
                    );

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
                    <Metadata title="Forgot Password" />

                    <div className="space-y-1">
                        <h1 className="text-2xl font-bold tracking-tight">
                            Forgot Password
                        </h1>
                        <p className="text-sm text-muted-foreground">
                            Request a password reset link by providing your
                            email address.
                        </p>
                    </div>

                    <ForgotPasswordForm
                        loading={isSaving}
                        onSubmit={onSubmit}
                    />
                </CardContent>
            </Card>
        </div>
    );
}
