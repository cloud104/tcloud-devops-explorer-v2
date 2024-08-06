using System.Windows.Forms;

public class SelectedTreeViewEventArgs : TreeViewEventArgs
{
    public ListViewItem SelectedItem { get; set; }

    public SelectedTreeViewEventArgs(TreeNode node, ListViewItem selectedItem) : base(node)
    {
        SelectedItem = selectedItem;
    }
}
