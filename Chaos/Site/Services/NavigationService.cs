using Chaos.Site.Models;
using Microsoft.Extensions.Options;

namespace Chaos.Site.Services;

public sealed class NavigationService(IOptionsSnapshot<SiteOptions> options)
{
    private readonly SiteOptions Options = options.Value;

    public IEnumerable<NavLink> GetNavLinks()
    {
        var navLinks = new List<NavLink>
        {
            new()
            {
                Text = "Home",
                Url = "/Index"
            }
        };

        if (Options.ShowItems)
            navLinks.Add(
                new NavLink
                {
                    Text = "Items",
                    Url = "/Items"
                });

        if (Options.ShowSkills)
            navLinks.Add(
                new NavLink
                {
                    Text = "Skills",
                    Url = "/Skills"
                });

        if (Options.ShowSpells)
            navLinks.Add(
                new NavLink
                {
                    Text = "Spells",
                    Url = "/Spells"
                });

        if (Options.ShowMonsters)
            navLinks.Add(
                new NavLink
                {
                    Text = "Monsters",
                    Url = "/Monsters"
                });

        return navLinks;
    }
}