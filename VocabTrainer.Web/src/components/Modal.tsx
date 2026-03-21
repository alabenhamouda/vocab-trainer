import { useEffect, useRef } from 'react';
import './Modal.css';

interface ModalProps {
    title: string;
    open: boolean;
    onClose: () => void;
    children: React.ReactNode;
}

export default function Modal({ title, open, onClose, children }: ModalProps) {
    const dialogRef = useRef<HTMLDialogElement>(null);

    useEffect(() => {
        const dialog = dialogRef.current;
        if (!dialog) return;
        if (open && !dialog.open) dialog.showModal();
        if (!open && dialog.open) dialog.close();
    }, [open]);

    return (
        <dialog
            ref={dialogRef}
            className="modal-dialog"
            onClose={onClose}
            onClick={e => { if (e.target === dialogRef.current) onClose(); }}
        >
            <div className="modal-content">
                <div className="modal-header">
                    <h3>{title}</h3>
                    <button className="modal-close" onClick={onClose} aria-label="Close">×</button>
                </div>
                <div className="modal-body">
                    {children}
                </div>
            </div>
        </dialog>
    );
}
