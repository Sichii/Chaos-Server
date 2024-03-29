# General Configuration

---

You can configure the Login and Lobby servers via `appsettings.json` file  
Here are a few quick tips, but there are more options available than are listed

- It's recommended to keep the staging data out of the repo, this base staging directory can be changed
  at `Options:ChaosOptions:StagingDirectory`
- Username/Password rules can be changed via `Options:ActiveDirectoryCreentialManagerOptions`
- If you want to spin up multiple worlds, or offer redirects to other people's worlds, you can add additional servers
  via `Options:LobbyOptions:Servers`
- If you want to accept redirects from other people, you need to communicate a reserved redirect id, and configure it
  via `Options:LoginOptions:ReservedRedirects`
- Edit your login notice message via `Options:LoginOptions:NoticeMessage`
- Edit your new character initial spawn point via `Options:LoginOptions:StartingMapInstanceId` and `Options:LoginOptions:StartingPointStr`  
- If you want to change the folder structure of the server, each cache's directory can be changed via their `Directory` option
- Cache expiration can also be changed via `ExpirationMins`
- By default the server uses `NLog` for logging, but feel free to change it to whatever you want to use
- Log levels are also configurable via `Options:Logging:LogLevel:Default` and `Options:Logging:NLog:rules:minLevel`