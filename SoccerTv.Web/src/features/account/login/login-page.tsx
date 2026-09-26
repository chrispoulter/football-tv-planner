import { useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { Card, CardContent } from '@/components/ui/card';
import { useLogin, useLoginTwoFactor } from '../account-queries';
import { GoogleButton } from '../components/google-button';
import { LoginForm, type LoginFormValues } from './login-form';
import { type TwoFactorCode, TwoFactorForm } from './two-factor-form';

// Set by the API when a Google login fails
const externalLoginErrors: Record<string, string> = {
    external: 'Unable to log in with Google, please try again.',
    locked: 'This account has been locked out, please try again later.',
};

export function LoginPage() {
    const navigate = useNavigate();

    const [searchParams] = useSearchParams();

    // Set once the password is accepted but a two-factor code is still needed
    const [twoFactor, setTwoFactor] = useState<{ rememberMe: boolean }>();

    const { mutate: login, isPending: isLoggingIn } = useLogin();

    const { mutate: loginTwoFactor, isPending: isVerifying } =
        useLoginTwoFactor();

    const isSaving = isLoggingIn || isVerifying;

    const externalError = externalLoginErrors[searchParams.get('error') ?? ''];

    function onSubmit({ emailAddress, ...values }: LoginFormValues) {
        login(
            { email: emailAddress, ...values },
            {
                onSuccess: ({ requiresTwoFactor }) => {
                    if (requiresTwoFactor) {
                        setTwoFactor({ rememberMe: values.rememberMe });
                        return;
                    }

                    navigate('/');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    function onTwoFactorSubmit(code: TwoFactorCode) {
        if (twoFactor) {
            loginTwoFactor(
                { ...twoFactor, ...code },
                {
                    onSuccess: () => navigate('/'),
                    onError: (error) => toast.error(error.message),
                }
            );
        }
    }

    return (
        <div className="flex flex-1 items-center justify-center">
            <Card className="w-full max-w-md">
                <CardContent className="space-y-6">
                    <Metadata title="Login" />

                    {twoFactor ? (
                        <>
                            <div className="space-y-1">
                                <h1 className="text-2xl font-bold tracking-tight">
                                    Two-Factor Authentication
                                </h1>
                                <p className="text-sm text-muted-foreground">
                                    Enter the code from your authenticator app,
                                    or one of your recovery codes.
                                </p>
                            </div>

                            <TwoFactorForm
                                loading={isSaving}
                                onSubmit={onTwoFactorSubmit}
                            />
                        </>
                    ) : (
                        <>
                            <div className="space-y-1">
                                <h1 className="text-2xl font-bold tracking-tight">
                                    Login
                                </h1>
                                <p className="text-sm text-muted-foreground">
                                    Enter your email address below to login to
                                    your account.
                                </p>
                            </div>

                            {externalError && (
                                <p className="text-sm text-destructive">
                                    {externalError}
                                </p>
                            )}

                            <LoginForm loading={isSaving} onSubmit={onSubmit} />

                            <GoogleButton />

                            <div className="space-y-2">
                                <p className="text-sm text-muted-foreground">
                                    Not already a member?{' '}
                                    <Link
                                        to="/account/register"
                                        className="underline underline-offset-4"
                                    >
                                        Register now
                                    </Link>
                                </p>
                                <p className="text-sm text-muted-foreground">
                                    Forgotten your password?{' '}
                                    <Link
                                        to="/account/forgot-password"
                                        className="underline underline-offset-4"
                                    >
                                        Request reset
                                    </Link>
                                </p>
                            </div>
                        </>
                    )}
                </CardContent>
            </Card>
        </div>
    );
}
