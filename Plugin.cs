using System;
using System.Reflection;
using System.Runtime.InteropServices;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using F7.Scripts.Screen;
using F7.Scripts.Dungeon.Base;

[BepInPlugin("kaeru.shiren6.azukariyacapacity", "Shiren 6 Azukariya Capacity", "1.4.2")]
public sealed class AzukariyaCapacityPlugin : BasePlugin
{
    internal static ManualLogSource? Logger;
    private Harmony? _harmony;

    private const int OriginalCapacity = 960;
    private const int PreviousCapacity = 2004;
    private const int NewCapacity = 9600;

    public override void Load()
    {
        Logger = Log;
        _harmony = new Harmony("kaeru.shiren6.azukariyacapacity");

        PatchGeneralScreen();
        PatchCgnSsr();

        Log.LogInfo("[Azukariya] v1.4.2 loaded. Capacity: 960/2004 -> 9600");
    }

    private void PatchGeneralScreen()
    {
        MethodInfo? target = AccessTools.Method(
            typeof(GeneralScreen),
            "bnhr",
            new[] { typeof(ekm) });

        if (target == null)
        {
            Log.LogWarning("[Azukariya] GeneralScreen.bnhr(ekm) not found.");
            return;
        }

        _harmony!.Patch(
            target,
            prefix: new HarmonyMethod(typeof(GeneralScreenPatch), nameof(GeneralScreenPatch.Prefix)));
    }

    private void PatchCgnSsr()
    {
        MethodInfo? target = AccessTools.Method(
            typeof(cgn),
            "ssr",
            new[] { typeof(ItemBase), typeof(bool), typeof(bool) });

        if (target == null)
        {
            Log.LogWarning("[Azukariya] cgn.ssr(ItemBase,bool,bool) not found.");
            return;
        }

        _harmony!.Patch(
            target,
            prefix: new HarmonyMethod(typeof(SsrPatch), nameof(SsrPatch.Prefix)));
    }

    private static void PromoteCapacity(Il2CppObjectBase? obj)
    {
        if (obj == null)
            return;

        try
        {
            IntPtr ptr = IL2CPP.Il2CppObjectBaseToPtr(obj);
            if (ptr == IntPtr.Zero)
                return;

            int a = Marshal.ReadInt32(ptr, 0x18);
            int b = Marshal.ReadInt32(ptr, 0x1C);

            // Accept either vanilla 960/960 or the previous modded 2004/2004 state.
            bool vanilla = a == OriginalCapacity && b == OriginalCapacity;
            bool previous = a == PreviousCapacity && b == PreviousCapacity;

            if (!vanilla && !previous)
                return;

            Marshal.WriteInt32(ptr, 0x18, NewCapacity);
            Marshal.WriteInt32(ptr, 0x1C, NewCapacity);
        }
        catch (Exception ex)
        {
            Logger?.LogWarning($"[Azukariya] Capacity patch failed: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static class GeneralScreenPatch
    {
        public static void Prefix(ekm a)
        {
            if (a == null)
                return;

            try
            {
                bsd? warehouse = a.eroa;
                if (warehouse != null)
                    PromoteCapacity((Il2CppObjectBase)(object)warehouse);
            }
            catch (Exception ex)
            {
                Logger?.LogWarning($"[Azukariya] GeneralScreen patch failed: {ex.GetType().Name}: {ex.Message}");
            }
        }
    }

    private static class SsrPatch
    {
        public static void Prefix(cgn __instance)
        {
            if (__instance == null)
                return;

            PromoteCapacity((Il2CppObjectBase)(object)__instance);
        }
    }
}