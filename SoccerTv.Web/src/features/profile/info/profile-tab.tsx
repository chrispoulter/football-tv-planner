import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { LoadingButton } from '@/components/loading-button';
import { Metadata } from '@/components/metadata';
import { Alert, AlertDescription } from '@/components/ui/alert';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { useResendConfirmationEmail } from '../profile-queries';
import { UpdateEmailForm } from './update-email-form';
import { UpdateProfileForm } from './update-profile-form';

export function ProfileTab() {
    const { user } = useAuth();

    const { mutate: resendConfirmation, isPending: isResending } =
        useResendConfirmationEmail();

    function onResendConfirmation() {
        resendConfirmation(undefined, {
            onSuccess: () =>
                toast.success(
                    `A confirmation link has been sent to ${user?.email}.`
                ),
            onError: (error) => toast.error(error.message),
        });
    }

    // RequireAuth only renders this once the user has loaded
    if (!user) {
        return null;
    }

    return (
        <div className="space-y-6">
            <Metadata title="Profile" />

            <Card>
                <CardHeader>
                    <CardTitle>Personal Information</CardTitle>
                    <CardDescription>
                        Update your personal details
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    <UpdateProfileForm name={user.name} />
                </CardContent>
            </Card>

            <Card>
                <CardHeader>
                    <CardTitle>Email Address</CardTitle>
                    <CardDescription>
                        Used to sign in to your account. A confirmation link is
                        sent to a new address before it changes.
                    </CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                    <p className="text-sm text-muted-foreground">
                        Current email:{' '}
                        <span className="font-medium break-all text-foreground">
                            {user.email}
                        </span>
                    </p>

                    {!user.isEmailConfirmed && (
                        <Alert>
                            <AlertDescription className="space-y-3">
                                <p>
                                    Your email address hasn&apos;t been
                                    confirmed yet.
                                </p>
                                <LoadingButton
                                    variant="outline"
                                    size="sm"
                                    loading={isResending}
                                    onClick={onResendConfirmation}
                                >
                                    Resend Confirmation Email
                                </LoadingButton>
                            </AlertDescription>
                        </Alert>
                    )}

                    <UpdateEmailForm />
                </CardContent>
            </Card>
        </div>
    );
}
