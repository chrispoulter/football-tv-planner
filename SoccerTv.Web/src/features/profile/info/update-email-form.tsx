import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { FormInputField } from '@/components/form/form-input-field';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { FieldGroup } from '@/components/ui/field';
import { useChangeEmail } from '../profile-queries';

const schema = z.object({
    newEmail: z.email('Invalid email address'),
});

type UpdateEmailFormValues = z.infer<typeof schema>;

export function UpdateEmailForm() {
    const [pendingEmail, setPendingEmail] = useState<string>();

    const { mutate: changeEmail, isPending } = useChangeEmail();

    const form = useForm<UpdateEmailFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            newEmail: '',
        },
    });

    function onSubmit(values: UpdateEmailFormValues) {
        changeEmail(values, {
            onSuccess: () => {
                setPendingEmail(values.newEmail);
                form.reset();
            },
            onError: (error) => toast.error(error.message),
        });
    }

    if (pendingEmail) {
        return (
            <div className="space-y-4">
                <Alert>
                    <AlertDescription>
                        <p>
                            A confirmation link has been sent to{' '}
                            <strong className="break-all">
                                {pendingEmail}
                            </strong>
                            .
                        </p>
                        <p>
                            Follow the link in that email to confirm the change.
                            Your current email remains active until then.
                        </p>
                    </AlertDescription>
                </Alert>
                <Button
                    variant="outline"
                    onClick={() => setPendingEmail(undefined)}
                    className="w-full sm:w-auto"
                >
                    Use a Different Email
                </Button>
            </div>
        );
    }

    return (
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
                        {isPending ? 'Sending Verification...' : 'Update Email'}
                    </Button>
                </div>
            </FieldGroup>
        </form>
    );
}
