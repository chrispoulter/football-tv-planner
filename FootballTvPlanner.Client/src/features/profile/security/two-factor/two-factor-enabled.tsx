import { Button } from '@/components/ui/button';

interface TwoFactorEnabledProps {
    recoveryCodesLeft: number;
    onDisable: () => void;
    onGenerateRecoveryCodes: () => void;
}

export function TwoFactorEnabled({
    recoveryCodesLeft,
    onDisable,
    onGenerateRecoveryCodes,
}: TwoFactorEnabledProps) {
    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                Enabled. You have {recoveryCodesLeft} recovery codes left.
            </p>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button variant="destructive" onClick={onDisable}>
                    Disable 2FA
                </Button>
                <Button variant="outline" onClick={onGenerateRecoveryCodes}>
                    Regenerate Recovery Codes
                </Button>
            </div>
        </div>
    );
}
