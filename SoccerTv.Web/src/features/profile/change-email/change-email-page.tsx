import { useState } from 'react';
import { Link } from 'react-router';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { TextField } from '@/components/form/text-field';
import { LoadingButton } from '@/components/loading-button';
import { Metadata } from '@/components/metadata';
import { Button } from '@/components/ui/button';
import {
    Card,
    CardContent,
    CardDescription,
    CardHeader,
    CardTitle,
} from '@/components/ui/card';
import { useChangeEmail } from '../profile-queries';

const schema = z.object({
    newEmail: z.email('New Email Address must be a valid email'),
});

type ChangeEmailFormValues = z.infer<typeof schema>;

export function ChangeEmailPage() {
    const { user } = useAuth();

    const [sentTo, setSentTo] = useState<string>();

    const { mutate: changeEmail, isPending: isSaving } = useChangeEmail();

    const form = useForm<ChangeEmailFormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            newEmail: '',
        },
    });

    function onSubmit(values: ChangeEmailFormValues) {
        changeEmail(values, {
            onSuccess: () => setSentTo(values.newEmail),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="mx-auto w-full max-w-2xl space-y-6">
            <Metadata title="Change Email" />

            <Card>
                <CardHeader>
                    <CardTitle className="text-2xl">Change Email</CardTitle>
                    <CardDescription>
                        {sentTo ? (
                            <>
                                We have sent a confirmation link to{' '}
                                <strong>{sentTo}</strong>. Your email address
                                will change once you follow the link.
                            </>
                        ) : (
                            <>
                                Your email address is currently{' '}
                                <strong>{user?.email}</strong>. It is used to
                                log in to your account.
                            </>
                        )}
                    </CardDescription>
                </CardHeader>
                <CardContent>
                    {sentTo ? (
                        <Button asChild variant="outline">
                            <Link to="/profile">Back to my account</Link>
                        </Button>
                    ) : (
                        <form
                            noValidate
                            onSubmit={form.handleSubmit(onSubmit)}
                            className="space-y-6"
                        >
                            <TextField
                                control={form.control}
                                name="newEmail"
                                label="New Email Address"
                                type="email"
                                maxLength={254}
                                autoComplete="email"
                                required
                                disabled={isSaving}
                            />

                            <div className="flex flex-col-reverse justify-end gap-2 sm:flex-row">
                                <Button asChild variant="outline">
                                    <Link to="/profile">Cancel</Link>
                                </Button>

                                <LoadingButton type="submit" loading={isSaving}>
                                    Submit
                                </LoadingButton>
                            </div>
                        </form>
                    )}
                </CardContent>
            </Card>
        </div>
    );
}
