import { useState, useEffect } from 'react';
import type { VocabEntryDto, NounDto } from '../api/types';
import { GenderLabels, ConfidenceLevel } from '../api/types';
import './Flashcard.css';

interface FlashcardProps {
    entry: VocabEntryDto;
    onReview?: (level: ConfidenceLevel) => void;
}

const confidenceButtons = [
    { level: ConfidenceLevel.Again, label: 'Again', hint: '< 1 min', key: '1', className: 'confidence-again' },
    { level: ConfidenceLevel.Hard, label: 'Hard', hint: '< 24h', key: '2', className: 'confidence-hard' },
    { level: ConfidenceLevel.Good, label: 'Good', hint: '2 days', key: '3', className: 'confidence-good' },
    { level: ConfidenceLevel.Easy, label: 'Easy', hint: '1 week', key: '4', className: 'confidence-easy' },
];

function isNoun(entry: VocabEntryDto): entry is NounDto {
    return entry.type === 'Noun';
}

export default function Flashcard({ entry, onReview }: FlashcardProps) {
    const [flipped, setFlipped] = useState(false);

    useEffect(() => {
        function handleKey(e: KeyboardEvent) {
            if (e.key === 'Enter' || e.key === ' ') {
                e.preventDefault();
                setFlipped(f => !f);
            }
            // Keyboard shortcuts for confidence (1-4) when flipped
            if (flipped && onReview) {
                const btn = confidenceButtons.find(b => b.key === e.key);
                if (btn) {
                    e.preventDefault();
                    onReview(btn.level);
                }
            }
        }
        globalThis.addEventListener('keydown', handleKey);
        return () => globalThis.removeEventListener('keydown', handleKey);
    }, [flipped, onReview]);

    const genderArticle = isNoun(entry) ? GenderLabels[entry.gender] : null;

    return (
        <div className="flashcard-wrapper">
            <div
                className={`flashcard ${flipped ? 'flipped' : ''}`}
                onClick={() => setFlipped(!flipped)}
                onKeyDown={e => { if (e.key === 'Enter' || e.key === ' ') { e.preventDefault(); setFlipped(!flipped); } }}
                tabIndex={0}
                role="button"
                aria-label={flipped ? 'Show term' : 'Show definition'}
            >
                <div className="flashcard-inner">
                    <div className="flashcard-front">
                        <div className="card-content">
                            {genderArticle && <span className="gender-badge">{genderArticle}</span>}
                            <span className="term">{entry.term}</span>
                            {isNoun(entry) && entry.pluralForm && (
                                <span className="plural">Pl. {entry.pluralForm}</span>
                            )}
                        </div>
                        {entry.type !== 'VocabEntry' && (
                            <span className="type-badge">{entry.type}</span>
                        )}
                        <span className="flip-hint">
                            <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                                <polyline points="17 1 21 5 17 9"/>
                                <path d="M3 11V9a4 4 0 0 1 4-4h14"/>
                                <polyline points="7 23 3 19 7 15"/>
                                <path d="M21 13v2a4 4 0 0 1-4 4H3"/>
                            </svg>
                            Tap to reveal
                        </span>
                    </div>
                    <div className="flashcard-back">
                        <div className="card-content">
                            {entry.definition && <p className="definition">{entry.definition}</p>}
                            {entry.englishTranslation && (
                                <p className="translation">{entry.englishTranslation}</p>
                            )}
                            {entry.example && <p className="example">"{entry.example}"</p>}
                        </div>
                    </div>
                </div>
            </div>

            {onReview && flipped ? (
                <div className="confidence-buttons">
                    {confidenceButtons.map(btn => (
                        <button
                            key={btn.level}
                            className={`confidence-btn ${btn.className}`}
                            onClick={(e) => { e.stopPropagation(); onReview(btn.level); }}
                        >
                            <span className="confidence-label">{btn.label}</span>
                            <span className="confidence-hint">{btn.hint}</span>
                            <kbd>{btn.key}</kbd>
                        </button>
                    ))}
                </div>
            ) : onReview ? (
                <p className="review-flip-hint">
                    Press <kbd>Enter</kbd> or tap card to reveal
                </p>
            ) : null}
        </div>
    );
}
