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
        
        Assert.AreEqual(5, token.WordLength);
        Assert.AreEqual(1, token.GameNumber);


        var encoded = _tokenService.EncodeToken(token);
        var decoded = _tokenService.GetToken("hello".Length, encoded);
        
        Assert.AreEqual(token.Guesses, decoded.Guesses);
        Assert.AreEqual(token.Nonce, decoded.Nonce);
        Assert.AreEqual(token.Token, decoded.Token);
        Assert.AreEqual(token.GameNumber, decoded.GameNumber);
        Assert.AreEqual(token.WordLength, decoded.WordLength);

        Assert.Throws<TokenCouldNotBeDecodedException>(() => _tokenService.GetToken("hello".Length, encoded + "FAIL"));

        var newToken = _tokenService.UpdateToken(token, "world", false);
        
        Assert.AreEqual(1, newToken.Guesses.Count);
        Assert.AreEqual(1, newToken.GameNumber);
        Assert.AreEqual(token.Token, newToken.Token);
        Assert.AreNotEqual(token.Nonce, newToken.Nonce);
        
        
        
    }
}