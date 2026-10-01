import { useEffect } from 'react';
import { useSearchParams } from 'react-router';
import { toast } from 'sonner';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
import { QueryError } from '@/components/query-error';
import { authProviders } from '@/lib/auth-providers';
import {
    linkAccountUrl,
    useGetLinkedAccounts,
    useRemoveLinkedAccount,
} from '../profile-queries';

const linkErrors: Record<string, string> = {
    link: 'Unable to link the account, please try again.',
    linked: 'That account is already linked to a different user.',
};

interface LinkedAccountsProps {
    hasPassword: boolean;
}

export function LinkedAccounts({ hasPassword }: LinkedAccountsProps) {
    const [searchParams, setSearchParams] = useSearchParams();

    const { data, isPending, isSuccess, error } = useGetLinkedAccounts();

    const {
        mutate: removeLinkedAccount,
        isPending: isRemoving,
        variables: removingProvider,
    } = useRemoveLinkedAccount();

    useEffect(() => {
        const linkError = searchParams.get('error');

        if (linkError) {
            toast.error(linkErrors[linkError] ?? linkErrors.link);
            setSearchParams(
                (params) => {
                    params.delete('error');
                    return params;
                },
                { replace: true }
            );
        }
    }, [searchParams, setSearchParams]);

    if (isPending) {
        return <Skeleton className="h-10 w-full" />;
    }

    if (!isSuccess) {
        return <QueryError error={error} />;
    }

    if (data.accounts.length === 0) {
        return (
            <p className="text-sm text-muted-foreground">
                No external sign-in providers are available.
            </p>
        );
    }

    const linkedCount = data.accounts.filter((a) => a.isLinked).length;

    // Keep at least one way to sign in
    const canUnlink = hasPassword || linkedCount > 1;

    function onUnlink(provider: string, displayName: string) {
        removeLinkedAccount(provider, {
            onSuccess: () =>
                toast.success(`${displayName} account disconnected`),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="space-y-4">
            {data.accounts.map((account) => (
                <div
                    key={account.provider}
                    className="flex items-center justify-between gap-4"
                >
                    <div className="flex items-center gap-3">
                        {
                            authProviders.find((p) => p.id === account.provider)
                                ?.icon
                        }
                        <div>
                            <p className="text-sm font-medium">
                                {account.displayName}
                            </p>
                            {account.isLinked ? (
                                <Badge variant="secondary">Connected</Badge>
                            ) : (
                                <Badge variant="outline">Not connected</Badge>
                            )}
                        </div>
                    </div>

                    {account.isLinked ? (
                        <Button
                            variant="outline"
                            size="sm"
                            disabled={
                                !canUnlink ||
                                (isRemoving &&
                                    removingProvider === account.provider)
                            }
                            title={
                                canUnlink
                                    ? undefined
                                    : 'Set a password before disconnecting your only way to sign in'
                            }
                            onClick={() =>
                                onUnlink(account.provider, account.displayName)
                            }
                        >
                            {isRemoving && removingProvider === account.provider
                                ? 'Disconnecting...'
                                : 'Disconnect'}
                        </Button>
                    ) : (
                        <Button asChild variant="outline" size="sm">
                            <a
                                href={linkAccountUrl(
                                    account.provider,
                                    '/profile/security'
                                )}
                            >
                                Connect
                            </a>
                        </Button>
                    )}
                </div>
            ))}

            {!canUnlink && (
                <p className="text-xs text-muted-foreground">
                    Set a password before disconnecting your only way to sign
                    in.
                </p>
            )}
        </div>
    );
}
