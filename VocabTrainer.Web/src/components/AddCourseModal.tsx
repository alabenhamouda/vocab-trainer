import { useEffect, useState } from 'react';
import { getCourses, addCoursesToDeck } from '../api/client';
import type { CourseDto } from '../api/types';
import Modal from './Modal';
import './AddCourseModal.css';

interface AddCourseModalProps {
    deckId: string;
    open: boolean;
    onClose: () => void;
    onAdded: () => void;
}

export default function AddCourseModal({ deckId, open, onClose, onAdded }: AddCourseModalProps) {
    const [courses, setCourses] = useState<CourseDto[]>([]);
    const [selected, setSelected] = useState<Set<string>>(new Set());
    const [loading, setLoading] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (!open) return;
        (async () => {
            try {
                setLoading(true);
                setError(null);
                const result = await getCourses();
                setCourses(result.items);
            } catch (e) {
                setError(e instanceof Error ? e.message : 'Failed to load courses');
            } finally {
                setLoading(false);
            }
        })();
    }, [open]);

    function toggle(id: string) {
        setSelected(prev => {
            const next = new Set(prev);
            if (next.has(id)) next.delete(id);
            else next.add(id);
            return next;
        });
    }

    function selectAll() {
        if (selected.size === courses.length) {
            setSelected(new Set());
        } else {
            setSelected(new Set(courses.map(c => c.id)));
        }
    }

    async function handleSubmit() {
        if (selected.size === 0) return;
        try {
            setSubmitting(true);
            setError(null);
            await addCoursesToDeck(deckId, [...selected]);
            setSelected(new Set());
            onAdded();
            onClose();
        } catch (e) {
            setError(e instanceof Error ? e.message : 'Failed to add courses');
        } finally {
            setSubmitting(false);
        }
    }

    function handleClose() {
        setSelected(new Set());
        setError(null);
        onClose();
    }

    return (
        <Modal title="Add Courses to Deck" open={open} onClose={handleClose}>
            {loading ? (
                <p className="loading">Loading courses…</p>
            ) : error ? (
                <p className="error-message">{error}</p>
            ) : courses.length === 0 ? (
                <p className="empty-state">No courses available.</p>
            ) : (
                <>
                    <div className="course-list-header">
                        <button className="btn btn-sm" onClick={selectAll}>
                            {selected.size === courses.length ? 'Deselect All' : 'Select All'}
                        </button>
                        <span className="selection-count">{selected.size} selected</span>
                    </div>
                    <ul className="course-list">
                        {courses.map(course => (
                            <li key={course.id} className="course-item">
                                <label>
                                    <input
                                        type="checkbox"
                                        checked={selected.has(course.id)}
                                        onChange={() => toggle(course.id)}
                                    />
                                    <div className="course-info">
                                        <span className="course-title">{course.title}</span>
                                        {course.description && (
                                            <span className="course-desc">{course.description}</span>
                                        )}
                                    </div>
                                </label>
                            </li>
                        ))}
                    </ul>
                    <div className="modal-actions">
                        <button className="btn" onClick={handleClose}>Cancel</button>
                        <button
                            className="btn btn-primary"
                            onClick={handleSubmit}
                            disabled={selected.size === 0 || submitting}
                        >
                            {submitting ? 'Adding…' : `Add ${selected.size} Course${selected.size !== 1 ? 's' : ''}`}
                        </button>
                    </div>
                </>
            )}
        </Modal>
    );
}
