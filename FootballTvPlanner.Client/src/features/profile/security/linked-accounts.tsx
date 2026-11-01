import { useEffect } from 'react';
import { useSearchParams } from 'react-router';
import { toast } from 'sonner';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { socialProviders } from '@/lib/social-providers';
import { linkAccountUrl, useRemoveLinkedAccount } from '../profile-queries';

const linkErrors: Record<string, string> = {
    link: 'Unable to link the account, please try again.',
    linked: 'That account is already linked to a different user.',
};

interface LinkedAccountsProps {
    providers: string[];
    hasPassword: boolean;
}

export function LinkedAccounts({
    providers,
    hasPassword,
}: LinkedAccountsProps) {
    const [searchParams, setSearchParams] = useSearchParams();

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

    const isLinked = (providerId: string) => providers.includes(providerId);

    const canUnlink = (providerId: string) =>
        hasPassword || providers.some((p) => p !== providerId);

    function onDisconnect(providerId: string, label: string) {
        removeLinkedAccount(providerId, {
            onSuccess: () => toast.success(`${label} account disconnected`),
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <div className="space-y-4">
            {socialProviders.map((provider) => {
                const linked = isLinked(provider.id);
                const unlinkable = canUnlink(provider.id);

                const inFlight = isRemoving && removingProvider === provider.id;

                return (
                    <div
                        key={provider.id}
                        className="flex items-center justify-between"
                    >
                        <div className="flex items-center gap-3">
                            {provider.icon}
                            <div>
                                <p className="text-sm font-medium">
                                    {provider.label}
                                </p>
                                {linked ? (
                                    <Badge variant="secondary">Connected</Badge>
                                ) : (
                                    <Badge variant="outline">
                                        Not connected
                                    </Badge>
                                )}
                            </div>
                        </div>

                        {linked ? (
                            <Button
                                variant="outline"
                                size="sm"
                                disabled={inFlight || !unlinkable}
                                onClick={() =>
                                    onDisconnect(provider.id, provider.label)
                                }
                            >
                                {inFlight ? 'Disconnecting...' : 'Disconnect'}
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
                );
            })}
        </div>
    );
}
