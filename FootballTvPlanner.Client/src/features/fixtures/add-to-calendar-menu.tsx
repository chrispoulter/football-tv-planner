import { CalendarPlus } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { googleCalendarUrl, outlookComUrl } from '@/lib/calendar-urls';
import type { FixtureSummary } from './fixtures-queries';

interface AddToCalendarMenuProps {
    fixture: FixtureSummary;
}

export function AddToCalendarMenu({ fixture }: AddToCalendarMenuProps) {
    const channels = fixture.channels.map((c) => c.name).join(', ');

    const event = {
        title: `${fixture.homeTeam.name} v ${fixture.awayTeam.name}`,
        description: `${fixture.competition.name}\nWatch on: ${channels}`,
        location: channels,
        start: fixture.kickoffUtc,
        durationMinutes: 120,
    };

    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" title="Add to calendar">
                    <CalendarPlus />
                    <span className="sr-only">
                        Add {event.title} to calendar
                    </span>
                </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
                <DropdownMenuLabel>Add to calendar</DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem asChild>
                    <a
                        href={googleCalendarUrl(event)}
                        target="_blank"
                        rel="noreferrer"
                    >
                        Google Calendar
                    </a>
                </DropdownMenuItem>
                <DropdownMenuItem asChild>
                    <a
                        href={outlookComUrl(event)}
                        target="_blank"
                        rel="noreferrer"
                    >
                        Outlook.com
                    </a>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem asChild>
                    <a href={`/api/fixtures/${fixture.id}/calendar.ics`}>
                        Apple / other
                    </a>
                </DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}
