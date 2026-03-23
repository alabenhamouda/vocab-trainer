import { useEffect, useState, useCallback, useRef } from 'react';
import { useParams, Link } from 'react-router';
import { getDueReviewEntries, recordReview } from '../api/client';
import { ConfidenceLevel } from '../api/types';
import type { VocabEntryDto } from '../api/types';
import Flashcard from '../components/Flashcard';
import './ReviewPage.css';

const DEFAULT_SESSION_SIZE = 10;
const SESSION_SIZE_OPTIONS = [5, 10, 15, 20, 30, 50];

export default function ReviewPage() {
    const { deckId } = useParams<{ deckId: string }>();
    const [sessionSize, setSessionSize] = useState(DEFAULT_SESSION_SIZE);
    const [queue, setQueue] = useState<VocabEntryDto[]>([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [reviewed, setReviewed] = useState(0);
    const [totalDue, setTotalDue] = useState(0);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const abortRef = useRef<AbortController | null>(null);

    const loadDueEntries = useCallback(async (size: number) => {
        if (!deckId) return;

        abortRef.current?.abort();
        const abort = new AbortController();
        abortRef.current = abort;

        try {
            setLoading(true);
            setQueue([]);
            setCurrentIndex(0);
            setReviewed(0);
            setTotalDue(0);

            const result = await getDueReviewEntries(deckId, 1, size);

            if (abort.signal.aborted) return;

            setQueue(result.items.map(r => r.entry));
            setTotalDue(result.totalCount);
            setError(null);
        } catch (e) {
            if (abort.signal.aborted) return;
            setError(e instanceof Error ? e.message : 'Failed to load review entries');
        } finally {
            if (!abort.signal.aborted) setLoading(false);
        }
    }, [deckId]);

    useEffect(() => {
        loadDueEntries(sessionSize);
        return () => { abortRef.current?.abort(); };
    }, [loadDueEntries, sessionSize]);

    const handleReview = useCallback(async (level: ConfidenceLevel) => {
        if (!deckId || currentIndex >= queue.length) return;

        const entry = queue[currentIndex];

        try {
            await recordReview(deckId, entry.id, level);
        } catch {
            // Continue even if the API call fails — don't block the session
        }

        if (level === ConfidenceLevel.Again) {
            setQueue(prev => [...prev, entry]);
        }

        setReviewed(r => r + 1);
        setCurrentIndex(i => i + 1);
    }, [deckId, currentIndex, queue]);

    const done = !loading && currentIndex >= queue.length;

    if (loading) return <div className="review-page"><p className="loading">Loading review…</p></div>;
    if (error) return <div className="review-page"><p className="error-message">{error}</p></div>;

    return (
        <div className="review-page">
            <nav className="review-nav">
                <Link to={`/decks/${deckId}`} className="back-link">← Back to deck</Link>
                {!done && (
                    <div className="review-nav-right">
                        <label className="session-size-label">
                            Cards:
                            <select
                                className="session-size-select"
                                value={sessionSize}
                                onChange={e => setSessionSize(Number(e.target.value))}
                            >
                                {SESSION_SIZE_OPTIONS.map(n => (
                                    <option key={n} value={n}>{n}</option>
                                ))}
                            </select>
                        </label>
                        <span className="review-progress">
                            {reviewed} / {queue.length} reviewed
                            {totalDue > queue.length && (
                                <span className="total-due"> ({totalDue} due)</span>
                            )}
                        </span>
                    </div>
                )}
            </nav>

            {queue.length === 0 ? (
                <div className="empty-state">
                    <p>No cards due for review. Come back later!</p>
                </div>
            ) : done ? (
                <div className="review-done">
                    <h2>All caught up!</h2>
                    <p>You reviewed {reviewed} card{reviewed !== 1 ? 's' : ''} this session.</p>
                    <Link to={`/decks/${deckId}`} className="btn btn-primary">Back to deck</Link>
                </div>
            ) : (
                <section className="flashcard-section">
                    <Flashcard
                        key={`${queue[currentIndex].id}-${currentIndex}`}
                        entry={queue[currentIndex]}
                        index={currentIndex}
                        total={queue.length}
                        onReview={handleReview}
                    />
                </section>
            )}
        </div>
    );
}
