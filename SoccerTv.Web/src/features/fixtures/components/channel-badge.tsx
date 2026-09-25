import { Badge } from '@/components/ui/badge';

interface ChannelColor {
    hue: number;
    chroma?: number;
}

/**
 * Colours by the first word of the name so a broadcaster's channels share a tint
 * (e.g. "Sky Sports Main Event" and "Sky Sports+").
 */
function getChannelColor(channel: string): ChannelColor {
    const broadcaster = channel.split(' ')[0].toLowerCase();

    let hash = 0;

    for (const char of broadcaster) {
        hash = (hash * 31 + char.charCodeAt(0)) >>> 0;
    }

    return { hue: hash % 360, chroma: 0.15 };
}

interface ChannelBadgeProps {
    channel: string;
}

export function ChannelBadge({ channel }: ChannelBadgeProps) {
    const { hue, chroma } = getChannelColor(channel);

    // Lightness and chroma are fixed so every broadcaster gets the same soft tint and
    // readable contrast; only the hue varies.
    return (
        <Badge
            variant="secondary"
            style={
                {
                    '--channel-hue': hue,
                    '--channel-chroma': chroma,
                } as React.CSSProperties
            }
            className="bg-[oklch(0.65_var(--channel-chroma)_var(--channel-hue)/0.12)] text-[oklch(0.45_var(--channel-chroma)_var(--channel-hue))] dark:bg-[oklch(0.7_var(--channel-chroma)_var(--channel-hue)/0.15)] dark:text-[oklch(0.82_var(--channel-chroma)_var(--channel-hue))]"
        >
            {channel}
        </Badge>
    );
}
