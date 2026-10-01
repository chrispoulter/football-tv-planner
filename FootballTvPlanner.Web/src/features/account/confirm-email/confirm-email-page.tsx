import { useEffect } from 'react';
import { useSearchParams } from 'react-router';
import { useQueryClient } from '@tanstack/react-query';
import { sessionKeys, useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import { PageLoading } from '@/components/page-loading';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { useConfirmEmail } from '../account-queries';
import { AccountLayout, AccountLink } from '../account-layout';

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
                    <AccountLink to={user ? '/profile' : '/login'}>
                        {user ? 'Back to profile' : 'Sign in'}
                    </AccountLink>
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
