using BepInEx;
using System.Runtime.CompilerServices;
using MoreSlugcats;
using BepInEx.Logging;

namespace HungrySpears;

[BepInPlugin(MOD_ID, "Hungry Spears", "0.1.0")]
class Plugin : BaseUnityPlugin
{
    public static bool isInit = false;

    public const string MOD_ID = "nc.HungrySpears";

    public static ConditionalWeakTable<Spear, Player> hungrySpears = new();

    /*
    public void ELog(object data, LogLevel level = LogLevel.Debug)
    {
        if (Options.enableExtraLogging.Value) Logger.Log(level, data);
    }
    */

    public void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorld_LoadOptions;
        On.Spear.ctor += Spear_ctor;
        On.Spear.Thrown += Spear_Thrown;
        On.Spear.HitSomething += Spear_HitSomething;
    }

    private bool Spear_HitSomething(On.Spear.orig_HitSomething orig, Spear self, SharedPhysics.CollisionResult result, bool eu)
    {
        bool hit = orig.Invoke(self, result, eu);
        if (hungrySpears.TryGetValue(self, out Player player) && result.obj is Creature && hit)
        {
            Creature creature = result.obj as Creature;
            if (ModManager.MSC ? player.SlugCatClass != MoreSlugcatsEnums.SlugcatStatsName.Spear : false && !creature.dead && creature is not GarbageWorm && creature is not Deer)
            {
                if (creature is Cicada || creature is TubeWorm || creature is Snail || creature is Leech || creature is VultureGrub || creature is SmallNeedleWorm || creature is Hazer || (creature is Centipede && (creature as Centipede).Small) || (creature is Vulture && result.onAppendagePos != null))
                {
                    for (int i = 0; i < 2; i++)
                    {
                        player.AddQuarterFood();
                    }
                }
                else if (creature is DaddyLongLegs && result.onAppendagePos != null)
                {
                    player.AddQuarterFood();
                }
                else if (creature is EggBug)
                {
                    player.AddFood(3);
                }
                else if (creature is BigSpider)
                {
                    if ((creature as BigSpider).borrowedTime == -1)
                        player.AddFood(1);
                }
                else
                {
                    player.AddFood(1);
                }

                if (player.room.game.IsStorySession && player.room.game.GetStorySession.playerSessionRecords != null)
                    player.room.game.GetStorySession.playerSessionRecords[(player.abstractCreature.state as PlayerState).playerNumber].AddEat(result.obj);
            }
            hungrySpears.Remove(self);
        }

        return hit;
    }

    private void Spear_Thrown(On.Spear.orig_Thrown orig, Spear self, Creature thrownBy, UnityEngine.Vector2 thrownPos, UnityEngine.Vector2? firstFrameTraceFromPos, RWCustom.IntVector2 throwDir, float frc, bool eu)
    {
        if (thrownBy is Player)
        {
            if (hungrySpears.TryGetValue(self, out Player _))
                hungrySpears.Remove(self);
            hungrySpears.Add(self, thrownBy as Player);
        }
        orig(self, thrownBy, thrownPos, firstFrameTraceFromPos, throwDir, frc, eu);
    }

    private void Spear_ctor(On.Spear.orig_ctor orig, Spear self, AbstractPhysicalObject abstractPhysicalObject, World world)
    {
        orig(self, abstractPhysicalObject, world);
        if (Options.makeAllSpearsNeedles.Value && ModManager.MSC && !self.spearmasterNeedle)
            self.Spear_makeNeedle(UnityEngine.Random.Range(0, 3), false);
    }

    private void RainWorld_LoadOptions(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);

        Logger.LogInfo("Loading remix options...");
        MachineConnector.SetRegisteredOI(MOD_ID, new Options());
        Logger.LogInfo("Loading complete!");
    }
}
