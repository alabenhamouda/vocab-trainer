import type { CourseDto, DeckDto, PaginatedList, VocabEntryDto } from './types';

const BASE = '/api';

async function handleResponse<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
  return res.json();
}

export async function getDecks(page = 1, pageSize = 50): Promise<PaginatedList<DeckDto>> {
  const res = await fetch(`${BASE}/decks?page=${page}&pageSize=${pageSize}`);
  return handleResponse(res);
}

export async function createDeck(title: string, description: string | null): Promise<string> {
  const res = await fetch(`${BASE}/decks`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ title, description }),
  });
  return handleResponse(res);
}

export async function getDeckVocab(
  deckId: string,
  page = 1,
  pageSize = 50,
): Promise<PaginatedList<VocabEntryDto>> {
  const res = await fetch(`${BASE}/decks/${deckId}/vocab?page=${page}&pageSize=${pageSize}`);
  return handleResponse(res);
}

export async function getCourses(page = 1, pageSize = 100): Promise<PaginatedList<CourseDto>> {
  const res = await fetch(`${BASE}/courses?page=${page}&pageSize=${pageSize}`);
  return handleResponse(res);
}

export async function addCoursesToDeck(deckId: string, courseIds: string[]): Promise<void> {
  const res = await fetch(`${BASE}/decks/${deckId}/courses`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ courseIds }),
  });
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || res.statusText);
  }
}
