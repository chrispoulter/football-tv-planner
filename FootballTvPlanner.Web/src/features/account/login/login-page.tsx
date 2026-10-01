import { useLocation, useNavigate, useSearchParams } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { returnUrl } from '@/lib/return-url';
import { useLogin } from '../account-queries';
import { AccountLayout, AccountLink } from '../components/account-layout';
import { SocialLoginButtons } from '../components/social-login-buttons';
import type { TwoFactorState } from '../two-factor/two-factor-state';
import { LoginForm, type LoginFormValues } from './login-form';

const externalLoginErrors: Record<string, string> = {
    external: 'Unable to sign in, please try again.',
    locked: 'This account has been locked out, please try again later.',
};

export function LoginPage() {
    const navigate = useNavigate();

    const location = useLocation();

    const [searchParams] = useSearchParams();

    const { mutate: login, isPending } = useLogin();

    const from = returnUrl(location.state);

    const externalError = externalLoginErrors[searchParams.get('error') ?? ''];

    function onSubmit({ emailAddress, ...values }: LoginFormValues) {
        login(
            { email: emailAddress, ...values },
            {
                onSuccess: ({ requiresTwoFactor }) => {
                    if (requiresTwoFactor) {
                        const state: TwoFactorState = {
                            rememberMe: values.rememberMe,
                            from,
                        };

                        navigate('/two-factor', { state });
                        return;
                    }

                    navigate(from, { replace: true });
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <AccountLayout
            title="Welcome Back"
            description="Sign in to your account"
            className="max-w-md"
            footer={
                <>
                    Don&apos;t have an account?{' '}
                    <AccountLink to="/register">Sign up</AccountLink>
                </>
            }
        >
            <Metadata title="Sign In" />

            {externalError && (
                <Alert variant="destructive">
                    <AlertDescription>{externalError}</AlertDescription>
                </Alert>
            )}

            <SocialLoginButtons returnUrl={from} />

            <LoginForm loading={isPending} onSubmit={onSubmit} />
        </AccountLayout>
    );
}
