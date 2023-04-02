# Dialogs

Dialogs are how a player interacts with various objects in the game. Typically you might think of a conversation between an NPC and the
player, but you can attach dialogs to many different types of objects in Chaos.

## Dialog Templates

A dialog template is used to create new instances of dialogs as required. Dialogs and any scripts attached to them are mostly stateless. Any
action that brings up a new dialog, whether is be clicking an NPC, or clicking Next or Previous on an existing dialog, the next dialog shown
will be a unique instance of a dialog created from a template.

By default, Map templates are stored at `Data\Templates\Dialogs`. Configuration of how map templates are loaded can be found
in `appsettings.json` at `Options:DialogTemplateCacheOptions`.

Map templates are initially serialized into [DialogTemplateSchema](<xref:Chaos.Schemas.Templates.DialogTemplateSchema>) before being mapped
to a non-schema type.

### DialogTemplateSchema

| Type                                                                              | Name          | Description                                                                                                                                                                                   |
|-----------------------------------------------------------------------------------|---------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| string                                                                            | TemplateKey   | A unique id specific to this dialog template<br />This must match the name of the folder containing this file                                                                                 |
| ICollection\<string\>                                                             | ScriptKeys    | A collection of names of map scripts to attach to this map by default                                                                                                                         |
| IDictionary\<string, [DynamicVars](<xref:Chaos.Collections.Common.DynamicVars>)\> | ScriptVars    | A collection of key-value pairs of key-value pairs<br />Each script that has variables needs a scriptName-Value pair, and the value of that entry is a dictionary of propertyName-Value pairs |
| [ChaosDialogType](<xref:Chaos.Common.Definitions.ChaosDialogType>)                | Type          | The type of dialog this is                                                                                                                                                                    |
| string?                                                                           | NextDialogKey | The template key of the dialog that should be displayed after this one<br />If specified and the Type is Normal, the dialog will have a next button                                           |
| string?                                                                           | PrevDialogKey | If specified and the Type is Normal, the dialog will have a previous button that will take them to the dialog with this template key                                                          |
| ICollection\<[DialogOptionSchema](<xref:Chaos.Schemas.Data.DialogOptionSchema>)\> | Options       | A collection of options that can be selected from this dialog                                                                                                                                 |
| string                                                                            | Text          | The text displayed on the template. This can be a string format with injectable parameters, but those parameters must be injected by a script                                                 |
| ushort?                                                                           | TextBoxLength | When the Type is DialogTextEntry, this will limit the length of the input text box                                                                                                            |
| bool                                                                              | Contextual    | Whether or not this dialog requires context passed to it from the previous dialog                                                                                                             |

### Example Dialog Template Json

Here is an example of a dialog template json for a dialog used to confirm a purchase from a shop. This dialog is contextual, and has a
script. The script injects those contextual parameters into the dialog's text. It gets it's context from dialogs that come before it which
collect the name of the item being bought, and how many of them.

[!code-json[](../../Data/Templates/Dialogs/generic/BuyShop/generic_buyshop_confirmation.json)]

## State and Context

Dialogs are mostly stateless, but there is a way to pass state between dialogs. This is done by setting the `Contextual` property of the
dialog template to true. This will cause any dialog leading to a contextual dialog to pass it's state to it. State is stored in 2 places...
`Dialog.MenuArgs` and `Dialog.Context`.

> [!NOTE]
> You can create chains of dialogs that share context by specifying `Contextual` on all dialogs in that chain except the first one

> [!CAUTION]
> Do not specify `Contextual` on a dialog that is the first in a chain of dialogs that share context

### Dialog.MenuArgs

MenuArgs is an [ArgumentCollection](<xref:Chaos.Collections.Common.ArgumentCollection>). This is used when specifying a dialog type of
MenuTextEntry, MenuTextEntryWithArgs, and DialogTextEntry. When the user types in input and clicks ok, MenuArgs will be populated with what
they typed in. The ArgumentCollection facilitates easier conversion of the input to the desired type. For example, if you want the user to
type in a number, ArgumentCollection can perform that conversion for you.

> [!CAUTION]
> A dialog type 'WithArgs' can have only 1 argument passed into it from MenuArgs. If the dialog has a type 'TextEntry', the client can only
> return a maximum of 2 arguments, one passed in and one user input, with the last one always being the most recent input

### Dialog.Context

Context is just an object, otherwise known as a Tag. This is meant to be used as context for scripts. The type of object stores is up to the
script, and any script receiving that context should know what that object is. This should rarely be used, only in situations where there
you need more than the 2 arguments MenuArgs can provide. When a dialogs `Context` property is passed to the next dialog, it is deep cloned
just in case there is a need to use that specific snapshot of the context later on.

### Scripting

Dialogs are scripted objects. One or more scripts can be attached to a dialog via the `ScriptKeys` property of the dialog template. Here are
the events available to dialogs and their context.

| Event Name   | Context                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
|--------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OnDisplaying | Called when the dialog is about to be displayed, but has not yet been displayed. You can use this event to modify the dialog before it is displayed, such as injecting text parameters into the dialog's text                                                                                                                                                                                                                                                                                            |
| OnDisplayed  | Called when the dialog has been displayed                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| OnNext       | Called when the player responds to the dialog, whether by clicking Next, selecting an option, or making any kind of selection<br />If an option is clicked, the optionIndex will be the index of the option that was clicked<br />If a shop selection was clicked, the text of that option will be in the MenuArgs property of the dialog<br />If a selection was made from the player's inventory, spell book, or skill book, the Slot that was selected will be in the MenuArgs property of the dialog |
| OnPrevious   | Called when the player clicks the Previous button on a Normal dialog                                                                                                                                                                                                                                                                                                                                                                                                                                     |

One example use case would be for a quest. Say you want to add an option to a menu if the player is on a specific step of a quest. Write a
new
class that inherits from `DialogScriptBase` that checks for that step and adds the option if needed.

## Special Arguments

There are a few special keywords that can be used in a few places for unique behavior.

| Argument | Context                                                                                                                                                                                      |
|----------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Close    | Specify "Close" as the NextDialogKey or Option.DialogKey to close the dialog                                                                                                                 |
| Skip     | Specify "Skip" as a dialog's text to fire the events for that dialog, but skip displaying it. This is useful for triggering actions on the "Success" part of a confirmation option selection |
| Top      | Specify "Top" as the NextDialogKey or Option.DialogKey to go back to the initial menu of the object you are interacting with.<br/>This is exactly like clicking the "Top" button in a menu   |