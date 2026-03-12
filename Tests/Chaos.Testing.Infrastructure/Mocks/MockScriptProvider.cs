#region
using Chaos.Collections;
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.Scripting.MapScripts.Abstractions;
using Chaos.Scripting.MerchantScripts.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
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
                .Returns(() => new Mock<IAislingScript>().Object);

        Instance.Setup(sp => sp.CreateScript<IMapScript, MapInstance>(It.IsAny<ICollection<string>>(), It.IsAny<MapInstance>()))
                .Returns(() => new Mock<IMapScript>().Object);

        Instance.Setup(sp => sp.CreateScript<IItemScript, Item>(It.IsAny<ICollection<string>>(), It.IsAny<Item>()))
                .Returns(() => new Mock<IItemScript>().Object);

            Instance.Setup(sp => sp.CreateScript<IMonsterScript, Monster>(It.IsAny<ICollection<string>>(), It.IsAny<Monster>()))
                    .Returns(() => new Mock<IMonsterScript>().Object);

            Instance.Setup(sp => sp.CreateScript<ISkillScript, Skill>(It.IsAny<ICollection<string>>(), It.IsAny<Skill>()))
                    .Returns(() => new Mock<ISkillScript>().Object);

            Instance.Setup(sp => sp.CreateScript<ISpellScript, Spell>(It.IsAny<ICollection<string>>(), It.IsAny<Spell>()))
                    .Returns(() => new Mock<ISpellScript>().Object);

            Instance.Setup(sp => sp.CreateScript<IMerchantScript, Merchant>(It.IsAny<ICollection<string>>(), It.IsAny<Merchant>()))
                    .Returns(() => new Mock<IMerchantScript>().Object);

            Instance.Setup(sp => sp.CreateScript<IReactorTileScript, ReactorTile>(It.IsAny<ICollection<string>>(), It.IsAny<ReactorTile>()))
                    .Returns(() => new Mock<IReactorTileScript>().Object);

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