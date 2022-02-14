using System;
using System.Linq;
using infinite_words.api.Error;
using infinite_words.api.Model;

namespace infinite_words.api.Service;
public interface IValidationService
{
    void PreValidation(string guess);
    void WordListValidation(string guess);
    void TokenValidation(string guess, ContinuationToken token);
    void WordValidation(string guess, string word);
}

public class ValidationService : IValidationService
{
    private readonly IWordService _wordService;

    public ValidationService(IWordService wordService)
    {
        _wordService = wordService;
    }
    public void PreValidation(string guess)
    {
        switch (guess.Length)
        {
            case < Constants.GuessMinLength:
                throw new GuessTooShortException(guess.Length);
            case > Constants.GuessMaxLength:
                throw new GuessTooLongException(guess.Length);
        }

        if (guess.Any(c => !char.IsLetter(c)))
            throw new GuessContainsInvalidCharactersException();
    }

    public void WordListValidation(string guess)
    {
        if (!_wordService.Contains(guess))
            throw new GuessNotInDictionaryException(guess);
    }

    public void TokenValidation(string guess, ContinuationToken token)
    {
        if (token.LastGameWon)
            return;
        if (token.Guesses.Contains(guess, StringComparer.InvariantCultureIgnoreCase))
            throw new WordAlreadyGuessedException(guess);
    }

    public void WordValidation(string guess, string word)
    {
        if (guess.Length != word.Length)
            throw new WordGuessLengthMismatchException(word.Length, guess.Length);
    }
}