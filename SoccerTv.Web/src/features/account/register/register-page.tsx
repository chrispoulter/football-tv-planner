import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { useLogin, useRegister } from '../account-queries';
import { GoogleButton } from '../components/google-button';
import { RegisterForm, type RegisterFormValues } from './register-form';
import { Card, CardContent } from '@/components/ui/card';

export function RegisterPage() {
    const navigate = useNavigate();

    const { mutate: register, isPending: isRegistering } = useRegister();

    const { mutate: login, isPending: isLoggingIn } = useLogin();

    // Log straight in, as the email doesn't need confirming first
    function onSubmit({ emailAddress, password }: RegisterFormValues) {
        const credentials = { email: emailAddress, password };

        register(credentials, {
            onSuccess: () =>
                login(
                    { ...credentials, rememberMe: false },
                    {
                        onSuccess: () => {
                            toast.success(
                                'Welcome! We have sent you an email to confirm your address.'
                            );
                            navigate('/');
                        },
                        onError: (error) => toast.error(error.message),
                    }
                ),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="flex flex-1 items-center justify-center">
            <Card className="w-full max-w-md">
                <CardContent className="space-y-6">
                    <Metadata title="Register" />

                    <div className="space-y-1">
                        <h1 className="text-2xl font-bold tracking-tight">
                            Register
                        </h1>
                        <p className="text-sm text-muted-foreground">
                            Register for a new account to access the full range
                            of features available on this site.
                        </p>
                    </div>

                    <RegisterForm
                        loading={isRegistering || isLoggingIn}
                        onSubmit={onSubmit}
                    />

                    <GoogleButton />

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
