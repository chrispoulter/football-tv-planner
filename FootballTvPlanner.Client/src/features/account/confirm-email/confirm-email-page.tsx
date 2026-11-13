import { useEffect } from 'react';
import { useSearchParams, Link } from 'react-router';
import { useQueryClient } from '@tanstack/react-query';
import { sessionKeys, useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import { PageLoading } from '@/components/page-loading';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { useConfirmEmail } from '../account-queries';
import { AccountLayout } from '../account-layout';

export function ConfirmEmailPage() {
    const [searchParams] = useSearchParams();
    const changedEmail = searchParams.get('changedEmail') ?? undefined;
    const userId = searchParams.get('userId') ?? '';
    const code = searchParams.get('code') ?? '';

    const queryClient = useQueryClient();

    const { user } = useAuth();

    const { isPending, isSuccess } = useConfirmEmail({
        userId,
        code,
        changedEmail,
    });

    useEffect(() => {
        if (isSuccess) {
            queryClient.invalidateQueries({ queryKey: sessionKeys.all });
        }
    }, [isSuccess, queryClient]);

    let title: string, message: string;

    if (isPending) {
        title = 'Confirming Email';
        message = 'Just a moment…';
    } else if (isSuccess) {
        if (changedEmail) {
            title = 'Email Changed';
            message = `Your email address is now ${changedEmail}.`;
        } else {
            title = 'Email Confirmed';
            message = 'Thank you for confirming your email address.';
        }
    } else {
        title = 'Unable to Confirm Email';
        message =
            'This link is invalid or has expired. Please request a new one.';
    }

    return (
        <AccountLayout
            title={title}
            footer={
                !isPending && (
                    <Link
                        className="underline underline-offset-4 hover:text-foreground"
                        to={user ? '/profile' : '/login'}
                    >
                        {user ? 'Back to profile' : 'Sign in'}
                    </Link>
                )
            }
        >
            <Metadata title="Confirm Email" />

            {isPending ? (
                <PageLoading />
            ) : (
                <Alert variant={isSuccess ? 'default' : 'destructive'}>
                    <AlertDescription>{message}</AlertDescription>
                </Alert>
            )}
        </AccountLayout>
    );
}
