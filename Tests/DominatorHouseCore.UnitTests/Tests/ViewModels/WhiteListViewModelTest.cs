using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using System.Windows;
using Dominator.Tests.Utils;
using DominatorUIUtility.ViewModel;
using DominatorHouseCore.Diagnostics;
using NSubstitute;
using DominatorHouseCore.Interfaces;
using SQLite;
using DominatorHouseCore.Enums.DHEnum;
using System.Threading;
using DominatorHouseCore.DatabaseHandler.DHTables;
using System;
using Socinator;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.Utility;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.ViewModels
{
    [TestClass]
    public class WhiteListViewModelTest : UnityInitializationTests
    {
        WhiteListViewModel WhiteListViewModel;
        private IGlobalDatabaseConnection _globalDb;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            var waitForApplicationRun = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var application = new Application();
                application.Startup += (s, e) => { waitForApplicationRun.SetResult(true); };
                application.Run();
            });
            waitForApplicationRun.Task.Wait();
        }
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            Application.Current.Dispatcher.Invoke(Application.Current.Shutdown);
        }
        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _globalDb = Substitute.For<IGlobalDatabaseConnection>();
            Container.RegisterInstance(_globalDb);
            WhiteListViewModel = new WhiteListViewModel();
            var whiteListdb = new SQLiteConnection(@"C:\Users\GLB-259\AppData\Local\Socinator\Index\Global\DB\WhiteListedUser\Quora.db");
            var blackListdb = new SQLiteConnection(@"C:\Users\GLB-259\AppData\Local\Socinator\Index\Global\DB\BlackListedUser\Quora.db");
            _globalDb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser).ReturnsForAnyArgs(whiteListdb);
            _globalDb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser).ReturnsForAnyArgs(blackListdb);

        }
        [TestMethod]
        public void InitializeData_method_should_initialize_whitelist_user()
        {
            WhiteListViewModel.InitializeData();
            _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
            _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
            WhiteListViewModel.LstWhiteListUsers.Should().NotBeNull();
        }
        [TestMethod]
        public void AddToWhiteList_method_should_add_user_to_whitelist_user_list()
        {
            WhiteListViewModel.WhitelistUser = "Kumar";
            WhiteListViewModel.InitializeData();
            var thread = new Thread(() => WhiteListViewModel.AddToWhiteList(new object()));
            thread.Start();
            thread.Join();
            _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
            _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
            WhiteListViewModel.LstWhiteListUsers.Should().NotBeNull();
            WhiteListViewModel.WhitelistUser.Should().BeEmpty();

        }
        [TestMethod]
        public void Refresh_method_should_refresh_whitelist_user_list()
        {
            WhiteListViewModel.InitializeData();
             WhiteListViewModel.Refresh(new object());
           
            _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
            _globalDb.Received(1).GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
            WhiteListViewModel.LstWhiteListUsers.Should().NotBeNull();
        }
    }

}
