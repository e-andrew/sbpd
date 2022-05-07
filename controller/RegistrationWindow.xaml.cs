using System.Windows;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }
        
        private void Confirm(object sender, RoutedEventArgs e)
        {
            Session.Register(AccountLoginBox.Text, AccountPasswordBox.Password, AccountRepeatBox.Password);
            ((ControlWindow) Owner).SetOperationStatus(Session.GetOperationStatus());
            Close();
        }
    }
}