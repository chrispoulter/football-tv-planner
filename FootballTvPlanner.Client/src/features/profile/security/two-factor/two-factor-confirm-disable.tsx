import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { useDisableTwoFactor } from '../../profile-queries';

interface TwoFactorConfirmDisableProps {
    onDisabled: () => void;
    onCancel: () => void;
}

export function TwoFactorConfirmDisable({
    onDisabled,
    onCancel,
}: TwoFactorConfirmDisableProps) {
    const { mutate: disableTwoFactor, isPending } = useDisableTwoFactor();

    function onDisable() {
        disableTwoFactor(undefined, {
            onSuccess: () => {
                toast.success('Two-factor authentication disabled');
                onDisabled();
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                You&apos;ll no longer need a code from your authenticator app to
                sign in. Your authenticator app entry and recovery codes will
                stop working.
            </p>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button type="button" variant="outline" onClick={onCancel}>
                    Cancel
                </Button>
                <Button
                    variant="destructive"
                    onClick={onDisable}
                    disabled={isPending}
                >
                    {isPending ? 'Disabling...' : 'Disable 2FA'}
                </Button>
            </div>
        </div>
    );
}
