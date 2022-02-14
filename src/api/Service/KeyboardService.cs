using System.Collections.Generic;
using System.Linq;
using infinite_words.api.Model;

namespace infinite_words.api.Service;
public interface IKeyboardService
{
    IDictionary<char, LetterColour> GetKeyboard(IList<string> guesses, string word);
}

public class KeyboardService : IKeyboardService
{
    public IDictionary<char, LetterColour> GetKeyboard(IList<string> guesses, string word)
    {
        var distinctLetters = guesses
            .SelectMany(g => g)
            .Distinct();

        var dict = Constants.Letters.ToDictionary(key => key, val =>
        {
            if (!word.Contains(val))
                return LetterColour.Grey;

            if (!distinctLetters.Contains(val))
                return LetterColour.Grey;

            return word.Where((t, i) => t == val && guesses.Any(g => g[i] == val)).Any() ? LetterColour.Green : LetterColour.Yellow;
        });

        return dict;
    }
}