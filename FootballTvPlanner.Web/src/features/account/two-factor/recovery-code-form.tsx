import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { FormInputField } from '@/components/form/form-input-field';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';

const schema = z.object({
    recoveryCode: z.string().trim().min(1, 'Recovery code is required'),
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
                <FormInputField
                    control={form.control}
                    name="recoveryCode"
                    label="Recovery code"
                    placeholder="xxxxx-xxxxx"
                    maxLength={20}
                    autoComplete="off"
                    autoFocus
                    required
                />

                <Button type="submit" disabled={loading}>
                    {loading ? 'Verifying...' : 'Verify'}
                </Button>
                <Button
                    type="button"
                    variant="link"
                    onClick={onUseAuthenticator}
                >
                    Use Authenticator App Instead
                </Button>
            </FieldGroup>
        </form>
    );
}
