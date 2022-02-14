using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using infinite_words.api.Error;
using infinite_words.api.Model;
using infinite_words.api.Service;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;

namespace infinite_words.api;

public class InfiniteWordsFunction
{
    private readonly IGameService _gameService;
    private readonly ILogger<InfiniteWordsFunction> _logger;
    public InfiniteWordsFunction(IGameService gameService, ILogger<InfiniteWordsFunction> logger)
    {
        _gameService = gameService;
        _logger = logger;
    }

    [FunctionName("Health")]
    public IActionResult Health([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "health")] HttpRequest r)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = Guid.NewGuid(),
            ["route"] = r.Path,
            ["utcStartTime"] = DateTime.UtcNow
        });

        try
        {
            var result1 = _gameService.PlayGame("one", string.Empty);
            _ = _gameService.PlayGame("two", result1.ContinuationToken);
            return new OkResult();
        }
        catch
        {
            return new StatusCodeResult((int)HttpStatusCode.ServiceUnavailable);
        }
    }   
    
    [FunctionName("Guess")]
    [OpenApiOperation("Guess", "name")] 
    [OpenApiParameter("guess", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The guess. Must be between 3 and 9 characters. Must be the same length as previous guesses.")] 
    [OpenApiParameter("continuationToken", In = ParameterLocation.Path, Required = false, Type = typeof(string), Description = "The continuation token returned by the previous guess")] 
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(string), Description = "The result of the guess, including continuation token to continue the game")] 
    [OpenApiResponseWithoutBody(HttpStatusCode.BadRequest, Description = "The guess or continuation token was not valid")]
    public IActionResult Guess(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "guess/{guess}/{continuationToken?}")]
        HttpRequest r,
        string guess,
        string continuationToken)
    {
        guess = guess.ToLower();

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = Guid.NewGuid(),
            ["route"] = r.Path,
            ["guess"] = guess,
            ["utcStartTime"] = DateTime.UtcNow
        });

        try
        {
            var result = _gameService.PlayGame(guess, continuationToken);
            return GetResult(result);
        }
        catch (GuessTooShortException e)
        {
            _logger.LogWarning("Guess was too short: {Guess}", guess);
            return new BadRequestObjectResult(e.Message);
        }
        catch (GuessTooLongException e)
        {
            _logger.LogWarning("Guess was too long: {Guess}", guess);
            return new BadRequestObjectResult(e.Message);
        }
        catch (GuessContainsInvalidCharactersException e)
        {
            _logger.LogWarning("Guess contained invalid characters");
            return new BadRequestObjectResult(e.Message);
        }
        catch (GuessNotInDictionaryException e)
        {
            _logger.LogWarning("Guess was not in dictionary: {Guess}", guess);
            return new BadRequestObjectResult(e.Message);
        }
        catch (WordAlreadyGuessedException e)
        {
            _logger.LogWarning("Guess already tried: {Guess}", guess);
            return new BadRequestObjectResult(e.Message);
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }

    private static OkObjectResult GetResult(GuessResult result)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        var json = JsonSerializer.Serialize(result, options);
        return new OkObjectResult(json);
    }
}