using System;

namespace infinite_words.api.Error;

public class TokenCouldNotBeDecodedException : Exception
{
    public TokenCouldNotBeDecodedException() : base("Token could not be decoded. This can happen if the token is from a previous day.") { }
}