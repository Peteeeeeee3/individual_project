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
    public static List<float> MOVE_SPEED_UPGRADE_VALUES = new List<float>()
    {
        150, 160, 170, 180, 190, 200
    };
    public static Dictionary<PlayerType, List<float>> DAMAGE_UPGRADE_VALUES = new Dictionary<PlayerType, List<float>>()
    {
        { PlayerType.BLUE, new List<float>()
            {
                10, 13, 16, 19, 21, 24
            }
        },
        { PlayerType.GREY, new List<float>()
            {
                30, 32, 34, 36, 38, 40
            }
        },
        { PlayerType.CREAM, new List<float>()
            {
                8, 12, 16, 20, 24, 28
            }
        }
    };
    public static List<float> ATTACK_RATE_UPGRADE_VALUES = new List<float>()
    {
        0.7f, 0.65f, 0.6f, 0.55f, 0.5f, 0.45f
    };
    public static List<float> ATTACK_RANGE_UPGRADE_VALUES = new List<float>()
    {
        1.0f, 1.05f, 1.1f, 1.15f, 1.2f, 1.25f
    };
}