import { LoadingButton } from '@/components/loading-button';
import { Button } from '@/components/ui/button';

interface TwoFactorConfirmProps {
    message: string;
    confirmLabel: string;
    loadingLabel: string;
    destructive?: boolean;
    loading?: boolean;
    onConfirm: () => void;
    onCancel: () => void;
}

/**
 * Asks before a change that can't be undone, such as disabling 2FA.
 */
export function TwoFactorConfirm({
    message,
    confirmLabel,
    loadingLabel,
    destructive,
    loading,
    onConfirm,
    onCancel,
}: TwoFactorConfirmProps) {
    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">{message}</p>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button variant="outline" onClick={onCancel}>
                    Cancel
                </Button>
                <LoadingButton
                    variant={destructive ? 'destructive' : 'default'}
                    loading={loading}
                    loadingText={loadingLabel}
                    onClick={onConfirm}
                >
                    {confirmLabel}
                </LoadingButton>
            </div>
        </div>
    );
}
