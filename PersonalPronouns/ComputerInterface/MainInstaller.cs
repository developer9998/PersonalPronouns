using ComputerInterface.Interfaces;
using Zenject;

namespace PersonalPronouns.ComputerInterface
{
    public class MainInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IComputerModEntry>().To<PronounEntry>().AsSingle();
        }
    }
}
