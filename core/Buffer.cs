﻿﻿﻿using System.Collections.ObjectModel;
   
namespace Cerberus.core
{
    public class Buffer
    {
        private string _storageStatus;
        private string _operationStatus;
        private string _login = "unknown";
        private string _role = "stranger";
        private int _code;
        private int _question;
        private int _answer;
        private Item _currentItem;
        private readonly ObservableCollection<Item> _items = new ObservableCollection<Item>();
        public void Enter(string login, string role, int code)
        {
            _login = login;
            _role = role;
            _code = code;
            _question = 0;
            _answer = 0;
            _currentItem = new Item("", "dir", 0, "");
            _items.Clear();
        }
        public void Verify(int question, int answer)
        {
            _question = question;
            _answer = answer;
        }
        
        public void Leave()
        {
            _login = "unknown";
            _role = "stranger";
            _code = 0;
            _question = 0;
            _answer = 0;
            _currentItem = new Item("", "dir", 0, "");
            _items.Clear();
        }

        public void SetStorageStatus(string status)
        {
            _storageStatus = status;
        }
        
        public void SetOperationStatus(string status)
        {
            _operationStatus = status;
        }
        
        public string GetStorageStatus()
        {
            return _storageStatus;
        }
        
        public string GetOperationStatus()
        {
            return _operationStatus;
        }
        
        public bool IsEntered()
        {
            return !_login.Equals("unknown");
        }

        public bool IsAdmin()
        {
            return _role.Equals("admin");
        }
        public string GetLogin()
        {
            return _login;
        }
        public int GetCode()
        {
            return _code;
        }
        
        public int GetQuestion()
        {
            return _question;
        }
        
        public int GetAnswer()
        {
            return _answer;
        }
        public Item GetCurrentItem()
        {
            return _currentItem;
        }
        public ObservableCollection<Item> GetItems()
        {
            return _items;
        }
        
        public void AddItem(string path, string type)
        {
            _items.Add(new Item(path, type, 0, ""));
        }
        
        public void SetCurrentItem(string path, string type)
        {
            _currentItem = new Item(path, type, 0, "");
            _items.Clear();
        }
        
        public void RemoveItem(Item storageItem)
        {
            _items.Remove(storageItem);
        }
    }
}