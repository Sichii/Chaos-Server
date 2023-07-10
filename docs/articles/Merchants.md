# Merchants

Merchants in Chaos are both templated and scripted objects. When you define a merchant, you are actually defining a
merchant template that will be used to create new instances of that merchant as requested. Each merchant that is created
is a fresh instance of that merchant created from it's template. This allows you to have multiple instances of the same
merchant across the world, even on the same map if desired.

## Merchant Templates

A merchant template is used to create new instances of merchants as required. It contains all the information required
to create an instance of a new merchant.

## How do I create them?

By default, merchant templates should be created in `Data\Configuration\Templates\Merchants`. Configuration of how
merchants are loaded can be found in `appsettings.json` at `Options:MerchantTemplateCacheOptions`.

Merchant templates are initially serialized
into [MerchantTemplateSchema](<xref:Chaos.Schemas.Templates.MerchantTemplateSchema>) before being mapped to
a [MerchantTemplate](<xref:Chaos.Models.Templates.MerchantTemplate>). The schema object is mapped via
the [MerchantMapperProfile](<xref:Chaos.Services.MapperProfiles.MerchantMapperProfile>).

See [MerchantTemplateSchema](<xref:Chaos.Schemas.Templates.MerchantTemplateSchema>) for a list of all configurable
properties with descriptions.

## How do I use them?

Merchants can be created by using the [MerchantFactory](<xref:Chaos.Services.Factories.MerchantFactory>), which is an
implementation of [IMerchantFactory](<xref:Chaos.Services.Factories.Abstractions.IMerchantFactory>).

[MapInstances](Maps.md) have a section for adding merchants to a map by their template key, but you can also create them
using the example below.

> [!NOTE]
> Each merchant is a fresh instance of a merchant created from a template. Any changes made to the template will apply
> to all instances of that merchant.

```cs
private readonly IMerchantFactory MerchantFactory;

public Foo(IMerchantFactory merchantFactory) => MerchantFactory = merchantFactory;

public void Bar()
{
    //create a new instance of the merchant "myMerchantTemplateKey"
    //mapInstance is the map instance that the merchant will be added to
    //point is the point on the map that the merchant will be added to
    //extraScriptKeys is optional, and can be used to pass in extra script keys that are not part of the templated merchant
    var merchant = MerchantFactory.Create("myMerchantTemplateKey", mapInstance, point, extraScriptKeys);
    
    //the merchant is not automatically added to the map when created, so you must do so yourself
    mapInstance.AddObject(merchant, point);
}
```

## Dialogs

If you want a merchant to have a dialog, you must add a script to that merchant that overrides the `OnClicked` event and
shows a dialog.

See [this example](Dialogs.md#how-do-i-use-them) on how to fetch a dialog

## Scripting

Merchants are scripted via [IMerchantScript](<xref:Chaos.Scripting.MerchantScripts.Abstractions.IMerchantScript>).

- Inherit from [MerchantScriptBase](<xref:Chaos.Scripting.MerchantScripts.Abstractions.MerchantScriptBase>) for a basic
  script that requires no external configuration
- Inherit
  from [ConfigurableMerchantScriptBase](<xref:Chaos.Scripting.MerchantScripts.Abstractions.ConfigurableMerchantScriptBase>)
  for a script that requires external configuration via ScriptVars

Specify any number of script keys in the `MerchantTemplate.ScriptKeys` property, and those scripts will automatically be
attached to the `Merchant` when it is created.

If the script is configurable, you must also have an entry for that script in the `MerchantTemplate.ScriptVars`
property.

> [!NOTE]
> The key of a script is the name of the class without 'Script' at the end

Here are the events overridable in merchant scripts:

| Event Name      | Description                                                                                                                                                                |
|-----------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| CanMove         | Called when the merchant is about to move. Return false to prevent the merchant from moving. This can be used to check for status effects and other things                 |
| CanSee          | Called to check if the merchant is able to see a `VisibleEntity`.                                                                                                          |
| CanTalk         | Called when the merchant is about to talk. Return false to prevent the merchant from talking. This can be used to check for status effects and other things                |
| CanTurn         | Called when the merchant is about to turn. Return false to prevent the merchant from turning. This can be used to check for status effects and other things                |
| CanUseSkill     | Called when the merchant is about to use a skill. Return false to prevent the merchant from using the skill. This can be used to check for status effects and other things |
| CanUseSpell     | Called when the merchant is about to use a spell. Return false to prevent the merchant from using the spell. This can be used to check for status effects and other things |
| OnApproached    | Called when any observable creature enters the merchant's awareness (15 tiles), or becomes observable within that area                                                     |
| OnAttacked      | Called after the merchant is attacked. This is called before OnDeath                                                                                                       |
| OnClicked       | Called when the merchant is clicked                                                                                                                                        |
| OnDeath         | Called when the merchant dies. This is called after OnAttacked                                                                                                             |
| OnDeparture     | Called when any observable creature exits the merchant's awareness (15 tiles), or becomes unobservable within that area                                                    |
| OnGoldDroppedOn | Called after gold is dropped on the merchant                                                                                                                               |
| OnHealed        | Called after the merchant is healed                                                                                                                                        |
| OnItemDroppedOn | Called after an item is dropped on the merchant                                                                                                                            |
| OnPublicMessage | Called after any creature sends a public message within the merchant's awareness (15 tiles)                                                                                |
| Update          | Called every time the map updates                                                                                                                                          |

## Examples

### Banker

Here is an example of a merchant that functions as a bank. The merchant has scripts to show the banking dialog, as well
as a script that allows aislings to use verbal commands to interact with the bank.

[!code-json[](../../Data/Configuration/Templates/Merchants/bank_tester.json)]

### Shopkeeper

Here is an example of a merchant that functions as a shop. The merchant has scripts to show the shop, as well as a
script that allows aisling to use verbal commands to interact with the shop. The shop restocks 100% of it's items every
hour.

[!code-json[](../../Data/Configuration/Templates/Merchants/shop_tester.json)]