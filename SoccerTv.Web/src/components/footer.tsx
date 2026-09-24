import { config } from '@/lib/config';
import { currentYear } from '@/lib/dates';

export function Footer() {
    return (
        <footer className="border-t py-4">
            <div className="container mx-auto flex items-center justify-between px-4 text-sm text-muted-foreground">
                <span>&copy; Chris Poulter {currentYear}</span>
                <span>v{config.VITE_APP_VERSION}</span>
            </div>
        </footer>
    );
}
