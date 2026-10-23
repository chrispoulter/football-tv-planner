import { useNavigate, Link } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { Separator } from '@/components/ui/separator';
import { useRegister } from '../account-queries';
import { AccountLayout } from '../account-layout';
import { SocialLoginButtons } from '../social-login-buttons';
import { RegisterForm, type RegisterFormValues } from './register-form';

export function RegisterPage() {
    const navigate = useNavigate();

    const { mutate: register, isPending } = useRegister();

    function onSubmit({ name, emailAddress, password }: RegisterFormValues) {
        register(
            { name, email: emailAddress, password },
            {
                onSuccess: () => {
                    toast.success('Account created! Welcome.');
                    navigate('/');
                },
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <AccountLayout
            title="Create an Account"
            description="Enter your details to get started"
            className="max-w-md"
            footer={
                <>
                    Already have an account?{' '}
                    <Link
                        className="underline underline-offset-4 hover:text-foreground"
                        to="/login"
                    >
                        Sign in
                    </Link>
                </>
            }
        >
            <Metadata title="Register" />

            <SocialLoginButtons />

            <div className="relative">
                <Separator />
                <span className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 bg-card px-2 text-xs text-muted-foreground">
                    or
                </span>
            </div>

            <RegisterForm loading={isPending} onSubmit={onSubmit} />
        </AccountLayout>
    );
}
