import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import { useSetupTwoFactor } from '../../profile-queries';

interface TwoFactorDisabledProps {
    onTotpSetup: (sharedKey: string, authenticatorUri: string) => void;
}

export function TwoFactorDisabled({ onTotpSetup }: TwoFactorDisabledProps) {
    const { mutate: setupTwoFactor, isPending } = useSetupTwoFactor();

    function onEnable() {
        setupTwoFactor(undefined, {
            onSuccess: (response) =>
                onTotpSetup(response.sharedKey, response.authenticatorUri),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <Button
            onClick={onEnable}
            disabled={isPending}
            className="w-full sm:w-auto"
        >
            {isPending ? 'Setting Up...' : 'Enable 2FA'}
        </Button>
    );
}
