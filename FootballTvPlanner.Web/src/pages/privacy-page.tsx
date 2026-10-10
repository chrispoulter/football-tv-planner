import { Link } from 'react-router';
import { Metadata } from '@/components/metadata';

export function PrivacyPage() {
    return (
        <>
            <Metadata title="Privacy Policy" />
            <article className="mx-auto w-full max-w-3xl space-y-8 text-muted-foreground [&_li]:ml-6 [&_ul]:list-disc [&_ul]:space-y-1">
                <header className="space-y-2">
                    <h1 className="text-3xl font-bold tracking-tight text-foreground">
                        Privacy Policy
                    </h1>
                    <p className="text-sm">Last updated: 10 October 2026</p>
                </header>

                <p>
                    Football TV Planner is a personal, non-commercial project
                    run by Chris Poulter. This policy explains what information
                    the app collects, why, and what you can do about it. In
                    short: we only keep what is needed to sign you in and save
                    your bookmarked fixtures. There are no ads, no analytics,
                    and your data is never sold or shared for marketing.
                </p>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        Information we collect
                    </h2>
                    <ul>
                        <li>
                            <strong className="text-foreground">
                                Account details
                            </strong>{' '}
                            — your name and email address, and a password
                            (stored only as a secure hash) if you register with
                            email.
                        </li>
                        <li>
                            <strong className="text-foreground">
                                Google sign-in
                            </strong>{' '}
                            — if you sign in with Google, we receive your name
                            and email address from your Google account, along
                            with an identifier Google issues to confirm your
                            identity. We request only the basic{' '}
                            <code>openid</code>, <code>email</code> and{' '}
                            <code>profile</code> scopes and cannot access your
                            Gmail, Drive, contacts or anything else.
                        </li>
                        <li>
                            <strong className="text-foreground">
                                Bookmarked fixtures
                            </strong>{' '}
                            — the fixtures you bookmark and when you bookmarked
                            them.
                        </li>
                        <li>
                            <strong className="text-foreground">
                                Calendar feed
                            </strong>{' '}
                            — a private, randomly generated link to your
                            personal calendar feed. Anyone with the link can see
                            your bookmarked fixtures, so keep it to yourself.
                            You can reset it at any time to stop the old link
                            working.
                        </li>
                        <li>
                            <strong className="text-foreground">
                                Two-factor authentication
                            </strong>{' '}
                            — if you enable it, the secret and recovery codes
                            needed to verify your codes.
                        </li>
                    </ul>
                    <p>
                        Like most websites, our servers may briefly record your
                        IP address in logs for security and troubleshooting.
                        These are not linked to your profile.
                    </p>
                </section>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        How we use it
                    </h2>
                    <p>Your information is used only to:</p>
                    <ul>
                        <li>create your account and sign you in;</li>
                        <li>store and display your bookmarked fixtures;</li>
                        <li>provide your personal calendar feed;</li>
                        <li>
                            send account emails, such as email verification and
                            password resets;
                        </li>
                        <li>keep your account secure.</li>
                    </ul>
                    <p>
                        The legal basis for this processing is that it is
                        necessary to provide the service you have signed up for.
                    </p>
                </section>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        Google user data
                    </h2>
                    <p>
                        Football TV Planner's use of information received from
                        Google APIs adheres to the{' '}
                        <a
                            href="https://developers.google.com/terms/api-services-user-data-policy"
                            target="_blank"
                            rel="noreferrer"
                            className="font-medium text-foreground underline underline-offset-4"
                        >
                            Google API Services User Data Policy
                        </a>
                        , including the Limited Use requirements. Google data is
                        used solely to sign you in and show your name in the
                        app. It is not transferred to anyone else, used for
                        advertising, or used to train AI models.
                    </p>
                </section>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        Cookies and local storage
                    </h2>
                    <p>
                        We use only essential cookies, to keep you signed in and
                        to complete Google and two-factor sign-in. Your
                        light/dark theme choice is saved in your browser's local
                        storage. There are no tracking or advertising cookies.
                    </p>
                </section>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        Third-party services
                    </h2>
                    <p>A small number of services are needed to run the app:</p>
                    <ul>
                        <li>
                            <strong className="text-foreground">Google</strong>{' '}
                            — for optional sign-in.
                        </li>
                        <li>
                            <strong className="text-foreground">
                                Fixture listings
                            </strong>{' '}
                            — fixture and TV channel information is gathered
                            from publicly available listings by our server. No
                            information about you is sent to these sources.
                        </li>
                        <li>
                            <strong className="text-foreground">
                                Calendar services
                            </strong>{' '}
                            — the "Add to calendar" options open Google Calendar
                            or Outlook with the fixture details filled in. These
                            are only used if you choose them, and are covered by
                            those services' own privacy policies.
                        </li>
                        <li>
                            <strong className="text-foreground">
                                Email and hosting providers
                            </strong>{' '}
                            — used to deliver account emails and to run the app
                            and its database.
                        </li>
                    </ul>
                </section>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        How long we keep it
                    </h2>
                    <p>
                        Your account, bookmarks and calendar feed are kept until
                        you delete your account. Sessions expire automatically,
                        and verification and password reset links expire shortly
                        after they are sent.
                    </p>
                </section>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        Your rights
                    </h2>
                    <p>
                        You can view and update your details from your{' '}
                        <Link
                            to="/profile"
                            className="font-medium text-foreground underline underline-offset-4"
                        >
                            profile
                        </Link>
                        , and delete your account at any time from the Danger
                        zone tab. Deleting your account permanently removes your
                        profile, linked sign-in methods, bookmarked fixtures and
                        calendar feed.
                    </p>
                    <p>
                        Under UK data protection law you also have the right to
                        request a copy of your data, ask for it to be corrected
                        or erased, and object to its processing. To do so, get
                        in touch using the details below. If you are unhappy
                        with how your data has been handled, you can complain to
                        the{' '}
                        <a
                            href="https://ico.org.uk/make-a-complaint/"
                            target="_blank"
                            rel="noreferrer"
                            className="font-medium text-foreground underline underline-offset-4"
                        >
                            Information Commissioner's Office
                        </a>
                        .
                    </p>
                </section>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        Changes to this policy
                    </h2>
                    <p>
                        If this policy changes, the updated version will be
                        posted on this page with a new "last updated" date.
                    </p>
                </section>

                <section className="space-y-3">
                    <h2 className="text-xl font-semibold tracking-tight text-foreground">
                        Contact
                    </h2>
                    <p>
                        Questions about this policy or your data? Email{' '}
                        <a
                            href="mailto:football-tv-planner@chrispoulter.com"
                            className="font-medium text-foreground underline underline-offset-4"
                        >
                            football-tv-planner@chrispoulter.com
                        </a>
                        .
                    </p>
                </section>
            </article>
        </>
    );
}
