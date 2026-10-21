import { Button } from '@/components/ui/button';

interface TwoFactorConfirmRecoveryCodesProps {
    loading?: boolean;
    onConfirm: () => void;
    onCancel: () => void;
}

export function TwoFactorConfirmRecoveryCodes({
    loading,
    onConfirm,
    onCancel,
}: TwoFactorConfirmRecoveryCodesProps) {
    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                Your existing recovery codes will stop working and be replaced
                with new ones.
            </p>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button variant="outline" onClick={onCancel}>
                    Cancel
                </Button>
                <Button onClick={onConfirm} disabled={loading}>
                    {loading ? 'Regenerating...' : 'Regenerate Codes'}
                </Button>
            </div>
        </div>
    );
}
