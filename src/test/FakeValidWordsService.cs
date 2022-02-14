using System.Collections.Generic;
using infinite_words.api.Service;

namespace infinite_words.api.test;

public class FakeValidWordsService : IValidWordsService
{
    private readonly IList<string> _wordList;
    
    public FakeValidWordsService(IList<string> wordList)
    {
        _wordList = wordList;
    }
    public IList<string> GetValidWords(int length)
    {
        return _wordList;
    }
}