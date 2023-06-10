# Initial Setup

## Fork the Chaos-Server repository

- Follow the instructions on [GitHub](https://help.github.com/articles/fork-a-repo/) to fork
  the [Chaos-Server repository](https://github.com/Sichii/Chaos-Server)
- Following along with the guide, clone your forked repository and configure the upstream repository as the master
  branch on the Chaos-Server repository

## Configure the staging directory

By default, the staging directory is inside the git repo, at the root directory, in the "Data" folder. This staging
directory can be changed if desired via `appsettings.json` at `Options:ChaosOptions:StagingDirectory`.

## Connecting

If you have the necessary skills, you can edit a Dark Ages client to connect to 127.0.0.1 (localhost) and use it for
testing purposes. Otherwise, you can download and use [Spark](https://github.com/ewrogers/Spark).

For anyone else trying to connect to your server, they can also use Spark. Make sure to forward the ports you configure
in the [LobbyOptions](LobbyOptions.md), [LoginOptions](LoginOptions.md), and [WorldOptions](WorldOptions.md).

## Configure the servers

- Configure [Logging](Logging.md)
- Configure [Access](AccessManager.md)
- Configure [LobbyOptions](LobbyOptions.md)
- Configure [LoginOptions](LoginOptions.md)
- Configure [WorldOptions](WorldOptions.md)

## Add content

> [!TIP]
> There's a tool to help create content, look for the [ChaosTool](ChaosTool.md)

- Add [Maps](Maps.md)
- Add [Items](Items.md)
- Add [Spells](Spells.md)
- Add [Skills](Skills.md)
- Add [Merchants](Merchants.md)
- Add [Dialogs](Dialogs.md)
- Add [Monsters](Monsters.md)
- Add [Loot Tables](LootTables.md)
- Write custom [Components](Components.md)
- Write custom [Scripts](Scripting.md)
- Write custom [Formulae](Formulae.md)
- Write custom [Functional Scripts](FunctionalScripts.md)
- Add additional [Meta Data](MetaData.md)