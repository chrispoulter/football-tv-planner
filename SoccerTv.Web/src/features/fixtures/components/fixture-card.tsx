import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';
import { toUkTime } from '@/lib/uk-time';
import type { FixtureSummary } from '../fixtures-queries';
import { AddToCalendarMenu } from './add-to-calendar-menu';
import { BookmarkButton } from './bookmark-button';
import { ChannelBadge } from './channel-badge';

interface FixtureCardProps {
    fixture: FixtureSummary;
    showCompetition?: boolean;
}

export function FixtureCard({ fixture, showCompetition }: FixtureCardProps) {
    const isOff =
        fixture.status === 'Cancelled' || fixture.status === 'Postponed';

    return (
        <div className="flex items-start gap-4 rounded-lg border p-4">
            <div className="w-12 shrink-0 pt-0.5 text-lg font-semibold tabular-nums">
                {toUkTime(fixture.kickoffUtc)}
            </div>

            <div className="min-w-0 flex-1 space-y-2">
                <div className="space-y-0.5">
                    {showCompetition && (
                        <div className="truncate text-xs text-muted-foreground">
                            {fixture.competition.name}
                        </div>
                    )}
                    <div
                        className={cn(
                            'text-base font-medium',
                            isOff && 'text-muted-foreground line-through'
                        )}
                    >
                        {fixture.homeTeam.name}{' '}
                        <span className="text-muted-foreground">v</span>{' '}
                        {fixture.awayTeam.name}
                    </div>
                </div>

                <div className="flex flex-wrap gap-1.5">
                    {isOff && (
                        <Badge variant="destructive">{fixture.status}</Badge>
                    )}
                    {fixture.channels.map((channel) => (
                        <ChannelBadge key={channel.id} channel={channel} />
                    ))}
                </div>
            </div>

            <div className="flex shrink-0 items-center">
                <BookmarkButton fixture={fixture} />
                <AddToCalendarMenu fixture={fixture} />
            </div>
        </div>
    );
}
