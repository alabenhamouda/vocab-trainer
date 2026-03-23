import { BrowserRouter, Routes, Route } from 'react-router';
import HomePage from './pages/HomePage';
import DeckPage from './pages/DeckPage';
import ReviewPage from './pages/ReviewPage';
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/decks/:deckId" element={<DeckPage />} />
                <Route path="/decks/:deckId/review" element={<ReviewPage />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
