using System;

namespace infinite_words.api.Error;

public class GuessNotInDictionaryException : Exception
{
    public GuessNotInDictionaryException(string guess) : base($"{guess} not in dictionary for words of length {guess.Length}"){ }
}