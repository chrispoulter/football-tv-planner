import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { FieldGroup } from '@/components/ui/field';
import { useChangePassword } from '../profile-queries';

const schema = z
    .object({
        currentPassword: z.string().min(1, 'Current password is required'),
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

type ChangePasswordFormValues = z.infer<typeof schema>;

export function ChangePasswordForm() {
    const { mutate: changePassword, isPending } = useChangePassword();

    const form = useForm<ChangePasswordFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            currentPassword: '',
            newPassword: '',
            confirmNewPassword: '',
        },
    });

    function onSubmit({
        currentPassword,
        newPassword,
    }: ChangePasswordFormValues) {
        changePassword(
            { currentPassword, newPassword },
            {
                onSuccess: () => {
                    toast.success('Password changed successfully');
                    form.reset();
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <TextField
                    control={form.control}
                    name="currentPassword"
                    label="Current password"
                    type="password"
                    placeholder="••••••••"
                    maxLength={50}
                    autoComplete="current-password"
                    required
                />

                <TextField
                    control={form.control}
                    name="newPassword"
                    label="New password"
                    type="password"
                    placeholder="••••••••"
                    maxLength={50}
                    autoComplete="new-password"
                    required
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
                />

                <div className="flex flex-col-reverse gap-2 sm:flex-row">
                    <LoadingButton
                        type="submit"
                        loading={isPending}
                        loadingText="Changing..."
                    >
                        Change Password
                    </LoadingButton>
                </div>
            </FieldGroup>
        </form>
    );
}
