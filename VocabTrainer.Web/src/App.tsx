import { BrowserRouter, Routes, Route } from 'react-router';
import Layout from './components/Layout';
import HomePage from './pages/HomePage';
import DeckPage from './pages/DeckPage';
import ReviewPage from './pages/ReviewPage';
import './App.css';

function App() {
    return (
        <BrowserRouter>
            <Layout>
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/decks/:deckId" element={<DeckPage />} />
                    <Route path="/decks/:deckId/review" element={<ReviewPage />} />
                </Routes>
            </Layout>
        </BrowserRouter>
    );
}

export default App;
