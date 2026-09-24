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
    onCompetitionChange: (competitionId?: string) => void;
    onProviderChange: (provider?: string) => void;
    disabled?: boolean;
}

export function FixtureFilters({
    competitionId,
    provider,
    onCompetitionChange,
    onProviderChange,
    disabled,
}: FixtureFiltersProps) {
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
        </div>
    );
}
