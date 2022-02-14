using infinite_words.api.Service;
using NUnit.Framework;

namespace infinite_words.api.test;

[TestFixture]
public class EncryptionServiceTests
{
    private readonly IEncryptionService _encryptionService = new EncryptionService();
    
    [Test]
    public void Encrypt_Then_Decrypt_Returns_Same_Result()
    {
        const string str = "Hello World";
        var encrypted = _encryptionService.EncryptString("password1", str);
        var decrypted = _encryptionService.DecryptString("password1", encrypted);
        
        Assert.AreEqual(str, decrypted);
    }
}