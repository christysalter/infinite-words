namespace infinite_words.api.Error;

public class GuessTooLongException : GuessLengthOutOfRangeException
{
    public GuessTooLongException(int length) : base($"Guess too long. Was {length} expected <= {Constants.GuessMaxLength}") { }
}