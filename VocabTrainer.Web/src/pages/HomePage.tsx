import { useEffect, useState } from 'react';
import { Link } from 'react-router';
import { getDecks, createDeck } from '../api/client';
import type { DeckDto } from '../api/types';
import './HomePage.css';

export default function HomePage() {
    const [decks, setDecks] = useState<DeckDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [showForm, setShowForm] = useState(false);
    const [title, setTitle] = useState('');
    const [description, setDescription] = useState('');
    const [creating, setCreating] = useState(false);

    async function loadDecks() {
        try {
            setLoading(true);
            const result = await getDecks();
            setDecks(result.items);
            setError(null);
        } catch (e) {
            setError(e instanceof Error ? e.message : 'Failed to load decks');
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => { loadDecks(); }, []);

    async function handleCreate(e: React.FormEvent) {
        e.preventDefault();
        if (!title.trim()) return;
        try {
            setCreating(true);
            await createDeck(title.trim(), description.trim() || null);
            setTitle('');
            setDescription('');
            setShowForm(false);
            await loadDecks();
        } catch (err) {
            setError(err instanceof Error ? err.message : 'Failed to create deck');
        } finally {
            setCreating(false);
        }
    }

    return (
        <div className="home-page page-container">
            <section className="decks-section">
                <div className="section-header">
                    <div className="section-header-text">
                        <h2>My Decks</h2>
                        <p className="section-subtitle">{decks.length > 0 ? `${decks.length} deck${decks.length !== 1 ? 's' : ''}` : 'Get started by creating a deck'}</p>
                    </div>
                    <button
                        className={`btn ${showForm ? 'btn-ghost' : 'btn-primary'}`}
                        onClick={() => setShowForm(!showForm)}
                    >
                        {showForm ? (
                            <>
                                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/>
                                </svg>
                                Cancel
                            </>
                        ) : (
                            <>
                                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                    <line x1="12" y1="5" x2="12" y2="19"/><line x1="5" y1="12" x2="19" y2="12"/>
                                </svg>
                                New Deck
                            </>
                        )}
                    </button>
                </div>

                {showForm && (
                    <form className="deck-form" onSubmit={handleCreate}>
                        <div className="form-fields">
                            <input
                                type="text"
                                placeholder="Deck title"
                                value={title}
                                onChange={e => setTitle(e.target.value)}
                                autoFocus
                                required
                                className="form-input"
                            />
                            <input
                                type="text"
                                placeholder="Description (optional)"
                                value={description}
                                onChange={e => setDescription(e.target.value)}
                                className="form-input"
                            />
                        </div>
                        <button className="btn btn-primary" type="submit" disabled={creating || !title.trim()}>
                            {creating ? 'Creating...' : 'Create Deck'}
                        </button>
                    </form>
                )}

                {error && <p className="error-message">{error}</p>}

                {loading ? (
                    <p className="loading">Loading decks...</p>
                ) : decks.length === 0 ? (
                    <div className="empty-state-home">
                        <div className="empty-illustration" aria-hidden="true">
                            <div className="empty-card empty-card-1"></div>
                            <div className="empty-card empty-card-2"></div>
                            <div className="empty-card empty-card-3"></div>
                        </div>
                        <p>No decks yet. Create your first deck to get started!</p>
                    </div>
                ) : (
                    <div className="deck-grid">
                        {decks.map((deck, i) => (
                            <Link
                                to={`/decks/${deck.id}`}
                                key={deck.id}
                                className="deck-card"
                                style={{ animationDelay: `${i * 0.06}s` }}
                            >
                                <div className="deck-card-accent" aria-hidden="true" />
                                <h3>{deck.title}</h3>
                                {deck.description && <p className="deck-card-desc">{deck.description}</p>}
                                <span className="deck-card-arrow" aria-hidden="true">
                                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                        <path d="M5 12h14"/><path d="M12 5l7 7-7 7"/>
                                    </svg>
                                </span>
                            </Link>
                        ))}
                    </div>
                )}
            </section>
        </div>
    );
}
