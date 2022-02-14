namespace infinite_words.api.Configuration;

public interface ISecretConfig
{
    string Salt { get; }
}