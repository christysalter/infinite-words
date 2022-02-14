using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using infinite_words.api.Configuration;
using infinite_words.api.Error;
using infinite_words.api.Model;

namespace infinite_words.api.Service;
public interface IContinuationTokenService
{
    ContinuationToken GetToken(int length, string tokenString = "");
    ContinuationToken UpdateToken(ContinuationToken token, string guess, bool won);
    string EncodeToken(ContinuationToken token);
}

public class ContinuationTokenService : IContinuationTokenService
{
    private readonly IEncryptionService _encryptionService;
    private readonly ISecretConfig _config;

    public ContinuationTokenService(IEncryptionService encryptionService, ISecretConfig config)
    {
        _encryptionService = encryptionService;
        _config = config;
    }

    public ContinuationToken GetToken(int length, string tokenString = "")
    {
        if (string.IsNullOrWhiteSpace(tokenString))
            return NewToken(length);
        
        var key = GetKey();
        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(tokenString));
            var json = _encryptionService.DecryptString(key, decoded);
            var token = DeserializeMinJson(json);
            return token;
        }
        catch (Exception)
        {
            throw new TokenCouldNotBeDecodedException();
        }
    }

    public ContinuationToken UpdateToken(ContinuationToken token, string guess, bool won)
    {
        var generatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var guesses = new List<string> { guess };
        if (!token.LastGameWon)
            guesses = new List<string>(token.Guesses) { guess };
        
        return token with
        {
            GeneratedAt = generatedAt,
            GameNumber = token.LastGameWon ? token.GameNumber + 1 : token.GameNumber,
            Nonce = GetNonce(),
            Guesses = guesses,
            LastGameWon = won
        };
    }

    private ContinuationToken NewToken(int length)
    {
        var generatedAt = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss");
        var random = RandomNumberGenerator.GetBytes(16);
        var nonce = Convert.ToBase64String(random).TrimEnd('=');
        var gameId = _encryptionService.Sha256(Guid.NewGuid().ToString())[0..10];
        return new ContinuationToken(generatedAt, length, 1, nonce, gameId, new List<string>(), false);
    }
    
    public string EncodeToken(ContinuationToken token)
    {
        var key = GetKey();
        var minified = GetMinJson(token);
        var cypherText = _encryptionService.EncryptString(key, minified);
        var base64Token = Convert.ToBase64String(Encoding.UTF8.GetBytes(cypherText));
        return base64Token;
    }

    private static string GetMinJson(ContinuationToken token)
    {
        var options = new JsonSerializerOptions {WriteIndented = false};
        
        var json = JsonSerializer.Serialize(token, options);
        
        return json
            .Replace($"\"{nameof(ContinuationToken.Guesses)}\"", "~A~")
            .Replace($"\"{nameof(ContinuationToken.Nonce)}\"", "~B~")
            .Replace($"\"{nameof(ContinuationToken.Token)}\"", "~C~")
            .Replace($"\"{nameof(ContinuationToken.GameNumber)}\"", "~D~")
            .Replace($"\"{nameof(ContinuationToken.GeneratedAt)}\"", "~E~")
            .Replace($"\"{nameof(ContinuationToken.WordLength)}\"", "~F~")
            .Replace($"\"{nameof(ContinuationToken.LastGameWon)}\"", "~G~");
    }

    private static ContinuationToken DeserializeMinJson(string json)
    {
        var unMinified = json
            .Replace("~A~", $"\"{nameof(ContinuationToken.Guesses)}\"")
            .Replace("~B~", $"\"{nameof(ContinuationToken.Nonce)}\"")
            .Replace("~C~", $"\"{nameof(ContinuationToken.Token)}\"")
            .Replace("~D~", $"\"{nameof(ContinuationToken.GameNumber)}\"")
            .Replace("~E~", $"\"{nameof(ContinuationToken.GeneratedAt)}\"")
            .Replace("~F~", $"\"{nameof(ContinuationToken.WordLength)}\"")
            .Replace("~G~", $"\"{nameof(ContinuationToken.LastGameWon)}\"");

        var token = JsonSerializer.Deserialize<ContinuationToken>(unMinified);
        ArgumentNullException.ThrowIfNull(token);
        return token;
    }
    
    private string GetKey() => $"{DateTime.UtcNow.Date:yyyy-MM-dd}:{_config.Salt}";
    
    private static string GetNonce()
    {
        var random = RandomNumberGenerator.GetBytes(32);
        var nonce = Convert.ToBase64String(random);
        return nonce;
    }
}