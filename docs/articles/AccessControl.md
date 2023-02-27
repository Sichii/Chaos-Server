# Access Control

The server can be in one of two modes: Blacklist or Whitelist. In Blacklist mode, the server will only allow connections from clients that
are not on the blacklist. In Whitelist mode, the server will only allow connections from clients that are on the whitelist. This mode is
controlled via a configuration option located at `Options:AccessManagerOptions.Mode`.

This mode can be set to `Blacklist` or `Whitelist`. Depending on the mode, when a client attempts to connect to the server, a file name "
Blacklist.txt" or "Whitelist.txt" located at the specified directory will be read and the client's IP address will be checked against the
contents of the file, and the specified behavior will be applied.

## Example `appsettings.json`

```json
"AccessManagerOptions": {
  "Mode": "Blacklist",
  "Directory": "Data\\Access"
}
```

## File contents

The contents of these files should be IP addresses. They can be IPv4 or IPv6 addresses. If the file contains a line that is not a valid IP,
it will be ignored.