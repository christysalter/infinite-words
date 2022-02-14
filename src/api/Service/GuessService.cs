using System.Collections.Generic;
using System.Linq;
using infinite_words.api.Model;

namespace infinite_words.api.Service;
public interface IGuessService
{
    GuessResult Guess(string word, string guess, ContinuationToken token);
}

public class GuessService : IGuessService
{
    private readonly IColourService _colourService;
    private readonly IKeyboardService _keyboardService;
    private readonly IContinuationTokenService _continuationTokenService;

    public GuessService(IColourService colourService,
        IKeyboardService keyboardService,
        IContinuationTokenService continuationTokenService)
    {
        _colourService = colourService;
        _keyboardService = keyboardService;
        _continuationTokenService = continuationTokenService;
    }

    public GuessResult Guess(string word, string guess, ContinuationToken token)
    {
        var guessResult = _colourService.GetColourResult(word, guess);
        var gameWon = guessResult.All(x => x.Colour == LetterColour.Green);
        var allGuesses = new List<string>(token.Guesses) {guess};
        var newToken = _continuationTokenService.UpdateToken(token, guess, gameWon);
        var result = new GuessResult(guess.Length, guessResult, gameWon,
            _keyboardService.GetKeyboard(allGuesses, word),
            _continuationTokenService.EncodeToken(newToken), newToken.GameNumber,
            newToken.Guesses.Count);
        
        return result;
    }
}