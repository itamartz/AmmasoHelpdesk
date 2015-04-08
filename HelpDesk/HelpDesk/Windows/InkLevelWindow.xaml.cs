using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for InkLevelWindow.xaml
    /// </summary>
    public partial class InkLevelWindow : Window
    {
       
       

        //private List<Commands.PrinterToner> Printertoners;

        public InkLevelWindow()
        {
            InitializeComponent();
        }
       
        public InkLevelWindow(List<Commands.PrinterToner> Printertoners, string printerName)
        {
            // TODO: Complete member initialization
            this.Printertoners = Printertoners;
            InitializeComponent();
            this.Title = string.Format("Printer {0} Ink Status", printerName);
            this.DataContext = this;
        }

        public List<Commands.PrinterToner> Printertoners { get; set; }
    }
}
