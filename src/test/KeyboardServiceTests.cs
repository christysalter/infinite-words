using System.Collections.Generic;
using System.Linq;
using infinite_words.api.Model;
using infinite_words.api.Service;
using NUnit.Framework;

namespace infinite_words.api.test;

public class KeyboardServiceTests
{
    private readonly IKeyboardService _keyboardService = new KeyboardService();

    [TestCase("wings", "world,waive", "w", "i")]
    public void Test(string word, string guesses, string green, string yellow)
    {
        var keyboard = _keyboardService.GetKeyboard(guesses.Split(','), word);

        KeyboardAssertions(keyboard, green.ToCharArray(), yellow.ToCharArray());
    }

    private void KeyboardAssertions(IDictionary<char, LetterColour> keyboard, char[] green, char[] yellow)
    {
        foreach (var greenChar in green)
            Assert.That(keyboard[greenChar], Is.EqualTo(LetterColour.Green),
                $"Expected {LetterColour.Green} for letter '{greenChar}' but found {keyboard[greenChar]}");
        
        foreach (var yellowChar in yellow)
            Assert.That(keyboard[yellowChar], Is.EqualTo(LetterColour.Yellow),
                $"Expected {LetterColour.Yellow} for letter '{yellowChar}' but found {keyboard[yellowChar]}");

        var grey = Constants.Letters.Except(green).Except(yellow);

        foreach (var greyChar in grey)
            Assert.That(keyboard[greyChar], Is.EqualTo(LetterColour.Grey),
                $"Expected {LetterColour.Grey} for letter '{greyChar}' but found {keyboard[greyChar]}");
    }
}
