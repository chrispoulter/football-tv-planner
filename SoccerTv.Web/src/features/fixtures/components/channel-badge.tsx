import { MonitorPlay, Tv } from 'lucide-react';
import { Badge } from '@/components/ui/badge';
import { cn } from '@/lib/utils';
import type { ChannelSummary } from '../fixtures-queries';

const providerStyles: Record<string, string> = {
    Sky: 'bg-red-600 text-white',
    TNT: 'bg-neutral-900 text-white dark:bg-neutral-100 dark:text-neutral-900',
    Amazon: 'bg-sky-500 text-white',
    BBC: 'bg-pink-600 text-white',
    ITV: 'bg-teal-600 text-white',
    'Premier Sports': 'bg-violet-700 text-white',
};

interface ChannelBadgeProps {
    channel: ChannelSummary;
}

export function ChannelBadge({ channel }: ChannelBadgeProps) {
    const Icon = channel.type === 'Streaming' ? MonitorPlay : Tv;

    return (
        <Badge
            variant="secondary"
            className={cn(providerStyles[channel.provider])}
            title={channel.type === 'Streaming' ? 'Streaming' : 'TV'}
        >
            <Icon data-icon="inline-start" />
            {channel.name}
        </Badge>
    );
}
