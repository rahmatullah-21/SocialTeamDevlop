using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for AnswerOnQuestions.xaml
    /// </summary>
    public partial class AnswerOnQuestions : UserControl
    {
        public AnswerOnQuestions(IAnswerOnQuestionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
