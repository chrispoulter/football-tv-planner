export function returnUrl(state: unknown) {
    const from = (
        state as {
            from?: string | { pathname?: string; search?: string };
        } | null
    )?.from;

    const path =
        typeof from === 'string'
            ? from
            : from?.pathname && `${from.pathname}${from.search ?? ''}`;

    return path?.startsWith('/') && !path.startsWith('//') ? path : '/';
}
