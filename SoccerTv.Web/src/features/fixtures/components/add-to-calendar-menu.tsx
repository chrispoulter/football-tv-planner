import { CalendarPlus } from 'lucide-react';
import { toast } from 'sonner';
import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuLabel,
    DropdownMenuSeparator,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import {
    googleCalendarUrl,
    outlook365Url,
    outlookComUrl,
} from '@/lib/calendar-links';
import {
    useDownloadFixtureCalendar,
    type FixtureSummary,
} from '../fixtures-queries';

interface AddToCalendarMenuProps {
    fixture: FixtureSummary;
}

export function AddToCalendarMenu({ fixture }: AddToCalendarMenuProps) {
    const { mutate: downloadCalendar } = useDownloadFixtureCalendar();

    const channels = fixture.channels.join(', ');

    const event = {
        title: `${fixture.homeTeam} v ${fixture.awayTeam}`,
        description: `${fixture.competition}\nWatch on: ${channels}`,
        location: channels,
        start: fixture.kickoffUtc,
        durationMinutes: 120,
    };

    function onDownload() {
        downloadCalendar(fixture, {
            onError: (error) => toast.error(error.message),
        });
    }

    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button variant="ghost" size="icon" title="Add to calendar">
                    <CalendarPlus />
                    <span className="sr-only">Add to calendar</span>
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
                <DropdownMenuItem asChild>
                    <a
                        href={outlook365Url(event)}
                        target="_blank"
                        rel="noreferrer"
                    >
                        Outlook (Microsoft 365)
                    </a>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={onDownload}>
                    Download .ics (Apple, Outlook desktop)
                </DropdownMenuItem>
            </DropdownMenuContent>
        </DropdownMenu>
    );
}
