using System;

namespace infinite_words.api.Error;

public class WordGuessLengthMismatchException : Exception
{
    public WordGuessLengthMismatchException(int wordLength, int guessLength) : base($"Guess Length was {guessLength}. Expected {wordLength}") { }
}