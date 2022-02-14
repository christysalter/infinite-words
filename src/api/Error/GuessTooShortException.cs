namespace infinite_words.api.Error;

public class GuessTooShortException : GuessLengthOutOfRangeException
{
    public GuessTooShortException(int length) : base($"Guess too short. Was {length} expected >= {Constants.GuessMinLength}") { }
}