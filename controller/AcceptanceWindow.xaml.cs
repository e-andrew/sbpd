using System.Windows;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class AcceptanceWindow : Window
    {
        private Operation _operation;
        
        public AcceptanceWindow(Operation operation)
        {
            InitializeComponent();
            _operation = operation;
            QuestionArea.Content = $@"Do you want to delete this {operation.GetValue("type")}?";
        }
        
        private void Accept(object sender, RoutedEventArgs e)
        {
            _operation.SetValue("answer", "yes");
            Session.DoneOperation(_operation);
            Close();
        }
        
        private void Reject(object sender, RoutedEventArgs e)
        {
            _operation.SetValue("answer", "no");
            Session.DoneOperation(_operation);
            Close();
        }
    }
}