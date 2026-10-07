import { useEffect } from 'react';
import { Star } from 'lucide-react';
import { toast } from 'sonner';
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { Skeleton } from '@/components/ui/skeleton';
import { CalendarFeedDialog } from '@/features/schedule/calendar-feed-dialog';
import { useGetChannels, useGetCompetitions } from './fixtures-queries';

const ALL = 'all';

interface FixtureFiltersProps {
    competitionId?: string;
    channelId?: string;
    mine?: boolean;
    onCompetitionChange: (competitionId?: string) => void;
    onChannelChange: (channelId?: string) => void;
    onMineChange: (mine: boolean) => void;
    disabled?: boolean;
}

export function FixtureFilters({
    competitionId,
    channelId,
    mine,
    onCompetitionChange,
    onChannelChange,
    onMineChange,
    disabled,
}: FixtureFiltersProps) {
    const { user, isLoading: isAuthLoading } = useAuth();

    const { data: competitions, isError: isCompetitionsError } =
        useGetCompetitions();

    const { data: channels, isError: isChannelsError } = useGetChannels();

    useEffect(() => {
        if (isCompetitionsError) {
            toast.error('Unable to load competitions');
        }
    }, [isCompetitionsError]);

    useEffect(() => {
        if (isChannelsError) {
            toast.error('Unable to load channels');
        }
    }, [isChannelsError]);

    return (
        <div className="flex flex-col gap-2 sm:flex-row">
            <Select
                value={competitionId ?? ALL}
                onValueChange={(value) =>
                    onCompetitionChange(value === ALL ? undefined : value)
                }
                disabled={disabled || !competitions}
            >
                <SelectTrigger
                    className="w-full sm:w-56"
                    aria-label="Competition"
                >
                    <SelectValue />
                </SelectTrigger>
                <SelectContent position="popper" align="start">
                    <SelectItem value={ALL}>All competitions</SelectItem>
                    {competitions?.items.map(({ id, name }) => (
                        <SelectItem key={id} value={id}>
                            {name}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>

            <Select
                value={channelId ?? ALL}
                onValueChange={(value) =>
                    onChannelChange(value === ALL ? undefined : value)
                }
                disabled={disabled || !channels}
            >
                <SelectTrigger className="w-full sm:w-56" aria-label="Channel">
                    <SelectValue />
                </SelectTrigger>
                <SelectContent position="popper" align="start">
                    <SelectItem value={ALL}>All channels</SelectItem>
                    {channels?.items.map(({ id, name }) => (
                        <SelectItem key={id} value={id}>
                            {name}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>

            {isAuthLoading && (
                <div className="flex gap-2">
                    <Skeleton className="h-9 flex-1 sm:w-36 sm:flex-none" />
                    <Skeleton className="h-9 flex-1 sm:w-30 sm:flex-none" />
                </div>
            )}

            {user && (
                <div className="flex gap-2">
                    <Button
                        variant={mine ? 'default' : 'outline'}
                        onClick={() => onMineChange(!mine)}
                        disabled={disabled}
                        aria-pressed={!!mine}
                        className="flex-1 sm:flex-none"
                    >
                        <Star className={mine ? 'fill-current' : undefined} />
                        My Schedule
                    </Button>

                    <CalendarFeedDialog
                        disabled={disabled}
                        className="flex-1 sm:flex-none"
                    />
                </div>
            )}
        </div>
    );
}
