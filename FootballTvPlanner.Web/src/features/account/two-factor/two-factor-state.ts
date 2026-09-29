/**
 * Passed from the login page in the navigation state, as the two-factor step
 * needs to know how to finish signing in.
 */
export interface TwoFactorState {
    rememberMe: boolean;
    from: string;
}
