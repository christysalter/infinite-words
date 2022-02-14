using System;

namespace infinite_words.api.Error;

public abstract class GuessLengthOutOfRangeException : Exception
{
    protected GuessLengthOutOfRangeException(string message) : base (message) { }
}