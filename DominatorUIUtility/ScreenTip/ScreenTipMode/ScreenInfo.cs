using DominatorUIUtility.Controls;
using DominatorUIUtility.Navigations;
using DominatorUIUtility.ScreenTip.ScreenTipMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorUIUtility.ScreenTipMode
{
  public  class ScreenInfo
    {
        /// <summary>
        /// Gets or sets the name of the tour.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the next button for each step is shown or not.
        /// The value can be also specified for each step separately using <see cref="Step.ShowNextButton"/>.
        /// </summary>
        public bool ShowNextButtonDefault { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates if the next button is always enabled (default false). 
        /// </summary>
        /// <remarks>
        /// Usually, the Next button is only enabled if the element, associated with the next step, is already loaded.
        /// If this property is set to true, the next button will be always enabled. In cases where the next element
        /// becomes not loaded during <see cref="IFeatureTourNavigator.OnStepEntering(string)"/>, the behavior is undefined.
        /// </remarks>
        public bool EnableNextButtonAlways { get; set; }

        /// <summary>
        /// Gets or sets a list of steps.
        /// </summary>
        public Step[] Steps { get; set; }

        /// <summary>
        /// Starts the tour.
        /// </summary>
        public void Start()
        {
            //Log.Debug("Starting tour: " + (Name ?? "<null>"));
            var run = new ScreenTipRun(this,
                ScreenTipHelper.VisualElementManager,
                ScreenTipHelper.WindowManager,
                ScreenTipHelper.PopupNavigator);
            try
            {
                FeatureTour.SetTourRun(run);
                run.Start();
            }
           
            catch (Exception)
            {
            }
        }


        public void Close()
        {
            var run = new ScreenTipRun(this,
                ScreenTipHelper.VisualElementManager,
                ScreenTipHelper.WindowManager,
                ScreenTipHelper.PopupNavigator);
            try
            {
                run.Close();
            }
            catch(Exception ex)
            {

            }
        }
    }
}
