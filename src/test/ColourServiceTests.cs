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
        
        Assert.AreEqual(expectedResult, result);
    }

    [TestCase("hello", "world")]
    [TestCase("its", "its")]
    [TestCase("test", "case")]
    public void GetColourResult_ReturnsExpectedResult(string word, string guess)
    {
        word = word.ToLower();
        guess = guess.ToLower();
        
        var result = _testService.GetColourResult(word, guess);
        Assert.AreEqual(guess.Length, result.Count);
        Assert.AreEqual(word.Length, result.Count);
        foreach (var (letter, letterColour, index) in result)
        {
            Assert.AreEqual(letter, guess[index]);
            
            if (word[index] == guess[index])
                Assert.AreEqual(LetterColour.Green, letterColour);
            
            else if (word.Contains(letter))
                Assert.AreEqual(LetterColour.Yellow, letterColour);
            
            else
                Assert.AreEqual(LetterColour.Grey, letterColour);
        }
    }
}