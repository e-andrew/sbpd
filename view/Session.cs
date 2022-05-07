using System.Collections.ObjectModel;
using Cerberus.controller;
using Cerberus.core;

namespace Cerberus.view
{
    public static class Session
    {
        private static ControlWindow _controlWindow;
        private static RegistrationModule _registrationModule;
        private static AuthenticationModule _authenticationModule;
        private static AuthorisationModule _authorisationModule;
        private static MonitoringModule _monitoringModule;
        private static readonly Storage Storage = new Storage();
        private static readonly Buffer Buffer = new Buffer();
        
        public static void Connect(ControlWindow controlWindow)
        {
            _controlWindow = controlWindow;
            _registrationModule = new RegistrationModule(Storage);
            _authenticationModule = new AuthenticationModule(Storage, _controlWindow);
            _authorisationModule = new AuthorisationModule(Storage);
            _monitoringModule = new MonitoringModule(Storage, Buffer);
            Storage.BlockStorage();
            Buffer.SetStorageStatus("blocked");
        }
        public static bool Enter(string login, string password)
        {
            _registrationModule.Enter(Buffer, login, password);

            if (Buffer.IsEntered())
            {
                _authenticationModule.Enter(Buffer);
                _authorisationModule.Enter(Buffer);
                _monitoringModule.Enter();
                Buffer.SetOperationStatus("logged in");
                _monitoringModule.OnLoginPass();
            }
            else
            {
                Buffer.SetOperationStatus("not logged in");
                _monitoringModule.OnLoginError(login);
            }

            return Buffer.IsEntered();
        }
        public static void Leave()
        {
            _authenticationModule.Leave();
            _authorisationModule.Leave();
            _monitoringModule.Leave();
            Buffer.Leave();
            Buffer.SetOperationStatus("logged out");
        }
        
        public static void Disconnect()
        {
            _controlWindow = null;
            Storage.UnblockStorage();
            Buffer.Leave();
            _authenticationModule.Disconnect();
            _authorisationModule.Leave();
            _monitoringModule.Leave();
        }
        
        public static string GetStorageStatus()
        {
            return Buffer.GetStorageStatus();
        }
        
        public static string GetOperationStatus()
        {
            return Buffer.GetOperationStatus();
        }
        public static bool IsAdmin()
        {
            return Buffer.IsAdmin();
        }
        public static void Register(string login, string password, string repeat)
        {
            if (_registrationModule.Register(login, password, repeat))
            {
                Buffer.SetOperationStatus("registered");
            }
            else
            {
                Buffer.SetOperationStatus("not registered");
            }
        }

        public static string AuthenticationRequest()
        {
            _authenticationModule.AuthenticationRequest();
            return Buffer.GetQuestion().ToString();
        }
        
        public static bool AuthenticationResponse(string answer)
        {
            _authenticationModule.AuthenticationResponse(answer);
            if (_authenticationModule.IsNeedLeave())
            {
                _monitoringModule.OnAuthenticationError(Buffer.GetQuestion().ToString(), answer);
                _controlWindow.Leave();
            } else if (_authenticationModule.IsNeedRegister())
            {
                _monitoringModule.OnAuthenticationError(Buffer.GetQuestion().ToString(), answer);
                _registrationModule.Unregister(Buffer);
                _authorisationModule.Cancel(Buffer);
                _controlWindow.Leave();
            }
            else
            {
                _monitoringModule.OnAuthenticationPass();
            }
            
            return _authenticationModule.IsAuthenticated();
        }
        public static void AuthenticationFinallyCheck()
        {
            if (_authenticationModule.IsNotAuthenticated())
            {
                _controlWindow.Leave();
            }
        }
        
        //Operations
        private static void UpdateStorageItems(string currentPath)
        {
            Storage.GetStorageItem(Buffer, currentPath);
            _authorisationModule.AuthoriseCurrentPath(Buffer);
            Storage.GetStorageItems(Buffer);
            _authorisationModule.Authorise(Buffer);
        }
        
        private static bool CheckRight(Item storageItem, uint right)
        {
            bool decision = _authorisationModule.CheckRight(storageItem.Rights, right);
            if(!decision) _monitoringModule.OnAuthorisationError(storageItem.Path);
            return decision;
        }
        
        public static ObservableCollection<Item> ConnectStorageItems()
        {
            UpdateStorageItems(Storage.GetRootPath());
            return Buffer.GetItems();
        }
        public static ObservableCollection<Item> ConnectAdminItems()
        {
            UpdateStorageItems(Storage.GetConfigPath());
            return Buffer.GetItems();
        }
        public static void CallOperation(OperationType operationType)
        {
            switch (operationType)
            {
                case OperationType.Return:
                    string path = Storage.GetParentPath(Buffer.GetCurrentItem().Path);
                    UpdateStorageItems(path);
                    _monitoringModule.OnReturn(path);
                    break;
                case OperationType.Create:
                    if (CheckRight(Buffer.GetCurrentItem(), _authorisationModule.ExpandRight))
                    {
                        Operation operation = new Operation(operationType);
                        operation.SetValue("path", Buffer.GetCurrentItem().Path);
                        _controlWindow.OpenOperationWindow(operation);
                    }
                    break;
            }
        }
        public static void CallOperation(Item storageItem, OperationType operationType)
        {
            if (storageItem != null)
            {
                switch (operationType)
                {
                    case OperationType.Open:
                        if (storageItem.Type == "dir" && CheckRight(storageItem, _authorisationModule.ExecuteRight))
                        {
                            UpdateStorageItems(storageItem.Path);
                            _monitoringModule.OnOpen(storageItem.Path); 
                        }
                        break;
                    case OperationType.Delete:
                        if (CheckRight(storageItem, _authorisationModule.DeleteRight) &&
                            CheckRight(Buffer.GetCurrentItem(), _authorisationModule.DeleteRight))
                        {
                            Operation operation = new Operation(operationType);
                            operation.SetValue("path", storageItem.Path);
                            operation.SetValue("type", storageItem.Type);
                            _controlWindow.OpenOperationWindow(operation);
                        }
                        break;
                    case OperationType.Read:
                        if (storageItem.Type == "file" && CheckRight(storageItem, _authorisationModule.ExecuteRight))
                        {
                            Operation operation = new Operation(operationType);
                            operation.SetValue("content", Storage.Read(storageItem.Path));
                            _controlWindow.OpenOperationWindow(operation);
                            _monitoringModule.OnRead(storageItem.Path);   
                        }
                        break;
                    case OperationType.Append:
                        if (storageItem.Type == "file" && CheckRight(storageItem, _authorisationModule.ExpandRight))
                        {
                            Operation operation = new Operation(operationType);
                            operation.SetValue("path", storageItem.Path);
                            _controlWindow.OpenOperationWindow(operation);
                        }
                        break;
                    case OperationType.Rewrite:
                        if (storageItem.Type == "file" && CheckRight(storageItem, _authorisationModule.RewriteRight))
                        {
                            Operation operation = new Operation(operationType);
                            operation.SetValue("path", storageItem.Path);
                            operation.SetValue("content", Storage.Read(storageItem.Path));
                            _controlWindow.OpenOperationWindow(operation);
                        }
                        break;
                    case OperationType.Rename:
                        if (CheckRight(storageItem, _authorisationModule.RenameRight))
                        {
                            Operation operation = new Operation(operationType);
                            operation.SetValue("path", Buffer.GetCurrentItem().Path);
                            operation.SetValue("type", storageItem.Type);
                            operation.SetValue("old", Storage.GetName(storageItem.Path));
                            _controlWindow.OpenOperationWindow(operation);
                        }
                        break;
                    case OperationType.Copy:
                        if (CheckRight(storageItem, _authorisationModule.CopyRight))
                        {
                            Operation operation = new Operation(operationType);
                            operation.SetValue("old", storageItem.Path);
                            operation.SetValue("type", storageItem.Type);
                            _controlWindow.OpenOperationWindow(operation);
                        }
                        break;
                    case OperationType.Replace:
                        if (CheckRight(storageItem, _authorisationModule.ReplaceRight))
                        {
                            Operation operation = new Operation(operationType);
                            operation.SetValue("old", storageItem.Path);
                            operation.SetValue("type", storageItem.Type);
                            _controlWindow.OpenOperationWindow(operation);
                        }
                        break;
                }
            }
        }
        public static void CallAdminOperation(Item storageItem, OperationType operationType)
        {
            if (storageItem != null)
            {
                switch (operationType)
                {
                    case OperationType.Rewrite:
                        if (storageItem.Type == "file" && CheckRight(storageItem, _authorisationModule.RewriteRight))
                        {
                            Operation operation = new Operation(operationType);
                            operation.SetValue("path", storageItem.Path);
                            operation.SetValue("content", Storage.Read(storageItem.Path));
                            _controlWindow.OpenAdminOperationWindow(operation);
                        }
                        break;
                    case OperationType.Encrypt:
                        //todo for next labs
                        break;
                }
            }
        }
        
        public static void DoneOperation(Operation operation)
        {
            switch (operation.GetOperationType())
            {
                case OperationType.Create:
                    Storage.Create(operation.GetValue("path"), operation.GetValue("name"), operation.GetValue("type") == "file");
                    UpdateStorageItems(Buffer.GetCurrentItem().Path);
                    _monitoringModule.OnCreate(operation.GetValue("path"));
                    break;
                case OperationType.Delete:
                    if (operation.GetValue("answer") == "yes")
                    {
                        Storage.Delete(operation.GetValue("path"), operation.GetValue("type") == "file");
                        UpdateStorageItems(Buffer.GetCurrentItem().Path);
                        _monitoringModule.OnDelete(operation.GetValue("path"));
                    }
                    break;
                case OperationType.Append:
                    Storage.Append(operation.GetValue("path"), operation.GetValue("content"));
                    _monitoringModule.OnAppend(operation.GetValue("path"));
                    break;
                case OperationType.Rewrite:
                    Storage.Write(operation.GetValue("path"), operation.GetValue("content"));
                    _monitoringModule.OnRewrite(operation.GetValue("path"));
                    break;
                case OperationType.Rename:
                    Storage.Rename(operation.GetValue("path"), operation.GetValue("old"), operation.GetValue("new"), operation.GetValue("type") == "file");
                    UpdateStorageItems(Buffer.GetCurrentItem().Path);
                    _monitoringModule.OnRename(operation.GetValue("path"), operation.GetValue("old"), operation.GetValue("new"));
                    break;
                case OperationType.Copy:
                    Storage.Copy(operation.GetValue("old"), operation.GetValue("new"), operation.GetValue("type") == "file");
                    UpdateStorageItems(Buffer.GetCurrentItem().Path);
                    _monitoringModule.OnCopy(operation.GetValue("old"), operation.GetValue("new"));
                    break;
                case OperationType.Replace:
                    Storage.Replace(operation.GetValue("old"), operation.GetValue("new"), operation.GetValue("type") == "file");
                    UpdateStorageItems(Buffer.GetCurrentItem().Path);
                    _monitoringModule.OnReplace(operation.GetValue("old"), operation.GetValue("new"));
                    break;
            }
        }
    }
}