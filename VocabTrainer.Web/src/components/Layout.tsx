import { Link, useLocation, useParams } from 'react-router';
import './Layout.css';

interface LayoutProps {
    children: React.ReactNode;
}

function Breadcrumbs() {
    const location = useLocation();
    const { deckId } = useParams<{ deckId: string }>();
    const path = location.pathname;

    const isHome = path === '/';
    const isDeck = deckId && !path.endsWith('/review');
    const isReview = deckId && path.endsWith('/review');

    return (
        <nav className="breadcrumb" aria-label="Breadcrumb">
            {isHome ? (
                <span className="breadcrumb-item active">My Decks</span>
            ) : (
                <Link to="/" className="breadcrumb-item">My Decks</Link>
            )}
            {(isDeck || isReview) && (
                <>
                    <span className="breadcrumb-sep" aria-hidden="true">
                        <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
                            <path d="M6 4l4 4-4 4" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                    </span>
                    {isReview ? (
                        <Link to={`/decks/${deckId}`} className="breadcrumb-item">Deck</Link>
                    ) : (
                        <span className="breadcrumb-item active">Deck</span>
                    )}
                </>
            )}
            {isReview && (
                <>
                    <span className="breadcrumb-sep" aria-hidden="true">
                        <svg width="14" height="14" viewBox="0 0 16 16" fill="none">
                            <path d="M6 4l4 4-4 4" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round"/>
                        </svg>
                    </span>
                    <span className="breadcrumb-item active">Review</span>
                </>
            )}
        </nav>
    );
}

export default function Layout({ children }: LayoutProps) {
    return (
        <>
            <header className="app-header">
                <Link to="/" className="app-brand">
                    <span className="app-brand-icon" aria-hidden="true">V</span>
                    <span className="app-brand-name">VocabTrainer</span>
                </Link>
                <Breadcrumbs />
            </header>
            <main className="app-main">{children}</main>
        </>
    );
}
