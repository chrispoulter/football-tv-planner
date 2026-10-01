import { Badge } from '@/components/ui/badge';

interface ChannelColor {
    hue: number;
    chroma?: number;
}

function getChannelColor(channel: string): ChannelColor {
    const broadcaster = channel.split(' ')[0].toLowerCase();

    let hash = 0;

    for (const char of broadcaster) {
        hash = (hash * 31 + char.charCodeAt(0)) >>> 0;
    }

    return { hue: Math.round((hash * 137.508) % 360), chroma: 0.15 };
}

interface ChannelBadgeProps {
    channel: string;
}

export function ChannelBadge({ channel }: ChannelBadgeProps) {
    const { hue } = getChannelColor(channel);

    return (
        <Badge
            variant="secondary"
            style={
                {
                    '--channel-hue': hue,
                    '--channel-chroma': 0.15,
                } as React.CSSProperties
            }
            className="bg-[oklch(0.65_var(--channel-chroma)_var(--channel-hue)/0.12)] text-[oklch(0.45_var(--channel-chroma)_var(--channel-hue))] dark:bg-[oklch(0.7_var(--channel-chroma)_var(--channel-hue)/0.15)] dark:text-[oklch(0.82_var(--channel-chroma)_var(--channel-hue))]"
        >
            {channel}
        </Badge>
    );
}
