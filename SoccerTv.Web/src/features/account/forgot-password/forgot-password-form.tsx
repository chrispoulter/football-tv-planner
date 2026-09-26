import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { FieldGroup } from '@/components/ui/field';

const schema = z.object({
    emailAddress: z.email('Email Address must be a valid email'),
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
                <TextField
                    control={form.control}
                    name="emailAddress"
                    label="Email"
                    type="email"
                    placeholder="john@example.com"
                    maxLength={254}
                    autoComplete="username"
                    required
                    disabled={loading}
                />

                <LoadingButton type="submit" loading={loading}>
                    Send Reset Link
                </LoadingButton>
            </FieldGroup>
        </form>
    );
}
