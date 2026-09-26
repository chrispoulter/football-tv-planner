import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { FieldGroup } from '@/components/ui/field';
import { useSetPassword } from '../profile-queries';

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

type SetPasswordFormValues = z.infer<typeof schema>;

/**
 * For accounts created with Google, so they can also sign in with their email.
 */
export function SetPasswordForm() {
    const { mutate: setPassword, isPending } = useSetPassword();

    const form = useForm<SetPasswordFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            newPassword: '',
            confirmNewPassword: '',
        },
    });

    function onSubmit({ newPassword }: SetPasswordFormValues) {
        setPassword(
            { newPassword },
            {
                onSuccess: () =>
                    toast.success(
                        'Your password has been set. You can now also sign in with your email address.'
                    ),
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
            <FieldGroup>
                <p className="text-sm text-muted-foreground">
                    You currently sign in with a linked account. Add a password
                    to also sign in with your email address.
                </p>

                <TextField
                    control={form.control}
                    name="newPassword"
                    label="Password"
                    type="password"
                    placeholder="••••••••"
                    maxLength={50}
                    autoComplete="new-password"
                    required
                />

                <TextField
                    control={form.control}
                    name="confirmNewPassword"
                    label="Confirm password"
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
                        loadingText="Setting..."
                    >
                        Set Password
                    </LoadingButton>
                </div>
            </FieldGroup>
        </form>
    );
}
