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
    public TreeNode parentTreeNode { get; private set; }
    public SizeDirectory parentSizeDirectory { get; private set; }
    public ListViewItem copiedListViewItem { get; private set; }
    public SizeItem copiedSizeItem { get; private set; }
    public bool isMoveItem { get; private set; }

    // Construtor público
    public CopiedItem(TreeNode parentTreeNode, SizeDirectory parentSizeDirectory, ListViewItem copiedListViewItem, SizeItem copiedSizeItem, bool isMoveItem)
    {
        this.parentTreeNode = parentTreeNode;
        this.parentSizeDirectory = parentSizeDirectory;
        this.copiedListViewItem = copiedListViewItem;
        this.copiedSizeItem = copiedSizeItem;
        this.isMoveItem = isMoveItem;
    }

    // Método privado para verificar se é um diretório
    public bool IsDirectory()
    {
        return Directory.Exists(copiedSizeItem.FullName);
    }
}

}
