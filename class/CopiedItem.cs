using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCLoudExplorer.@class;

namespace TCLoudExplorer
{
    public class CopiedItem
    {
        public TreeNode parentDestination { get; private set; }
        public TreeNode parentSource { get; private set; }
        public SizeDirectory parentSizeDirectory { get; private set; }
        public List<ListViewItem> copiedListViewItem { get; private set; }
        public bool isMoveItem { get; private set; }

        // Public constructor
        public CopiedItem(TreeNode parentSource, SizeDirectory parentSizeDirectory, List<ListViewItem> copiedListViewItem, bool isMoveItem)
        {
            this.parentSource = parentSource;
            this.parentSizeDirectory = parentSizeDirectory;
            this.copiedListViewItem = copiedListViewItem;
            this.isMoveItem = isMoveItem;
        }

        public CopiedItem(TreeNode parentDestination, TreeNode parentSource, SizeDirectory parentSizeDirectory, List<ListViewItem> copiedListViewItem, bool isMoveItem)
            : this(parentSource, parentSizeDirectory, copiedListViewItem, isMoveItem)
        {
            this.parentDestination = parentDestination;
        }
    }
}
