using System.Windows;
using System.Windows.Controls;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class CreationWindow : Window
    {
        private Operation _operation;
        public CreationWindow(Operation operation)
        {
            InitializeComponent();
            _operation = operation;
            _operation.SetValue("type", "file");
        }
        private void ItemChecked(object sender, RoutedEventArgs e)
        {
            _operation.SetValue("type", ((RadioButton) sender).Content.ToString());
        }
        private void Create(object sender, RoutedEventArgs e)
        {
            if (ItemNameBox.Text != "")
            {
                _operation.SetValue("name", ItemNameBox.Text);
                Session.DoneOperation(_operation);
                Close();
            }
        }
        
        
    }
}