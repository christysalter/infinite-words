namespace infinite_words.api.Model;

public record LetterResult(char Letter, LetterColour Colour, int Index)
{
    public override string ToString() => $"{Letter}: {Colour}";
}