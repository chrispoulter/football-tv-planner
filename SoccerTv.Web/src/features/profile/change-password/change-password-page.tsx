import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { Metadata } from '@/components/metadata';
import { useChangePassword } from '../profile-queries';
import {
    ChangePasswordForm,
    type ChangePasswordFormValues,
} from './change-password-form';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';

export function ChangePasswordPage() {
    const navigate = useNavigate();

    const { mutate: changePassword, isPending: isSaving } = useChangePassword();

    function onSubmit(values: ChangePasswordFormValues) {
        changePassword(
            {
                currentPassword: values.currentPassword,
                newPassword: values.newPassword,
            },
            {
                onSuccess: () => {
                    toast.success('Your password has been changed.');
                    navigate('/profile');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <div className="mx-auto w-full max-w-2xl space-y-6">
            <Metadata title="Change Password" />

            <Card>
                <CardHeader>
                    <CardTitle className="text-2xl">Change Password</CardTitle>
                    <CardDescription>
                        Change your password below. Choose a strong password and
                        don&apos;t reuse it for other accounts. For security
                        reasons, change your password on a regular basis.
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <ChangePasswordForm
                        onSubmit={onSubmit}
                        loading={isSaving}
                        disabled={isSaving}
                    >
                        <Button asChild variant="outline">
                            <Link to="/profile">Cancel</Link>
                        </Button>
                    </ChangePasswordForm>
                </CardContent>
            </Card>

            <p className="text-sm text-muted-foreground">
                Forgotten your password, or signed up with Google?{' '}
                <Link
                    to="/account/forgot-password"
                    className="underline underline-offset-4"
                >
                    Request reset
                </Link>
            </p>
        </div>
    );
}
