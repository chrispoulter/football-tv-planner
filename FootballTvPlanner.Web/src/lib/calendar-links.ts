import { addMinutes, toCompactUtc, toUtcIso } from '@/lib/date-time';

interface CalendarEvent {
    title: string;
    description: string;
    location: string;
    start: string;
    durationMinutes: number;
}

function getEnd({ start, durationMinutes }: CalendarEvent) {
    return addMinutes(start, durationMinutes);
}

export function googleCalendarUrl(event: CalendarEvent) {
    const params = new URLSearchParams({
        action: 'TEMPLATE',
        text: event.title,
        dates: `${toCompactUtc(event.start)}/${toCompactUtc(getEnd(event))}`,
        details: event.description,
        location: event.location,
    });

    return `https://calendar.google.com/calendar/render?${params}`;
}

export function outlookComUrl(event: CalendarEvent) {
    const params = new URLSearchParams({
        path: '/calendar/action/compose',
        rru: 'addevent',
        subject: event.title,
        startdt: toUtcIso(event.start),
        enddt: getEnd(event),
        body: event.description,
        location: event.location,
    });

    return `https://outlook.live.com/calendar/0/deeplink/compose?${params}`;
}

export function googleSubscribeUrl(webcalUrl: string) {
    return `https://calendar.google.com/calendar/r?cid=${encodeURIComponent(webcalUrl)}`;
}

export function outlookComSubscribeUrl(httpsUrl: string, name: string) {
    const params = new URLSearchParams({ url: httpsUrl, name });
    return `https://outlook.live.com/calendar/0/addfromweb?${params}`;
}
