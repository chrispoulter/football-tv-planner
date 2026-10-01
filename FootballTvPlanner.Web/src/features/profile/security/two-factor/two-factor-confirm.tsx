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
                <Button
                    variant={destructive ? 'destructive' : 'default'}
                    onClick={onConfirm}
                    disabled={loading}
                >
                    {loading ? loadingLabel : confirmLabel}
                </Button>
            </div>
        </div>
    );
}
