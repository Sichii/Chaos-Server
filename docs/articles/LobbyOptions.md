# Lobby Options

Lobby options are changed in `appsettings.json` via section "Options:LobbyOptions".  
This section is serialized into [LobbyOptions](<xref:Chaos.Services.Servers.Options.LobbyOptions>)

## Lobby-Server Settings

---

### Port

The port the server will listen on. Default is 4200, if this value is changed, the client will need to be edited to connect to a different
port.

### Servers

When a client connects to a server, the first thing is displays to the player is a list of available login servers to connect to.  
When the original game first launched, there were 3 servers, and you would select the server you wanted to connect to at the lobby. For each
of these servers, the name and a short description would display, and each would connect you to a unique world with it's own login
server.

Each server is serialized into [ServerInfo](<xref:Chaos.Networking.Options.ServerInfo>)

### ServerInfo

|  Type  |    Name     | Description                                                                                                                                                                                                                                                                                                                                                                                                                                                                 |
|:------:|:-----------:|:----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
|  int   |     Id      | The id that will be used to redirect the client from the lobby server to the specified login server <br /> If the selected login server is also being hosted through this process, the object that manages redirects will recognize the id as being from an internal redirect source. <br/> If not, the redirect will be considered external, and will need this id placed in the [LoginOptions](<xref:Chaos.Services.Servers.Options.LoginOptions>) as a reserved redirect |
| string |    Name     | The name of the server as it will be displayed to players.<br/> Must be 9 or less characters long.                                                                                                                                                                                                                                                                                                                                                                          |
| string |  Hostname   | The dns entry to look up to find the ip address of the login server                                                                                                                                                                                                                                                                                                                                                                                                         |
|  int   |    Port     | The port the login server is hosted on                                                                                                                                                                                                                                                                                                                                                                                                                                      |
| string | Description | A short description of the login server.<br/>Must be 18 or less characters long.                                                                                                                                                                                                                                                                                                                                                                                            |

### Examples

The recommended way to use this configuration would be to set the port in your "appsettings.json", but leave the servers blank so they can
be overridden depending on environment like so

#### appsettings.json lobby options

```json
"LobbyOptions": {
"Port": 4200,
"Servers": []
}
```

#### appsettings.local.json lobby options

```json
"LobbyOptions": {
  "Servers": [
    {
      "Id": 0,
      "Name": "LocalTest",
      "HostName": "localhost",
      "Port": 4201,
      "Description": "Test server"
    }
  ]
}
```

#### appsettings.prod.json lobby options

```json
"LobbyOptions": {
  "Servers": [
    {
      "Id": 0,
      "Name": "Chaos",
      "HostName": "chaos-server.net",
      "Port": 4201,
      "Description": "Private DA server"
    }
  ]
}
```