import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { authProviders } from '@/lib/auth-providers';
import { externalLoginUrl } from '../account-queries';

interface SocialLoginButtonsProps {
    returnUrl?: string;
}

export function SocialLoginButtons({ returnUrl }: SocialLoginButtonsProps) {
    return (
        <>
            {authProviders.map((provider) => (
                <Button
                    key={provider.id}
                    asChild
                    variant="outline"
                    className="w-full"
                >
                    <a href={externalLoginUrl(provider.id, returnUrl)}>
                        {provider.icon}
                        Continue with {provider.label}
                    </a>
                </Button>
            ))}

            <div className="relative">
                <Separator />
                <span className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 bg-card px-2 text-xs text-muted-foreground">
                    or
                </span>
            </div>
        </>
    );
}
