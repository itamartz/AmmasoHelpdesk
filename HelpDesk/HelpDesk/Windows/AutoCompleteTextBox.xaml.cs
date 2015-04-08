using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using HelpDesk;


namespace WPFAutoCompleteTextbox
{
    /// <summary>
    /// Interaction logic for AutoCompleteTextBox.xaml
    /// </summary>    
    public partial class AutoCompleteTextBox : Canvas
    {
        #region Members
        private VisualCollection controls;
        private TextBox textBox;
        private ComboBox comboBox;
        private ObservableCollection<AutoCompleteEntry> autoCompletionList;
        private System.Timers.Timer keypressTimer;
        private delegate void TextChangedCallback();
        private bool insertText;
        private int delayTime;
        private int searchThreshold;

        #endregion

        #region Constructor
        public AutoCompleteTextBox()
        {
            controls = new VisualCollection(this);
            InitializeComponent();

            autoCompletionList = new ObservableCollection<AutoCompleteEntry>();
            searchThreshold = 0;        // default threshold to 2 char

            // set up the key press timer
            keypressTimer = new System.Timers.Timer();
            keypressTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);

            // set up the text box and the combo box
            comboBox = new ComboBox();
            comboBox.IsSynchronizedWithCurrentItem = true;
            comboBox.IsTabStop = false;
            //comboBox.SelectionChanged += new SelectionChangedEventHandler(comboBox_SelectionChanged);
            comboBox.DropDownOpened += comboBox_DropDownOpened;
            //comboBox.IsTextSearchEnabled = true;
            comboBox.IsEditable = true;
            comboBox.IsReadOnly = false;
            comboBox.KeyDown += comboBox_KeyDown;


            textBox = new TextBox();
            textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
            textBox.GotFocus += textBox_GotFocus;
            textBox.LostFocus += textBox_LostFocus;
            textBox.VerticalContentAlignment = VerticalAlignment.Center;

            controls.Add(comboBox);
            //controls.Add(textBox);
        }

        void comboBox_DropDownOpened(object sender, EventArgs e)
        {
            ImplementDropDown();
        }


        void comboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ImplementDropDown();
        }

        private void ImplementDropDown()
        {
            try
            {
                //comboBox.Items.Clear();
               
                if (comboBox.Text.Length >= searchThreshold)
                {
                    foreach (AutoCompleteEntry entry in autoCompletionList)
                    {
                        foreach (string word in entry.KeywordStrings)
                        {
                            //word.StartsWith
                            
                                ComboBoxItem cbItem = new ComboBoxItem();
                                cbItem.Content = entry.ToString();
                                comboBox.Items.Add(cbItem);
                                break;
                            
                        }

                    }
                    comboBox.IsDropDownOpen = comboBox.HasItems;
                }
                else
                {
                    comboBox.IsDropDownOpen = false;
                    comboBox.Items.Clear();
                }
            }
            catch { }
        }






        #region Itamar Events

        void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox.Text))
                textBox.Text = (string)textBox.Tag;
        }
        void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox.Tag = AutoCompleteText;
            //if (textBox.Text == _AutoCompleteText)
            textBox.Text = null;



        }
        #endregion

        #endregion

        #region Methods
        public string Text
        {
            //get { return textBox.Text; }
            //set
            //{
            //    insertText = true;
            //    textBox.Text = value;
            //}
            get { return comboBox.Text; }
            set { comboBox.Text = value; }
        }

        public Int32 TabIndex 
        {
            get { return comboBox.TabIndex; }
            set { comboBox.TabIndex = value; } 
        }


        string _AutoCompleteText;
        public string AutoCompleteText
        {
            get { return _AutoCompleteText; }
            set
            {
                _AutoCompleteText = value;
                textBox.Text = (string)value;
            }
        }


        public int DelayTime
        {
            get { return delayTime; }
            set { delayTime = value; }
        }

        public int Threshold
        {
            get { return searchThreshold; }
            set { searchThreshold = value; }
        }

        public void AddItem(AutoCompleteEntry entry)
        {
            autoCompletionList.Add(entry);
            
        }
        public void ClearItems()
        {
            autoCompletionList.Clear();

        }
        public int CountItems()
        {
            return autoCompletionList.Count();

        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(comboBox.Text))
                return;
            if (null != comboBox.SelectedItem)
            {
                //finsertText = true;
                ComboBoxItem cbItem = (ComboBoxItem)comboBox.SelectedItem;
                textBox.Text = cbItem.Content.ToString();
                Debug.WriteLine(cbItem.Content.ToString());
            }
        }

        private void TextChanged()
        {
            try
            {
                comboBox.Items.Clear();
                if (textBox.Text.Length >= searchThreshold)
                {
                    foreach (AutoCompleteEntry entry in autoCompletionList)
                    {
                        foreach (string word in entry.KeywordStrings)
                        {
                            //word.StartsWith
                            if (word.ToLower().StartsWith(textBox.Text.ToLower(), StringComparison.CurrentCultureIgnoreCase) ||
                                word.ToLower().Contains(textBox.Text.ToLower()))
                            {
                                ComboBoxItem cbItem = new ComboBoxItem();
                                cbItem.Content = entry.ToString();
                                comboBox.Items.Add(cbItem);
                                break;
                            }
                        }
                        //if (entry.DisplayName.StartsWith(textBox.Text, StringComparison.CurrentCultureIgnoreCase) || entry.DisplayName.ToLower().Contains(textBox.Text.ToLower()))
                        //{
                        //    ComboBoxItem cbItem = new ComboBoxItem();
                        //    cbItem.Content = entry.ToString();
                        //    comboBox.Items.Add(cbItem);
                        //    break;
                        //}

                    }
                    comboBox.IsDropDownOpen = comboBox.HasItems;
                }
                else
                {
                    comboBox.IsDropDownOpen = false;
                }
            }
            catch { }
        }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            keypressTimer.Stop();
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal,
                new TextChangedCallback(this.TextChanged));
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            // text was not typed, do nothing and consume the flag
            if (insertText == true) insertText = false;


            // if the delay time is set, delay handling of text changed
            else
            {
                if (delayTime > 0)
                {
                    keypressTimer.Interval = delayTime;
                    keypressTimer.Start();
                }
                else TextChanged();
            }

        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            textBox.Arrange(new Rect(arrangeSize));
            comboBox.Arrange(new Rect(arrangeSize));
            return base.ArrangeOverride(arrangeSize);
        }

        protected override Visual GetVisualChild(int index)
        {
            return controls[index];
        }

        protected override int VisualChildrenCount
        {
            get { return controls.Count; }
        }
        #endregion

    }
    public static class DependencyObjectExtensions
    {

        public static T FindAncestor<T>(this DependencyObject obj) where T : DependencyObject
        {
            return obj.FindAncestor(typeof(T)) as T;
        }

        public static DependencyObject FindAncestor(this DependencyObject obj, Type ancestorType)
        {
            var tmp = VisualTreeHelper.GetParent(obj);
            while (tmp != null && !ancestorType.IsAssignableFrom(tmp.GetType()))
            {
                tmp = VisualTreeHelper.GetParent(tmp);
            }
            return tmp;
        }

    }

}