using System.Collections.Generic;
using System.Linq;
using infinite_words.api.Model;

namespace infinite_words.api.Service;
public interface IColourService
{
    LetterColour GetColour(string word, string guess, int index);
    IList<LetterResult> GetColourResult(string word, string guess);
}

public class ColourService : IColourService
{
    public LetterColour GetColour(string word, string guess, int index)
    {
        if (word[index] == guess[index])
            return LetterColour.Green;
        return word.Contains(guess[index]) ? LetterColour.Yellow : LetterColour.Grey;
    }

    public IList<LetterResult> GetColourResult(string word, string guess)
    {
        var result = Enumerable.Range(0, guess.Length)
            .Select(i => new LetterResult(guess[i], GetColour(word, guess, i), i))
            .ToList();

        return result;
    }
}