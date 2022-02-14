using System;

namespace infinite_words.api.Error;

public class WordAlreadyGuessedException : Exception
{
    public WordAlreadyGuessedException(string word) : base($"{word} already guessed") { }
}