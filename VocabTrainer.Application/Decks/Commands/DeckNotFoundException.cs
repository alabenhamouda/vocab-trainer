namespace VocabTrainer.Application.Decks.Commands;

public class DeckNotFoundException(Guid deckId) : Exception($"Deck '{deckId}' not found.");
