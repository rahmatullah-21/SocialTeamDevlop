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
        public static void StartIntroduction()
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
                      
                }
            };
            tour.Start();
        }


        public static void StartOverView()
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
    }
}
