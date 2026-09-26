import { useState } from 'react';
import { Link } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { useRegister } from '../account-queries';
import { GoogleButton } from '../components/google-button';
import { RegisterForm, type RegisterFormValues } from './register-form';
import { Card, CardContent } from '@/components/ui/card';

export function RegisterPage() {
    const [registeredEmail, setRegisteredEmail] = useState<string>();

    const { mutate: register, isPending: isSaving } = useRegister();

    function onSubmit({ emailAddress, password }: RegisterFormValues) {
        register(
            { email: emailAddress, password },
            {
                onSuccess: () => setRegisteredEmail(emailAddress),
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <div className="flex flex-1 items-center justify-center">
            <Card className="w-full max-w-md">
                <CardContent className="space-y-6">
                    <Metadata title="Register" />

                    {registeredEmail ? (
                        <div className="space-y-1">
                            <h1 className="text-2xl font-bold tracking-tight">
                                Check your email
                            </h1>
                            <p className="text-sm text-muted-foreground">
                                We have sent a confirmation link to{' '}
                                <strong>{registeredEmail}</strong>. Follow the
                                link to activate your account.
                            </p>
                        </div>
                    ) : (
                        <>
                            <div className="space-y-1">
                                <h1 className="text-2xl font-bold tracking-tight">
                                    Register
                                </h1>
                                <p className="text-sm text-muted-foreground">
                                    Register for a new account to access the
                                    full range of features available on this
                                    site.
                                </p>
                            </div>

                            <RegisterForm
                                loading={isSaving}
                                onSubmit={onSubmit}
                            />

                            <GoogleButton />
                        </>
                    )}

                    <p className="text-sm text-muted-foreground">
                        Already have an account?{' '}
                        <Link
                            to="/account/login"
                            className="underline underline-offset-4"
                        >
                            Log in now
                        </Link>
                    </p>
                </CardContent>
            </Card>
        </div>
    );
}
