#region
using System.Collections.ObjectModel;
using Chaos.Schemas.Data;
#endregion

// ReSharper disable CollectionNeverQueried.Global

namespace ChaosTool.Definitions;

public static class SampleData
{
    public static ObservableCollection<DialogOptionSchema> DialogOptions { get; } =
        [
            new()
            {
                OptionText = "Option1",
                DialogKey = "to_some_dialog_1"
            },
            new()
            {
                OptionText = "Option2",
                DialogKey = "to_some_dialog_2"
            },
            new()
            {
                OptionText = "Option3",
                DialogKey = "to_some_dialog_3"
            },
            new()
            {
                OptionText = "Option4",
                DialogKey = "to_some_dialog_4"
            },
            new()
            {
                OptionText = "Option5",
                DialogKey = "to_some_dialog_5"
            }
        ];

    public static ObservableCollection<ItemDetailsSchema> ItemDetails { get; } =
        [
            new()
            {
                ItemTemplateKey = "some_item_1",
                Stock = 50
            },
            new()
            {
                ItemTemplateKey = "some_item_2",
                Stock = 10
            },
            new()
            {
                ItemTemplateKey = "some_item_3",
                Stock = 100
            },
            new()
            {
                ItemTemplateKey = "some_item_4",
                Stock = 1
            }
        ];

    public static ObservableCollection<ItemRequirementSchema> ItemRequirements { get; } =
        [
            new()
            {
                ItemTemplateKey = "grand_stick",
                AmountRequired = 42
            },
            new()
            {
                ItemTemplateKey = "useless boot",
                AmountRequired = 500
            },
            new()
            {
                ItemTemplateKey = "lethal umbrella",
                AmountRequired = 1
            }
        ];

    public static ObservableCollection<LootDropSchema> LootDrops { get; } =
        [
            new()
            {
                ItemTemplateKey = "some_item_1",
                DropChance = .50m
            },
            new()
            {
                ItemTemplateKey = "some_item_2",
                DropChance = .10m
            },
            new()
            {
                ItemTemplateKey = "some_item_3",
                DropChance = .100m
            },
            new()
            {
                ItemTemplateKey = "some_item_4",
                DropChance = 1.0m
            }
        ];

    public static ObservableCollection<string> Strings { get; } =
        [
            "consumable",
            "beothaichQuest"
        ];
}