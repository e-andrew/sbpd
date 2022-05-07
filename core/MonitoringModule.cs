﻿﻿using System;
using System.IO;
using Microsoft.Win32;

namespace Cerberus.core
{
    public class MonitoringModule
    {
        private Storage _storage;
        private Buffer _buffer;
        public MonitoringModule(Storage storage, Buffer buffer)
        {
            _storage = storage;
            _buffer = buffer;
        }

        public void Enter()
        {
            SystemEvents.TimeChanged += OnSystemTimeChange;
        }
        
        public void Leave()
        {
            SystemEvents.TimeChanged += EmptySystemEventHandler;
        }
        
        public void OnOpen(string path)
        {
            AppendRecord("0-level", "OPEN", path);
        }
        
        public void OnReturn(string path)
        {
            AppendRecord("0-level", "RETURN", path);
        }
        
        public void OnCreate(string path)
        {
            AppendRecord("0-level", "CREATE", path);
        }
        
        public void OnDelete(string path)
        {
            AppendRecord("0-level", "DELETE", path);
        }
        
        public void OnRead(string path)
        {
            AppendRecord("0-level", "READ", path);
        }
        
        public void OnAppend(string path)
        {
            AppendRecord("0-level", "APPEND", path);
        }
        
        public void OnRewrite(string path)
        {
            AppendRecord("0-level", "REWRITE", path);
        }
        
        public void OnRename(string path, string oldName, string newName)
        {
            AppendRecord("0-level", "RENAME", $"{path} : {oldName} => {newName}");
        }
        
        public void OnCopy(string oldPath, string newPath)
        {
            AppendRecord("0-level", "COPY", $"{oldPath} => {newPath}");
        }
        
        public void OnReplace(string oldPath, string newPath)
        {
            AppendRecord("0-level", "REPLACE", $"{oldPath} => {newPath}");
        }
        
        public void OnLoginPass()
        {
            AppendRecord("0-level", "LOGIN_PASS", "without details");
        }
        
        public void OnAuthenticationPass()
        {
            AppendRecord("0-level", "AUTHENTICATION_PASS", "without details");
        }
        
        public void OnLoginError(string login)
        {
            AppendRecord("1-level", "LOGIN_ERROR", login);
        }
        
        public void OnAuthorisationError(string path)
        {
            AppendRecord("2-level", "AUTHORISATION_ERROR", path);
        }
        
        public void OnAuthenticationError(string request, string response)
        {
            AppendRecord( "3-level", "AUTHENTICATION_ERROR", $"{request}=>{response}");
        }
        
        public void OnFileSystemError(object sender, ErrorEventArgs e) =>
            PrintException(e.GetException());

        public void PrintException(Exception? ex)
        {
            if (ex != null)
            {
                AppendRecord("4-level", "FILESYSTEM_ERROR", $"{ex.Message}");
                PrintException(ex.InnerException);
            }
        }
        
        public void OnDecipherError(string path)
        {
            AppendRecord("4-level", "DECIPHER_ERROR", path);
        }
        
        public void OnSystemTimeChange(object sender, EventArgs e)
        {
            AppendRecord("4-level", "SYSTEM_TIME_CHANGE", "without details");
        }
        
        public void OnAnotherSystemEvent(object sender, EventArgs e)
        {
            AppendRecord("4-level", "ANOTHER_SYSTEM_EVENT", "without details");
        }

        public void EmptySystemEventHandler(object sender, EventArgs e) { }
        
        private void AppendRecord(string dangerLevel, string actionType, string actionDetail)
        {
            DateTime actionTime = DateTime.Now;
            Config config = _storage.GetConfig("log_" + actionTime.ToString("yyyy.MM.dd"));
            string[] record = new string[5];
            record[0] = actionTime.ToString("yyyy.MM.dd.HH.mm.ss");
            record[1] = _buffer.GetLogin();
            record[2] = dangerLevel;
            record[3] = actionType;
            record[4] = actionDetail;
            config.Insert(record);
        }
    }
}