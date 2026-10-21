import { useLocation, useNavigate } from 'react-router';
import { Star } from 'lucide-react';
import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils';
import { useToggleSchedule } from './schedule-queries';

interface BookmarkButtonProps {
    fixtureId: string;
    isBookmarked: boolean;
}

export function BookmarkButton({
    fixtureId,
    isBookmarked,
}: BookmarkButtonProps) {
    const navigate = useNavigate();

    const location = useLocation();

    const { user } = useAuth();

    const { mutate: toggleSchedule } = useToggleSchedule();

    const label = isBookmarked
        ? 'Remove from my schedule'
        : 'Add to my schedule';

    function onClick() {
        if (!user) {
            toast.info('Sign in to build your own schedule');
            navigate('/login', { state: { from: location } });
            return;
        }

        toggleSchedule(
            { fixtureId, isBookmarked: !isBookmarked },
            {
                onError: (error) => toast.error(error.message),
            }
        );
    }

    return (
        <Button
            variant="ghost"
            size="icon"
            onClick={onClick}
            aria-pressed={isBookmarked}
            title={label}
        >
            <Star
                className={cn(
                    isBookmarked && 'fill-yellow-400 text-yellow-500'
                )}
            />
            <span className="sr-only">{label}</span>
        </Button>
    );
}
