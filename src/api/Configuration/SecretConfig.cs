namespace infinite_words.api.Configuration;

public record SecretConfig (string Salt) : ISecretConfig;