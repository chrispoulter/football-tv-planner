import { useEffect, useRef } from 'react';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { addDays, toDayLabel, todayLocal } from '@/lib/date-time';

const DAYS_SHOWN = 14;

interface DayStripProps {
    date: string;
    onChange: (date: string) => void;
    disabled?: boolean;
}

export function DayStrip({ date, onChange, disabled }: DayStripProps) {
    const selectedRef = useRef<HTMLButtonElement>(null);

    const today = todayLocal();

    const start = date > addDays(today, DAYS_SHOWN - 1) ? date : today;

    const isFirstDay = date <= today;

    const days = Array.from({ length: DAYS_SHOWN }, (_, i) =>
        addDays(start, i)
    );

    useEffect(() => {
        selectedRef.current?.scrollIntoView({
            block: 'nearest',
            inline: 'center',
        });
    }, [date]);

    return (
        <div className="flex items-center gap-1">
            <Button
                variant="outline"
                size="icon"
                onClick={() => onChange(addDays(date, -1))}
                disabled={disabled || isFirstDay}
            >
                <ChevronLeft />
                <span className="sr-only">Previous day</span>
            </Button>

            <div className="flex min-w-0 flex-1 scrollbar-thin scrollbar-thumb-border scrollbar-track-transparent gap-1 overflow-x-auto py-1 hover:scrollbar-thumb-muted-foreground">
                {days.map((day) => {
                    const selected = day === date;

                    return (
                        <Button
                            key={day}
                            ref={selected ? selectedRef : undefined}
                            variant={selected ? 'default' : 'ghost'}
                            size="sm"
                            className="shrink-0"
                            onClick={() => onChange(day)}
                            disabled={disabled}
                            aria-current={selected ? 'date' : undefined}
                        >
                            {day === today ? 'Today' : toDayLabel(day)}
                        </Button>
                    );
                })}
            </div>

            <Button
                variant="outline"
                size="icon"
                onClick={() => onChange(addDays(date, 1))}
                disabled={disabled}
            >
                <ChevronRight />
                <span className="sr-only">Next day</span>
            </Button>
        </div>
    );
}
