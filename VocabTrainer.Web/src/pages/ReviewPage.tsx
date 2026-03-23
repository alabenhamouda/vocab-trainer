import { useEffect, useState, useCallback, useRef } from 'react';
import { useParams, Link } from 'react-router';
import { getDueReviewEntries, recordReview } from '../api/client';
import { ConfidenceLevel } from '../api/types';
import type { VocabEntryDto } from '../api/types';
import Flashcard from '../components/Flashcard';
import './ReviewPage.css';

const SESSION_SIZE_OPTIONS = [5, 10, 15, 20, 30, 50];

type ReviewPhase = 'setup' | 'reviewing' | 'complete';

export default function ReviewPage() {
    const { deckId } = useParams<{ deckId: string }>();
    const [phase, setPhase] = useState<ReviewPhase>('setup');
    const [sessionSize, setSessionSize] = useState(10);
    const [queue, setQueue] = useState<VocabEntryDto[]>([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [reviewed, setReviewed] = useState(0);
    const [totalDue, setTotalDue] = useState<number | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const abortRef = useRef<AbortController | null>(null);

    // Load due count on mount for the setup screen
    const loadDueCount = useCallback(async () => {
        if (!deckId) return;
        try {
            setLoading(true);
            const result = await getDueReviewEntries(deckId, 1, 1);
            setTotalDue(result.totalCount);
            setError(null);
        } catch (e) {
            setError(e instanceof Error ? e.message : 'Failed to load review data');
        } finally {
            setLoading(false);
        }
    }, [deckId]);

    useEffect(() => {
        loadDueCount();
        return () => { abortRef.current?.abort(); };
    }, [loadDueCount]);

    const startSession = useCallback(async () => {
        if (!deckId) return;

        abortRef.current?.abort();
        const abort = new AbortController();
        abortRef.current = abort;

        try {
            setLoading(true);
            setQueue([]);
            setCurrentIndex(0);
            setReviewed(0);

            const result = await getDueReviewEntries(deckId, 1, sessionSize);

            if (abort.signal.aborted) return;

            const entries = result.items.map(r => r.entry);
            setQueue(entries);
            setTotalDue(result.totalCount);
            setError(null);

            if (entries.length === 0) {
                setPhase('setup');
            } else {
                setPhase('reviewing');
            }
        } catch (e) {
            if (abort.signal.aborted) return;
            setError(e instanceof Error ? e.message : 'Failed to load review entries');
        } finally {
            if (!abort.signal.aborted) setLoading(false);
        }
    }, [deckId, sessionSize]);

    const handleReview = useCallback(async (level: ConfidenceLevel) => {
        if (!deckId || currentIndex >= queue.length) return;

        const entry = queue[currentIndex];

        try {
            await recordReview(deckId, entry.id, level);
        } catch {
            // Continue even if the API call fails
        }

        if (level === ConfidenceLevel.Again) {
            setQueue(prev => [...prev, entry]);
        }

        const newReviewed = reviewed + 1;
        const newIndex = currentIndex + 1;
        setReviewed(newReviewed);
        setCurrentIndex(newIndex);

        // Check if done
        if (newIndex >= queue.length + (level === ConfidenceLevel.Again ? 1 : 0)) {
            setPhase('complete');
        }
    }, [deckId, currentIndex, queue, reviewed]);

    // Check completion in effect (handles queue updates from Again)
    useEffect(() => {
        if (phase === 'reviewing' && currentIndex >= queue.length && queue.length > 0) {
            setPhase('complete');
        }
    }, [currentIndex, queue.length, phase]);

    const progress = queue.length > 0 ? (reviewed / queue.length) * 100 : 0;

    if (error && phase === 'setup') {
        return (
            <div className="review-page page-container">
                <p className="error-message">{error}</p>
                <div style={{ textAlign: 'center' }}>
                    <Link to={`/decks/${deckId}`} className="btn">Back to Deck</Link>
                </div>
            </div>
        );
    }

    return (
        <div className="review-page page-container">
            {/* ─── Setup Phase ─── */}
            {phase === 'setup' && (
                <div className="review-setup">
                    <div className="setup-header">
                        <div className="setup-icon" aria-hidden="true">
                            <svg width="32" height="32" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
                                <path d="M2 3h6a4 4 0 0 1 4 4v14a3 3 0 0 0-3-3H2z"/>
                                <path d="M22 3h-6a4 4 0 0 0-4 4v14a3 3 0 0 1 3-3h7z"/>
                            </svg>
                        </div>
                        <h1 className="setup-title">Review Session</h1>
                        <p className="setup-subtitle">
                            {loading ? (
                                'Checking due cards...'
                            ) : totalDue === 0 ? (
                                'No cards are due for review right now.'
                            ) : (
                                <><span className="setup-due-count">{totalDue}</span> card{totalDue !== 1 ? 's' : ''} due for review</>
                            )}
                        </p>
                    </div>

                    {totalDue !== null && totalDue > 0 && (
                        <>
                            <div className="setup-section">
                                <label className="setup-label">Session size</label>
                                <div className="size-pills">
                                    {SESSION_SIZE_OPTIONS.map(n => (
                                        <button
                                            key={n}
                                            className={`size-pill ${sessionSize === n ? 'active' : ''} ${n > totalDue ? 'dimmed' : ''}`}
                                            onClick={() => setSessionSize(n)}
                                        >
                                            {n}
                                        </button>
                                    ))}
                                </div>
                                {sessionSize > totalDue && (
                                    <p className="setup-note">
                                        Only {totalDue} card{totalDue !== 1 ? 's' : ''} available — you'll review all of them.
                                    </p>
                                )}
                            </div>

                            <button
                                className="btn-start"
                                onClick={startSession}
                                disabled={loading}
                            >
                                {loading ? (
                                    <span className="btn-start-loading">Loading...</span>
                                ) : (
                                    <>
                                        Begin Review
                                        <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                            <path d="M5 12h14"/>
                                            <path d="M12 5l7 7-7 7"/>
                                        </svg>
                                    </>
                                )}
                            </button>
                        </>
                    )}

                    {totalDue === 0 && !loading && (
                        <div className="setup-empty">
                            <div className="setup-empty-icon" aria-hidden="true">
                                <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1" strokeLinecap="round" strokeLinejoin="round">
                                    <circle cx="12" cy="12" r="10"/>
                                    <path d="M8 14s1.5 2 4 2 4-2 4-2"/>
                                    <line x1="9" y1="9" x2="9.01" y2="9"/>
                                    <line x1="15" y1="9" x2="15.01" y2="9"/>
                                </svg>
                            </div>
                            <p>All caught up! Come back later when more cards are due.</p>
                            <Link to={`/decks/${deckId}`} className="btn btn-primary">Back to Deck</Link>
                        </div>
                    )}
                </div>
            )}

            {/* ─── Reviewing Phase ─── */}
            {phase === 'reviewing' && (
                <div className="review-active">
                    <div className="review-header">
                        <div className="review-progress-info">
                            <span className="review-count">{reviewed}<span className="review-count-sep">/</span>{queue.length}</span>
                            {totalDue !== null && totalDue > queue.length && (
                                <span className="review-total-due">{totalDue} total due</span>
                            )}
                        </div>
                        <div className="progress-track">
                            <div
                                className="progress-fill"
                                style={{ width: `${progress}%` }}
                            />
                        </div>
                    </div>

                    {currentIndex < queue.length && (
                        <section className="flashcard-section">
                            <Flashcard
                                key={`${queue[currentIndex].id}-${currentIndex}`}
                                entry={queue[currentIndex]}
                                onReview={handleReview}
                            />
                        </section>
                    )}
                </div>
            )}

            {/* ─── Complete Phase ─── */}
            {phase === 'complete' && (
                <div className="review-complete">
                    <div className="complete-decoration" aria-hidden="true">
                        <span className="confetti c1"></span>
                        <span className="confetti c2"></span>
                        <span className="confetti c3"></span>
                        <span className="confetti c4"></span>
                        <span className="confetti c5"></span>
                    </div>
                    <div className="complete-icon" aria-hidden="true">
                        <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
                            <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
                            <polyline points="22 4 12 14.01 9 11.01"/>
                        </svg>
                    </div>
                    <h2 className="complete-title">Session Complete</h2>
                    <p className="complete-stat">
                        You reviewed <strong>{reviewed}</strong> card{reviewed !== 1 ? 's' : ''}
                    </p>
                    <div className="complete-actions">
                        <button
                            className="btn btn-primary"
                            onClick={() => {
                                setPhase('setup');
                                loadDueCount();
                            }}
                        >
                            Review More
                        </button>
                        <Link to={`/decks/${deckId}`} className="btn">Back to Deck</Link>
                    </div>
                </div>
            )}
        </div>
    );
}
