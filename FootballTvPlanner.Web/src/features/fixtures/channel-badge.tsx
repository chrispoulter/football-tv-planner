import { Badge } from '@/components/ui/badge';

const CHANNEL_COLORS = [
    'bg-red-50 text-red-700 dark:bg-red-950 dark:text-red-300',
    'bg-orange-50 text-orange-700 dark:bg-orange-950 dark:text-orange-300',
    'bg-amber-50 text-amber-700 dark:bg-amber-950 dark:text-amber-300',
    'bg-lime-50 text-lime-700 dark:bg-lime-950 dark:text-lime-300',
    'bg-green-50 text-green-700 dark:bg-green-950 dark:text-green-300',
    'bg-emerald-50 text-emerald-700 dark:bg-emerald-950 dark:text-emerald-300',
    'bg-teal-50 text-teal-700 dark:bg-teal-950 dark:text-teal-300',
    'bg-cyan-50 text-cyan-700 dark:bg-cyan-950 dark:text-cyan-300',
    'bg-sky-50 text-sky-700 dark:bg-sky-950 dark:text-sky-300',
    'bg-blue-50 text-blue-700 dark:bg-blue-950 dark:text-blue-300',
    'bg-indigo-50 text-indigo-700 dark:bg-indigo-950 dark:text-indigo-300',
    'bg-violet-50 text-violet-700 dark:bg-violet-950 dark:text-violet-300',
    'bg-purple-50 text-purple-700 dark:bg-purple-950 dark:text-purple-300',
    'bg-fuchsia-50 text-fuchsia-700 dark:bg-fuchsia-950 dark:text-fuchsia-300',
    'bg-pink-50 text-pink-700 dark:bg-pink-950 dark:text-pink-300',
    'bg-rose-50 text-rose-700 dark:bg-rose-950 dark:text-rose-300',
];

function getChannelColor(channel: string): string {
    let hash = 0;
    for (const char of channel.trim().toLowerCase()) {
        hash = (hash * 31 + char.charCodeAt(0)) | 0;
    }
    return CHANNEL_COLORS[Math.abs(hash) % CHANNEL_COLORS.length];
}

interface ChannelBadgeProps {
    channel: string;
}

export function ChannelBadge({ channel }: ChannelBadgeProps) {
    return <Badge className={getChannelColor(channel)}>{channel}</Badge>;
}
