using BepInEx;
using Bepinject;
using HarmonyLib;
using PersonalPronouns.ComputerInterface;
using PersonalPronouns.Scripts;
using System.Reflection;

namespace PersonalPronouns
{
    [BepInDependency("tonimacaroni.computerinterface", "1.5.2")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public void Awake()
        {
            Harmony harmonyPatch = new Harmony(PluginInfo.GUID);
            harmonyPatch.PatchAll(Assembly.GetExecutingAssembly());

            Utils.InitPronouns();
            Zenjector.Install<MainInstaller>().OnProject();
        }
    }
}
