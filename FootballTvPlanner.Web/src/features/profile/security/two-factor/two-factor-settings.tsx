import { useState } from 'react';
import { toast } from 'sonner';
import { QueryError } from '@/components/query-error';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import {
    useDisableTwoFactor,
    useEnableTwoFactor,
    useForgetTwoFactorMachine,
    useGenerateRecoveryCodes,
    useGetTwoFactor,
    useSetupTwoFactor,
} from '../../profile-queries';
import { TwoFactorRecoveryCodes } from './two-factor-recovery-codes';
import { TwoFactorConfirm } from './two-factor-confirm';
import { TwoFactorQRCode } from './two-factor-qr-code';
import {
    TwoFactorVerifyForm,
    type TwoFactorVerifyFormValues,
} from './two-factor-verify-form';

type TwoFactorStep =
    | 'idle'
    | 'qr'
    | 'verify'
    | 'recovery-codes'
    | 'confirm-disable'
    | 'confirm-regenerate';

export function TwoFactorSettings() {
    const [step, setStep] = useState<TwoFactorStep>('idle');

    const [recoveryCodes, setRecoveryCodes] = useState<string[]>([]);

    const { data: status, isPending, isSuccess, error } = useGetTwoFactor();

    const {
        mutate: setup,
        data: setupData,
        isPending: isSettingUp,
    } = useSetupTwoFactor();

    const { mutate: enable, isPending: isEnabling } = useEnableTwoFactor();

    const { mutate: disable, isPending: isDisabling } = useDisableTwoFactor();

    const { mutate: generateRecoveryCodes, isPending: isGenerating } =
        useGenerateRecoveryCodes();

    const { mutate: forgetMachine, isPending: isForgetting } =
        useForgetTwoFactorMachine();

    if (isPending) {
        return <Skeleton className="h-9 w-full sm:w-32" />;
    }

    if (!isSuccess) {
        return <QueryError error={error} />;
    }

    function onSetup() {
        setup(undefined, {
            onSuccess: () => setStep('qr'),
            onError: (error) => toast.error(error.message),
        });
    }

    function onVerify({ code }: TwoFactorVerifyFormValues) {
        enable(
            { code },
            {
                onSuccess: (response) => {
                    toast.success('Two-factor authentication enabled');
                    setRecoveryCodes(response.recoveryCodes);
                    setStep('recovery-codes');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    function onDisable() {
        disable(undefined, {
            onSuccess: () => {
                toast.success('Two-factor authentication disabled');
                setStep('idle');
            },
            onError: (error) => toast.error(error.message),
        });
    }

    function onRegenerate() {
        generateRecoveryCodes(undefined, {
            onSuccess: (response) => {
                toast.success('Recovery codes regenerated');
                setRecoveryCodes(response.recoveryCodes);
                setStep('recovery-codes');
            },
            onError: (error) => toast.error(error.message),
        });
    }

    function onForgetMachine() {
        forgetMachine(undefined, {
            onSuccess: () =>
                toast.success('This browser will ask for a code next time'),
            onError: (error) => toast.error(error.message),
        });
    }

    switch (step) {
        case 'qr':
            return (
                setupData && (
                    <TwoFactorQRCode
                        sharedKey={setupData.sharedKey}
                        authenticatorUri={setupData.authenticatorUri}
                        onDone={() => setStep('verify')}
                        onCancel={() => setStep('idle')}
                    />
                )
            );

        case 'verify':
            return (
                <TwoFactorVerifyForm
                    loading={isEnabling}
                    onSubmit={onVerify}
                    onBack={() => setStep('qr')}
                />
            );

        case 'recovery-codes':
            return (
                <TwoFactorRecoveryCodes
                    codes={recoveryCodes}
                    onDone={() => {
                        setRecoveryCodes([]);
                        setStep('idle');
                    }}
                />
            );

        case 'confirm-disable':
            return (
                <TwoFactorConfirm
                    message="You'll no longer need a code from your authenticator app to sign in. Your authenticator app entry and recovery codes will stop working."
                    confirmLabel="Disable 2FA"
                    loadingLabel="Disabling..."
                    destructive
                    loading={isDisabling}
                    onConfirm={onDisable}
                    onCancel={() => setStep('idle')}
                />
            );

        case 'confirm-regenerate':
            return (
                <TwoFactorConfirm
                    message="Your existing recovery codes will stop working and be replaced with new ones."
                    confirmLabel="Regenerate Codes"
                    loadingLabel="Regenerating..."
                    loading={isGenerating}
                    onConfirm={onRegenerate}
                    onCancel={() => setStep('idle')}
                />
            );
    }

    if (!status.isEnabled) {
        return (
            <Button
                onClick={onSetup}
                className="w-full sm:w-auto"
                disabled={isSettingUp}
            >
                {isSettingUp ? 'Setting Up...' : 'Enable 2FA'}
            </Button>
        );
    }

    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                Enabled. You have {status.recoveryCodesLeft ?? 0} recovery codes
                left.
            </p>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button
                    variant="destructive"
                    onClick={() => setStep('confirm-disable')}
                >
                    Disable 2FA
                </Button>
                <Button
                    variant="outline"
                    onClick={() => setStep('confirm-regenerate')}
                >
                    Regenerate Recovery Codes
                </Button>
                {status.isMachineRemembered && (
                    <Button
                        variant="outline"
                        onClick={onForgetMachine}
                        disabled={isForgetting}
                    >
                        {isForgetting ? 'Forgetting...' : 'Forget This Browser'}
                    </Button>
                )}
            </div>
        </div>
    );
}
