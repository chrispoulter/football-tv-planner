import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { useSendVerificationEmail } from '../profile-queries';

export function VerifyEmailForm() {
    const { user } = useAuth();

    const {
        mutate: sendVerificationEmail,
        isPending,
        isSuccess,
    } = useSendVerificationEmail();

    const email = user?.email ?? '';

    function onSend() {
        sendVerificationEmail(undefined, {
            onError: (error) => toast.error(error.message),
        });
    }

    if (isSuccess) {
        return (
            <Alert>
                <AlertDescription>
                    <p>
                        A verification link has been sent to{' '}
                        <strong className="break-all">{email}</strong>.
                    </p>
                    <p>Click the link in that email to verify your address.</p>
                </AlertDescription>
            </Alert>
        );
    }

    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                <span className="font-medium break-all text-foreground">
                    {email}
                </span>{' '}
                hasn&apos;t been verified yet.
            </p>
            <Button
                onClick={onSend}
                disabled={isPending}
                className="w-full sm:w-auto"
            >
                {isPending ? 'Sending...' : 'Send Verification Email'}
            </Button>
        </div>
    );
}
