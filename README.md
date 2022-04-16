# Infinite Words API

### Getting Started

Check the API is healthy by calling the /health endpoint:

```curl -i https://api.infinitewords.uk/api/health```

If the API has gone 'cold' then this will also wake it up so that subsequent calls will return quickly. From cold it is usually ~2 seconds if you're located in Europe, and roughly ~100ms if the API is warm.

To initiate a new game, simply call the Guess endpoint like so:

```curl -i https://api.infinitewords.uk/api/guess/hello```

Where hello is the guess. This will initiate a game for words of length 5 (hello is 5 characters. The API supports games of length 3 - 9 inclusive).

The response from this call will look something like this:
```
{
	"wordLength": 5,
	"result": [
		{
			"letter": "h",
			"colour": "yellow",
			"index": 0
		},
		{
			"letter": "e",
			"colour": "yellow",
			"index": 1
		},
		{
			"letter": "l",
			"colour": "grey",
			"index": 2
		},
		{
			"letter": "l",
			"colour": "grey",
			"index": 3
		},
		{
			"letter": "o",
			"colour": "yellow",
			"index": 4
		}
	],
	"correctWord": false,
	"keyBoard": {
		"a": "grey",
		"b": "grey",
		"c": "grey",
		"d": "grey",
		"e": "yellow",
		"f": "grey",
		"g": "grey",
		"h": "yellow",
		"i": "grey",
		"j": "grey",
		"k": "grey",
		"l": "grey",
		"m": "grey",
		"n": "grey",
		"o": "yellow",
		"p": "grey",
		"q": "grey",
		"r": "grey",
		"s": "grey",
		"t": "grey",
		"u": "grey",
		"v": "grey",
		"w": "grey",
		"x": "grey",
		"y": "grey",
		"z": "grey"
	},
	"continuationToken": "Z3lBU3FEYmpPZ...FkaW55SVd3aXlsZz09",
	"gameNumber": 1,
	"numberOfGuesses": 1
}
```

To continue this game, simply append the continuationToken to the url like this:

```curl -i https://api.infinitewords.uk/api/guess/world/Z3lBU3FEYmpPZ...FkaW55SVd3aXlsZz09```

and you will recieve a response indicating the new state of the game containing a new continuation token.

For each subsequent guess simply append the continuation token from the previous response. If the correct word is guessed the correctWord field will be true. If you then guess a new word with the continuation token from the winning guess, a new game will begin.

Everybody in the world will have the same sequence of words for a given UTC calandar day, after which it will reset and a new random sequence of words will be chosen.
