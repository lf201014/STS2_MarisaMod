using BaseLib.Config;
using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace marisamod.Scripts;

[ModInitializer("Init")]
public class Entry
{
    private const string LogPrefix = "[TestMod]";

    public static void Init()
    {
        Log.Info($"{LogPrefix} Init called");
        ModConfigRegistry.Register("test", new ModConfig());
        var harmony = new Harmony("marisamod");
        harmony.PatchAll(typeof(Entry).Assembly);
        Log.Info($"{LogPrefix} Harmony PatchAll completed");
        ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);

        const string gamePath = "res://images/atlases/ui_atlas.sprites/card/energy_test.tres";
        const string modPath = "res://test/images/atlases/ui_atlas.sprites/card/energy_test.tres";
        Log.Info($"{LogPrefix} energy_test.tres 存在性: res://images/... = {ResourceLoader.Exists(gamePath)}, res://test/images/... = {ResourceLoader.Exists(modPath)}");
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

    [HarmonyPatch(typeof(NCreature), "_Ready")]
    static class NCreature_Ready_SpineReplace_Patch
    {
        private static readonly Dictionary<string, string> CharacterSkeletonPaths = new()
        {
            ["IRONCLAD"] = "res://test/test_skin.tres",
        };

        /// <summary>
        /// 自定义骨架加载失败时的回退路径（使用游戏原版资源）
        /// </summary>
        private const string FallbackIroncladSkeleton = "res://animations/characters/ironclad/ironclad_skel_data.tres";

        private static readonly Dictionary<string, Resource> _skeletonDataCache = [];

        static void Postfix(NCreature __instance)
        {
            if (!ModConfig.EnableSkinReplace) return;
            if (__instance?.Entity?.Player == null) return;

            var characterId = __instance.Entity.Player.Character.Id.Entry;
            if (!CharacterSkeletonPaths.TryGetValue(characterId, out var path)) return;

            var visuals = __instance.Visuals;
            if (visuals?.Body == null || !visuals.HasSpineAnimation)
            {
                Log.Warn($"{LogPrefix} Skip: visuals invalid (Body={visuals?.Body != null}, HasSpineAnimation={visuals?.HasSpineAnimation})");
                return;
            }

            var skeletonData = GetOrLoadSkeletonData(path);
            if (skeletonData == null)
            {
                Log.Warn($"{LogPrefix} Custom skeleton failed, trying fallback '{FallbackIroncladSkeleton}'");
                skeletonData = GetOrLoadSkeletonData(FallbackIroncladSkeleton);
                if (skeletonData == null)
                {
                    Log.Error($"{LogPrefix} Fallback also failed, skeleton replace aborted");
                    return;
                }
            }

            try
            {
                new MegaSprite(visuals.Body).SetSkeletonDataRes(new MegaSkeletonDataResource(skeletonData));
                Log.Info($"{LogPrefix} Skeleton replaced for {characterId}");
            }
            catch (Exception ex)
            {
                Log.Error($"{LogPrefix} SetSkeletonDataRes failed: {ex.Message}\n{ex.StackTrace}");
            }
        }

        private static Resource? GetOrLoadSkeletonData(string skeletonPath)
        {
            if (_skeletonDataCache.TryGetValue(skeletonPath, out var cached) &&
                GodotObject.IsInstanceValid(cached))
            {
                return cached;
            }

            if (!ResourceLoader.Exists(skeletonPath))
            {
                Log.Warn($"{LogPrefix} Resource does not exist: '{skeletonPath}'");
                return null;
            }

            try
            {
                var data = ResourceLoader.Load<Resource>(skeletonPath);
                if (data != null)
                {
                    _skeletonDataCache[skeletonPath] = data;
                    Log.Info($"{LogPrefix} Loaded: '{skeletonPath}' (type={data.GetType().Name})");
                }
                else
                {
                    Log.Warn($"{LogPrefix} ResourceLoader.Load returned null for '{skeletonPath}' (可能需要 .import 文件，请在带 spine-godot 的编辑器中重新导入 .atlas/.skel)");
                }

                return data;
            }
            catch (Exception ex)
            {
                Log.Warn($"{LogPrefix} Load exception for '{skeletonPath}': {ex.Message}");
                return null;
            }
        }
    }
}

public class ModConfig : SimpleModConfig
{
    public static bool Test1 { get; set; } = true;
    public static bool Test2 { get; set; } = false;
    public static bool Test3 { get; set; } = true;

    /// <summary> 是否启用皮肤替换（调试时可设为 false） </summary>
    public static bool EnableSkinReplace { get; set; } = true;
}