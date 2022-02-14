using System.Collections.Generic;

namespace infinite_words.api.Model;

public record GuessResult(int WordLength,
        IList<LetterResult> Result,
        bool CorrectWord,
        IDictionary<char, LetterColour> KeyBoard,
        string ContinuationToken,
        int GameNumber,
        int NumberOfGuesses);