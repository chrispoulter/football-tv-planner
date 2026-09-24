export const reminderOptions = [
    { value: 0, label: 'No reminder' },
    { value: 15, label: '15 minutes before' },
    { value: 30, label: '30 minutes before' },
    { value: 60, label: '1 hour before' },
    { value: 120, label: '2 hours before' },
    { value: 1440, label: '1 day before' },
];

export function toReminderLabel(minutes: number) {
    return (
        reminderOptions.find((option) => option.value === minutes)?.label ??
        `${minutes} minutes before`
    );
}
