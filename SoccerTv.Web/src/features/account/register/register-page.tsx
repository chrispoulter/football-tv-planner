import { useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Metadata } from '@/components/metadata';
import { useRegister } from '../account-queries';
import { AccountLayout, AccountLink } from '../components/account-layout';
import { SocialLoginButtons } from '../components/social-login-buttons';
import { RegisterForm, type RegisterFormValues } from './register-form';

export function RegisterPage() {
    const navigate = useNavigate();

    const { mutate: register, isPending } = useRegister();

    // Logs straight in, as the email doesn't need confirming first
    function onSubmit({ name, emailAddress, password }: RegisterFormValues) {
        register(
            { name, email: emailAddress, password },
            {
                onSuccess: () => {
                    toast.success(
                        'Welcome! We have sent you an email to confirm your address.'
                    );
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
                    <AccountLink to="/account/login">Sign in</AccountLink>
                </>
            }
        >
            <Metadata title="Register" />

            <SocialLoginButtons />

            <RegisterForm loading={isPending} onSubmit={onSubmit} />
        </AccountLayout>
    );
}
