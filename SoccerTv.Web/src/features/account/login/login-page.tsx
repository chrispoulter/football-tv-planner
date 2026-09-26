import { useState } from 'react';
import { Link, useNavigate, useSearchParams } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { Card, CardContent } from '@/components/ui/card';
import { getProblemDetail } from '@/lib/api-client';
import { type LoginRequest, useLogin } from '../account-queries';
import { GoogleButton } from '../components/google-button';
import { LoginForm, type LoginFormValues } from './login-form';
import { type TwoFactorCode, TwoFactorForm } from './two-factor-form';

// Identity reports why a login failed in the problem detail
const loginErrors: Record<string, string> = {
    Failed: 'The credentials provided were invalid.',
    LockedOut: 'This account has been locked out, please try again later.',
};

// Set by the API when a Google login fails
const externalLoginErrors: Record<string, string> = {
    external: 'Unable to log in with Google, please try again.',
    locked: loginErrors.LockedOut,
};

export function LoginPage() {
    const navigate = useNavigate();

    const [searchParams] = useSearchParams();

    const [credentials, setCredentials] = useState<LoginRequest>();

    const { mutate: login, isPending: isSaving } = useLogin();

    const externalError = externalLoginErrors[searchParams.get('error') ?? ''];

    function submit(request: LoginRequest) {
        login(request, {
            onSuccess: () => navigate('/'),
            onError: (error) => {
                const detail = getProblemDetail(error) ?? '';

                if (detail === 'RequiresTwoFactor') {
                    setCredentials(request);
                    return;
                }

                toast.error(
                    credentials && detail === 'Failed'
                        ? 'The code provided was invalid.'
                        : (loginErrors[detail] ?? error.message)
                );
            },
        });
    }

    function onSubmit({ emailAddress, ...values }: LoginFormValues) {
        submit({ email: emailAddress, ...values });
    }

    function onTwoFactorSubmit(code: TwoFactorCode) {
        if (credentials) {
            submit({ ...credentials, ...code });
        }
    }

    return (
        <div className="flex flex-1 items-center justify-center">
            <Card className="w-full max-w-md">
                <CardContent className="space-y-6">
                    <Metadata title="Login" />

                    {credentials ? (
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
