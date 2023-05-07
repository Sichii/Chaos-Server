# Commands

Commands are a way to execute some code within the game by using the chat window. Commands have some configuration that
can be changed in the `appsettings.json` file. The prefix can be changed
via `Options:AislingCommandInterceptorOptions:Prefix`

New commands are created by inheriting from the [ICommand<T>](<xref:Chaos.Messaging.Abstractions.ICommand`1>) interface,
and adding the [CommandAttribute](<xref:Chaos.Messaging.CommandAttribute>) attribute to the class. In the attribute is
specified the command name that will be used to execute the command, and whether or not the command requires admin
privileges.

Chaos comes with many commands out of the box, and you can find them in the [Chaos.Messaging](<xref:Chaos.Messaging>)
namespace.

> [!NOTE]
> Admin privileges are granted by adding the `"IsAdmin": true` to an aisling's json file

## Execution

Commands are intercepted by looking for the configured prefix, followed by a command's name. Commands are created newly
for each execution, and are dependency injected.

## Arguments

Commands are provided the object that executed the command (in this case,
an [Aisling](<xref:Chaos.Models.World.Aisling>)), and
an [ArgumentCollection](<xref:Chaos.Collections.Common.ArgumentCollection>). The ArgumentCollection helps to facilitate
conversion of strings to other data types, as well as usage of familiar patterns in command line interfaces, such as
preserving arguments that have spaces within double quotes("").

The ArgumentCollection can be accessed by index via
the [TryGet<T>](<xref:Chaos.Collections.Common.ArgumentCollection.TryGet*>) method, or by accessing arguments linearly
via the [TryGetNext<T>](<xref:Chaos.Collections.Common.ArgumentCollection.TryGetNext*>) method.

> [!NOTE]
> TryGetNext will not increment the index if it returns false