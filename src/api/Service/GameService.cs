using System.Collections.Generic;
using infinite_words.api.Model;
using Microsoft.Extensions.Logging;

namespace infinite_words.api.Service;
public interface IGameService
{
    GuessResult PlayGame(string guess, string continuationToken);
}

public class GameService : IGameService
{
    private readonly IValidationService _validationService;
    private readonly IValidWordsService _validWordsService;
    private readonly IGuessService _guessService;
    private readonly IContinuationTokenService _tokenService;
    private readonly IWordService _wordService;
    private readonly ILogger<GameService> _logger;

    public GameService(IValidationService validationService,
        IValidWordsService validWordsService,
        IGuessService guessService,
        IContinuationTokenService tokenService,
        IWordService wordService,
        ILogger<GameService> logger)
    {
        _validationService = validationService;
        _validWordsService = validWordsService;
        _guessService = guessService;
        _tokenService = tokenService;
        _wordService = wordService;
        _logger = logger;
    }
    public GuessResult PlayGame(string guess, string continuationToken)
    {
        _logger.LogTrace("Running pre-validation against {Guess}", guess);
        _validationService.PreValidation(guess);
            
        _logger.LogTrace("Decoding token");
        var token = _tokenService.GetToken(guess.Length, continuationToken);
        using var scope = _logger.BeginScope(new Dictionary<string, object> {["Token"] = token.Token});
            
        _logger.LogTrace("Validating against token");
        _validationService.TokenValidation(guess, token);
            
        var validWords = _validWordsService.GetValidWords(guess.Length);
        _logger.LogTrace("Retrieved {WordCount} valid words for game length {GameLength}", validWords.Count, guess.Length);
        _wordService.SetWordList(validWords);

        _logger.LogTrace("Validating against word list");
        _validationService.WordListValidation(guess);
        
        _logger.LogTrace("Retrieving answer");
        var word = _wordService.GetWord(guess.Length, token.GameNumber);
            
        _logger.LogInformation("Answer is {Answer}", word);
        _validationService.WordValidation(guess, word);
            
        _logger.LogTrace("Retrieving result");
        var result = _guessService.Guess(word, guess, token);
            
        _logger.LogInformation("Returning result for {Guess}", guess);

        return result;
    }
}