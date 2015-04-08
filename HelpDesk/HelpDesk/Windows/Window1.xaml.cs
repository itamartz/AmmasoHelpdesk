using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace HelpDesk.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            load();

        }

        private void load()
        {
            for (int i = 0; i < 1000; i++)
            {
                MyComboBox.Items.Add(string.Format("Items{0}", i));
                MyComboBox.Items.Add(string.Format("Items{0}", i));
                MyComboBox.Items.Add(string.Format("Items{0}", i));
                MyComboBox.Items.Add(string.Format("Items{0}", i));
                MyComboBox.Items.Add(string.Format("Items{0}", i));
                MyComboBox.Items.Add(string.Format("Items{0}", i));
                MyComboBox.Items.Add(string.Format("Items{0}", i));
                MyComboBox.Items.Add(string.Format("Items{0}", i));
            } 
            
           //
        }

        private void MyComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            MyComboBox.IsDropDownOpen = MyComboBox.HasItems;
            MyComboBox.IsSynchronizedWithCurrentItem = true;

        }
    }
}
