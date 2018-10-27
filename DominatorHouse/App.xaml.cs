using AutoMapper;
using DominatorHouse.AutoMapping;
using System.Windows;
using Unity;
using DominatorHouseCore;
namespace Socinator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            InitializeAutoMapper();
            base.OnStartup(e);
        }

        private void InitializeAutoMapper()
        {
            var moduleProfiles = IoC.Container.ResolveAll<Profile>();
            AutoMapperConfiguration.Init(moduleProfiles);
        }
    }
}
