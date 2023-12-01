using Chaos.Client.Common.Abstractions;
using Chaos.Extensions.Client.Common;
using Chaos.Extensions.Common;
using DALib.Data;
using DALib.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace Chaos.Client.Data.Repositories;

public sealed class ArchiveRepository : RepositoryBase
{
    public DataArchive Cious { get; }
    public DataArchive Hades { get; }
    public DataArchive Ia { get; }
    public DataArchive Khanmad { get; }
    public DataArchive Khanmeh { get; }
    public DataArchive Khanmim { get; }
    public DataArchive Khanmns { get; }
    public DataArchive Khanmtz { get; }
    public DataArchive Khanpal { get; }
    public DataArchive Khanwad { get; }
    public DataArchive Khanweh { get; }
    public DataArchive Khanwim { get; }
    public DataArchive Khanwns { get; }
    public DataArchive Khanwtz { get; }
    public DataArchive Legend { get; }
    public DataArchive Misc { get; }
    public DataArchive National { get; }
    public DataArchive Npcbase { get; }
    public DataArchive Roh { get; }
    public DataArchive Seo { get; }
    public DataArchive Setoa { get; }

    /// <inheritdoc />
    public ArchiveRepository(IMemoryCache cache)
        : base(cache)
    {
        Cious = Get(nameof(Cious));
        Hades = Get(nameof(Hades));
        Ia = Get(nameof(Ia));
        Khanmad = Get(nameof(Khanmad));
        Khanmeh = Get(nameof(Khanmeh));
        Khanmim = Get(nameof(Khanmim));
        Khanmns = Get(nameof(Khanmns));
        Khanmtz = Get(nameof(Khanmtz));
        Khanpal = Get(nameof(Khanpal));
        Khanwad = Get(nameof(Khanwad));
        Khanweh = Get(nameof(Khanweh));
        Khanwim = Get(nameof(Khanwim));
        Khanwns = Get(nameof(Khanwns));
        Khanwtz = Get(nameof(Khanwtz));
        Legend = Get(nameof(Legend));
        Misc = Get(nameof(Misc));
        National = Get(nameof(National));
        Roh = Get(nameof(Roh));
        Seo = Get(nameof(Seo));
        Setoa = Get(nameof(Setoa));
        Npcbase = Get(nameof(Npcbase));
    }

    public DataArchive Get(string key)
    {
        const string ENTRY_PREFIX = "ARCHIVE_";

        key = key.WithExtension(".dat");
        var entryKey = ENTRY_PREFIX + key;

        return Cache.SafeGetOrCreate(
            entryKey,
            entry =>
            {
                entry.SetPriority(CacheItemPriority.NeverRemove);

                if (key.EqualsI("npcbase.dat"))
                    return DataArchive.FromFile($"npc/{key}");

                return DataArchive.FromFile(key);
            });
    }
}