/**
 * The API stores and returns UTC instants. These helpers present them in the viewer's
 * local time zone, and turn local calendar days (yyyy-MM-dd) back into UTC ranges.
 */

const timeFormat = new Intl.DateTimeFormat(undefined, {
    hour: '2-digit',
    minute: '2-digit',
});

const dayFormat = new Intl.DateTimeFormat(undefined, {
    weekday: 'short',
    day: 'numeric',
    month: 'short',
});

const longDayFormat = new Intl.DateTimeFormat(undefined, {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
});

const pad = (value: number) => value.toString().padStart(2, '0');

function parseLocalDate(date: string) {
    const [year, month, day] = date.split('-').map(Number);
    return new Date(year, month - 1, day);
}

/**
 * Returns the local calendar date (yyyy-MM-dd) for an instant.
 */
export function toLocalDate(value: string | Date) {
    const date = new Date(value);
    return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
}

export function todayLocal() {
    return toLocalDate(new Date());
}

/**
 * Formats the local kick-off time for an instant.
 */
export function toLocalTime(value: string | Date) {
    return timeFormat.format(new Date(value));
}

/**
 * Adds calendar days to a yyyy-MM-dd date. Uses local dates so DST changes don't shift
 * the day.
 */
export function addDays(date: string, days: number) {
    const value = parseLocalDate(date);
    value.setDate(value.getDate() + days);
    return toLocalDate(value);
}

/**
 * Returns the UTC instants for local midnight at the start and end of a day. Days are not
 * always 24 hours long when the clocks change.
 */
export function toUtcDayRange(date: string) {
    return {
        from: parseLocalDate(date).toISOString(),
        to: parseLocalDate(addDays(date, 1)).toISOString(),
    };
}

export function toDayLabel(date: string) {
    return dayFormat.format(parseLocalDate(date));
}

export function toLongDayLabel(date: string) {
    const today = todayLocal();

    if (date === today) {
        return 'Today';
    }

    if (date === addDays(today, 1)) {
        return 'Tomorrow';
    }

    return longDayFormat.format(parseLocalDate(date));
}

export function isDateString(value: string) {
    return /^\d{4}-\d{2}-\d{2}$/.test(value);
}
