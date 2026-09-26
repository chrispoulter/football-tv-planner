import { useLocation, useNavigate } from 'react-router';
import { Star } from 'lucide-react';
import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils';
import { useToggleSchedule } from '@/features/schedule/schedule-queries';
import type { FixtureSummary } from '../fixtures-queries';

interface BookmarkButtonProps {
    fixture: FixtureSummary;
}

export function BookmarkButton({ fixture }: BookmarkButtonProps) {
    const navigate = useNavigate();

    const location = useLocation();

    const { user } = useAuth();

    const { mutate: toggleSchedule } = useToggleSchedule();

    const isBookmarked = !!fixture.isBookmarked;

    const label = isBookmarked
        ? 'Remove from my schedule'
        : 'Add to my schedule';

    function onClick() {
        if (!user) {
            toast.info('Sign in to build your own schedule');
            navigate('/account/login', { state: { from: location } });
            return;
        }

        toggleSchedule(
            { fixtureId: fixture.id, isBookmarked: !isBookmarked },
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
