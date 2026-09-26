import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { FieldGroup } from '@/components/ui/field';

const schema = z
    .object({
        newPassword: z
            .string()
            .min(8, 'Password must be at least 8 characters')
            .max(50, 'Password must be no more than 50 characters'),
        confirmNewPassword: z.string(),
    })
    .refine((data) => data.newPassword === data.confirmNewPassword, {
        message: 'Passwords do not match',
        path: ['confirmNewPassword'],
    });

export type ResetPasswordFormValues = z.infer<typeof schema>;

interface ResetPasswordFormProps {
    loading?: boolean;
    onSubmit: (values: ResetPasswordFormValues) => void;
}

export function ResetPasswordForm({
    loading,
    onSubmit,
}: ResetPasswordFormProps) {
    const form = useForm<ResetPasswordFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            newPassword: '',
            confirmNewPassword: '',
        },
    });

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <TextField
                    control={form.control}
                    name="newPassword"
                    label="New password"
                    type="password"
                    placeholder="••••••••"
                    maxLength={50}
                    autoComplete="new-password"
                    required
                    disabled={loading}
                />

                <TextField
                    control={form.control}
                    name="confirmNewPassword"
                    label="Confirm new password"
                    type="password"
                    placeholder="••••••••"
                    maxLength={50}
                    autoComplete="new-password"
                    required
                    disabled={loading}
                />

                <LoadingButton
                    type="submit"
                    loading={loading}
                    loadingText="Resetting..."
                >
                    Reset Password
                </LoadingButton>
            </FieldGroup>
        </form>
    );
}
