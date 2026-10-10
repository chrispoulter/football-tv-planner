import { Link } from 'react-router';
import { config } from '@/lib/config';

export function Footer() {
    return (
        <footer className="border-t py-4">
            <div className="container mx-auto flex items-center justify-between px-4 text-sm text-muted-foreground">
                <span>&copy; Chris Poulter {new Date().getFullYear()}</span>
                <div className="flex items-center gap-4">
                    <Link to="/privacy" className="hover:text-foreground">
                        Privacy
                    </Link>
                    <span>v{config.VITE_APP_VERSION}</span>
                </div>
            </div>
        </footer>
    );
}
