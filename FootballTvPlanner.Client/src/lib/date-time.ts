import {
    addDays as addCalendarDays,
    addMinutes as addMinutesToDate,
    format,
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

function parseLocalDate(date: string) {
    return parse(date, DATE_FORMAT, new Date());
}

export function toLocalDate(value: string | Date) {
    return format(new Date(value), DATE_FORMAT);
}

export function todayLocal() {
    return toLocalDate(new Date());
}

export function toLocalTime(value: string | Date) {
    return timeFormat.format(new Date(value));
}

export function addDays(date: string, days: number) {
    return toLocalDate(addCalendarDays(parseLocalDate(date), days));
}

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

export function isDateString(value: string) {
    return /^\d{4}-\d{2}-\d{2}$/.test(value) && isMatch(value, DATE_FORMAT);
}

export function addMinutes(value: string | Date, minutes: number) {
    return toUtcIso(addMinutesToDate(new Date(value), minutes));
}

export function toUtcIso(value: string | Date) {
    return new Date(value).toISOString();
}

export function toCompactUtc(value: string | Date) {
    return format(new UTCDate(new Date(value)), "yyyyMMdd'T'HHmmss'Z'");
}
