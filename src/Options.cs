using Menu.Remix.MixedUI;
using UnityEngine;

namespace HungrySpears;

sealed class Options : OptionInterface
{
    //taken from https://github.com/Dual-Iron/no-damage-rng/blob/master/src/Plugin.cs
    //thanks dual, you're a life saver

    public static Configurable<bool> makeAllSpearsNeedles;
    //public static Configurable<bool> enableExtraLogging;

    public Options()
    {
        makeAllSpearsNeedles = config.Bind("nc_MakeAllSpearsNeedles", false);
        //enableExtraLogging = config.Bind("nc_EnableExtraLogging", false);
    }

    public override void Initialize()
    {
        base.Initialize();

        Tabs = new OpTab[] { new(this) };

        var labelTitle = new OpLabel(20, 600 - 30, "Hungry Spears Options", true);

        var top = 200;
        var labelMakeAllSpearsNeedles = new OpLabel(new(100, 600 - top), Vector2.zero, "Make all spears needles", FLabelAlignment.Left);
        var checkMakeAllSpearsNeedles = new OpCheckBox(makeAllSpearsNeedles, new Vector2(20, 600 - top - 6))
        {
            description = "If true, all spears in the game will be Spearmaster needles(only works with MSC enabled)",
        };

        /*
        var labelEnableExtraLogging = new OpLabel(new(100, 750 - top), Vector2.zero, "Enable extra logging", FLabelAlignment.Left);
        var checkEnableExtraLogging = new OpCheckBox(makeAllSpearsNeedles, new Vector2(20, 750 - top - 6))
        {
            description = "Turn this on for trouble shooting",
        };
        */

        Tabs[0].AddItems(
            labelTitle,
            labelMakeAllSpearsNeedles,
            checkMakeAllSpearsNeedles
        //labelEnableExtraLogging,
        //checkEnableExtraLogging
        );
    }
}
