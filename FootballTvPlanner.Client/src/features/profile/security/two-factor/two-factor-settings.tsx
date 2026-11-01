import { useState } from 'react';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Skeleton } from '@/components/ui/skeleton';
import { useGetTwoFactor } from '../../profile-queries';
import { TwoFactorConfirmDisable } from './two-factor-confirm-disable';
import { TwoFactorConfirmRecoveryCodes } from './two-factor-confirm-recovery-codes';
import { TwoFactorDisabled } from './two-factor-disabled';
import { TwoFactorEnabled } from './two-factor-enabled';
import { TwoFactorQrCode } from './two-factor-qr-code';
import { TwoFactorRecoveryCodes } from './two-factor-recovery-codes';
import { TwoFactorVerify } from './two-factor-verify';

type TwoFactorStep =
    | 'idle'
    | 'qr'
    | 'verify'
    | 'confirm-disable'
    | 'recovery-codes'
    | 'confirm-recovery-codes';

interface TotpSetup {
    sharedKey: string;
    authenticatorUri: string;
}

export function TwoFactorSettings() {
    const [step, setStep] = useState<TwoFactorStep>('idle');
    const [totpSetup, setTotpSetup] = useState<TotpSetup>();
    const [recoveryCodes, setRecoveryCodes] = useState<string[]>([]);

    const { data: status, isPending, isSuccess } = useGetTwoFactor();

    if (isPending) {
        return <Skeleton className="h-9 w-full sm:w-32" />;
    }

    if (!isSuccess) {
        return (
            <Alert variant="destructive">
                <AlertTitle>Error</AlertTitle>
                <AlertDescription>
                    Failed to load two-factor settings. Please try again.
                </AlertDescription>
            </Alert>
        );
    }

    function onRecoveryCodes(codes: string[]) {
        setRecoveryCodes(codes);
        setStep('recovery-codes');
    }

    function onRecoveryCodesDone() {
        setRecoveryCodes([]);
        setStep('idle');
    }

    if (status.isEnabled) {
        return (
            <div className="space-y-4">
                {step === 'idle' && (
                    <TwoFactorEnabled
                        recoveryCodesLeft={status.recoveryCodesLeft ?? 0}
                        onDisable={() => setStep('confirm-disable')}
                        onGenerateRecoveryCodes={() =>
                            setStep('confirm-recovery-codes')
                        }
                    />
                )}

                {step === 'confirm-recovery-codes' && (
                    <TwoFactorConfirmRecoveryCodes
                        onRegenerated={onRecoveryCodes}
                        onCancel={() => setStep('idle')}
                    />
                )}

                {step === 'recovery-codes' && (
                    <TwoFactorRecoveryCodes
                        recoveryCodes={recoveryCodes}
                        onDone={onRecoveryCodesDone}
                    />
                )}

                {step === 'confirm-disable' && (
                    <TwoFactorConfirmDisable
                        onDisabled={() => setStep('idle')}
                        onCancel={() => setStep('idle')}
                    />
                )}
            </div>
        );
    }

    return (
        <div className="space-y-4">
            {step === 'idle' && (
                <TwoFactorDisabled
                    onTotpSetup={(sharedKey, authenticatorUri) => {
                        setTotpSetup({ sharedKey, authenticatorUri });
                        setStep('qr');
                    }}
                />
            )}

            {step === 'qr' && totpSetup && (
                <TwoFactorQrCode
                    sharedKey={totpSetup.sharedKey}
                    authenticatorUri={totpSetup.authenticatorUri}
                    onDone={() => setStep('verify')}
                    onCancel={() => setStep('idle')}
                />
            )}

            {step === 'verify' && (
                <TwoFactorVerify
                    onSuccess={onRecoveryCodes}
                    onCancel={() => setStep('qr')}
                />
            )}

            {step === 'recovery-codes' && (
                <TwoFactorRecoveryCodes
                    recoveryCodes={recoveryCodes}
                    onDone={onRecoveryCodesDone}
                />
            )}
        </div>
    );
}
