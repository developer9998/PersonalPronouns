using HarmonyLib;

namespace PersonalPronouns.Scoreboard
{
    [HarmonyPatch(typeof(GorillaPlayerScoreboardLine), "Start", MethodType.Normal)]
    internal class LineStartPatch
    {
        internal static void Prefix(GorillaPlayerScoreboardLine __instance)
        {
            if (__instance.GetComponent<PronounLine>() == null)
            {
                __instance.gameObject.AddComponent<PronounLine>().baseLine = __instance;
            }
        }
    }
}
