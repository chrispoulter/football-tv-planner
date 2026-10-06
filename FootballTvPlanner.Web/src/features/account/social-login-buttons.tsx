import { Button } from '@/components/ui/button';
import { socialProviders } from '@/lib/social-providers';
import { externalLoginUrl } from './account-queries';

interface SocialLoginButtonsProps {
    returnUrl?: string;
}

export function SocialLoginButtons({ returnUrl }: SocialLoginButtonsProps) {
    return (
        <>
            {socialProviders.map((provider) => (
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
        </>
    );
}
