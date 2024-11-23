using infinite_words.api.Configuration;
using infinite_words.api.Error;
using infinite_words.api.Service;
using NUnit.Framework;

namespace infinite_words.api.test;

public class ContinuationTokenServiceTests
{
    private readonly IContinuationTokenService _tokenService = new ContinuationTokenService(new EncryptionService(), new SecretConfig("test-salt"));
    
    [Test]
    public void TokenService_BehavesAsExpected()
    {
        var token = _tokenService.GetToken("hello".Length);
        
        Assert.That(token.WordLength, Is.EqualTo(5));
        Assert.That(token.GameNumber, Is.EqualTo(1));


        var encoded = _tokenService.EncodeToken(token);
        var decoded = _tokenService.GetToken("hello".Length, encoded);
        
        Assert.That(decoded.Guesses, Is.EqualTo(token.Guesses));
        Assert.That(decoded.Nonce, Is.EqualTo(token.Nonce));
        Assert.That(decoded.Token, Is.EqualTo(token.Token));
        Assert.That(decoded.GameNumber, Is.EqualTo(token.GameNumber));
        Assert.That(decoded.WordLength, Is.EqualTo(token.WordLength));

        Assert.Throws<TokenCouldNotBeDecodedException>(() => _tokenService.GetToken("hello".Length, encoded + "FAIL"));

        var newToken = _tokenService.UpdateToken(token, "world", false);
        
        Assert.That(newToken.Guesses.Count, Is.EqualTo(1));
        Assert.That(newToken.GameNumber, Is.EqualTo(1));
        Assert.That(newToken.Token, Is.EqualTo(token.Token));
        Assert.That(newToken.Nonce, Is.Not.EqualTo(token.Nonce));
    }
}