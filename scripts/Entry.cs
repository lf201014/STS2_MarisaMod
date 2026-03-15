using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace MarisaMod.scripts
{
    [ModInitializer("Init")]
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
            Log.Debug("Mod initialized!");
            ScriptManagerBridge.LookupScriptsInAssembly(typeof(Entry).Assembly);
        }
    }
}