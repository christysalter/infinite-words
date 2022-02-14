using System.Collections.Generic;
using infinite_words.api.Configuration;
using infinite_words.api.Model;
using infinite_words.api.Service;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace infinite_words.api.test;

public class GameServiceTests
{
    private IGameService _gameService = null!;

    [OneTimeSetUp]
    public void SetUp()
    {
        var wordList = new List<string> { "hello", "world", "today", "dizzy", "quick", "jumps", "aloha" };
        var validWordsService = new FakeValidWordsService(wordList);
        var wordService = new FakeWordService("hello");
        var validationService = new ValidationService(wordService);
        var colourService = new ColourService();
        var keyboardService = new KeyboardService();
        var encryptionService = new EncryptionService();
        var secretConfig = new SecretConfig("test-salt");
        var tokenService = new ContinuationTokenService(encryptionService, secretConfig);
        var guessService = new GuessService(colourService, keyboardService, tokenService);

        _gameService = new GameService(validationService, validWordsService, guessService, tokenService, wordService, new NullLogger<GameService>());
    }
    
    [Test]
    public void CompleteGame_HappyPath()
    {
        var guess1 = _gameService.PlayGame("world", string.Empty);
        Assert.AreEqual(LetterColour.Green, guess1.KeyBoard['l']);
        Assert.AreEqual(LetterColour.Grey, guess1.KeyBoard['d']);
        Assert.AreEqual(LetterColour.Yellow, guess1.KeyBoard['o']);
        Assert.AreEqual(1, guess1.GameNumber);
        Assert.AreEqual(1, guess1.NumberOfGuesses);
        
        var guess2 = _gameService.PlayGame("dizzy", guess1.ContinuationToken);
        Assert.AreEqual(1, guess2.GameNumber);
        Assert.AreEqual(2, guess2.NumberOfGuesses);

        var guess3 = _gameService.PlayGame("hello", guess2.ContinuationToken);
        Assert.IsTrue(guess3.CorrectWord);
        Assert.AreEqual(1, guess3.GameNumber);
        Assert.AreEqual(LetterColour.Green, guess3.KeyBoard['h']);
        Assert.AreEqual(LetterColour.Green, guess3.KeyBoard['e']);
        Assert.AreEqual(LetterColour.Green, guess3.KeyBoard['l']);
        Assert.AreEqual(LetterColour.Green, guess3.KeyBoard['l']);
        Assert.AreEqual(LetterColour.Green, guess3.KeyBoard['o']);

        var guess4 = _gameService.PlayGame("quick", guess3.ContinuationToken);
        Assert.AreEqual(2, guess4.GameNumber);
        Assert.AreEqual(1, guess4.NumberOfGuesses);
        Assert.IsFalse(guess4.CorrectWord);

        var guess5 = _gameService.PlayGame("aloha", guess4.ContinuationToken);
        Assert.AreEqual(2, guess5.GameNumber);
        Assert.AreEqual(2, guess5.NumberOfGuesses);
        Assert.IsFalse(guess5.CorrectWord);
    }

    [Test]
    public void TwoGamesInARow_GuessedOnFirstGuess()
    {
        var guess1 = _gameService.PlayGame("hello", string.Empty);
        Assert.AreEqual(1, guess1.GameNumber);
        Assert.AreEqual(1, guess1.NumberOfGuesses);
        Assert.IsTrue(guess1.CorrectWord);
        
        var guess2 = _gameService.PlayGame("hello", guess1.ContinuationToken);
        Assert.AreEqual(2, guess2.GameNumber);
        Assert.AreEqual(1, guess2.NumberOfGuesses);
        Assert.IsTrue(guess2.CorrectWord);
        
        var guess3 = _gameService.PlayGame("hello", guess2.ContinuationToken);
        Assert.AreEqual(3, guess3.GameNumber);
        Assert.AreEqual(1, guess3.NumberOfGuesses);
        Assert.IsTrue(guess3.CorrectWord);
    }
}