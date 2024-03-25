using System.Collections.Generic;

public static class Globals
{
    public static string ACTIVE_USER_ID = "";
    public static int[] LEVEL_EXP_REQUIREMENTS =
    {
        0, 200, 400, 600, 800, 1000, 
        1200, 1400, 1600, 1800, 2000,
        2200, 2400, 2600, 2800, 3000,
        3200, 3400, 3600, 3800, 4000
    };
    public static Dictionary<string, float> BLUE_DEFAULY_STATS = new Dictionary<string, float>()
    {
        { "move-speed", 150 },
        { "damage", 10 },
        { "fire-rate", 0.7f },
        { "explosion-range", 0 }
    };
    public static Dictionary<string, float> GREY_DEFAULT_STATS = new Dictionary<string, float>()
    {
        { "move-speed", 150 },
        { "damage", 30 },
        { "fire-rate", 0.7f },
        { "explosion-range", 1 }
    };
    public static Dictionary<string, float> CREAM_DEFAULT_STATS = new Dictionary<string, float>()
    {
        { "move-speed", 150 },
        { "damage", 8 },
        { "fire-rate", 0.7f },
        { "explosion-range", 0 }
    };
}