import { NavLink } from 'react-router';
import { Tv } from 'lucide-react';
import { Button } from '@/components/ui/button';

const navItems = [{ to: '/fixtures', label: 'Fixtures', icon: Tv }];

export function MainMenu() {
    return (
        <nav className="flex items-center gap-1">
            {navItems.map(({ to, label, icon: Icon }) => (
                <Button
                    key={to}
                    variant="ghost"
                    asChild
                    className="aria-[current=page]:bg-secondary aria-[current=page]:text-secondary-foreground"
                >
                    <NavLink to={to}>
                        <Icon className="sm:hidden" />
                        <span className="sr-only sm:not-sr-only">{label}</span>
                    </NavLink>
                </Button>
            ))}
        </nav>
    );
}
