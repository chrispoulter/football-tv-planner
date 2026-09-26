import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { Button } from '@/components/ui/button';

const schema = z.object({
    code: z
        .string({ message: 'Code must be a valid string' })
        .trim()
        .min(1, 'Code is a required field'),
});

type TwoFactorFormValues = z.infer<typeof schema>;

export interface TwoFactorCode {
    code?: string;
    recoveryCode?: string;
}

interface TwoFactorFormProps {
    loading?: boolean;
    onSubmit: (values: TwoFactorCode) => void;
}

export function TwoFactorForm({ loading, onSubmit }: TwoFactorFormProps) {
    const [useRecoveryCode, setUseRecoveryCode] = useState(false);

    const form = useForm<TwoFactorFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            code: '',
        },
    });

    function onValid({ code }: TwoFactorFormValues) {
        onSubmit(useRecoveryCode ? { recoveryCode: code } : { code });
    }

    function onToggle() {
        setUseRecoveryCode(!useRecoveryCode);
        form.reset();
    }

    return (
        <form
            noValidate
            onSubmit={form.handleSubmit(onValid)}
            className="space-y-6"
        >
            {useRecoveryCode ? (
                <TextField
                    key="recovery-code"
                    control={form.control}
                    name="code"
                    label="Recovery Code"
                    autoComplete="off"
                    required
                    disabled={loading}
                />
            ) : (
                <TextField
                    key="authenticator-code"
                    control={form.control}
                    name="code"
                    label="Authenticator Code"
                    inputMode="numeric"
                    maxLength={6}
                    autoComplete="one-time-code"
                    autoFocus
                    required
                    disabled={loading}
                />
            )}

            <div className="flex flex-col-reverse justify-between gap-2 sm:flex-row">
                <Button
                    type="button"
                    variant="link"
                    className="px-0"
                    onClick={onToggle}
                    disabled={loading}
                >
                    {useRecoveryCode
                        ? 'Use your authenticator app'
                        : 'Use a recovery code'}
                </Button>

                <LoadingButton type="submit" loading={loading}>
                    Verify
                </LoadingButton>
            </div>
        </form>
    );
}
