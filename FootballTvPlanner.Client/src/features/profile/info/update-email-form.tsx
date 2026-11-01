import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { FormInputField } from '@/components/form/form-input-field';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';
import { useChangeEmail } from '../profile-queries';

const schema = z.object({
    newEmail: z
        .email('Invalid email address')
        .max(254, 'Email must be no more than 254 characters'),
});

type UpdateEmailFormValues = z.infer<typeof schema>;

export function UpdateEmailForm() {
    const { user } = useAuth();

    const {
        mutate: changeEmail,
        isPending,
        isSuccess,
        variables,
        reset,
    } = useChangeEmail();

    const email = user?.email ?? '';

    const form = useForm<UpdateEmailFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            newEmail: '',
        },
    });

    function onSubmit(values: UpdateEmailFormValues) {
        changeEmail(values, {
            onSuccess: () => form.reset(),
            onError: (error) => toast.error(error.message),
        });
    }

    if (isSuccess) {
        return (
            <div className="space-y-4">
                <p className="text-sm text-muted-foreground">
                    Current email:{' '}
                    <span className="font-medium break-all text-foreground">
                        {email}
                    </span>
                </p>
                <Alert>
                    <AlertDescription>
                        <p>
                            A verification link has been sent to{' '}
                            <strong className="break-all">
                                {variables.newEmail}
                            </strong>
                            .
                        </p>
                        <p>
                            Click the link in that email to confirm the change.
                            Your current email remains active until then.
                        </p>
                    </AlertDescription>
                </Alert>
                <Button
                    variant="outline"
                    onClick={reset}
                    className="w-full sm:w-auto"
                >
                    Use a Different Email
                </Button>
            </div>
        );
    }

    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                Current email:{' '}
                <span className="font-medium break-all text-foreground">
                    {email}
                </span>
            </p>
            <form noValidate onSubmit={form.handleSubmit(onSubmit)}>
                <FieldGroup>
                    <FormInputField
                        control={form.control}
                        name="newEmail"
                        label="New email address"
                        type="email"
                        placeholder="new@example.com"
                        maxLength={254}
                        autoComplete="email"
                        required
                    />

                    <div className="flex flex-col-reverse gap-2 sm:flex-row">
                        <Button type="submit" disabled={isPending}>
                            {isPending
                                ? 'Sending Verification...'
                                : 'Update Email'}
                        </Button>
                    </div>
                </FieldGroup>
            </form>
        </div>
    );
}
