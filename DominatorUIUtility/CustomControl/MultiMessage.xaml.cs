using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Models;
using MahApps.Metro.Controls;

namespace DominatorUIUtility.CustomControl
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
