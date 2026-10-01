import { Button } from '@/components/ui/button';
import { Separator } from '@/components/ui/separator';
import { googleLoginUrl } from '../account-queries';
import { GoogleIcon } from './google-icon';

interface SocialLoginButtonsProps {
    returnUrl?: string;
}

export function SocialLoginButtons({ returnUrl }: SocialLoginButtonsProps) {
    return (
        <>
            <Button asChild variant="outline" className="w-full">
                <a href={googleLoginUrl(returnUrl)}>
                    <GoogleIcon />
                    Continue with Google
                </a>
            </Button>

            <div className="relative">
                <Separator />
                <span className="absolute top-1/2 left-1/2 -translate-x-1/2 -translate-y-1/2 bg-card px-2 text-xs text-muted-foreground">
                    or
                </span>
            </div>
        </>
    );
}
