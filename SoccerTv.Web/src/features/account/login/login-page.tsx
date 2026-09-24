import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import { useLogin } from '../account-queries';
import { LoginForm, type LoginFormValues } from './login-form';
import { Card, CardContent } from '@/components/ui/card';

export function LoginPage() {
    const navigate = useNavigate();

    const { setAuth } = useAuth();

    const { mutate: login, isPending: isSaving } = useLogin();

    function onSubmit(values: LoginFormValues) {
        login(values, {
            onSuccess: (data) => {
                setAuth(data.accessToken);
                navigate('/');
            },
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="flex flex-1 items-center justify-center">
            <Card className="w-full max-w-md">
                <CardContent className="space-y-6">
                    <Metadata title="Login" />

                    <div className="space-y-1">
                        <h1 className="text-2xl font-bold tracking-tight">
                            Login
                        </h1>
                        <p className="text-sm text-muted-foreground">
                            Enter your email address below to login to your
                            account.
                        </p>
                    </div>

                    <LoginForm loading={isSaving} onSubmit={onSubmit} />

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
                </CardContent>
            </Card>
        </div>
    );
}
