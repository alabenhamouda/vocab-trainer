import { useState, useEffect } from 'react';
import type { VocabEntryDto, NounDto } from '../api/types';
import { GenderLabels, ConfidenceLevel } from '../api/types';
import './Flashcard.css';

interface FlashcardProps {
    entry: VocabEntryDto;
    index: number;
    total: number;
    onReview?: (level: ConfidenceLevel) => void;
}

const confidenceButtons = [
    { level: ConfidenceLevel.Again, label: 'Again', hint: '< 1 min', className: 'confidence-again' },
    { level: ConfidenceLevel.Hard, label: 'Hard', hint: '< 24h', className: 'confidence-hard' },
    { level: ConfidenceLevel.Good, label: 'Good', hint: '2 days', className: 'confidence-good' },
    { level: ConfidenceLevel.Easy, label: 'Easy', hint: '1 week', className: 'confidence-easy' },
];

function isNoun(entry: VocabEntryDto): entry is NounDto {
    return entry.type === 'Noun';
}

export default function Flashcard({ entry, index, total, onReview }: FlashcardProps) {
    const [flipped, setFlipped] = useState(false);

    useEffect(() => {
        function handleKey(e: KeyboardEvent) {
            if (e.key === 'Enter') setFlipped(f => !f);
        }
        globalThis.addEventListener('keydown', handleKey);
        return () => globalThis.removeEventListener('keydown', handleKey);
    }, []);

    const genderArticle = isNoun(entry) ? GenderLabels[entry.gender] : null;

    return (
        <div className="flashcard-wrapper">
            <div className="flashcard-counter">{index + 1} / {total}</div>
            <div
                className={`flashcard ${flipped ? 'flipped' : ''}`}
                onClick={() => setFlipped(!flipped)}
                onKeyDown={e => { if (e.key === 'Enter' || e.key === ' ') setFlipped(!flipped); }}
                tabIndex={0}
                role="button"
                aria-label={flipped ? 'Show term' : 'Show definition'}
            >
                <div className="flashcard-inner">
                    <div className="flashcard-front">
                        {genderArticle && <span className="gender-badge">{genderArticle}</span>}
                        <span className="term">{entry.term}</span>
                        {isNoun(entry) && entry.pluralForm && (
                            <span className="plural">(Pl. {entry.pluralForm})</span>
                        )}
                        {entry.type !== 'VocabEntry' && (
                            <span className="type-badge">{entry.type}</span>
                        )}
                    </div>
                    <div className="flashcard-back">
                        {entry.definition && <p className="definition">{entry.definition}</p>}
                        {entry.englishTranslation && (
                            <p className="translation">{entry.englishTranslation}</p>
                        )}
                        {entry.example && <p className="example">"{entry.example}"</p>}
                    </div>
                </div>
            </div>
            {onReview && flipped ? (
                <div className="confidence-buttons">
                    {confidenceButtons.map(btn => (
                        <button
                            key={btn.level}
                            className={`confidence-btn ${btn.className}`}
                            onClick={() => onReview(btn.level)}
                        >
                            <span className="confidence-label">{btn.label}</span>
                            <span className="confidence-hint">{btn.hint}</span>
                        </button>
                    ))}
                </div>
            ) : (
                <p className="flip-hint">Click to flip</p>
            )}
        </div>
    );
}
