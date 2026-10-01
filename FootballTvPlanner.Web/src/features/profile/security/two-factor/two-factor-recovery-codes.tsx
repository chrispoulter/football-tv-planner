import { toast } from 'sonner';
import { Button } from '@/components/ui/button';

interface TwoFactorRecoveryCodesProps {
    recoveryCodes: string[];
    onDone: () => void;
}

export function TwoFactorRecoveryCodes({
    recoveryCodes,
    onDone,
}: TwoFactorRecoveryCodesProps) {
    const handleCopyAllCodes = () => {
        navigator.clipboard.writeText(recoveryCodes.join('\n'));
        toast.success('Recovery codes copied to clipboard');
    };

    return (
        <div className="space-y-4">
            <p className="text-sm font-medium">
                Save your recovery codes. Each code can only be used once.
            </p>
            <div className="grid grid-cols-2 gap-2 rounded-md border p-4">
                {recoveryCodes.map((code) => (
                    <code key={code} className="font-mono text-sm select-all">
                        {code}
                    </code>
                ))}
            </div>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button
                    type="button"
                    variant="outline"
                    onClick={handleCopyAllCodes}
                >
                    Copy all
                </Button>
                <Button type="button" onClick={onDone}>
                    Done
                </Button>
            </div>
        </div>
    );
}
