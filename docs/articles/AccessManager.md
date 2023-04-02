# Access Manager

The access manager is exactly what it sounds like, but also more. This is either a poorly named class, or an object that has too many jobs.
Maybe I'll remedy this in the future, but for now it is what it is.

It handles not just access, but also player username and password rules and other various rules for character creation.

## Configuration

The access manager is configured via the `Options:AccessManagerOptions` section of the `appsettings.json` file. These options are serialized
into [AccessManagerOptions](<xref:Chaos.Security.Options.AccessManagerOptions>)

| Type   | Name                   | Description                                                                                                         |
|--------|------------------------|---------------------------------------------------------------------------------------------------------------------|
| string | Directory              | The relative directory where the Access Manager will store its files                                                |
| string | HashAlgorithmName      | The name of the hash algorithm used to hash passwords                                                               |
| string | LockoutMins            | The number of minutes an IP is locked out for after failing to login too many times                                 |
| string | MaxCredentialAttempts  | The maximum number of times an IP can fail to login or change password before being locked out for a period of time |
| string | MaxPasswordLength      | The maximum number of characters allowed in a password                                                              |
| string | MaxUsernameLength      | The maximum number of characters allowed in a username                                                              |
| string | MinPasswordLength      | The minimum number of characters allowed in a password                                                              |
| string | MinUsernameLength      | The minimum number of characters allowed in a username                                                              |
| string | Mode                   | The mode in which that the access manager operates                                                                  |
| string | PhraseFilter           | A list of phrases/words that a username can not contains                                                            |
| string | ReservedUsernames      | A list of usernames that are reserved and can not be used                                                           |
| string | ValidCharactersPattern | A regular expression used to validate that a username only contains valid characters                                |
| string | ValidFormatPattern     | A regular expression used to validate that a username is in the correct format                                      |

> [!NOTE]
> I suggest using https://regex101.com/ to test your regular expressions

## Example `appsettings.json`

```json
"AccessManagerOptions": {
  "Directory": "Data\\Saved",
  "ValidCharactersPattern": "[a-zA-Z0-9 ]+",
  "ValidFormatPattern": "^[a-zA-Z]{3,}$|[a-zA-Z]{3,} ?[a-zA-Z]{3,}",
  "MaxUsernameLength": 12,
  "MinUsernameLength": 3,
  "MaxPasswordLength": 8,
  "MinPasswordLength": 5,
  "ReservedUsernames": [],
  "PhraseFilter": [],
  "HashAlgorithmName": "SHA512",
  "Mode": "Blacklist",
  "MaxCredentialAttempts": 5,
  "LockoutMins": 5
}
```

## Mode

The mode can be either `Whitelist`, or `Blacklist`. In whitelist mode, only the IPs in the whitelist file are allowed to connect. In
blacklist mode, all IPs are allowed to connect, except those in the blacklist file. These files are located in the directory specified and
the contents of these files should be IP addresses. They can be IPv4 or IPv6 addresses. If the file contains a line that is not a valid IP,
it will be ignored.