using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCLoudExplorer.@class
{
    public class ItemCreatedEventArgs
    {
        public string ItemName { get; }
        public string RootPath { get; }

        public string ItemFullName{ get; }

        public ItemCreatedEventArgs(string itemName, string rootPath)
        {
            ItemName = itemName;
            RootPath = rootPath;
            ItemFullName = Path.Combine(rootPath, itemName);
        }
    }
}
