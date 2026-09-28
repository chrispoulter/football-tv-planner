import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { FormCheckboxField } from '@/components/form/form-checkbox-field';
import { FormOtpField } from '@/components/form/form-otp-field';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';

const schema = z.object({
    code: z.string().length(6, 'Code must be 6 digits'),
    rememberMachine: z.boolean(),
});

export type AuthenticatorCodeFormValues = z.infer<typeof schema>;

interface AuthenticatorCodeFormProps {
    loading?: boolean;
    onSubmit: (values: AuthenticatorCodeFormValues) => void;
    onUseRecoveryCode: () => void;
}

export function AuthenticatorCodeForm({
    loading,
    onSubmit,
    onUseRecoveryCode,
}: AuthenticatorCodeFormProps) {
    const form = useForm<AuthenticatorCodeFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            code: '',
            rememberMachine: false,
        },
        // Focusing the invalid code input on submit stops its error showing
        shouldFocusError: false,
    });

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <FormOtpField
                    control={form.control}
                    name="code"
                    label="Authentication code"
                    autoFocus
                    className="items-center *:w-auto"
                />

                <FormCheckboxField
                    control={form.control}
                    name="rememberMachine"
                    label="Don't ask again on this browser"
                />

                <Button type="submit" disabled={loading}>
                    {loading ? 'Verifying...' : 'Verify'}
                </Button>
                <Button
                    type="button"
                    variant="link"
                    onClick={onUseRecoveryCode}
                >
                    Use Recovery Code Instead
                </Button>
            </FieldGroup>
        </form>
    );
}
