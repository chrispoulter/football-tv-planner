import { NavLink, Outlet } from 'react-router';

const navItems = [
    { to: '/profile', label: 'Profile' },
    { to: '/profile/security', label: 'Security' },
    { to: '/profile/danger', label: 'Danger zone' },
];

export function ProfileLayout() {
    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-2xl font-bold tracking-tight">Profile</h1>
                <p className="text-sm text-muted-foreground">
                    Manage your account settings
                </p>
            </div>

            <nav>
                <ul className="flex h-9 items-center gap-1">
                    {navItems.map((item) => (
                        <li key={item.to} className="h-full">
                            <NavLink
                                to={item.to}
                                end
                                className="relative inline-flex h-full items-center rounded-md px-2 py-1 text-sm font-medium whitespace-nowrap text-foreground/60 transition-colors outline-none after:absolute after:inset-x-0 after:-bottom-0.5 after:h-0.5 after:bg-foreground after:opacity-0 after:transition-opacity hover:text-foreground focus-visible:ring-[3px] focus-visible:ring-ring/50 aria-[current=page]:text-foreground aria-[current=page]:after:opacity-100 dark:text-muted-foreground dark:hover:text-foreground dark:aria-[current=page]:text-foreground"
                            >
                                {item.label}
                            </NavLink>
                        </li>
                    ))}
                </ul>
            </nav>

            <Outlet />
        </div>
    );
}
