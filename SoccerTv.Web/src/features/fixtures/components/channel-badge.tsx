import { MonitorPlay, Tv } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import type { ChannelSummary } from '../fixtures-queries';

interface ProviderColor {
    hue: number;
    chroma?: number;
}

function getProviderColor(provider: string): ProviderColor {
     let hash = 0;

    for (const char of provider) {
        hash = (hash * 31 + char.charCodeAt(0)) >>> 0;
    }

    return { hue: hash % 360, chroma: 0.15 };
}

interface ChannelBadgeProps {
    channel: ChannelSummary;
}

export function ChannelBadge({ channel }: ChannelBadgeProps) {
    const Icon = channel.type === 'Streaming' ? MonitorPlay : Tv;

    const { hue, chroma } = getProviderColor(channel.provider);

    // Lightness and chroma are fixed so every provider gets the same soft tint and
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
            title={channel.type === 'Streaming' ? 'Streaming' : 'TV'}
        >
            <Icon data-icon="inline-start" />
            {channel.name}
        </Badge>
    );
}
