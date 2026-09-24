import { Star } from 'lucide-react';
import { useAuth } from '@/components/auth-provider';
import { Button } from '@/components/ui/button';
import { CalendarFeedDialog } from '@/features/schedule/components/calendar-feed-dialog';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { useGetChannels, useGetCompetitions } from '../fixtures-queries';

const ALL = 'all';

interface FixtureFiltersProps {
    competitionId?: string;
    provider?: string;
    mine?: boolean;
    onCompetitionChange: (competitionId?: string) => void;
    onProviderChange: (provider?: string) => void;
    onMineChange: (mine: boolean) => void;
    disabled?: boolean;
}

export function FixtureFilters({
    competitionId,
    provider,
    mine,
    onCompetitionChange,
    onProviderChange,
    onMineChange,
    disabled,
}: FixtureFiltersProps) {
    const { user } = useAuth();

    const { data: competitions } = useGetCompetitions();
    const { data: channels } = useGetChannels();

    const providers = Array.from(
        new Set(channels?.map((channel) => channel.provider))
    );

    return (
        <div className="flex flex-col gap-2 sm:flex-row">
            <Select
                value={competitionId ?? ALL}
                onValueChange={(value) =>
                    onCompetitionChange(value === ALL ? undefined : value)
                }
                disabled={disabled}
            >
                <SelectTrigger
                    className="w-full sm:flex-1"
                    aria-label="Competition"
                >
                    <SelectValue />
                </SelectTrigger>
                <SelectContent>
                    <SelectItem value={ALL}>All competitions</SelectItem>
                    {competitions?.map((competition) => (
                        <SelectItem key={competition.id} value={competition.id}>
                            {competition.name}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>

            <Select
                value={provider ?? ALL}
                onValueChange={(value) =>
                    onProviderChange(value === ALL ? undefined : value)
                }
                disabled={disabled}
            >
                <SelectTrigger
                    className="w-full sm:flex-1"
                    aria-label="Broadcaster"
                >
                    <SelectValue />
                </SelectTrigger>
                <SelectContent>
                    <SelectItem value={ALL}>All broadcasters</SelectItem>
                    {providers.map((name) => (
                        <SelectItem key={name} value={name}>
                            {name}
                        </SelectItem>
                    ))}
                </SelectContent>
            </Select>

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
