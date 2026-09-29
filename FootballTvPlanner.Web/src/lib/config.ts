import { z } from 'zod';

declare global {
    interface Window {
        __ENV__?: Record<string, string>;
    }
}

const configSchema = z.object({
    VITE_APP_VERSION: z.string(),
    VITE_FEATURE_X: z.string().optional(),
});

export const config = configSchema.parse({
    ...import.meta.env,
    ...window.__ENV__,
});
