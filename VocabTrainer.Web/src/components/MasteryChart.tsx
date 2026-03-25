import { useEffect, useState } from 'react';
import { getDeckReviewStats } from '../api/client';
import type { DeckReviewStatsDto } from '../api/types';
import './MasteryChart.css';

interface MasteryChartProps {
    deckId: string;
}

interface Segment {
    key: string;
    label: string;
    count: number;
    color: string;
}

export default function MasteryChart({ deckId }: MasteryChartProps) {
    const [stats, setStats] = useState<DeckReviewStatsDto | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        let cancelled = false;
        (async () => {
            try {
                setLoading(true);
                const result = await getDeckReviewStats(deckId);
                if (!cancelled) setStats(result);
            } catch {
                // Silently fail — chart is non-critical
            } finally {
                if (!cancelled) setLoading(false);
            }
        })();
        return () => { cancelled = true; };
    }, [deckId]);

    if (loading || !stats || stats.totalEntries === 0) return null;

    const segments: Segment[] = [
        { key: 'new', label: 'New', count: stats.new, color: 'var(--color-text-muted)' },
        { key: 'again', label: 'Again', count: stats.again, color: 'var(--color-confidence-again)' },
        { key: 'hard', label: 'Hard', count: stats.hard, color: 'var(--color-confidence-hard)' },
        { key: 'good', label: 'Good', count: stats.good, color: 'var(--color-confidence-good)' },
        { key: 'easy', label: 'Easy', count: stats.easy, color: 'var(--color-confidence-easy)' },
    ];

    const total = stats.totalEntries;

    return (
        <section className="mastery-chart">
            <div className="mastery-header">
                <h3 className="mastery-title">Mastery</h3>
                <span className="mastery-total">{total} cards</span>
            </div>

            <div className="mastery-bars">
                {segments.map(seg => {
                    const pct = total > 0 ? (seg.count / total) * 100 : 0;
                    return (
                        <div key={seg.key} className="mastery-bar-col">
                            <span className="mastery-bar-count">{seg.count}</span>
                            <div className="mastery-bar-track">
                                <div
                                    className="mastery-bar-fill"
                                    style={{
                                        height: `${pct}%`,
                                        backgroundColor: seg.color,
                                    }}
                                />
                            </div>
                            <span className="mastery-bar-label">{seg.label}</span>
                        </div>
                    );
                })}
            </div>
        </section>
    );
}
