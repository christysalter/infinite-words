using System;

namespace infinite_words.api.Error;

public class GuessContainsInvalidCharactersException : Exception
{
    public GuessContainsInvalidCharactersException() : base("Guess contains invalid characters. Only [a-z] is valid") { }
}