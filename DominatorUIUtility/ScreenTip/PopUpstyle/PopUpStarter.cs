using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ScreenTipMode;
using DominatorUIUtility.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorUIUtility.ScreenTip.PopUpstyle
{
    public static class PopUpStarter
    {
        public static void StartIntroduction(object sender)
        {

            var tour = new ScreenInfo
            {
                Name = "Introduction",
                ShowNextButtonDefault = true,
                Steps = new[]
                {
                    new Step("Account", "Introduction", "Login with Accounts "),
                     new Step("AccountGrowth", "GrowthMode", "Shows you growth rate of accounts"),
                      new Step("DashBoard", "DashboardInfo", "Shows you Info regarding dashboard"),
                       new Step("AccountsActivity", "AccountsActivityInfo", "Shows you Info regarding dashboard"),
                        new Step("Sociopublisher", "SociopublisherInfo", "Shows you Info regarding dashboard"),

                }
            };
            tour.Start();
        }

        public static void StartOverView(object sender)
        {
            var tour = new ScreenInfo
            {
                Name = "Overview",
                ShowNextButtonDefault = false,
                Steps = new[]
               {
                    new Step("ButtonOverView", "Welcome - Select a tour", DominatorAccountViewModel.Instance)
                    {
                        ContentDataTemplateKey = "SelectTourDataTemplate"
                    },
                }
            };
            tour.Start();
        }

        public static void CloseView()
        {

        }


        public static Action<object> StartModuleOverView<TViewModel>(TViewModel objview)where TViewModel : class
        {
            var tour = new ScreenInfo
            {
                Name = "Overview",
                ShowNextButtonDefault = false,
                Steps = new[]
               {
                    new Step("ModuleButtonOverView", "Welcome - Start your Tutorial", objview)
                    {
                        ContentDataTemplateKey = "SelectSearchQueryTemplate"
                    },
                }
            };
            tour.Start();
            return null;
        }


        public static void StartSelectQueryIntro(object sender)
        {
            var tour = new ScreenInfo
            {
                Name = "Introduction",
                ShowNextButtonDefault = true,
                Steps = new[]
                {
                    new Step("SearchQuery", "SearchQuery", "Login with Accounts "),
                     new Step("JobConfiguration", "JobConfiguration", "Shows you growth rate of accounts"),
                      new Step("AfterFollow", "AfterFollow", "Shows you Info regarding dashboard"),
                       new Step("OtherConfiguration", "AccountsActivityInfo", "Shows you Info regarding dashboard"),
                        new Step("BlacklistUsers", "BlacklistUsers", "Shows you Info regarding dashboard"),

                }
            };
            tour.Start();
        }

        public static bool istutorial = false;
        public static void StartActiveTour(object sender)
        {
            istutorial = true;
            var tour = new ScreenInfo
            {
                Name = "Active Tour",
                ShowNextButtonDefault = false,
                Steps = new[]
                {
                    new Step("ComboBoxOption", "Select any Query", "Choose \"Keyword\" from the dropdown."),
                    new Step("TextBox", "Enter Value", "Please enter \"Flowers\" in the textbox."),
                    new Step("ButtonAdd", "Add Queries", "Click here to Add to List"),
                }
            };
            tour.Start();
        }

        public static void StartAddingQuries(object sender)
        {
            var tour = new ScreenInfo
            {
                Name = "Active Tour",
                ShowNextButtonDefault = false,
                Steps = new[]
                {
                    new Step("ComboBoxOption", "Select any Query", "Choose \"Hashtag User(S)\" from the dropdown."),
                    new Step("TextBoxPath", "Select Path", "Select a path from the desktop."),
                    new Step("ButtonAdd", "Add Queries", "Click here to Add to List"),
                    new Step("ButtonDelete", "Delete Query", "You can Click to delete Queries") { ShowNextButton = true },//IsCheckQuery
                    new Step("IsCheckQuery", "Select Query", "Please select Query to Delete "){ ShowNextButton = true },
                    new Step("ButtonAllDelete", "Delete Query", "You can Delete Selected Queries"),

                }
            };
            tour.Start();
        }
    }
}
