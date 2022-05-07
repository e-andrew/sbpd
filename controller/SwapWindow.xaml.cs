using System.Windows;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class SwapWindow : Window
    {
        private Operation _operation;
        
        public SwapWindow(Operation operation)
        {
            InitializeComponent();
            _operation = operation;
            switch (_operation.GetOperationType())
            {
                case OperationType.Rename:
                    OldBox.Text = _operation.GetValue("old");
                    break;
                case OperationType.Copy:
                    OldBox.Text = _operation.GetValue("old");
                    break;
                case OperationType.Replace:
                    OldBox.Text = _operation.GetValue("old");
                    break;
            }
        }
        
        private void Done(object sender, RoutedEventArgs e)
        {
            if (NewBox.Text != "")
            {
                _operation.SetValue("new", NewBox.Text);
                Session.DoneOperation(_operation);
                Close(); 
            }
        }
    }
}