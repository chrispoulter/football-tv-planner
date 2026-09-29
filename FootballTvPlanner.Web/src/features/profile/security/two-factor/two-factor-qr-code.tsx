import { QRCodeSVG } from 'qrcode.react';
import { Button } from '@/components/ui/button';

interface TwoFactorQRCodeProps {
    sharedKey: string;
    authenticatorUri: string;
    onDone: () => void;
    onCancel: () => void;
}

// Group the key into blocks of four so it's easier to type
function formatKey(sharedKey: string) {
    return (
        sharedKey
            .toLowerCase()
            .match(/.{1,4}/g)
            ?.join(' ') ?? sharedKey
    );
}

export function TwoFactorQRCode({
    sharedKey,
    authenticatorUri,
    onDone,
    onCancel,
}: TwoFactorQRCodeProps) {
    return (
        <div className="space-y-4">
            <p className="text-sm font-medium">
                Scan this QR code with an authenticator app such as Google
                Authenticator, Microsoft Authenticator or 1Password:
            </p>
            <div className="inline-block rounded-lg border bg-white p-4">
                <QRCodeSVG value={authenticatorUri} size={176} />
            </div>
            <p className="text-sm text-muted-foreground">
                Or enter the key manually:{' '}
                <code className="rounded bg-muted px-1 font-mono break-all text-foreground">
                    {formatKey(sharedKey)}
                </code>
            </p>
            <div className="flex flex-col-reverse gap-2 sm:flex-row">
                <Button variant="outline" onClick={onCancel}>
                    Cancel
                </Button>
                <Button onClick={onDone}>I&apos;ve Scanned the Code</Button>
            </div>
        </div>
    );
}
