namespace Cerberus.core
{
    public class Item
    { 
        public string Path { get; set; }
        public string Type { get; set; }
        public uint Rights { get; set; }
        public string Owner { get; set; }
        
        public Item(string path, string type, uint rights, string owner)
        {
            Path = path;
            Type = type;
            Rights = rights;
            Owner = owner;
        }
    }
}