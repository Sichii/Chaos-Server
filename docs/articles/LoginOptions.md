# Login Options

Login options are changed in `appsettings.json` via section `Options:LoginOptions`.  
This section is serialized into [LoginOptions](<xref:Chaos.Servers.Options.LoginOptions>)

## Login-Server Settings

---

### Port

The port the server will listen on. Default is 4201, if this value is changed, the client will need to be edited to connect to a different
port.

### WorldRedirect

When a client logs into the game world the login server is for, that client is redirected to a World server. These are the details needed
for that redirection

| Type   | Name     | Description                                                         |
|--------|----------|---------------------------------------------------------------------|
| string | HostName | The dns entry to look up to find the ip address of the world server |
| int    | Port     | The port the world server is hosted on                              |

### ReservedRedirects

When a client is redirected from a lobby (server selection) to a login server, a redirect id is used to identify the client being
redirected. If the client is unable to be identified, the connection is dropped. Because of this, if you want to accept redirects from
external lobby servers, a redirect id will need to be communicated between them and yourself. This collection is used to store those
reserved redirect ids.

| Type   | Name | Description                                                                                         |
|--------|------|-----------------------------------------------------------------------------------------------------|
| int    | Id   | The id that will be used to redirect the client from the lobby server to the specified login server |
| string | Name | The name passed along with the redirect information, this should probably be the server name        |

### NoticeMessage

This is the message that will be displayed to the player when they log in. This is a good place to put a message about the server being down
for maintenance, or a message about the server being in beta.

### StartingMapInstanceId

The instanceId of the map that the player will be placed in when they first create a character.

### StartingPointStr

The coordinates of the point that the player will be placed at when they first create a character.

> [!TIP]
> The format for this string has several options  
> "StartingPointStr": "25 25"  
> "StartingPointStr": "25, 25"  
> "StartingPointStr": "(25, 25)"

### Examples

#### appsettings.json login options

```json
"LoginOptions": {
  "HostName": "localhost",
  "Port": 4201,
  "WorldRedirect": {
    "HostName": "localhost",
    "Port": 4202
  },
  "StartingMapInstanceId": "Tutorial1",
  "StartingPointStr": "(5, 5)",
  "NoticeMessage": "under construction",
  "ReservedRedirects": []
}
```

#### appsettings.prod.json lobby options

```json
"LoginOptions": {
  "HostName": "chaotic-minds.dynu.net",
  "Port": 6901,
  "WorldRedirect": {
  "HostName": "chaotic-minds.dynu.net",
  "Port": 6902
  },
  "StartingMapInstanceId": "Tutorial1",
  "StartingPointStr": "(5, 5)",
  "NoticeMessage": "This server is in beta\nPlease report any bugs to the discord",
  "ReservedRedirects": [
    {
      "Id": 1,
      "Name": "Other Lobby"
    }
  ]
}
```