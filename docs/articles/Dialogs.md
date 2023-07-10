# Dialogs

Dialogs in Chaos are both templated and scripted objects. When you define a dialog, you are defining a template that
will be used to create new instances of that dialog as requested. Each dialog that is displays is a fresh instance of
that dialog created from it's template.

## Dialog Templates

A dialog template is used to create new instances of dialogs as required. Any action that brings up a new dialog,
whether it be clicking an NPC, or clicking Next or Previous on an existing dialog, the next dialog shown will be a
fresh instance of a dialog created from a template.

## How do I create them?

By default, dialog templates should be created in `Data\Configuration\Templates\Dialogs`. Configuration of how map
templates are loaded can be found in `appsettings.json` at `Options:DialogTemplateCacheOptions`.

Map templates are initially serialized into [DialogTemplateSchema](<xref:Chaos.Schemas.Templates.DialogTemplateSchema>)
before being mapped to a [DialogTemplate](<xref:Chaos.Models.Templates.DialogTemplate>). The schema object is mapped via
the [DialogMapperProfile](<xref:Chaos.Services.MapperProfiles.DialogMapperProfile>).

See [DialogTemplateSchema](<xref:Chaos.Schemas.Templates.DialogTemplateSchema>) for a list of all configurable
properties with descriptions.

## How do I use them?

Dialogs can be created by using the [DialogFactory](<xref:Chaos.Services.Factories.DialogFactory>), which is an
implementation of [IDialogFactory](<xref:Chaos.Services.Factories.Abstractions.IDialogFactory).

> [!NOTE]
> Each dialog is a fresh instance of a dialog created from a template. Any changes made to the template will apply to
> all instances of that dialog.

```cs
private readonly IDialogFactory DialogFactory;

public Foo(IDialogFactory dialogFactory) => DialogFactory = dialogFactory;

public void Bar()
{
    //create a new dialog with dialogSource as the source of the dialog
    //dialogSource can be almost any object (items, monsters, merchants, aislings, etc)
    //extraScriptKeys is optional, and can be used to pass in extra script keys that are not part of the templated dialog
    var dialog = DialogFactory.Create("myDialogTemplateKey", dialogSource, extraScriptKeys);
}
```

## State and Context

Dialogs can have context passed between them by setting the `Contextual` property of the dialog template to true. This
will cause any dialog leading to a contextual dialog to pass it's state to it. State is stored in 2
places, `Dialog.MenuArgs` and `Dialog.Context`.

> [!NOTE]
> You can create chains of dialogs that share context by specifying `Contextual` on all dialogs in that chain except the
> first one

> [!CAUTION]
> Do not specify `Contextual` on a dialog that is the first in a chain of dialogs that share context

### Dialog.MenuArgs

MenuArgs is an [ArgumentCollection](<xref:Chaos.Collections.Common.ArgumentCollection>). This is used when specifying a
dialog type of MenuTextEntry, MenuTextEntryWithArgs, and DialogTextEntry. When the user types in input and clicks ok,
MenuArgs will be populated with what they typed in. The ArgumentCollection facilitates easier conversion of the input to
the desired type. For example, if you want the user to type in a number, ArgumentCollection can perform that conversion
for you.

> [!CAUTION]
> A dialog type 'WithArgs' can have only 1 argument passed into it from MenuArgs. If the dialog has a type 'TextEntry',
> the client can only return a maximum of 2 arguments, one passed in and one user input, with the last one always being
> the most recent input

### Dialog.Context

Context is just an object, otherwise known as a `Tag`. This is meant to be used as context for dialog scripts. The type
of object stored is up to the script, and any script receiving that context should know what that object is. This should
rarely be used, only in situations where there you need more than the 2 arguments `MenuArgs` can provide. When a
dialogs `Context` property is passed to the next dialog, it is deep cloned just in case there is a need to use that
specific snapshot of the context later on.

## Special Arguments

There are a few special keywords that can be used in a few places for unique behavior.

| Argument | Context                                                                                                                                                                                      |
|----------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Close    | Specify "Close" as the NextDialogKey or Option.DialogKey to close the dialog                                                                                                                 |
| Skip     | Specify "Skip" as a dialog's text to fire the events for that dialog, but skip displaying it. This is useful for triggering actions on the "Success" part of a confirmation option selection |
| Top      | Specify "Top" as the NextDialogKey or Option.DialogKey to go back to the initial menu of the object you are interacting with.<br/>This is exactly like clicking the "Top" button in a menu   |

## Scripting

Dialogs are scripted via [IDialogScript](<xref:Chaos.Scripting.DialogScripts.Abstractions.IDialogScript>).

- Inherit from [DialogScriptBase](<xref:Chaos.Scripting.DialogScripts.Abstractions.DialogScriptBase>) for a basic script
  that requires no external configuration
- Inherit
  from [ConfigurableDialogScriptBase](<xref:Chaos.Scripting.DialogScripts.Abstractions.ConfigurableDialogScriptBase>)
  for a script that requires external configuration via ScriptVars

Specify any number of script keys in the `DialogTemplate.ScriptKeys` property, and those scripts will automatically be
attached to the `Dialog` when it is created.

If the script is configurable, you must also have an entry for that script in the `DialogTemplate.ScriptVars` property.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in dialog scripts:

| Event Name   | Context                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  |
|--------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| OnDisplaying | Called when the dialog is about to be displayed, but has not yet been displayed. You can use this event to modify the dialog before it is displayed, such as injecting text parameters into the dialog's text                                                                                                                                                                                                                                                                                            |
| OnDisplayed  | Called when the dialog has been displayed                                                                                                                                                                                                                                                                                                                                                                                                                                                                |
| OnNext       | Called when the player responds to the dialog, whether by clicking Next, selecting an option, or making any kind of selection<br />If an option is clicked, the optionIndex will be the index of the option that was clicked<br />If a shop selection was clicked, the text of that option will be in the MenuArgs property of the dialog<br />If a selection was made from the player's inventory, spell book, or skill book, the Slot that was selected will be in the MenuArgs property of the dialog |
| OnPrevious   | Called when the player clicks the Previous button on a Normal dialog                                                                                                                                                                                                                                                                                                                                                                                                                                     |

See [Scripting](Scripting.md) for more details on scripting in general

## Example

Here is an example of a dialog template json for a dialog used to confirm a purchase from a shop. This dialog is
contextual, and has a script. The script injects those contextual parameters into the dialog's text. It gets it's
context from dialogs that come before it which collect the name of the item being bought, and how many of them.

[!code-json[](../../Data/Configuration/Templates/Dialogs/generic/Shop/BuyShop/generic_buyshop_confirmation.json)]