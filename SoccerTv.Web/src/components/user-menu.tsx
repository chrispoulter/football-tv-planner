import { Link, useNavigate } from 'react-router';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { useAuth } from './auth-provider';

export function UserMenu() {
    const navigate = useNavigate();

    const { user, clearAuth } = useAuth();

    function onLogout() {
        clearAuth();
        navigate('/');
    }

    if (!user) {
        return (
            <Button asChild variant="secondary">
                <Link to="/account/login">Login</Link>
            </Button>
        );
    }

    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" className="rounded-full">
                    <Avatar>
                        <AvatarFallback className="bg-primary text-primary-foreground">
                            {user.given_name[0]}
                            {user.family_name[0]}
                        </AvatarFallback>
                    </Avatar>
                    <span className="sr-only">Toggle profile menu</span>
                </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-48">
                <div className="px-2 py-1.5">
                    <p className="truncate text-sm font-medium">
                        {user.given_name} {user.family_name}
                    </p>
                    <p className="truncate text-xs text-muted-foreground">
                        {user.email}
                    </p>
                </div>

                <DropdownMenuSeparator />

                <DropdownMenuItem asChild>
                    <Link to="/profile">My Account</Link>
                </DropdownMenuItem>

                <DropdownMenuSeparator />

                <DropdownMenuItem onClick={onLogout}>Log out</DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}
