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
        <div className="home-page">
            <header className="home-header">
                <h1>VocabTrainer</h1>
                <p className="subtitle">Your German vocabulary learning companion</p>
            </header>

            <section className="decks-section">
                <div className="section-header">
                    <h2>My Decks</h2>
                    <button className="btn btn-primary" onClick={() => setShowForm(!showForm)}>
                        {showForm ? 'Cancel' : '+ New Deck'}
                    </button>
                </div>

                {showForm && (
                    <form className="deck-form" onSubmit={handleCreate}>
                        <input
                            type="text"
                            placeholder="Deck title"
                            value={title}
                            onChange={e => setTitle(e.target.value)}
                            autoFocus
                            required
                        />
                        <input
                            type="text"
                            placeholder="Description (optional)"
                            value={description}
                            onChange={e => setDescription(e.target.value)}
                        />
                        <button className="btn btn-primary" type="submit" disabled={creating || !title.trim()}>
                            {creating ? 'Creating…' : 'Create Deck'}
                        </button>
                    </form>
                )}

                {error && <p className="error-message">{error}</p>}

                {loading ? (
                    <p className="loading">Loading decks…</p>
                ) : decks.length === 0 ? (
                    <div className="empty-state">
                        <p>No decks yet. Create your first deck to get started!</p>
                    </div>
                ) : (
                    <div className="deck-grid">
                        {decks.map(deck => (
                            <Link to={`/decks/${deck.id}`} key={deck.id} className="deck-card">
                                <h3>{deck.title}</h3>
                                {deck.description && <p>{deck.description}</p>}
                            </Link>
                        ))}
                    </div>
                )}
            </section>
        </div>
    );
}
