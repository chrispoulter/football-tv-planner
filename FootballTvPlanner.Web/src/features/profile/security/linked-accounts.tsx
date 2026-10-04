import { useEffect } from 'react';
import { useSearchParams } from 'react-router';
import { toast } from 'sonner';
import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Skeleton } from '@/components/ui/skeleton';
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

    const { data, isPending, isSuccess } = useGetLinkedAccounts();

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
        return (
            <Alert variant="destructive">
                <AlertTitle>Error</AlertTitle>
                <AlertDescription>
                    Failed to load linked accounts. Please try again.
                </AlertDescription>
            </Alert>
        );
    }

    const isLinked = (providerId: string) =>
        data.providers.includes(providerId);

    const canUnlink = hasPassword || data.providers.length > 1;

    function onUnlink(providerId: string, label: string) {
        removeLinkedAccount(providerId, {
            onSuccess: () => toast.success(`${label} account disconnected`),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="space-y-4">
            {authProviders.map((provider) => (
                <div
                    key={provider.id}
                    className="flex items-center justify-between gap-4"
                >
                    <div className="flex items-center gap-3">
                        {provider.icon}
                        <div>
                            <p className="text-sm font-medium">
                                {provider.label}
                            </p>
                            {isLinked(provider.id) ? (
                                <Badge variant="secondary">Connected</Badge>
                            ) : (
                                <Badge variant="outline">Not connected</Badge>
                            )}
                        </div>
                    </div>

                    {isLinked(provider.id) ? (
                        <Button
                            variant="outline"
                            size="sm"
                            disabled={
                                !canUnlink ||
                                (isRemoving && removingProvider === provider.id)
                            }
                            onClick={() =>
                                onUnlink(provider.id, provider.label)
                            }
                        >
                            {isRemoving && removingProvider === provider.id
                                ? 'Disconnecting...'
                                : 'Disconnect'}
                        </Button>
                    ) : (
                        <Button asChild variant="outline" size="sm">
                            <a
                                href={linkAccountUrl(
                                    provider.id,
                                    '/profile/security'
                                )}
                            >
                                Connect
                            </a>
                        </Button>
                    )}
                </div>
            ))}
        </div>
    );
}
