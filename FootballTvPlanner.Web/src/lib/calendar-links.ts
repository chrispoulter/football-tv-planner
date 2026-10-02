interface CalendarEvent {
    title: string;
    description: string;
    location: string;
    start: string;
    durationMinutes: number;
}

function toCompactUtc(value: Date) {
    return value
        .toISOString()
        .replace(/[-:]/g, '')
        .replace(/\.\d{3}/, '');
}

function getEnd({ start, durationMinutes }: CalendarEvent) {
    return new Date(new Date(start).getTime() + durationMinutes * 60 * 1000);
}

export function googleCalendarUrl(event: CalendarEvent) {
    const params = new URLSearchParams({
        action: 'TEMPLATE',
        text: event.title,
        dates: `${toCompactUtc(new Date(event.start))}/${toCompactUtc(getEnd(event))}`,
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
        startdt: new Date(event.start).toISOString(),
        enddt: getEnd(event).toISOString(),
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
