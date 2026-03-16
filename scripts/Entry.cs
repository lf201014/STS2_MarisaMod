using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace MarisaMod.scripts
{
    [ModInitializer(nameof(Initialize))]
    public class Entry
    {
        // 打patch（即修改游戏代码的功能）用
        private static Harmony? _harmony;

        // 初始化函数
        public static void Init()
        {
            // 传入参数随意，只要不和其他人撞车即可
            _harmony = new Harmony("MarisaMod");
            _harmony.PatchAll();
            Log.Info("Mod initialized!");
            ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        }

        public const string ModId = "MarisaMod"; //Used for resource filepath

        public static Logger Logger { get; } = new(ModId, LogType.Generic);

        public static void Initialize()
        {
            Harmony harmony = new(ModId);

            harmony.PatchAll();
            Logger.Info($"{ModId} initialized!");
        }
    }
}