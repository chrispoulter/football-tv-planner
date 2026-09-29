import { toast } from 'sonner';
import { Button } from '@/components/ui/button';

interface RecoveryCodesProps {
    codes: string[];
    onDone: () => void;
}

export function RecoveryCodes({ codes, onDone }: RecoveryCodesProps) {
    const text = codes.join('\n');

    async function onCopy() {
        await navigator.clipboard.writeText(text);
        toast.success('Recovery codes copied to clipboard');
    }

    function onDownload() {
        const url = URL.createObjectURL(
            new Blob([text], { type: 'text/plain' })
        );
        const link = document.createElement('a');
        link.href = url;
        link.download = 'footballtvplanner-recovery-codes.txt';
        link.click();
        URL.revokeObjectURL(url);
    }

    return (
        <div className="space-y-4">
            <p className="text-sm text-muted-foreground">
                Save these recovery codes somewhere safe. Each one can be used
                once to sign in if you lose access to your authenticator app.
                They won&apos;t be shown again.
            </p>

            <ul className="grid grid-cols-2 gap-2 rounded-md border p-4 font-mono text-sm">
                {codes.map((code) => (
                    <li key={code} className="select-all">
                        {code}
                    </li>
                ))}
            </ul>

            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button variant="outline" onClick={onCopy}>
                    Copy
                </Button>
                <Button variant="outline" onClick={onDownload}>
                    Download
                </Button>
                <Button onClick={onDone}>Done</Button>
            </div>
        </div>
    );
}
