namespace Chaos.Services.Other.Abstractions;

/// <summary>
///     Provides methods to manage the stock of items a object has
/// </summary>
public interface IStockService
{
    /// <summary>
    ///     Gets the stock of the specified item
    /// </summary>
    /// <param name="key">The template key of the object who's stock to check</param>
    /// <param name="itemTemplateKey">The template key of the item who's stock amount to return</param>
    /// <returns></returns>
    int GetStock(string key, string itemTemplateKey);

    /// <summary>
    ///     Determines whether or not the merchant has any stock of the item
    /// </summary>
    /// <param name="key">The template key of the object who's stock to check</param>
    /// <param name="itemTemplateKey">The template key of the item to check stock of</param>
    /// <returns><c>true</c> if the object exists and has any stock of the specified item, otherwise <c>false</c></returns>
    bool HasStock(string key, string itemTemplateKey);

    /// <summary>
    ///     Registers the stock of a object
    /// </summary>
    /// <param name="key">The template key of the object who's stock to register</param>
    /// <param name="stock">The stock the object holds by default</param>
    /// <param name="restockInterval">How often this object's wares are restocked</param>
    /// <param name="restockPercent">The percentage of stock that is recouped during a restock</param>
    void RegisterStock(
        string key,
        IEnumerable<(string ItemTemplateKey, int MaxStock)> stock,
        TimeSpan restockInterval,
        decimal restockPercent
    );

    /// <summary>
    ///     Restocks the specified object, adding back a percentage of the maximum stock of each item they sell
    /// </summary>
    void Restock(string key, decimal percent);

    /// <summary>
    ///     Attempts to decrease the stock of an item
    /// </summary>
    /// <param name="key">The template key of the object who's stock to decrease</param>
    /// <param name="itemTemplateKey">The template key of the item do decrease stock of</param>
    /// <param name="amount">The amount to decrease the stock by</param>
    /// <returns>
    ///     <c>true</c> if the merchant exists and has stock greater than or equals to the amount being removed, otherwise <c>false</c>
    /// </returns>
    bool TryDecrementStock(string key, string itemTemplateKey, int amount = 1);
}