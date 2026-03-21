import { useEffect, useState, useCallback } from 'react';
import { useParams, Link } from 'react-router';
import { getDeckVocab } from '../api/client';
import type { VocabEntryDto } from '../api/types';
import Flashcard from '../components/Flashcard';
import AddToDeckDropdown from '../components/AddToDeckDropdown';
import type { AddOption } from '../components/AddToDeckDropdown';
import AddCourseModal from '../components/AddCourseModal';
import Modal from '../components/Modal';
import './DeckPage.css';

export default function DeckPage() {
    const { deckId } = useParams<{ deckId: string }>();
    const [entries, setEntries] = useState<VocabEntryDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [infoExpanded, setInfoExpanded] = useState(false);
    const [courseModalOpen, setCourseModalOpen] = useState(false);
    const [comingSoonModal, setComingSoonModal] = useState<string | null>(null);

    const loadVocab = useCallback(async () => {
        if (!deckId) return;
        try {
            setLoading(true);
            const result = await getDeckVocab(deckId, 1, 100);
            setEntries(result.items);
            setError(null);
        } catch (e) {
            setError(e instanceof Error ? e.message : 'Failed to load vocabulary');
        } finally {
            setLoading(false);
        }
    }, [deckId]);

    useEffect(() => { loadVocab(); }, [loadVocab]);

    const prev = useCallback(() => {
        setCurrentIndex(i => (i > 0 ? i - 1 : entries.length - 1));
    }, [entries.length]);

    const next = useCallback(() => {
        setCurrentIndex(i => (i < entries.length - 1 ? i + 1 : 0));
    }, [entries.length]);

    useEffect(() => {
        function handleKey(e: KeyboardEvent) {
            if (e.key === 'ArrowLeft') prev();
            if (e.key === 'ArrowRight') next();
        }
        window.addEventListener('keydown', handleKey);
        return () => window.removeEventListener('keydown', handleKey);
    }, [prev, next]);

    if (loading) return <div className="deck-page"><p className="loading">Loading…</p></div>;
    if (error) return <div className="deck-page"><p className="error-message">{error}</p></div>;

    function handleAddOption(option: AddOption) {
        if (option === 'course') setCourseModalOpen(true);
        else setComingSoonModal(option === 'lesson' ? 'Add Lesson' : 'Add Entry');
    }

    return (
        <div className="deck-page">
            <nav className="deck-nav">
                <Link to="/" className="back-link">← Back to decks</Link>
                <AddToDeckDropdown onSelect={handleAddOption} />
            </nav>

            {entries.length === 0 ? (
                <div className="empty-state">
                    <p>This deck has no vocabulary entries yet. Add lessons, courses, or individual entries to get started.</p>
                </div>
            ) : (
                <section className="flashcard-section">
                    <Flashcard
                        key={entries[currentIndex].id}
                        entry={entries[currentIndex]}
                        index={currentIndex}
                        total={entries.length}
                    />
                    <div className="nav-buttons">
                        <button className="btn" onClick={prev} aria-label="Previous card">← Prev</button>
                        <button className="btn" onClick={next} aria-label="Next card">Next →</button>
                    </div>
                </section>
            )}

            <section className="info-section">
                <button
                    className="info-toggle"
                    onClick={() => setInfoExpanded(!infoExpanded)}
                    aria-expanded={infoExpanded}
                >
                    <span>Deck Info</span>
                    <span className={`chevron ${infoExpanded ? 'open' : ''}`}>▸</span>
                </button>
                {infoExpanded && (
                    <div className="info-content">
                        <p><strong>Total entries:</strong> {entries.length}</p>
                        <p>More details coming soon — review stats, lesson list, and progress tracking.</p>
                    </div>
                )}
            </section>

            {deckId && (
                <AddCourseModal
                    deckId={deckId}
                    open={courseModalOpen}
                    onClose={() => setCourseModalOpen(false)}
                    onAdded={() => { setCurrentIndex(0); loadVocab(); }}
                />
            )}

            <Modal
                title={comingSoonModal ?? ''}
                open={comingSoonModal !== null}
                onClose={() => setComingSoonModal(null)}
            >
                <p style={{ textAlign: 'center', color: '#888' }}>Coming soon!</p>
            </Modal>
        </div>
    );
}
