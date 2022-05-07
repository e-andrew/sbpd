﻿﻿﻿﻿﻿using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Cerberus.core
{ 
    public class Storage
    {
        private readonly string _storagePath;
        private readonly string _configPath;
        private readonly uint NothingShare = 0x0;
        private readonly uint BackupSemanticsFlag = 0x02000000;
        private IntPtr _storageHandler = IntPtr.Subtract(IntPtr.Zero, 1);
        private readonly IntPtr _errorHandler = IntPtr.Subtract(IntPtr.Zero, 1);
        public Storage()
        {
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), ".storage");
            _configPath = Path.Combine(_storagePath, ".config");
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
                File.SetAttributes(_storagePath, FileAttributes.Normal);
            }
            if (!Directory.Exists(_configPath))
            {
                Directory.CreateDirectory(_configPath);
                File.SetAttributes(_configPath, FileAttributes.Hidden);
            }
        }
        
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CreateFile(
            [MarshalAs(UnmanagedType.LPTStr)] string filename,
            [MarshalAs(UnmanagedType.U4)] FileAccess access,
            uint share,
            IntPtr securityAttributes, // optional SECURITY_ATTRIBUTES struct or IntPtr.Zero
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);
        
        [DllImport("kernel32.dll", SetLastError=true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);
        public void BlockStorage()
        {
            while (_storageHandler == _errorHandler)
            {
                _storageHandler = CreateFile(_storagePath, FileAccess.ReadWrite, NothingShare, IntPtr.Zero,
                    FileMode.Open, BackupSemanticsFlag,IntPtr.Zero);
            }
        }
        
        public void UnblockStorage()
        {
            CloseHandle(_storageHandler);
            _storageHandler = _errorHandler;
        }
        public string GetRootPath()
        {
            return ToInternalPath(_storagePath);
        }
        
        public string GetConfigPath()
        {
            return ToInternalPath(_configPath);
        }
        
        public Config GetConfig(string filename)
        {
            return new Config(Path.Combine(_configPath, filename));
        }
        
        private string ToInternalPath(string path)
        {
            return path.Replace(_storagePath + Path.DirectorySeparatorChar, "").Replace(_storagePath, ""); //todo dummy idea
        }
        
        private string ToExternalPath(string path)
        {
            return Path.Combine(_storagePath, path);
        }
        
        public string GetParentPath(string path)
        {
            return path == "" ? "" : ToInternalPath(Directory.GetParent(ToExternalPath(path)).FullName);
        }
        
        public string GetName(string path)
        {
            string[] elements = path.Split('\\');
            return elements[elements.Length-1];
        }
        
        public void GetStorageItem(Buffer buffer, string path)
        {
            buffer.SetCurrentItem(path, "dir");
        }
        public void GetStorageItems(Buffer buffer)
        {
            UnblockStorage();
            string[] directoriesPaths = Directory.GetDirectories(ToExternalPath(buffer.GetCurrentItem().Path), "*", SearchOption.TopDirectoryOnly);
            
            foreach (string path in directoriesPaths)
            {
                if (path != _configPath)
                {
                    buffer.AddItem(ToInternalPath(path), "dir");
                }
            }
            
            string[] filesPaths = Directory.GetFiles(ToExternalPath(buffer.GetCurrentItem().Path), "*", SearchOption.TopDirectoryOnly);
            
            foreach (string path in filesPaths)
            {
                buffer.AddItem(ToInternalPath(path), "file");
            }
            BlockStorage();
        }
        public string Read(string path)
        {
            StreamReader streamReader = new StreamReader(ToExternalPath(path));
            string content = streamReader.ReadToEnd();
            streamReader.Close();
            return content;
        }
        public void Write(string path, string content)
        {
            StreamWriter streamWriter = new StreamWriter(ToExternalPath(path));
            streamWriter.Write(content);
            streamWriter.Close();
        }
        public void Append(string path, string content)
        {
            StreamWriter streamWriter = new StreamWriter(ToExternalPath(path), true);
            streamWriter.Write(content);
            streamWriter.Close();
        }

        public void Rename(string path, string oldName, string newName, bool isFile)
        {
            UnblockStorage();
            if (isFile)
            {
                if (newName.Replace(" ", "") != "")
                {
                    string externalPath = ToExternalPath(path);
                    File.Move(Path.Combine(externalPath, oldName),Path.Combine(externalPath, newName));
                }
            }
            else
            {
                if (newName.Replace(" ", "") != "")
                {
                    string externalPath = ToExternalPath(path);
                    Directory.Move(Path.Combine(externalPath, oldName),Path.Combine(externalPath, newName));   
                }
            }
            BlockStorage();
        }

        public void Replace(string oldPath, string newPath, bool isFile)
        {
            UnblockStorage();
            if (isFile)
            {
                if (newPath.Replace(" ", "") != "")
                {
                    File.Move(ToExternalPath(oldPath), ToExternalPath(newPath));
                }
            }
            else
            {
                if (newPath.Replace(" ", "") != "")
                {
                    Directory.Move(ToExternalPath(oldPath), ToExternalPath(newPath));
                }
            }
            BlockStorage();
        }

        public void Copy(string oldPath, string newPath, bool isFile)
        {
            UnblockStorage();
            if (isFile)
            {
                if (newPath.Replace(" ", "") != "" && !File.Exists(ToExternalPath(newPath)))
                {
                    File.Copy(ToExternalPath(oldPath), ToExternalPath(newPath), false);
                }
            }
            else
            {
                if (newPath.Replace(" ", "") != "" && !Directory.Exists(ToExternalPath(newPath)))
                {
                    
                    foreach (string path in Directory.GetDirectories(ToExternalPath(oldPath), "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(path.Replace(ToExternalPath(oldPath), ToExternalPath(newPath)));
                    }
                    
                    foreach (string path in Directory.GetFiles(ToExternalPath(oldPath), "*.*",SearchOption.AllDirectories))
                    {
                        File.Copy(path, path.Replace(ToExternalPath(oldPath), ToExternalPath(newPath)), true);
                    }
                }
            }
            
            BlockStorage();
        }
        
        public void Delete(string path, bool isFile)
        {
            UnblockStorage();
            if (isFile)
            {
                File.Delete(ToExternalPath(path));
            }
            else
            {
                Directory.Delete(ToExternalPath(path), true);
            }
            BlockStorage();
        }

        public void Create(string path, string name, bool isFile)
        {
            UnblockStorage();
            string fullPath = ToExternalPath(Path.Combine(path, name));
            
            if (isFile)
            {
                File.Create(fullPath).Close();
            }
            else
            {
                Directory.CreateDirectory(fullPath);
            }
            BlockStorage();
        }
    }
}