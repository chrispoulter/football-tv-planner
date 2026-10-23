import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { FormInputField } from '@/components/form/form-input-field';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';

const schema = z.object({
    emailAddress: z.email('Invalid email address'),
});

export type ForgotPasswordFormValues = z.infer<typeof schema>;

interface ForgotPasswordFormProps {
    loading?: boolean;
    onSubmit: (values: ForgotPasswordFormValues) => void;
}

export function ForgotPasswordForm({
    loading,
    onSubmit,
}: ForgotPasswordFormProps) {
    const form = useForm<ForgotPasswordFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            emailAddress: '',
        },
    });

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <FormInputField
                    control={form.control}
                    name="emailAddress"
                    label="Email"
                    type="email"
                    placeholder="john@example.com"
                    maxLength={254}
                    autoComplete="username"
                    required
                />

                <Button type="submit" disabled={loading}>
                    {loading ? 'Sending...' : 'Send Reset Link'}
                </Button>
            </FieldGroup>
        </form>
    );
}
