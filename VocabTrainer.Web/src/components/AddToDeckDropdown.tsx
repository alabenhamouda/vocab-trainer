import { useState, useRef, useEffect } from 'react';
import './AddToDeckDropdown.css';

export type AddOption = 'course' | 'lesson' | 'entry';

interface AddToDeckDropdownProps {
    onSelect: (option: AddOption) => void;
}

export default function AddToDeckDropdown({ onSelect }: AddToDeckDropdownProps) {
    const [open, setOpen] = useState(false);
    const containerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        function handleClickOutside(e: MouseEvent) {
            if (containerRef.current && !containerRef.current.contains(e.target as Node)) {
                setOpen(false);
            }
        }
        if (open) document.addEventListener('mousedown', handleClickOutside);
        return () => document.removeEventListener('mousedown', handleClickOutside);
    }, [open]);

    function handleSelect(option: AddOption) {
        setOpen(false);
        onSelect(option);
    }

    return (
        <div className="add-dropdown" ref={containerRef}>
            <button className="btn btn-primary" onClick={() => setOpen(!open)}>
                + Add to Deck
            </button>
            {open && (
                <ul className="dropdown-menu" role="menu">
                    <li role="menuitem">
                        <button onClick={() => handleSelect('course')}>Add Course</button>
                    </li>
                    <li role="menuitem">
                        <button onClick={() => handleSelect('lesson')}>Add Lesson</button>
                    </li>
                    <li role="menuitem">
                        <button onClick={() => handleSelect('entry')}>Add Entry</button>
                    </li>
                </ul>
            )}
        </div>
    );
}
