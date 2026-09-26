import { Button } from '@/components/ui/button';
import { googleLoginUrl } from '../account-queries';

export function GoogleButton() {
    return (
        <div className="space-y-6">
            <div className="flex items-center gap-4 text-xs text-muted-foreground uppercase">
                <span className="h-px flex-1 bg-border" />
                or
                <span className="h-px flex-1 bg-border" />
            </div>

            {/* A full page navigation, as Google redirects back to the API */}
            <Button asChild variant="outline" className="w-full">
                <a href={googleLoginUrl()}>Continue with Google</a>
            </Button>
        </div>
    );
}
