using System.Windows;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class EntryWindow : Window
    {
        public EntryWindow()
        {
            InitializeComponent();
        }
        
        private void Entry(object sender, RoutedEventArgs e)
        {
            if (Session.Enter(AccountLoginBox.Text, AccountPasswordBox.Password))
            {
                ((ControlWindow) Owner).EntryButton.Visibility = Visibility.Collapsed;
                ((ControlWindow) Owner).RegisterButton.Visibility = Visibility.Collapsed;
                ((ControlWindow) Owner).ActionButton.Visibility = Visibility.Visible;
                if (Session.IsAdmin()) ((ControlWindow) Owner).AdminButton.Visibility = Visibility.Visible;
                ((ControlWindow) Owner).LeaveButton.Visibility = Visibility.Visible;
                
            }
            ((ControlWindow) Owner).SetOperationStatus(Session.GetOperationStatus());
            Close();
        }
    }
}