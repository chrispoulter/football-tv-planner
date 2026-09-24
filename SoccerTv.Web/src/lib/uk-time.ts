const UK_TIME_ZONE = 'Europe/London';

const dateFormat = new Intl.DateTimeFormat('en-CA', {
    timeZone: UK_TIME_ZONE,
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
});

const timeFormat = new Intl.DateTimeFormat('en-GB', {
    timeZone: UK_TIME_ZONE,
    hour: '2-digit',
    minute: '2-digit',
});

const dayFormat = new Intl.DateTimeFormat('en-GB', {
    timeZone: 'UTC',
    weekday: 'short',
    day: 'numeric',
    month: 'short',
});

const longDayFormat = new Intl.DateTimeFormat('en-GB', {
    timeZone: 'UTC',
    weekday: 'long',
    day: 'numeric',
    month: 'long',
});

/**
 * Returns the UK calendar date (yyyy-MM-dd) for an instant.
 */
export function toUkDate(value: string | Date) {
    return dateFormat.format(new Date(value));
}

export function todayUk() {
    return toUkDate(new Date());
}

/**
 * Formats the UK kick-off time (HH:mm) for an instant.
 */
export function toUkTime(value: string | Date) {
    return timeFormat.format(new Date(value));
}

/**
 * Adds days to a yyyy-MM-dd date string without any time zone drift.
 */
export function addDays(date: string, days: number) {
    const value = new Date(`${date}T00:00:00Z`);
    value.setUTCDate(value.getUTCDate() + days);
    return value.toISOString().slice(0, 10);
}

export function toDayLabel(date: string) {
    return dayFormat.format(new Date(`${date}T00:00:00Z`));
}

export function toLongDayLabel(date: string) {
    const today = todayUk();

    if (date === today) {
        return 'Today';
    }

    if (date === addDays(today, 1)) {
        return 'Tomorrow';
    }

    return longDayFormat.format(new Date(`${date}T00:00:00Z`));
}

export function isUkDate(value: string) {
    return /^\d{4}-\d{2}-\d{2}$/.test(value);
}
