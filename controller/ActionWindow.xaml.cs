using System.Windows;
using Cerberus.core;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class ActionWindow : Window
    {
        public ActionWindow()
        {
            InitializeComponent();
            ListViewItems.ItemsSource = Session.ConnectStorageItems();
        }
        private void Open(object sender, RoutedEventArgs e)
        {
            Session.CallOperation((Item) ListViewItems.SelectedItem, OperationType.Open);
        }
        
        private void Return(object sender, RoutedEventArgs e)
        {
            Session.CallOperation(OperationType.Return);
        }
        
        private void Create(object sender, RoutedEventArgs e)
        {
            Session.CallOperation(OperationType.Create);
        }
        
        private void Delete(object sender, RoutedEventArgs e)
        {
            Session.CallOperation((Item) ListViewItems.SelectedItem, OperationType.Delete);
        }
        
        private void Read(object sender, RoutedEventArgs e)
        {
            Session.CallOperation((Item) ListViewItems.SelectedItem, OperationType.Read);
        }
        
        private void Append(object sender, RoutedEventArgs e)
        {
            Session.CallOperation((Item) ListViewItems.SelectedItem, OperationType.Append);
        }
        
        private void Rewrite(object sender, RoutedEventArgs e)
        {
            Session.CallOperation((Item) ListViewItems.SelectedItem, OperationType.Rewrite);
        }
        private void Rename(object sender, RoutedEventArgs e)
        {
            Session.CallOperation((Item) ListViewItems.SelectedItem, OperationType.Rename);
        }
        private void Copy(object sender, RoutedEventArgs e)
        {
            Session.CallOperation((Item) ListViewItems.SelectedItem, OperationType.Copy);
        }
        
        private void Replace(object sender, RoutedEventArgs e)
        {
            Session.CallOperation((Item) ListViewItems.SelectedItem, OperationType.Replace);
        }
        public void OpenOperationWindow(Operation operation)
        {
            OperationType operationType = operation.GetOperationType();

            switch (operationType)
            {
                case OperationType.Create:
                    CreationWindow creationWindow = new CreationWindow(operation);
                    creationWindow.Owner = this;
                    creationWindow.Show();
                    break;
                case OperationType.Delete:
                    AcceptanceWindow deleteAcceptanceWindow = new AcceptanceWindow(operation);
                    deleteAcceptanceWindow.Owner = this;
                    deleteAcceptanceWindow.Show();
                    break;
                case OperationType.Read:
                    ContentWindow readContentWindow = new ContentWindow(operation);
                    readContentWindow.Owner = this;
                    readContentWindow.Show();
                    break;
                case OperationType.Append:
                    ContentWindow appendContentWindow = new ContentWindow(operation);
                    appendContentWindow.Owner = this;
                    appendContentWindow.Show();
                    break;
                case OperationType.Rewrite:
                    ContentWindow rewriteContentWindow = new ContentWindow(operation);
                    rewriteContentWindow.Owner = this;
                    rewriteContentWindow.Show();
                    break;
                case OperationType.Rename:
                    SwapWindow renameSwapWindow = new SwapWindow(operation);
                    renameSwapWindow.Owner = this;
                    renameSwapWindow.Show();
                    break;
                case OperationType.Copy:
                    SwapWindow copySwapWindow = new SwapWindow(operation);
                    copySwapWindow.Owner = this;
                    copySwapWindow.Show();
                    break;
                case OperationType.Replace:
                    SwapWindow replaceSwapWindow = new SwapWindow(operation);
                    replaceSwapWindow.Owner = this;
                    replaceSwapWindow.Show();
                    break;
            }
        }
    }
}