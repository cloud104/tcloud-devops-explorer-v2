using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCLoudExplorer
{
    public class FolderToFilter
    {
        public string folderName { get; set; }
        public int totFolders { get; set; }
        public FolderToFilter(string folderName, int totFolders)
        {
            this.folderName = folderName;
            this.totFolders = totFolders;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            FolderToFilter other = (FolderToFilter)obj;
            return folderName == other.folderName;
        }

        public override int GetHashCode()
        {
            return folderName.GetHashCode();
        }
    }
}
