import { Button } from '@/components/ui/button';

interface TwoFactorConfirmDisableProps {
    loading?: boolean;
    onConfirm: () => void;
    onCancel: () => void;
}

export function TwoFactorConfirmDisable({
    loading,
    onConfirm,
    onCancel,
}: TwoFactorConfirmDisableProps) {
    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                You&apos;ll no longer need a code from your authenticator app to
                sign in. Your authenticator app entry and recovery codes will
                stop working.
            </p>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button variant="outline" onClick={onCancel}>
                    Cancel
                </Button>
                <Button
                    variant="destructive"
                    onClick={onConfirm}
                    disabled={loading}
                >
                    {loading ? 'Disabling...' : 'Disable 2FA'}
                </Button>
            </div>
        </div>
    );
}
