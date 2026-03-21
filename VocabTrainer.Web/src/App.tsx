import { BrowserRouter, Routes, Route } from 'react-router';
import HomePage from './pages/HomePage';
import DeckPage from './pages/DeckPage';
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/decks/:deckId" element={<DeckPage />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
