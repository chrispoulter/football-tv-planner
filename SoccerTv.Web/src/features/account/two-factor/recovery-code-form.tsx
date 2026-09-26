import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';

const schema = z.object({
    recoveryCode: z
        .string({ message: 'Recovery Code must be a valid string' })
        .trim()
        .min(1, 'Recovery Code is a required field'),
});

export type RecoveryCodeFormValues = z.infer<typeof schema>;

interface RecoveryCodeFormProps {
    loading?: boolean;
    onSubmit: (values: RecoveryCodeFormValues) => void;
    onUseAuthenticator: () => void;
}

export function RecoveryCodeForm({
    loading,
    onSubmit,
    onUseAuthenticator,
}: RecoveryCodeFormProps) {
    const form = useForm<RecoveryCodeFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            recoveryCode: '',
        },
    });

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <TextField
                    control={form.control}
                    name="recoveryCode"
                    label="Recovery code"
                    placeholder="xxxxx-xxxxx"
                    maxLength={20}
                    autoComplete="off"
                    autoFocus
                    required
                    disabled={loading}
                />

                <LoadingButton type="submit" loading={loading}>
                    Verify
                </LoadingButton>
                <Button
                    type="button"
                    variant="link"
                    onClick={onUseAuthenticator}
                    disabled={loading}
                >
                    Use Authenticator App Instead
                </Button>
            </FieldGroup>
        </form>
    );
}
