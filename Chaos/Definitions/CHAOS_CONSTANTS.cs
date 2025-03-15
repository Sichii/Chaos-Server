#region
using System.Collections.Frozen;
using Chaos.DarkAges.Definitions;
#endregion

namespace Chaos.Definitions;

public static class CHAOS_CONSTANTS
{
    public static readonly MessageColor DEFAULT_CHANNEL_MESSAGE_COLOR = MessageColor.Gainsboro;

    public const int MAX_NOTEPAD_MESSAGE_LENGTH = 3500;

    // @formatter:wrap_array_initializer_style wrap_if_long
    // @formatter:max_array_initializer_elements_on_line 10000
    public static readonly FrozenSet<ushort> WATER_TILE_IDS = new ushort[]
    {
        16976, 16977, 16978, 16979, 16980, 16981, 16982, 16983, 16984, 16985, 16986, 16987, 16988, 16989, 16990, 16991, 16992, 16993, 16994,
        16995, 16996, 16997, 16998, 16999, 17000, 17001, 17002, 17003, 17004, 17005, 17006, 17007, 17020, 17021, 17022, 17023, 17024, 17025,
        17026, 17027, 17040, 17044, 17045, 17048, 17049, 17052, 17053, 17058, 17059, 17062, 17063, 17066, 17067, 17070, 17071, 17084, 17088,
        17106, 17107, 17135, 17136, 17137, 17138, 17139, 17140, 17141, 17142, 17143, 17144, 17146, 17147, 17151, 17155, 17159, 17160, 17161,
        17162, 17163, 17164, 17165, 17166, 17167, 17168, 17172, 17173, 17176, 17177, 17178, 17179, 17180, 17181, 17182, 17183, 17184, 17185,
        17186, 17187, 17188, 17189, 17192, 17196

        //add water tile ids here
    }.ToFrozenSet();

    // @formatter:max_array_initializer_elements_on_line restore
    // @formatter:wrap_array_initializer_style restore

    // @formatter:wrap_array_initializer_style wrap_if_long
    // @formatter:max_array_initializer_elements_on_line 10000
    public static readonly FrozenSet<ushort> DOOR_TILE_IDS = new ushort[]
    {
        1994, 1996, 1997, 2000, 2001, 2003, 2004, 2163, 2164, 2165, 2196, 2197, 2198, 2227, 2228, 2229, 2260, 2261, 2262, 2291, 2292, 2293,
        2328, 2329, 2330, 2432, 2436, 2461, 2465, 2673, 2674, 2675, 2680, 2681, 2682, 2687, 2688, 2689, 2694, 2695, 2696, 2714, 2715, 2721,
        2722, 2727, 2728, 2734, 2735, 2761, 2762, 2768, 2769, 2776, 2777, 2783, 2784, 2850, 2851, 2852, 2857, 2858, 2859, 2874, 2875, 2876,
        2881, 2882, 2883, 2897, 2898, 2903, 2904, 2923, 2924, 2929, 2930, 2945, 2946, 2951, 2952, 2971, 2972, 2977, 2978, 2993, 2994, 2999,
        3000, 3019, 3020, 3025, 3026, 3058, 3059, 3066, 3067, 3090, 3091, 3098, 3099, 3118, 3119, 3126, 3127, 3150, 3151, 3158, 3159, 3178,
        3179, 3186, 3187, 3210, 3211, 3218, 3219, 4519, 4520, 4521, 4523, 4524, 4525, 4527, 4528, 4529, 4532, 4533, 4534, 4536, 4537, 4538,
        4540, 4541, 4542
    }.ToFrozenSet();

    // @formatter:max_array_initializer_elements_on_line restore
    // @formatter:wrap_array_initializer_style restore

    // @formatter:wrap_array_initializer_style wrap_if_long
    // @formatter:max_array_initializer_elements_on_line 10000
    public static readonly FrozenSet<ushort> HAIR_SPRITES = new ushort[]
    {
        0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34,
        35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 161, 253, 254, 255, 263,
        264, 265, 266, 313, 314, 321, 324, 325, 326, 327, 333, 342, 343, 344, 345, 346, 347, 349, 383, 392, 397, 411, 412, 433, 435, 437,
        438, 440, 441, 447, 448, 449, 459, 460, 461, 476, 482, 483
    }.ToFrozenSet();

    // @formatter:max_array_initializer_elements_on_line restore
    // @formatter:wrap_array_initializer_style restore
}