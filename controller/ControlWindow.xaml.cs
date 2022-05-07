using System;
using System.Windows;
using Cerberus.view;

namespace Cerberus.controller
{
    public partial class ControlWindow : Window
    {
        private EntryWindow _entryWindow;
        private RegistrationWindow _registrationWindow;
        private AuthenticationWindow _authenticationWindow;
        private ActionWindow _actionWindow;
        private AdminWindow _adminWindow;
        
        public ControlWindow()
        {
            InitializeComponent();
            Session.Connect(this);
            Closed += OnControlWindowClosed;
            StorageStatusArea.Text = Session.GetStorageStatus();
        }
        private void OnControlWindowClosed(object sender, EventArgs e)
        {
            Session.Disconnect();
        }
        public void SetOperationStatus(string status)
        {
            OperationStatusArea.Text = status;
        }
        private void OpenEntryWindow(object sender, RoutedEventArgs e)
        {
            if (_entryWindow == null)
            {
                _entryWindow = new EntryWindow();
                _entryWindow.Owner = this;
                _entryWindow.Closed += OnEntryWindowClosed;
                _entryWindow.Show();
            }
        }
        private void OnEntryWindowClosed(object sender, EventArgs eventArgs)
        {
            _entryWindow = null;
        }
        private void OpenRegistrationWindow(object sender, RoutedEventArgs e)
        {
            if (_registrationWindow == null)
            {
                _registrationWindow = new RegistrationWindow();
                _registrationWindow.Owner = this;
                _registrationWindow.Closed += OnRegistrationWindowClosed;
                _registrationWindow.Show();
            }
        }
        private void OnRegistrationWindowClosed(object sender, EventArgs e)
        {
            _registrationWindow = null;
        }
        public void OpenAuthenticationWindow()
        {
            if (_authenticationWindow == null)
            {
                if (_actionWindow != null) { _actionWindow.Hide(); }
                if (_adminWindow != null) { _adminWindow.Hide(); }
                ActionButton.Visibility = Visibility.Collapsed;
                AdminButton.Visibility = Visibility.Collapsed;
                _authenticationWindow = new AuthenticationWindow();
                _authenticationWindow.Owner = this;
                _authenticationWindow.Closed += OnAuthenticationWindowClosed;
                _authenticationWindow.Show();
            }
        }
        private void OnAuthenticationWindowClosed(object sender, EventArgs e)
        {
            _authenticationWindow = null;
            Session.AuthenticationFinallyCheck();
        }
        private void OpenActionWindow(object sender, RoutedEventArgs e)
        {
            if (_actionWindow == null)
            {
                _actionWindow = new ActionWindow();
                _actionWindow.Owner = this;
                _actionWindow.Closed += OnActionWindowClosed;
            }
            _actionWindow.Show();
        }
        private void OnActionWindowClosed(object sender, EventArgs e)
        {
            _actionWindow = null;
        }
        private void OpenAdminWindow(object sender, RoutedEventArgs e)
        {
            if (_adminWindow == null)
            {
                _adminWindow = new AdminWindow();
                _adminWindow.Owner = this;
                _adminWindow.Closed += OnAdminWindowClosed;
            }
            _adminWindow.Show();
        }
        private void OnAdminWindowClosed(object sender, EventArgs e)
        {
            _adminWindow = null;
        }
        private void Leave(object sender, RoutedEventArgs e)
        {
            if (_authenticationWindow != null) { _authenticationWindow.Close(); }
            if (_actionWindow != null) { _actionWindow.Close(); }
            if (_adminWindow != null) { _adminWindow.Close(); }
            EntryButton.Visibility = Visibility.Visible;
            RegisterButton.Visibility = Visibility.Visible;
            ActionButton.Visibility = Visibility.Collapsed;
            AdminButton.Visibility = Visibility.Collapsed;
            LeaveButton.Visibility = Visibility.Collapsed;
            Session.Leave();
            SetOperationStatus(Session.GetOperationStatus());
        }
        public void Leave()
        {
            Leave(null, new RoutedEventArgs());
        }
        public void OpenOperationWindow(Operation operation)
        {
            if (_actionWindow != null)
            {
                _actionWindow.OpenOperationWindow(operation);
            }
        }
        
        public void OpenAdminOperationWindow(Operation operation)
        {
            if (_adminWindow != null)
            {
                _adminWindow.OpenAdminOperationWindow(operation);
            }
        }
    }
}