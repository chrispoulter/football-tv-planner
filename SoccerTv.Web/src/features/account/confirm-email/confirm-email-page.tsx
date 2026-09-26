import { useEffect } from 'react';
import { Link, useSearchParams } from 'react-router';
import { useQueryClient } from '@tanstack/react-query';
import { sessionKeys, useAuth } from '@/components/auth-provider';
import { Metadata } from '@/components/metadata';
import { Card, CardContent } from '@/components/ui/card';
import { Spinner } from '@/components/ui/spinner';
import { useConfirmEmail } from '../account-queries';

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

    // Pick up the new email address when it has been changed
    useEffect(() => {
        if (isSuccess && changedEmail) {
            queryClient.invalidateQueries({ queryKey: sessionKeys.all });
        }
    }, [isSuccess, changedEmail, queryClient]);

    const [title, message] = isSuccess
        ? changedEmail
            ? ['Email changed', `Your email address is now ${changedEmail}.`]
            : [
                  'Email confirmed',
                  'Thank you for confirming your email address.',
              ]
        : [
              'Unable to confirm email',
              'This link is invalid or has expired. Please request a new one.',
          ];

    return (
        <div className="flex flex-1 items-center justify-center">
            <Card className="w-full max-w-md">
                <CardContent className="space-y-6">
                    <Metadata title="Confirm Email" />

                    {isPending ? (
                        <div className="flex justify-center">
                            <Spinner />
                        </div>
                    ) : (
                        <>
                            <div className="space-y-1">
                                <h1 className="text-2xl font-bold tracking-tight">
                                    {title}
                                </h1>
                                <p className="text-sm text-muted-foreground">
                                    {message}
                                </p>
                            </div>

                            <p className="text-sm text-muted-foreground">
                                <Link
                                    to={user ? '/profile' : '/account/login'}
                                    className="underline underline-offset-4"
                                >
                                    {user ? 'Back to my account' : 'Log in now'}
                                </Link>
                            </p>
                        </>
                    )}
                </CardContent>
            </Card>
        </div>
    );
}
