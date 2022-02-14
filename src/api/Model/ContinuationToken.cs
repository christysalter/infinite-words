using System.Collections.Generic;

namespace infinite_words.api.Model;

public record ContinuationToken(string GeneratedAt,
    int WordLength,
    int GameNumber,
    string Token,
    string Nonce,
    IList<string> Guesses,
    bool LastGameWon);