#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Panel.Abstractions;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     Provides methods for managing knowledge books (skill/spell panels)
/// </summary>
/// <typeparam name="T">
///     An implementation of <see cref="PanelEntityBase" />
/// </typeparam>
public interface IKnowledgeBook<T> : IPanel<T> where T: PanelEntityBase
{
    /// <summary>
    ///     Retrieves the first slot index for the specified page
    /// </summary>
    /// <param name="page">
    ///     The page for which to retrieve the first slot index
    /// </param>
    byte GetFirstSlotInPage(PageType page);

    /// <summary>
    ///     Retrieves the last slot index available in the specified page
    /// </summary>
    /// <param name="page">
    ///     The page type for which the last slot index is to be retrieved
    /// </param>
    /// <returns>
    ///     The last slot index in the given page
    /// </returns>
    byte GetLastSlotInPage(PageType page);

    /// <summary>
    ///     Attempts to add the specified object to the next available slot within the specified page
    /// </summary>
    /// <param name="page">
    ///     The page to add the object to
    /// </param>
    /// <param name="obj">
    ///     The object to be added
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the object is successfully added to an available slot in the specified page, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryAddToNextSlot(PageType page, T obj);
}