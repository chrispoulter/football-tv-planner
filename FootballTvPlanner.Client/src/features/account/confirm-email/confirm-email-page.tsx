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
        ? ['Verifying Email', 'Just a moment…']
        : isSuccess
          ? changedEmail
              ? ['Email Changed', `Your email address is now ${changedEmail}.`]
              : [
                    'Email Verified',
                    'Thank you for verifying your email address.',
                ]
          : [
                'Unable to Verify Email',
                'This link is invalid or has expired. Please request a new one.',
            ];

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
            <Metadata title="Verify Email" />

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
