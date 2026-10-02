import { Star } from 'lucide-react';
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
import { useGetChannels, useGetCompetitions } from '../fixtures-queries';

const ALL = 'all';

interface FixtureFiltersProps {
    competition?: string;
    channel?: string;
    mine?: boolean;
    onCompetitionChange: (competition?: string) => void;
    onChannelChange: (channel?: string) => void;
    onMineChange: (mine: boolean) => void;
    disabled?: boolean;
}

export function FixtureFilters({
    competition,
    channel,
    mine,
    onCompetitionChange,
    onChannelChange,
    onMineChange,
    disabled,
}: FixtureFiltersProps) {
    const { user, isLoading: isAuthLoading } = useAuth();

    const { data: competitions, isPending: isCompetitionsPending } =
        useGetCompetitions();

    const { data: channels = [], isPending: isChannelsPending } =
        useGetChannels();

    return (
        <div className="flex flex-col gap-2 sm:flex-row">
            <Select
                value={competition ?? ALL}
                onValueChange={(value) =>
                    onCompetitionChange(value === ALL ? undefined : value)
                }
                disabled={disabled || isCompetitionsPending}
            >
                <SelectTrigger
                    className="w-full sm:w-56"
                    aria-label="Competition"
                >
                    <SelectValue />
                </SelectTrigger>
                <SelectContent position="popper" align="start">
                    <SelectItem value={ALL}>All competitions</SelectItem>
                    {competitions?.map((name) => (
                        <SelectItem key={name} value={name}>
                            {name}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>

            <Select
                value={channel ?? ALL}
                onValueChange={(value) =>
                    onChannelChange(value === ALL ? undefined : value)
                }
                disabled={disabled || isChannelsPending}
            >
                <SelectTrigger className="w-full sm:w-56" aria-label="Channel">
                    <SelectValue />
                </SelectTrigger>
                <SelectContent position="popper" align="start">
                    <SelectItem value={ALL}>All channels</SelectItem>
                    {channels.map((name) => (
                        <SelectItem key={name} value={name}>
                            {name}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>

            {isAuthLoading && <Skeleton className="h-9 w-full sm:w-36" />}

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

                    {mine && <CalendarFeedDialog disabled={disabled} />}
                </div>
            )}
        </div>
    );
}
