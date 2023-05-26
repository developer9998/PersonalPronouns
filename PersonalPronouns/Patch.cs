using HarmonyLib;
using PersonalPronouns.Scripts;

namespace PersonalPronouns
{
    [HarmonyPatch(typeof(VRRig), "SharedStart")]
    internal class Patch
    {
        internal static void Postfix(VRRig __instance)
        {
            if (__instance.photonView != null && __instance.photonView.IsMine) return;
            __instance.gameObject.AddComponent<Client>().Local = __instance;
        }
    }
}
