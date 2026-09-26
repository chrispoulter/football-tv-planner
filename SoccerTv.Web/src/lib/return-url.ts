/**
 * Where to go after signing in. RequireAuth passes the page it redirected from
 * as a location, and the login page passes it on as a path.
 */
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

    // Only ever return to a page on this site
    return path?.startsWith('/') && !path.startsWith('//') ? path : '/';
}
