using System.Collections.Generic;
using infinite_words.api.Service;

namespace infinite_words.api.test;

public class FakeWordService : IWordService
{
    private IList<string> _wordList;
    private readonly string _word;
    
    public FakeWordService(string word)
    {
        _word = word;
        _wordList = new List<string>();
    }

    public void SetWordList(IList<string> wordList)
    {
        _wordList = wordList;
    }

    public string GetWord(int length, int n)
    {
        return _word;
    }

    public bool Contains(string guess)
    {
        return _wordList.Contains(guess);
    }
}