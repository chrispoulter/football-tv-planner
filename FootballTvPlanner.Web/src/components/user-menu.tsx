import { Link, useNavigate } from 'react-router';
import { toast } from 'sonner';
import { Avatar, AvatarFallback } from '@/components/ui/avatar';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Skeleton } from '@/components/ui/skeleton';
import { useLogout } from '@/features/account/account-queries';
import { useAuth } from './auth-provider';

export function UserMenu() {
    const navigate = useNavigate();

    const { user, isLoading } = useAuth();

    const { mutate: logout, isPending } = useLogout();

    function onLogout() {
        logout(undefined, {
            onSuccess: () => navigate('/'),
            onError: (error) => toast.error(error.message),
        });
    }

    if (isLoading) {
        return <Skeleton className="size-9 rounded-full" />;
    }

    if (!user) {
        return (
            <Button asChild variant="secondary">
                <Link to="/login">Sign In</Link>
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
                    <span className="sr-only">Account menu</span>
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
                    <Link to="/profile">Profile</Link>
                </DropdownMenuItem>

                <DropdownMenuSeparator />

                <DropdownMenuItem onClick={onLogout} disabled={isPending}>
                    {isPending ? 'Signing Out...' : 'Sign Out'}
                </DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}
