import { Link } from 'react-router';
import { ModeToggle } from './mode-toggle';
import { MainMenu } from './main-menu';
import { UserMenu } from './user-menu';

export function Header() {
    return (
        <header className="sticky top-0 z-50 w-full border-b bg-background/95 backdrop-blur supports-backdrop-filter:bg-background/60">
            <div className="container mx-auto flex h-14 items-center justify-between px-4">
                <Link to="/" className="font-semibold">
                    Soccer TV
                </Link>

                <div className="flex items-center gap-2">
                    <MainMenu />
                    <ModeToggle />
                    <UserMenu />
                </div>
            </div>
        </header>
    );
}
