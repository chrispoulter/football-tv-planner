import { useEffect } from 'react';
import { useSearchParams } from 'react-router';
import { useQueryClient } from '@tanstack/react-query';
import { sessionKeys, useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Spinner } from '@/components/ui/spinner';
import { useConfirmEmail } from '../account-queries';
import { AccountLayout, AccountLink } from '../components/account-layout';

export function ConfirmEmailPage() {
    const [searchParams] = useSearchParams();

    const changedEmail = searchParams.get('changedEmail') ?? undefined;

    const queryClient = useQueryClient();

    const { user } = useAuth();

    const { isPending, isSuccess } = useConfirmEmail({
        userId: searchParams.get('userId') ?? '',
        code: searchParams.get('code') ?? '',
        changedEmail,
    });

    // Pick up the confirmed or changed email address
    useEffect(() => {
        if (isSuccess) {
            queryClient.invalidateQueries({ queryKey: sessionKeys.all });
        }
    }, [isSuccess, queryClient]);

    const [title, message] = isPending
        ? ['Confirming Email', 'Just a moment…']
        : isSuccess
          ? changedEmail
              ? ['Email Changed', `Your email address is now ${changedEmail}.`]
              : [
                    'Email Confirmed',
                    'Thank you for confirming your email address.',
                ]
          : [
                'Unable to Confirm Email',
                'This link is invalid or has expired. Please request a new one.',
            ];

    return (
        <AccountLayout
            title={title}
            footer={
                !isPending && (
                    <AccountLink to={user ? '/profile' : '/account/login'}>
                        {user ? 'Back to my account' : 'Sign in'}
                    </AccountLink>
                )
            }
        >
            <Metadata title="Confirm Email" />

            {isPending ? (
                <div className="flex justify-center">
                    <Spinner />
                </div>
            ) : (
                <Alert variant={isSuccess ? 'default' : 'destructive'}>
                    <AlertDescription>{message}</AlertDescription>
                </Alert>
            )}
        </AccountLayout>
    );
}
