using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace infinite_words.api.Service;
public interface IWordService
{
    void SetWordList(IList<string> wordList);
    string GetWord(int length, int n);
    bool Contains(string guess);
}

public class WordService : IWordService
{
    private IList<string>? _wordList;
    public void SetWordList(IList<string> wordList)
    {
        _wordList = wordList;
    }

    public string GetWord(int length, int gameNumber)
    {
        ArgumentNullException.ThrowIfNull(_wordList);
        
        var offset = GetOffset(length, gameNumber);
        if (offset > _wordList.Count)
            throw new IndexOutOfRangeException($"Expected number < {_wordList.Count}, but was {offset}");

        return _wordList[offset].ToLower();
    }

    public bool Contains(string guess)
    {
        ArgumentNullException.ThrowIfNull(_wordList);

        return _wordList.Contains(guess);
    }

    private static int GetOffset(int length, int gameNumber)
    {
        var str = $"{{\"n\": \"{gameNumber}\", \"date\": \"{DateTime.UtcNow:yyyy-MM-dd}\" }}";
        var seedBytes = Encoding.UTF8.GetBytes(str);
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(seedBytes);
        var number = Math.Abs(BitConverter.ToInt32(hashBytes, 0));
        var (_, remainder) = Math.DivRem(number, length);
        return remainder;
    }
}