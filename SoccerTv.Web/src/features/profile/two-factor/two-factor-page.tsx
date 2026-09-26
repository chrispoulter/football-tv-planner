import { useState } from 'react';
import { Link } from 'react-router';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { QRCodeSVG } from 'qrcode.react';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { Metadata } from '@/components/metadata';
import { QueryError } from '@/components/query-error';
import { Button } from '@/components/ui/button';
import {
    Card,
    CardContent,
    CardDescription,
    CardFooter,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { ProfileLoading } from '../profile/profile-loading';
import {
    useDisableTwoFactor,
    useEnableTwoFactor,
    useForgetTwoFactorMachine,
    useGenerateRecoveryCodes,
    useGetTwoFactor,
    useSetupTwoFactor,
} from '../profile-queries';
import { RecoveryCodes } from './recovery-codes';

const schema = z.object({
    code: z
        .string({ message: 'Code must be a valid string' })
        .trim()
        .min(1, 'Code is a required field'),
});

type VerifyFormValues = z.infer<typeof schema>;

// Group the key into blocks of four so it's easier to type
function formatKey(sharedKey: string) {
    return (
        sharedKey
            .toLowerCase()
            .match(/.{1,4}/g)
            ?.join(' ') ?? sharedKey
    );
}

export function TwoFactorPage() {
    const [recoveryCodes, setRecoveryCodes] = useState<string[]>();

    const { data: status, isPending, isSuccess, error } = useGetTwoFactor();

    const isEnabled = !!status?.isEnabled;

    const setup = useSetupTwoFactor(isSuccess && !isEnabled);

    const { mutate: enable, isPending: isEnabling } = useEnableTwoFactor();

    const { mutate: disable, isPending: isDisabling } = useDisableTwoFactor();

    const { mutate: generateRecoveryCodes, isPending: isGenerating } =
        useGenerateRecoveryCodes();

    const { mutate: forgetMachine, isPending: isForgetting } =
        useForgetTwoFactorMachine();

    const isSaving = isEnabling || isDisabling || isGenerating || isForgetting;

    const form = useForm<VerifyFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            code: '',
        },
    });

    if (isPending || (!isEnabled && setup.isPending)) {
        return <ProfileLoading />;
    }

    if (!isSuccess || (!isEnabled && !setup.isSuccess)) {
        return <QueryError error={error ?? setup.error} />;
    }

    function onEnable({ code }: VerifyFormValues) {
        enable(
            { code },
            {
                onSuccess: (response) => {
                    toast.success(
                        'Two-factor authentication has been enabled.'
                    );
                    setRecoveryCodes(response.recoveryCodes);
                    form.reset();
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    function onGenerateRecoveryCodes() {
        generateRecoveryCodes(undefined, {
            onSuccess: (response) => {
                toast.success('New recovery codes have been generated.');
                setRecoveryCodes(response.recoveryCodes);
            },
            onError: (error) => toast.error(error.message),
        });
    }

    function onForgetMachine() {
        forgetMachine(undefined, {
            onSuccess: () =>
                toast.success('This browser will ask for a code next time.'),
            onError: (error) => toast.error(error.message),
        });
    }

    function onDisable() {
        disable(undefined, {
            onSuccess: () => {
                toast.success('Two-factor authentication has been disabled.');
                setRecoveryCodes(undefined);
                // Re-enabling starts again with a new key
                setup.refetch();
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="mx-auto w-full max-w-2xl space-y-6">
            <Metadata title="Two-Factor Authentication" />

            <Card>
                <CardHeader>
                    <CardTitle className="text-2xl">
                        Two-Factor Authentication
                    </CardTitle>
                    <CardDescription>
                        {isEnabled
                            ? `Two-factor authentication is enabled. You have ${status.recoveryCodesLeft ?? 0} recovery codes left.`
                            : 'Protect your account by requiring a code from an authenticator app when you log in with your password.'}
                    </CardDescription>
                </CardHeader>

                <CardContent className="space-y-6">
                    {recoveryCodes && <RecoveryCodes codes={recoveryCodes} />}

                    {!isEnabled && setup.data && (
                        <>
                            <ol className="list-decimal space-y-2 pl-5 text-sm text-muted-foreground">
                                <li>
                                    Install an authenticator app such as Google
                                    Authenticator, Microsoft Authenticator or
                                    1Password.
                                </li>
                                <li>
                                    Scan the QR code, or enter the key{' '}
                                    <code className="rounded bg-muted px-1 font-mono">
                                        {formatKey(setup.data.sharedKey)}
                                    </code>
                                </li>
                                <li>Enter the 6-digit code from the app.</li>
                            </ol>

                            <div className="w-fit rounded-md bg-white p-4">
                                <QRCodeSVG
                                    value={setup.data.authenticatorUri}
                                    size={176}
                                />
                            </div>

                            <form
                                noValidate
                                onSubmit={form.handleSubmit(onEnable)}
                                className="space-y-6"
                            >
                                <TextField
                                    control={form.control}
                                    name="code"
                                    label="Verification Code"
                                    inputMode="numeric"
                                    maxLength={6}
                                    autoComplete="one-time-code"
                                    required
                                    disabled={isSaving}
                                />

                                <div className="flex flex-col-reverse justify-end gap-2 sm:flex-row">
                                    <Button asChild variant="outline">
                                        <Link to="/profile">Cancel</Link>
                                    </Button>

                                    <LoadingButton
                                        type="submit"
                                        loading={isSaving}
                                    >
                                        Enable
                                    </LoadingButton>
                                </div>
                            </form>
                        </>
                    )}
                </CardContent>

                {isEnabled && (
                    <CardFooter className="flex flex-col gap-2 sm:flex-row">
                        <Button
                            variant="outline"
                            disabled={isSaving}
                            onClick={onGenerateRecoveryCodes}
                        >
                            New Recovery Codes
                        </Button>

                        {status.isMachineRemembered && (
                            <Button
                                variant="outline"
                                disabled={isSaving}
                                onClick={onForgetMachine}
                            >
                                Forget This Browser
                            </Button>
                        )}

                        <Button
                            variant="destructive"
                            disabled={isSaving}
                            onClick={onDisable}
                        >
                            Disable
                        </Button>
                    </CardFooter>
                )}
            </Card>

            <Button asChild variant="link" className="px-0">
                <Link to="/profile">Back to my account</Link>
            </Button>
        </div>
    );
}
