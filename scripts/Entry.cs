using System.Reflection;
using BaseLib.Config;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using marisamod.Scripts.Cards;
using marisamod.Scripts.Characters;
using marisamod.Scripts.Powers;
using marisamod.Scripts.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Managers;
// ReSharper disable InconsistentNaming

namespace marisamod.Scripts;

[ModInitializer("Init")]
public class Entry
{
    private const string LogPrefix = "[MarisaMod]";

    public static void Init()
    {
        Log.Info($"{LogPrefix} Init called");
        ModConfigRegistry.Register("test", new ModConfig());
        var harmony = new Harmony("marisamod");
        harmony.PatchAll(typeof(Entry).Assembly);
        Log.Info($"{LogPrefix} Harmony PatchAll completed");
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        //const string gamePath = "res://images/atlases/ui_atlas.sprites/card/energy_test.tres";
        //const string modPath = "res://marisamod/images/atlases/ui_atlas.sprites/card/energy_test.tres";
        //Log.Info($"{LogPrefix} energy_test.tres 存在性: res://images/... = {ResourceLoader.Exists(gamePath)}, res://marisamod/images/... = {ResourceLoader.Exists(modPath)}");
    }

    [HarmonyPatch(typeof(ProgressSaveManager), "ObtainCharUnlockEpoch")]
    public static class ProgressSaveManager_ObtainCharUnlockEpoch_Patch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer)
        {
            return localPlayer.Character is not MarisaCharacter;
        }
    }


    [HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenElitesDefeatedEpoch")]
    public static class ProgressSaveManager_CheckFifteenElitesDefeatedEpoch_Patch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer)
        {
            return localPlayer.Character is not MarisaCharacter;
        }
    }


    [HarmonyPatch(typeof(ProgressSaveManager), "CheckFifteenBossesDefeatedEpoch")]
    public static class ProgressSaveManager_CheckFifteenBossesDefeatedEpoch_Patch
    {
        private static bool Prefix(ProgressSaveManager __instance, Player localPlayer)
        {
            return !(localPlayer.Character.Id.ToString().Contains("MarisaMod", StringComparison.OrdinalIgnoreCase));
        }
    }

    [HarmonyPatch(typeof(CardModel), nameof(CardModel.PortraitPath), MethodType.Getter)]
    public static class CardModel_GetPortrait_Patch
    {
        private static readonly Dictionary<string, string> CustomPortraits = new(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(StrikeIronclad)] = "res://test/images/image.png",
            [nameof(DefendIronclad)] = "res://test/images/image.png",
        };

        static void Postfix(CardModel __instance, ref string __result)
        {
            var className = __instance?.GetType().Name;
            if (string.IsNullOrEmpty(className)) return;
            if (!CustomPortraits.TryGetValue(className, out var path)) return;
            if (!ResourceLoader.Exists(path)) return;
            __result = path;
        }
    }


    [HarmonyPatch(typeof(TheArchitect), "WinRun")]
    internal static class WatcherArchitectWinRunPatch
    {
        private static bool Prefix(TheArchitect __instance, ref Task __result)
        {
            FieldInfo fieldInfo = AccessTools.Field(typeof(TheArchitect), "_dialogue");
            if (((fieldInfo != null) ? fieldInfo.GetValue(__instance) : null) != null)
            {
                return true;
            }

            if (LocalContext.IsMe(__instance.Owner))
            {
                RunManager.Instance.ActChangeSynchronizer.SetLocalPlayerReady();
            }

            __result = Task.CompletedTask;
            return false;
        }
    }

    [HarmonyPatch(typeof(Entomancer), "SpitMove")]
    internal static class EntomancerSpitMovePatch
    {
        private static AsyncLocal<Func<Task>> _asyncWork = new();

        private static bool Prefix(Entomancer __instance, ref Task __result)
        {
            if (__instance.Creature.HasPower<PersonalHivePower>())
            {
                return true;
            }

            _asyncWork.Value = async () =>
            {
                var fieldInfo = AccessTools.Field(typeof(Entomancer), "CastSfx");
                if ((fieldInfo != null ? fieldInfo.GetValue(__instance) : null) != null)
                    SfxCmd.Play((string)fieldInfo.GetValue(__instance));
                await CreatureCmd.TriggerAnim(__instance.Creature, "Cast", 0.5f);
                await PowerCmd.Apply<PersonalHivePower>(__instance.Creature, 1, __instance.Creature, null);
            };

            __result = _asyncWork.Value();

            return false;
        }

        private static void Postfix()
        {
            _asyncWork.Value = null!;
        }
    }

    // //well I just cannot make it right
    // [HarmonyPatch(typeof(NParticlesContainer), "Restart")]
    // internal static class ParticlesContainerRestartPatch
    // {
    //     private static bool Prefix(NParticlesContainer __instance)
    //     {
    //         FieldInfo fieldInfo = AccessTools.Field(typeof(NParticlesContainer), "_particles");
    //         if ((fieldInfo?.GetValue(__instance)) != null)
    //         {
    //             return true;
    //         }

    //         return false;
    //     }
    // }

    [HarmonyPatch(typeof(TouchOfOrobas), "GetUpgradedStarterRelic")]
    internal static class TouchOfOrobasGetUpgradedStarterRelicPatch
    {
        private static bool Prefix(TouchOfOrobas __instance, RelicModel starterRelic, ref RelicModel __result)
        {
            if (starterRelic is MiniHakkero)
            {
                __result = ModelDb.Relic<BewitchedHakkero>();
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceStarterCard")]
    internal static class ArchaicToothGetTranscendenceStarterCardPatch
    {
        private static bool Prefix(ArchaicTooth __instance, Player player, ref CardModel? __result)
        {
            if (player.Character is MarisaCharacter)
            {
                __result = player.Deck.Cards.FirstOrDefault(c => c is MasterSpark); ;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceTransformedCard")]
    internal static class ArchaicToothGetTranscendenceTransformedCardPatch
    {
        private static bool Prefix(ArchaicTooth __instance, CardModel starterCard, ref CardModel? __result)
        {
            if (starterCard is MasterSpark)
            {
                __result = starterCard.Owner.RunState.CreateCard(ModelDb.Card<FinalMasterSpark>(), starterCard.Owner);
                if (starterCard.IsUpgraded)
                {
                    CardCmd.Upgrade(__result);
                }
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(DustyTome), "SetupForPlayer")]
    internal static class DustyTomeSetupForPlayerPatch
    {
        private static bool Prefix(DustyTome __instance, Player player)
        {
            if (player.Character is MarisaCharacter)
            {
                __instance.AncientCard = ModelDb.Card<MagicAndRedDream>().Id;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(Burn), nameof(Burn.OnTurnEndInHand))]
    internal static class BurnOnTurnEndInHandPatch
    {
        private static bool Prefix(Burn __instance)
        {
            if (__instance.Owner.Creature.HasPower<SuperNovaPower>())
            {
                return false;
            }

            return true;
        }
    }

    // [HarmonyPatch(typeof(EnergyIconHelper), nameof(EnergyIconHelper.GetPath), typeof(string))]
    // internal static class EnergyIconHelperGetPathPatch
    // {
    //     private static bool Prefix(string prefix, ref string __result)
    //     {
    //         if (prefix == "marisa")
    //         {
    //             __result = "res://marisamod/images/ui/cardEnergyMarisa.png";
    //             return false;
    //         }

    //         return true;
    //     }
    // }

    // [HarmonyPatch(typeof(NCreature), "_Ready")]
    // static class NCreature_Ready_SpineReplace_Patch
    // {
    //     private static readonly Dictionary<string, string> CharacterSkeletonPaths = new()
    //     {
    //         ["IRONCLAD"] = "res://test/test_skin.tres",
    //     };
    //
    //     /// <summary>
    //     /// 自定义骨架加载失败时的回退路径（使用游戏原版资源）
    //     /// </summary>
    //     private const string FallbackIroncladSkeleton = "res://animations/characters/ironclad/ironclad_skel_data.tres";
    //
    //     private static readonly Dictionary<string, Resource> _skeletonDataCache = [];
    //
    //     static void Postfix(NCreature __instance)
    //     {
    //         if (!ModConfig.EnableSkinReplace) return;
    //         if (__instance?.Entity?.Player == null) return;
    //
    //         var characterId = __instance.Entity.Player.Character.Id.Entry;
    //         if (!CharacterSkeletonPaths.TryGetValue(characterId, out var path)) return;
    //
    //         var visuals = __instance.Visuals;
    //         if (visuals?.Body == null || !visuals.HasSpineAnimation)
    //         {
    //             Log.Warn($"{LogPrefix} Skip: visuals invalid (Body={visuals?.Body != null}, HasSpineAnimation={visuals?.HasSpineAnimation})");
    //             return;
    //         }
    //
    //         var skeletonData = GetOrLoadSkeletonData(path);
    //         if (skeletonData == null)
    //         {
    //             Log.Warn($"{LogPrefix} Custom skeleton failed, trying fallback '{FallbackIroncladSkeleton}'");
    //             skeletonData = GetOrLoadSkeletonData(FallbackIroncladSkeleton);
    //             if (skeletonData == null)
    //             {
    //                 Log.Error($"{LogPrefix} Fallback also failed, skeleton replace aborted");
    //                 return;
    //             }
    //         }
    //
    //         try
    //         {
    //             new MegaSprite(visuals.Body).SetSkeletonDataRes(new MegaSkeletonDataResource(skeletonData));
    //             Log.Info($"{LogPrefix} Skeleton replaced for {characterId}");
    //         }
    //         catch (Exception ex)
    //         {
    //             Log.Error($"{LogPrefix} SetSkeletonDataRes failed: {ex.Message}\n{ex.StackTrace}");
    //         }
    //     }
    //
    //     private static Resource? GetOrLoadSkeletonData(string skeletonPath)
    //     {
    //         if (_skeletonDataCache.TryGetValue(skeletonPath, out var cached) &&
    //             GodotObject.IsInstanceValid(cached))
    //         {
    //             return cached;
    //         }
    //
    //         if (!ResourceLoader.Exists(skeletonPath))
    //         {
    //             Log.Warn($"{LogPrefix} Resource does not exist: '{skeletonPath}'");
    //             return null;
    //         }
    //
    //         try
    //         {
    //             var data = ResourceLoader.Load<Resource>(skeletonPath);
    //             if (data != null)
    //             {
    //                 _skeletonDataCache[skeletonPath] = data;
    //                 Log.Info($"{LogPrefix} Loaded: '{skeletonPath}' (type={data.GetType().Name})");
    //             }
    //             else
    //             {
    //                 Log.Warn($"{LogPrefix} ResourceLoader.Load returned null for '{skeletonPath}' (可能需要 .import 文件，请在带 spine-godot 的编辑器中重新导入 .atlas/.skel)");
    //             }
    //
    //             return data;
    //         }
    //         catch (Exception ex)
    //         {
    //             Log.Warn($"{LogPrefix} Load exception for '{skeletonPath}': {ex.Message}");
    //             return null;
    //         }
    //     }
    // }
}

public class ModConfig : SimpleModConfig
{
    public static bool Test1 { get; set; } = true;
    public static bool Test2 { get; set; } = false;
    public static bool Test3 { get; set; } = true;

    /// <summary> 是否启用皮肤替换（调试时可设为 false） </summary>
    public static bool EnableSkinReplace { get; set; } = true;
}