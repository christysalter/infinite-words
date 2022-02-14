using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace infinite_words.api.Service;
public interface IEncryptionService
{
    string EncryptString(string key, string plainText);
    string DecryptString(string key, string cipherText);
    string Sha256(string value);
}

public class EncryptionService : IEncryptionService
{
    private static byte[] ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = SHA256.Create();
        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return bytes;
    }
    public string EncryptString(string key, string plainText)
    {
        var iv = new byte[16];
        byte[] array;

        using (var aes = Aes.Create())
        {
            var bytes = ComputeSha256Hash(key);
            aes.Key = bytes;
            aes.IV = iv;

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using MemoryStream memoryStream = new();
            using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
            using (StreamWriter streamWriter = new(cryptoStream))
                streamWriter.Write(plainText);

            array = memoryStream.ToArray();
        }

        return Convert.ToBase64String(array);
    }

    public string DecryptString(string key, string cipherText)
    {
        var iv = new byte[16];
        var buffer = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        var bytes = ComputeSha256Hash(key);
        aes.Key = bytes;
        aes.IV = iv;
        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream memoryStream = new(buffer);
        using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
        using StreamReader streamReader = new(cryptoStream);
        return streamReader.ReadToEnd();
    }
    
    public string Sha256(string value) {
        var sb = new StringBuilder();
        using var hash = SHA256.Create();
        var enc = Encoding.UTF8;
        var result = hash.ComputeHash(enc.GetBytes(value));
        foreach (var b in result)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}
