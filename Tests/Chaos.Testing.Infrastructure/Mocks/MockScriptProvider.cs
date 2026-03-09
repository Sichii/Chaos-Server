#region
using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Moq;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

/// <summary>
///     Provides a shared IScriptProvider mock and ICloningService&lt;Item&gt; mock used by domain object factories
///     (MockAisling, MockMapInstance, MockItem)
/// </summary>
public static class MockScriptProvider
{
    public static Mock<IScriptProvider> Instance { get; }
    public static Mock<ICloningService<Item>> ItemCloner { get; }

    static MockScriptProvider()
    {
        Instance = new Mock<IScriptProvider>();

        Instance.Setup(sp => sp.CreateScript<IAislingScript, Aisling>(It.IsAny<ICollection<string>>(), It.IsAny<Aisling>()))
                .Returns(new Mock<IAislingScript>().Object);

        Instance.Setup(sp => sp.CreateScript<IMapScript, MapInstance>(It.IsAny<ICollection<string>>(), It.IsAny<MapInstance>()))
                .Returns(new Mock<IMapScript>().Object);

        Instance.Setup(sp => sp.CreateScript<IItemScript, Item>(It.IsAny<ICollection<string>>(), It.IsAny<Item>()))
                .Returns(new Mock<IItemScript>().Object);

        ItemCloner = new Mock<ICloningService<Item>>();

        ItemCloner.Setup(c => c.Clone(It.IsAny<Item>()))
                  .Returns<Item>(original =>
                  {
                      var clone = new Item(original.Template, Instance.Object);
                      clone.Count = original.Count;

                      return clone;
                  });
    }
}