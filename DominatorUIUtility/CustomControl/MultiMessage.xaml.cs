using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Models;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for MultiMessage.xaml
    /// </summary>
    public partial class MultiMessage : UserControl
    { 
        public MultiMessagesModel MultiMessagesModel { get; set; } = new MultiMessagesModel();

        public MultiMessage()
        {
            InitializeComponent();
            ChatControl.DataContext = MultiMessagesModel;
        }
        private void NumericUpDown_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (e.OldValue != null)
            {
                if (e.OldValue > e.NewValue)
                    for (double? i = e.OldValue; i > e.NewValue; i--)
                        MultiMessagesModel.LstMessages.RemoveAt(MultiMessagesModel.LstMessages.Count - 1);
                else if (e.OldValue < e.NewValue)
                {
                    MultiMessagesModel.LstMessages.Clear();
                    for (int i = 0; i < e.NewValue; i++)
                        MultiMessagesModel.LstMessages.Add("");

                }
            }
            else
            {
                MultiMessagesModel.LstMessages.Clear();
                for (int i = 0; i < e.NewValue; i++)
                    MultiMessagesModel.LstMessages.Add("");

            }
        }
    }
}
