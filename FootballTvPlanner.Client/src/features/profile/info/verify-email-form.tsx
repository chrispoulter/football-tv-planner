import { toast } from 'sonner';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { useSendConfirmationEmail } from '../profile-queries';

interface VerifyEmailFormProps {
    email: string;
}

export function VerifyEmailForm({ email }: VerifyEmailFormProps) {
    const {
        mutate: sendConfirmationEmail,
        isPending,
        isSuccess,
    } = useSendConfirmationEmail();

    const handleSend = () =>
        sendConfirmationEmail(undefined, {
            onError: (error) => toast.error(error.message),
        });

    if (isSuccess) {
        return (
            <Alert>
                <AlertDescription>
                    <p>
                        A verification link has been sent to{' '}
                        <strong className="break-all">{email}</strong>.
                    </p>
                    <p>Follow the link in that email to verify your address.</p>
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
                onClick={handleSend}
                disabled={isPending}
                className="w-full sm:w-auto"
            >
                {isPending ? 'Sending...' : 'Send Verification Email'}
            </Button>
        </div>
    );
}
