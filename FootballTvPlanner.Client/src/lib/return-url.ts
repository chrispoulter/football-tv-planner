type ReturnUrlState = {
    from?: string | { pathname?: string; search?: string };
};

export function returnUrl(state: unknown) {
    const from = (state as ReturnUrlState | null)?.from;

    const path =
        typeof from === 'string'
            ? from
            : from?.pathname && `${from.pathname}${from.search ?? ''}`;

    return path?.startsWith('/') && !path.startsWith('//') ? path : '/';
}
