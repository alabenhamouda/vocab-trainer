import { useEffect, useState, useCallback, useRef } from 'react';
import { useParams, Link } from 'react-router';
import { getDeckVocab } from '../api/client';
import type { VocabEntryDto } from '../api/types';
import Flashcard from '../components/Flashcard';
import AddToDeckDropdown from '../components/AddToDeckDropdown';
import type { AddOption } from '../components/AddToDeckDropdown';
import AddCourseModal from '../components/AddCourseModal';
import Modal from '../components/Modal';
import './DeckPage.css';

const PAGE_SIZE = 100;

export default function DeckPage() {
    const { deckId } = useParams<{ deckId: string }>();
    const [entries, setEntries] = useState<VocabEntryDto[]>([]);
    const [totalCount, setTotalCount] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [infoExpanded, setInfoExpanded] = useState(false);
    const [courseModalOpen, setCourseModalOpen] = useState(false);
    const [comingSoonModal, setComingSoonModal] = useState<string | null>(null);
    const abortRef = useRef<AbortController | null>(null);

    const loadVocab = useCallback(async () => {
        if (!deckId) return;

        abortRef.current?.abort();
        const abort = new AbortController();
        abortRef.current = abort;

        try {
            setLoading(true);
            setEntries([]);
            setTotalCount(null);

            let page = 1;
            let hasNext = true;

            while (hasNext) {
                if (abort.signal.aborted) return;

                const result = await getDeckVocab(deckId, page, PAGE_SIZE);

                if (abort.signal.aborted) return;

                setEntries(prev => [...prev, ...result.items]);
                setTotalCount(result.totalCount);
                hasNext = result.hasNextPage;
                page++;

                if (page === 2) setLoading(false);
            }

            setError(null);
        } catch (e) {
            if (abort.signal.aborted) return;
            setError(e instanceof Error ? e.message : 'Failed to load vocabulary');
        } finally {
            if (!abort.signal.aborted) setLoading(false);
        }
    }, [deckId]);

    useEffect(() => {
        loadVocab();
        return () => { abortRef.current?.abort(); };
    }, [loadVocab]);

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
        globalThis.addEventListener('keydown', handleKey);
        return () => globalThis.removeEventListener('keydown', handleKey);
    }, [prev, next]);

    if (loading) return <div className="deck-page"><p className="loading">Loading...</p></div>;
    if (error) return <div className="deck-page"><p className="error-message">{error}</p></div>;

    function handleAddOption(option: AddOption) {
        if (option === 'course') setCourseModalOpen(true);
        else setComingSoonModal(option === 'lesson' ? 'Add Lesson' : 'Add Entry');
    }

    return (
        <div className="deck-page page-container">
            <nav className="deck-nav">
                <div className="deck-nav-actions">
                    <Link to={`/decks/${deckId}/review`} className="btn btn-primary">
                        <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                            <path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/>
                            <path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/>
                        </svg>
                        Review
                    </Link>
                    <AddToDeckDropdown onSelect={handleAddOption} />
                </div>
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
                    />
                    <div className="nav-buttons">
                        <button className="nav-btn" onClick={prev} aria-label="Previous card">
                            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <path d="M19 12H5"/><path d="M12 19l-7-7 7-7"/>
                            </svg>
                            Prev
                        </button>
                        <span className="nav-counter">{currentIndex + 1} / {entries.length}</span>
                        <button className="nav-btn" onClick={next} aria-label="Next card">
                            Next
                            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <path d="M5 12h14"/><path d="M12 5l7 7-7 7"/>
                            </svg>
                        </button>
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
                    <svg
                        className={`info-chevron ${infoExpanded ? 'open' : ''}`}
                        width="16" height="16" viewBox="0 0 24 24" fill="none"
                        stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round"
                    >
                        <polyline points="6 9 12 15 18 9"/>
                    </svg>
                </button>
                {infoExpanded && (
                    <div className="info-content">
                        <p><strong>Total entries:</strong> {totalCount ?? entries.length}{totalCount !== null && entries.length < totalCount && ` (${entries.length} loaded...)`}</p>
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
                <p className="coming-soon-text">Coming soon!</p>
            </Modal>
        </div>
    );
}
