import { useLocation, useNavigate } from 'react-router';
import { Star } from 'lucide-react';
import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils';
import { useAddToSchedule, useRemoveFromSchedule } from './schedule-queries';

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

    const { mutate: addToSchedule, isPending: isAdding } = useAddToSchedule();

    const { mutate: removeFromSchedule, isPending: isRemoving } =
        useRemoveFromSchedule();

    const label = isBookmarked
        ? 'Remove from my schedule'
        : 'Add to my schedule';

    const handleToggleBookmark = () => {
        if (!user) {
            toast.info('Sign in to build your own schedule');
            navigate('/login', { state: { from: location } });
            return;
        }

        const mutate = isBookmarked ? removeFromSchedule : addToSchedule;

        mutate(fixtureId, {
            onError: (error) => toast.error(error.message),
        });
    };

    return (
        <Button
            variant="ghost"
            size="icon"
            onClick={handleToggleBookmark}
            disabled={isAdding || isRemoving}
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
