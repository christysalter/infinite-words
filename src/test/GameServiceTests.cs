using System.Collections.Generic;
using infinite_words.api.Configuration;
using infinite_words.api.Model;
using infinite_words.api.Service;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Extensions;
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
        Assert.That(LetterColour.Green, Is.EqualTo(guess1.KeyBoard['l']));
        Assert.That(LetterColour.Grey, Is.EqualTo(guess1.KeyBoard['d']));
        Assert.That(LetterColour.Yellow, Is.EqualTo( guess1.KeyBoard['o']));
        Assert.That(guess1.GameNumber, Is.EqualTo(1));
        Assert.That(guess1.NumberOfGuesses, Is.EqualTo(1));
        
        var guess2 = _gameService.PlayGame("dizzy", guess1.ContinuationToken);
        Assert.That(guess2.GameNumber, Is.EqualTo(1));
        Assert.That(guess2.NumberOfGuesses, Is.EqualTo(2));

        var guess3 = _gameService.PlayGame("hello", guess2.ContinuationToken);
        Assert.That(guess3.CorrectWord, Is.True);
        Assert.That(guess3.GameNumber, Is.EqualTo(1));
        Assert.That(guess3.KeyBoard['h'], Is.EqualTo(LetterColour.Green));
        Assert.That(guess3.KeyBoard['e'], Is.EqualTo(LetterColour.Green));
        Assert.That(guess3.KeyBoard['l'], Is.EqualTo(LetterColour.Green));
        Assert.That(guess3.KeyBoard['l'], Is.EqualTo(LetterColour.Green));
        Assert.That(guess3.KeyBoard['o'], Is.EqualTo(LetterColour.Green));

        var guess4 = _gameService.PlayGame("quick", guess3.ContinuationToken);
        Assert.That(guess4.GameNumber, Is.EqualTo(2));
        Assert.That(guess4.NumberOfGuesses, Is.EqualTo(1));
        Assert.That(guess4.CorrectWord, Is.False);

        var guess5 = _gameService.PlayGame("aloha", guess4.ContinuationToken);
        Assert.That(guess5.GameNumber, Is.EqualTo(2));
        Assert.That(guess5.NumberOfGuesses, Is.EqualTo(2));
        Assert.That(guess5.CorrectWord, Is.False);
    }

    [Test]
    public void TwoGamesInARow_GuessedOnFirstGuess()
    {
        var guess1 = _gameService.PlayGame("hello", string.Empty);
        Assert.That(guess1.GameNumber, Is.EqualTo(1));
        Assert.That(guess1.NumberOfGuesses, Is.EqualTo(1));
        Assert.That(guess1.CorrectWord, Is.True);
        
        var guess2 = _gameService.PlayGame("hello", guess1.ContinuationToken);
        Assert.That(guess2.GameNumber, Is.EqualTo(2));
        Assert.That(guess2.NumberOfGuesses, Is.EqualTo(1));
        Assert.That(guess2.CorrectWord, Is.True);
        
        var guess3 = _gameService.PlayGame("hello", guess2.ContinuationToken);
        Assert.That(guess3.GameNumber, Is.EqualTo(3));
        Assert.That(guess3.NumberOfGuesses, Is.EqualTo(1));
        Assert.That(guess3.CorrectWord, Is.True);
    }
}