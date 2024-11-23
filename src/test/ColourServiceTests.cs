using infinite_words.api.Model;
using infinite_words.api.Service;
using NUnit.Framework;

namespace infinite_words.api.test;

public class ColourServiceTests
{
    private readonly IColourService _testService = new ColourService();

    [TestCase("hello", "world", 3, LetterColour.Green)]
    [TestCase("hello", "world", 1, LetterColour.Grey)]
    [TestCase("hello", "world", 2, LetterColour.Yellow)]
    public void GetColour_ReturnsExpectedResult(string guess, string word, int i, LetterColour expectedResult)
    {
        var result = _testService.GetColour(word, guess, i);
        
        Assert.That(result, Is.EqualTo(expectedResult));
    }

    [TestCase("hello", "world")]
    [TestCase("its", "its")]
    [TestCase("test", "case")]
    public void GetColourResult_ReturnsExpectedResult(string word, string guess)
    {
        word = word.ToLower();
        guess = guess.ToLower();
        
        var result = _testService.GetColourResult(word, guess);
        Assert.That(result.Count, Is.EqualTo(guess.Length));
        Assert.That(result.Count, Is.EqualTo(word.Length));
        foreach (var (letter, letterColour, index) in result)
        {
            Assert.That(guess[index], Is.EqualTo(letter));
            
            if (word[index] == guess[index])
                Assert.That(letterColour, Is.EqualTo(LetterColour.Green));
            
            else if (word.Contains(letter))
                Assert.That(letterColour, Is.EqualTo(LetterColour.Yellow));
            
            else
                Assert.That(letterColour, Is.EqualTo(LetterColour.Grey));
        }
    }
}