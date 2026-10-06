import {
    addDays as addCalendarDays,
    addMinutes as addMinutesToDate,
    format,
    getYear,
    isMatch,
    isToday,
    isTomorrow,
    parse,
} from 'date-fns';
import { UTCDate } from '@date-fns/utc';

const DATE_FORMAT = 'yyyy-MM-dd';

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

/**
 * Parses a yyyy-MM-dd date as local midnight.
 */
function parseLocalDate(date: string) {
    return parse(date, DATE_FORMAT, new Date());
}

/**
 * Returns the local calendar date (yyyy-MM-dd) for an instant.
 */
export function toLocalDate(value: string | Date) {
    return format(new Date(value), DATE_FORMAT);
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
    return toLocalDate(addCalendarDays(parseLocalDate(date), days));
}

/**
 * Returns the UTC instants for local midnight at the start and end of a day. Days are not
 * always 24 hours long when the clocks change.
 */
export function toUtcDayRange(date: string) {
    const start = parseLocalDate(date);

    return {
        from: start.toISOString(),
        to: addCalendarDays(start, 1).toISOString(),
    };
}

export function toDayLabel(date: string) {
    return dayFormat.format(parseLocalDate(date));
}

export function toLongDayLabel(date: string) {
    const value = parseLocalDate(date);

    if (isToday(value)) {
        return 'Today';
    }

    if (isTomorrow(value)) {
        return 'Tomorrow';
    }

    return longDayFormat.format(value);
}

/**
 * Checks for a real yyyy-MM-dd calendar date (rejects e.g. 2026-02-31).
 */
export function isDateString(value: string) {
    return /^\d{4}-\d{2}-\d{2}$/.test(value) && isMatch(value, DATE_FORMAT);
}

/**
 * Adds minutes to an instant, returning a UTC ISO string.
 */
export function addMinutes(value: string | Date, minutes: number) {
    return toUtcIso(addMinutesToDate(new Date(value), minutes));
}

/**
 * Formats an instant as a UTC ISO string (yyyy-MM-ddTHH:mm:ss.sssZ).
 */
export function toUtcIso(value: string | Date) {
    return new Date(value).toISOString();
}

/**
 * Formats an instant in the compact UTC form used by calendar links (yyyyMMddTHHmmssZ).
 */
export function toCompactUtc(value: string | Date) {
    return format(new UTCDate(new Date(value)), "yyyyMMdd'T'HHmmss'Z'");
}
