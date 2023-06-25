using System.Collections.ObjectModel;
using Chaos.Schemas.Data;

// ReSharper disable CollectionNeverQueried.Global

namespace ChaosTool.Definitions;

public static class SampleData
{
    public static ObservableCollection<DialogOptionSchema> DialogOptions { get; } = new()
    {
        new DialogOptionSchema
        {
            OptionText = "Option1",
            DialogKey = "to_some_dialog_1"
        },
        new DialogOptionSchema
        {
            OptionText = "Option2",
            DialogKey = "to_some_dialog_2"
        },
        new DialogOptionSchema
        {
            OptionText = "Option3",
            DialogKey = "to_some_dialog_3"
        },
        new DialogOptionSchema
        {
            OptionText = "Option4",
            DialogKey = "to_some_dialog_4"
        },
        new DialogOptionSchema
        {
            OptionText = "Option5",
            DialogKey = "to_some_dialog_5"
        }
    };

    public static ObservableCollection<ItemDetailsSchema> ItemDetails { get; } = new()
    {
        new ItemDetailsSchema
        {
            ItemTemplateKey = "some_item_1",
            Stock = 50
        },
        new ItemDetailsSchema
        {
            ItemTemplateKey = "some_item_2",
            Stock = 10
        },
        new ItemDetailsSchema
        {
            ItemTemplateKey = "some_item_3",
            Stock = 100
        },
        new ItemDetailsSchema
        {
            ItemTemplateKey = "some_item_4",
            Stock = 1
        }
    };

    public static ObservableCollection<ItemRequirementSchema> ItemRequirements { get; } = new()
    {
        new ItemRequirementSchema
        {
            ItemTemplateKey = "grand_stick",
            AmountRequired = 42
        },
        new ItemRequirementSchema
        {
            ItemTemplateKey = "useless boot",
            AmountRequired = 500
        },
        new ItemRequirementSchema
        {
            ItemTemplateKey = "lethal umbrella",
            AmountRequired = 1
        }
    };

    public static ObservableCollection<string> Strings { get; } = new()
    {
        "consumable",
        "beothaichQuest"
    };
}