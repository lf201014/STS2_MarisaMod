using System.Reflection;
using BaseLib.Config;
using Godot;
using HarmonyLib;
using marisamod.Scripts.Cards;
using marisamod.Scripts.Characters;
using marisamod.Scripts.Powers;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
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
using MegaCrit.Sts2.Core.TestSupport;

// ReSharper disable InconsistentNaming

namespace marisamod.Scripts;

[ModInitializer("Init")]
public class Entry
{
    private const string LogPrefix = "[MarisaMod]";

    public static void Init()
    {
        //ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        var assembly = Assembly.GetExecutingAssembly();
        Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);

        Log.Info($"{LogPrefix} Init called");
        //ModConfigRegistry.Register("marisamod", new MehModConfig());
        var harmony = new Harmony("marisamod");
        harmony.PatchAll(typeof(Entry).Assembly);
        Log.Info($"{LogPrefix} Harmony PatchAll completed");

        Log.Info($"{LogPrefix} Init Done");
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
    internal static class TheArchitectWinRunPatch
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
                if (fieldInfo != null)
                {
                    if (fieldInfo.GetValue(__instance) != null && fieldInfo.GetValue(__instance) is string sfxName)
                        SfxCmd.Play(sfxName);
                }

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

    // [HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceStarterCard")]
    // internal static class ArchaicToothGetTranscendenceStarterCardPatch
    // {
    //     private static bool Prefix(ArchaicTooth __instance, Player player, ref CardModel? __result)
    //     {
    //         if (player.Character is MarisaCharacter)
    //         {
    //             __result = player.Deck.Cards.FirstOrDefault(c => c is MasterSpark);
    //             ;
    //             return false;
    //         }
    //
    //         return true;
    //     }
    // }
    //
    // [HarmonyPatch(typeof(ArchaicTooth), "GetTranscendenceTransformedCard")]
    // internal static class ArchaicToothGetTranscendenceTransformedCardPatch
    // {
    //     private static bool Prefix(ArchaicTooth __instance, CardModel starterCard, ref CardModel? __result)
    //     {
    //         if (starterCard is MasterSpark)
    //         {
    //             __result = starterCard.Owner.RunState.CreateCard(ModelDb.Card<FinalMasterSpark>(), starterCard.Owner);
    //             if (starterCard.IsUpgraded)
    //             {
    //                 CardCmd.Upgrade(__result);
    //             }
    //
    //             return false;
    //         }
    //
    //         return true;
    //     }
    // }
    //
    // [HarmonyPatch(typeof(DustyTome), "SetupForPlayer")]
    // internal static class DustyTomeSetupForPlayerPatch
    // {
    //     private static bool Prefix(DustyTome __instance, Player player)
    //     {
    //         if (player.Character is MarisaCharacter)
    //         {
    //             __instance.AncientCard = ModelDb.Card<MagicAndRedDream>().Id;
    //             return false;
    //         }
    //
    //         return true;
    //     }
    // }

    [HarmonyPatch(typeof(Burn), nameof(Burn.HasTurnEndInHandEffect), MethodType.Getter)]
    internal static class BurnHasTurnEndInHandEffectPatch
    {
        private static bool Prefix(Burn __instance, ref bool __result)
        {
            if (__instance.Owner.Creature.HasPower<SuperNovaPower>())
            {
                __result = false;
                return false;
            }

            return true;
        }
    }

    private const string CookiePath = "yummy_cookie_marisa";

    [HarmonyPatch(typeof(YummyCookie), "IconBaseName", MethodType.Getter)]
    internal static class YummyCookieIconBaseNamePatch
    {
        private static bool Prefix(YummyCookie __instance, ref string __result)
        {
            if (__instance.IsCanonical)
            {
                return true;
            }

            if (__instance.Owner.Character is MarisaCharacter)
            {
                __result = CookiePath;
                return false;
            }

            return true;
        }
    }


    [HarmonyPatch(typeof(PhantasmalGardener), nameof(PhantasmalGardener.GenerateAnimator))]
    internal static class PhantasmalGardenerGenerateAnimatorPatch
    {
        private static bool Prefix(PhantasmalGardener __instance, ref CreatureAnimator __result, MegaSprite controller)
        {
            AnimState animState = new AnimState("idle_loop", isLooping: true);
            AnimState animState2 = new AnimState("buff");
            AnimState animState3 = new AnimState("attack");
            AnimState animState4 = new AnimState("attack_multi");
            AnimState animState5 = new AnimState("hurt_extended");
            AnimState animState6 = new AnimState("hurt");
            AnimState state = new AnimState("die");
            AnimState nextState = new AnimState("block_loop", isLooping: true);
            AnimState animState7 = new AnimState("block_start");
            AnimState animState8 = new AnimState("block_end");
            animState2.NextState = animState;
            animState3.NextState = animState;
            animState4.NextState = animState;
            animState5.NextState = animState;
            animState6.NextState = nextState;
            animState7.NextState = nextState;
            animState8.NextState = animState;
            CreatureAnimator creatureAnimator = new CreatureAnimator(animState, controller);
            creatureAnimator.AddAnyState("Idle", animState);
            creatureAnimator.AddAnyState("Cast", animState2);
            creatureAnimator.AddAnyState("Attack", animState3);
            creatureAnimator.AddAnyState("AttackMulti", animState4);
            creatureAnimator.AddAnyState("Dead", state);
            creatureAnimator.AddAnyState("Hit", animState5, () => __instance.Creature.HasPower<SkittishPower>() && !__instance.Creature.GetPower<SkittishPower>()!.HasGainedBlockThisTurn);
            creatureAnimator.AddAnyState("Hit", animState6, () => !__instance.Creature.HasPower<SkittishPower>() || __instance.Creature.GetPower<SkittishPower>()!.HasGainedBlockThisTurn);
            creatureAnimator.AddAnyState("BlockStart", animState7);
            creatureAnimator.AddAnyState("BlockEnd", animState8);

            __result = creatureAnimator;
            return false;
        }
    }

    [HarmonyPatch(typeof(Parafright), nameof(PhantasmalGardener.GenerateAnimator))]
    internal static class ParafrightGenerateAnimatorPatch
    {
        private static bool Prefix(Parafright __instance, ref CreatureAnimator __result, MegaSprite controller)
        {
            AnimState nextState = new AnimState("idle_loop", isLooping: true);
            AnimState animState = new AnimState("attack");
            AnimState state = new AnimState("die");
            AnimState animState2 = new AnimState("hurt");
            AnimState animState3 = new AnimState("hurt_stunned");
            AnimState nextState2 = new AnimState("stunned_loop", isLooping: true);
            AnimState animState4 = new AnimState("spawn");
            AnimState animState5 = new AnimState("wake_up");
            AnimState animState6 = new AnimState("stun");
            animState4.NextState = nextState;
            animState.NextState = nextState;
            animState2.NextState = nextState;
            animState5.NextState = nextState;
            animState6.NextState = nextState2;
            animState3.NextState = nextState2;
            CreatureAnimator creatureAnimator = new CreatureAnimator(animState4, controller);
            creatureAnimator.AddAnyState("Attack", animState);
            creatureAnimator.AddAnyState("Hit", animState2, () => __instance.Creature.HasPower<SkittishPower>() && !__instance.Creature.GetPower<IllusionPower>()!.IsReviving);
            creatureAnimator.AddAnyState("Hit", animState3, () => !__instance.Creature.HasPower<SkittishPower>() || __instance.Creature.GetPower<IllusionPower>()!.IsReviving);
            creatureAnimator.AddAnyState("StunTrigger", animState6);
            creatureAnimator.AddAnyState("WakeUpTrigger", animState5);
            creatureAnimator.AddAnyState("Dead", state, () => !__instance.CombatState.GetTeammatesOf(__instance.Creature).Any(t => t is { IsPrimaryEnemy: true, IsAlive: true }));

            __result = creatureAnimator;
            return false;
        }
    }


    // [HarmonyPatch(typeof(RunManager), "UpdateRichPresence")]
    // internal static class RunManagerUpdateRichPresencePatch
    // {
    //     private static bool Prefix(RunManager __instance)
    //     {
    //         if (AccessTools.Field(typeof(RunManager), "State").GetValue(__instance) is RunState State)
    //         {
    //             var player = LocalContext.GetMe(State);
    //             if (player != null && player.Character is MarisaCharacter)
    //             {
    //                 
    //                 
    //                 return false;
    //             }
    //         }
    //
    //         return true;
    //     }
    // }

    [HarmonyPatch(typeof(RunManager))]
    public static class RichPresencePatch
    {
        private static readonly Lazy<MethodInfo?> SteamSetRichPresence = new(() =>
        {
            var t = AccessTools.TypeByName("Steamworks.SteamFriends");
            return t == null ? null : AccessTools.Method(t, "SetRichPresence", new[] { typeof(string), typeof(string) });
        });

        private static readonly Lazy<PropertyInfo?> StateProp = new(() =>
            AccessTools.DeclaredProperty(typeof(RunManager), "State"));

        [HarmonyPostfix]
        [HarmonyPatch("UpdateRichPresence")]
        public static void UpdateRichPresence_Postfix(RunManager? __instance)
        {
            if (__instance == null) return;
            var State = StateProp.Value?.GetValue(__instance) as RunState;
            if (!TestMode.IsOn && State != null)
            {
                var player = LocalContext.GetMe(State);
                if (player != null)
                {
                    var character = player.Character;
                    if (character is MarisaCharacter)
                    {
                        SteamSetRichPresence.Value?.Invoke(null, new object[] { "Character", "DEFECT" });
                        SteamSetRichPresence.Value?.Invoke(null, new object[] { "Ascension", "Marisa - A" + State.AscensionLevel });
                    }
                }
            }
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

public class MehModConfig : SimpleModConfig
{
    // [ConfigHoverTip]
    // public static bool NerfHakkero { get; set; } = true;
}