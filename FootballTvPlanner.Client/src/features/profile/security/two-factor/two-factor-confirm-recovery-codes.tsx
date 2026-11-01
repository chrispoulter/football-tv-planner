import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { useGenerateRecoveryCodes } from '../../profile-queries';

interface TwoFactorConfirmRecoveryCodesProps {
    onRegenerated: (newCodes: string[]) => void;
    onCancel: () => void;
}

export function TwoFactorConfirmRecoveryCodes({
    onRegenerated,
    onCancel,
}: TwoFactorConfirmRecoveryCodesProps) {
    const { mutate: generateRecoveryCodes, isPending } =
        useGenerateRecoveryCodes();

    function onRegenerate() {
        generateRecoveryCodes(undefined, {
            onSuccess: (response) => onRegenerated(response.recoveryCodes),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                Your existing recovery codes will stop working and be replaced
                with new ones.
            </p>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button type="button" variant="outline" onClick={onCancel}>
                    Cancel
                </Button>
                <Button onClick={onRegenerate} disabled={isPending}>
                    {isPending ? 'Regenerating...' : 'Regenerate Codes'}
                </Button>
            </div>
        </div>
    );
}
