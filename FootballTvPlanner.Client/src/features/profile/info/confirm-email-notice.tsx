import { useState } from 'react';
import { toast } from 'sonner';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { useResendConfirmationEmail } from '../profile-queries';

export function ConfirmEmailNotice() {
    const [sent, setSent] = useState(false);

    const { mutate: resend, isPending } = useResendConfirmationEmail();

    function onResend() {
        resend(undefined, {
            onSuccess: () => setSent(true),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <Alert>
            <AlertDescription className="space-y-3">
                <p>
                    {sent
                        ? 'A new confirmation link has been sent. Follow the link in that email to confirm your address.'
                        : 'Your email address has not been confirmed yet. Check your inbox for the confirmation link.'}
                </p>
                {!sent && (
                    <Button
                        variant="outline"
                        size="sm"
                        disabled={isPending}
                        onClick={onResend}
                    >
                        {isPending ? 'Sending...' : 'Resend Confirmation Email'}
                    </Button>
                )}
            </AlertDescription>
        </Alert>
    );
}
