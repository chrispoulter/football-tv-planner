import { Link, useNavigate } from 'react-router';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useLogout } from '@/features/account/account-queries';
import { useAuth } from './auth-provider';

export function UserMenu() {
    const navigate = useNavigate();

    const { user } = useAuth();

    const { mutate: logout } = useLogout();

    function onLogout() {
        logout(undefined, { onSettled: () => navigate('/') });
    }

    if (!user) {
        return (
            <Button asChild variant="secondary">
                <Link to="/account/login">Sign In</Link>
            </Button>
        );
    }

    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="rounded-full">
                    <Avatar>
                        <AvatarFallback className="bg-primary text-primary-foreground">
                            {(user.name || user.email)[0].toUpperCase()}
                        </AvatarFallback>
                    </Avatar>
                    <span className="sr-only">Toggle profile menu</span>
                </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-48">
                <div className="px-2 py-1.5">
                    {user.name && (
                        <p className="truncate text-sm font-medium">
                            {user.name}
                        </p>
                    )}
                    <p
                        className={
                            user.name
                                ? 'truncate text-xs text-muted-foreground'
                                : 'truncate text-sm font-medium'
                        }
                    >
                        {user.email}
                    </p>
                </div>

                <DropdownMenuSeparator />

                <DropdownMenuItem asChild>
                    <Link to="/profile">My Account</Link>
                </DropdownMenuItem>

                <DropdownMenuSeparator />

                <DropdownMenuItem onClick={onLogout}>Sign Out</DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}
