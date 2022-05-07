using System.Windows;
using Cerberus.core;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            ListViewItems.ItemsSource = Session.ConnectAdminItems();
        }
        private void Rewrite(object sender, RoutedEventArgs e)
        {
            Session.CallAdminOperation((Item) ListViewItems.SelectedItem, OperationType.Rewrite);
        }
        
        private void Encrypt(object sender, RoutedEventArgs e)
        {
            Session.CallAdminOperation((Item) ListViewItems.SelectedItem, OperationType.Encrypt);
        }
        
        public void OpenAdminOperationWindow(Operation operation)
        {
            ContentWindow contentWindow = new ContentWindow(operation);
            contentWindow.Owner = this;
            contentWindow.Show();
        }
    }
}