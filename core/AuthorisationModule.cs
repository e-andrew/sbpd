namespace Cerberus.core
{
    public class AuthorisationModule
    {
        private readonly Storage _storage;
        private readonly Config _ownersConfig;
        private readonly Config _allConfig;
        private Config _ownerConfig;
        private const string AllOwner = "*";
        private const uint NothingAllowedRight = 0x0; 
        public readonly uint ExecuteRight = 0x1; 
        public readonly uint ExpandRight = 0x2; 
        public readonly uint RewriteRight = 0x4; 
        public readonly uint DeleteRight = 0x8; 
        public readonly uint CopyRight = 0x10; 
        public readonly uint RenameRight = 0x20; 
        public readonly uint ReplaceRight = 0x40; 
        private const uint AllAllowedRight = 0x7f;
        public AuthorisationModule(Storage storage)
        {
            _storage = storage;
            _ownersConfig = _storage.GetConfig("owners");
            _allConfig = _storage.GetConfig("rules_for_all");
        }
        public void Enter(Buffer buffer)
        {
            _ownerConfig = _storage.GetConfig($"rules_{buffer.GetLogin()}");
        }
        public void Leave(){}
        public void Authorise(Buffer buffer)
        {
            if (buffer.IsAdmin())
            {
                foreach (Item storageItem in buffer.GetItems())
                {
                    storageItem.Owner = GetOwner(storageItem.Path);
                    storageItem.Rights = AllAllowedRight;
                }
            }
            else
            {
                foreach (Item storageItem in buffer.GetItems())
                {
                    storageItem.Owner = GetOwner(storageItem.Path);
                    string[] rights;
                    
                    if (storageItem.Owner == AllOwner)
                    {
                        rights = _allConfig.FindRecord(storageItem.Path, 0, new string[0]);
                        storageItem.Rights = rights.Length == 0 ? AllAllowedRight : uint.Parse(rights[1]);
                    }
                    else if (storageItem.Owner == buffer.GetLogin())
                    {
                        rights = _ownerConfig.FindRecord(storageItem.Path, 0, new string[0]);
                        storageItem.Rights = rights.Length == 0 ? AllAllowedRight : uint.Parse(rights[1]);
                    }
                    else
                    {
                        buffer.RemoveItem(storageItem);
                    }
                }
            }
        }
        
        public void AuthoriseCurrentPath(Buffer buffer)
        {
            if (buffer.IsAdmin())
            {
                buffer.GetCurrentItem().Rights = AllAllowedRight;
            }
            else
            {
                string owner = GetOwner(buffer.GetCurrentItem().Path);
                string[] rights;
                    
                if (owner == AllOwner)
                {
                    rights = _allConfig.FindRecord(buffer.GetCurrentItem().Path, 0, new string[0]);
                    buffer.GetCurrentItem().Rights =
                        rights.Length == 0 ? AllAllowedRight : uint.Parse(rights[1]);
                }
                else 
                {
                    rights = _ownerConfig.FindRecord(buffer.GetCurrentItem().Path, 0, new string[0]);
                    buffer.GetCurrentItem().Rights = rights.Length == 0 ? AllAllowedRight : uint.Parse(rights[1]);
                }
            }
        }
        public void Cancel(Buffer buffer)
        {
            if (!buffer.IsAdmin())
            {
                _ownerConfig.Delete();
                _ownersConfig.ExcludeAll(buffer.GetLogin(), 0);
            }
        }
        private string GetOwner(string path)
        {
            string[] ownerRecord = _ownersConfig.FindRecord(path, 0, new string[0]);
            return ownerRecord.Length == 0 ? AllOwner : ownerRecord[1];
        }
        
        public bool CheckRight(uint rights, uint right)
        {
            return (rights & right) > 0;
        }
    }
}