using PDEX.DAL;
using PDEX.DAL.Interfaces;
using PDEX.WPF.ViewModel;
using Microsoft.Practices.Unity;
using PDEX.Repository.Interfaces;
using PDEX.Repository;
using PDEX.Core;

namespace PDEX.WPF
{
    public class Bootstrapper
    {
        public IUnityContainer Container { get; set; }

        public Bootstrapper()
        {
            Container = new UnityContainer();
            ConfigureContainer();
        }

        private void ConfigureContainer()
        {
            Singleton.SqlceFileName = "PdexDb";
            Singleton.SeedDefaults = true;

            Container.RegisterType<IDbContext, PDEXDbContext>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IUnitOfWork, UnitOfWork>();

            Container.RegisterType<MainViewModel>();
        }
    }
}
