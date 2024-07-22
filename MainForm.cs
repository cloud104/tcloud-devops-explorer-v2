using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using TCLoudExplorer;
using TCLoudExplorer.@class;
using TCLoudExplorer.Properties;

namespace TCloudExplorer
{
    public partial class MainForm : Form
    {
        private CopiedItem _copiedItem;

        private const int UpdateMs = 200;
        private const string ToolName = "TCloudExplorer";

        private ImageList _imageList = new();

        private string _lastOpenedPath;
        private string _lastCurrentOpenedPath;
        private bool _cancelled = false;

        private const string ICON_FOLDER = nameof(Resources.folder_o);
        private const string ICON_FOLDER_OPEN = nameof(Resources.folder_open_o);

        private const string ICON_FILE = nameof(Resources.file_o);

        private const string ICON_ERROR = nameof(Resources.exclamation);

        private string _titleBase = $"{ToolName} v{Application.ProductVersion}";

        // For list view sorting: default on size descending
        private bool _sortAscending = false;
        private int _sortColumnIndex = 1;

        private ToolStripStatusLabel[] _columnStatusLabels;

        // The current selected list item (a SizeItem, i.e. directory or file)
        private SizeItem SelectedListItem => (SizeItem)_listView.SelectedItems[0].Tag!;
        // The current selected tree directory
        private SizeDirectory SelectedTreeDirectory => (SizeDirectory)_treeView.SelectedNode.Tag;

        public MainForm()
        {
            InitializeComponent();

            Icon = Resources.TCloudExplorer;
            Text = _titleBase;

            // Set up the image list at 24x24, 32-bits
            _imageList.ColorDepth = ColorDepth.Depth32Bit;
            _imageList.ImageSize = new Size(24, 24);

            _imageList.Images.Add(ICON_FILE, Resources.file_o);
            _imageList.Images.Add(ICON_FOLDER, Resources.folder_o);
            _imageList.Images.Add(ICON_FOLDER_OPEN, Resources.folder_open_o);
            _imageList.Images.Add(ICON_ERROR, Resources.exclamation);

            _treeView.ImageList = _imageList;
            _listView.SmallImageList = _imageList;

            _lastOpenedPath = Environment.CurrentDirectory;

            // Show only the default tool strip
            _loadingStatusStrip.Visible = false;

            // Start by sorting on size descending
            //_listView.ListViewItemSorter = new SizeSorter(false);

            // The status labels corresponding to the list view columns
            _columnStatusLabels = new[] {
            _listNameStatusLabel,
            _listSizeStatusLabel,
            _listPercentageStatusLabel,
            _listFoldersStatusLabel,
            _listFilesStatusLabel,
            _listBytesStatusLabel,
            _listLastModifiedStatusLabel
          };

            foreach (var label in _columnStatusLabels)
                label.Text = "";
        }

        /// <summary>
        /// Load the folder specified on the command line if specified
        /// </summary>
        private async void Form_Load(object sender, EventArgs e)
        {
            // [0] is the name of the executable, [1] is a folder to open
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                string path = Environment.GetCommandLineArgs()[1];
                await LoadSizesAsync(path);
                // Save this path for Refresh and Open
                _lastOpenedPath = Path.GetFullPath(path);
            }
        }

        //private async Task<string> GetToolVersion(string toolName)
        /// <summary>
        /// Load the sizes of a folder and subfolders
        /// </summary>
        /// <param name="path">The path to load. May be relative</param>
        private async Task LoadSizesAsync(string path)
        {
            _splitContainer.Enabled = false;
            _splitContainer.UseWaitCursor = true; // Doesn't work?

            try
            {
                // We haven't cancelled yet
                _cancelled = false;

                // Swap the normal status strip and the loading status strip:
                _topStatusStrip.Visible = false;
                _loadingLabel.Text = $"Loading '{path}'...";
                _loadingStatusStrip.Visible = true;

                // Keep a (case insensitive!) tab on which directory was added to the tree where
                var nodeDict = new Dictionary<string, TreeNode>(StringComparer.OrdinalIgnoreCase);

                // Add the root path as the only node
                _treeView.Nodes.Clear();
                TreeNode firstNode = new TreeNode(Path.GetFullPath(path)) { Name = Path.GetFullPath(path) };
                firstNode.ImageKey = ICON_FOLDER;
                _treeView.Nodes.Add(firstNode);
                nodeDict.Add(firstNode.Text, firstNode);
                firstNode.Expand();

                _listView.Items.Clear();

                // We only update the status label every so many ms, to prevent it eating CPU
                var lastUpdateTime = DateTime.Now.AddHours(-1);
                // This method is called on every folder. Return true to cancel
                var callback = (string fullPath) =>
                {
                    // Skip paths we already added
                    if (!nodeDict.ContainsKey(fullPath))
                    {
                        // Find the parent:
                        var parentPath = Path.GetDirectoryName(fullPath);
                        // Update only first level nodes to show progress
                        if (nodeDict.TryGetValue(parentPath!, out TreeNode? node) && node == firstNode)
                        {
                            // We found the parent: add the node
                            Invoke(() =>
                            {
                                  var newNode = node.Nodes.Add(fullPath, Path.GetFileName(fullPath));
                                  newNode.ImageKey = ICON_FOLDER;
                                  nodeDict.Add(fullPath, newNode);
                                // Show this node
                                  newNode.EnsureVisible();
                                  _treeView.EndUpdate();
                                  _treeView.Update();
                                  _treeView.BeginUpdate();
                              });
                        }
                    }

                    var time = DateTime.Now;
                    if (time.Subtract(lastUpdateTime).TotalMilliseconds > UpdateMs)
                    {
                        // Update the status label USING INVOKE()
                        Invoke(() =>
                {
                        _loadingLabel.Text = fullPath;
                    });

                        lastUpdateTime = time;
                    }
                    return _cancelled;
                };

                try
                {
                    // Block updates to the tree
                    _treeView.BeginUpdate();
                    SizeDirectory rootDir = await Task.Run(() => SizeDirectory.FromPath(path, callback));
                    _loadingLabel.Text = "Displaying results...";

                    _treeView.BeginUpdate();

                    _treeView.Nodes.Clear();

                    // Add the root node with its full path
                    var rootNode = _treeView.Nodes.Add(rootDir.FullName);
                    ColorNode(rootNode, rootDir);

                    rootNode.Tag = rootDir;
                    AddNodes(rootDir, rootNode);
                    rootNode.Expand();
                    _treeView.SelectedNode = rootNode;

                    _treeView.EndUpdate();

                    // Update the window title
                    Text = $"{rootDir.FullName} - {_titleBase}";
                }
                catch (Exception ex)
                {
                    // We failed somehow - no result
                    MessageBox.Show(this, ex.Message, $"Error loading '{path}'", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    _treeView.EndUpdate();
                }

                _topStatusStrip.Visible = true;
                _loadingStatusStrip.Visible = false;

                // Re-enable Open button
                _topStatusStrip.Enabled = true;
            }
            finally
            {
                _splitContainer.Enabled = true;
                _splitContainer.UseWaitCursor = false;
            }

            // Once we loaded, we can refresh:
            _refreshToolButton.Enabled = true;
        }

        /// <summary>
        /// Add a node and subnodes to the tree
        /// </summary>
        private void AddNodes(SizeDirectory dir, TreeNode node)
        {
            foreach (var d in dir.Directories)
            {
                var child = node.Nodes.Add(d.FullName, d.Name);
                child.Tag = d;
                ColorNode(child, d);

                AddNodes(d, child);
            }
        }

        /// <summary>
        /// Set node properties based on AclDirectory
        /// </summary>
        private void ColorNode(TreeNode node, SizeDirectory dir)
        {
            if (dir.Exception != null)
            {
                node.ImageKey = ICON_ERROR;
                node.SelectedImageKey = ICON_ERROR;
            }
            else
            {
                node.ImageKey = ICON_FOLDER;
                node.SelectedImageKey = ICON_FOLDER_OPEN;
            }
        }

        /// <summary>
        /// Return a "Nice size" string for a length in bytes, e.g. "1.2 GB"
        /// </summary>
        private string NiceSize(long bytes)
        {
            const long _1kB = 1024L;
            const long _1MB = _1kB * 1024;
            const long _1GB = _1MB * 1024;
            if (bytes > _1GB)
                return $"{(double)bytes / _1GB:#,,0.0} GB";
            if (bytes > _1MB)
                return $"{(double)bytes / _1MB:#,,0.0} MB";
            if (bytes > _1kB)
                return $"{(double)bytes / _1kB:#,,0.0} kB";
            return $"{bytes:#,,0} B";
        }

        /// <summary>
        /// Return a percentage of a whole
        /// </summary>
        private string PercentageOf(long n, long total)
        {
            if (total == 0)
                return "-";
            return ((double)n / total).ToString("##0%");
        }

        /// <summary>
        /// Update the list view when a tree node is clicked
        /// </summary>
        private void _treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _listView.Items.Clear();

            if (e.Node != null && e.Node.Tag != null)
            {
                var dir = (SizeDirectory)e.Node.Tag;
                _treeStatusLabel.Text = dir.FullName;
                _listNameStatusLabel.Text = $"{dir.Directories.Count:#,,0} folder(s), {dir.Files.Count:#,,0} file(s)";
                _listSizeStatusLabel.Text = NiceSize(dir.SizeInBytes);
                _listPercentageStatusLabel.Text = "100%";
                _listFoldersStatusLabel.Text = dir.TotalDirectoryCount.ToString("#,,0");
                _listFilesStatusLabel.Text = dir.TotalFileCount.ToString("#,,0");
                _listBytesStatusLabel.Text = dir.SizeInBytes.ToString("#,,0");
                _lastCurrentOpenedPath = dir.FullName;

                foreach (var subdir in dir.Directories)
                {
                    ListViewItem item = new ListViewItem(new[] {
                        subdir.Name,
                        NiceSize(subdir.SizeInBytes),
                        PercentageOf(subdir.SizeInBytes, dir.SizeInBytes),
                        subdir.TotalDirectoryCount.ToString("#,,0"),
                        subdir.TotalFileCount.ToString("#,,0"),
                        subdir.SizeInBytes.ToString("#,,0"),
                        subdir.LastModified.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    subdir.Exception != null ? ICON_ERROR : ICON_FOLDER);

                    // Definir o Name do ListViewItem com o nome do subdiret�rio
                    item.Name = subdir.FullName;

                    // Definir a propriedade Tag para armazenar o objeto subdir
                    item.Tag = subdir;

                    // Adicionar o item ao ListView
                    _listView.Items.Add(item);
                }

                foreach (var file in dir.Files)
                {
                    ListViewItem fileItem = new ListViewItem(new[] {
                        file.Name,
                        NiceSize(file.SizeInBytes),
                        PercentageOf(file.SizeInBytes, dir.SizeInBytes),
                        0.ToString("#,,0"),
                        0.ToString("#,,0"),
                        file.SizeInBytes.ToString("#,,0"),
                        file.LastModified.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                    file.Exception != null ? ICON_ERROR : ICON_FILE);

                    // Definir o Name do ListViewItem com o nome do subdiret�rio
                    fileItem.Name = file.FullName;

                    // Definir a propriedade Tag para armazenar o objeto subdir
                    fileItem.Tag = file;

                    // Adicionar o item ao ListView
                    _listView.Items.Add(fileItem);
                }

                _upToolButton.Enabled = e.Node.Level > 0;
            }
        }

        private void _treeViewContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _showDirInExplorerMenuItem.Enabled = _treeView.SelectedNode != null;
            _openDirInExplorerMenuItem.Enabled = _treeView.SelectedNode != null;
        }

        /// <summary>
        /// When a node in the tree is (not left)-clicked, select it
        /// This sets the right node for the context menu
        /// </summary>
        private void _treeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Button != MouseButtons.Left)
                _treeView.SelectedNode = e.Node;
        }

        /// <summary>
        /// "Open" a directory when it's double-clicked in the list view
        /// </summary>
        private void _listView_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedFolder();
        }

        private void _listView_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '\r': // Enter
                    OpenSelectedFolder();
                    e.Handled = true;
                    break;
                case '\b': // Backspace
                    GotoParentDirectory();
                    e.Handled = true;
                    break;
            }
        }

        private void OpenSelectedFolder()
        {
            // We must have a selected item on the right AND a selected folder
            var selectedFolder = GetSelectedNodeTree();
            if (_listView.SelectedItems.Count > 0 && selectedFolder != null)
            {
                // Find the file object
                var dir = SelectedListItem as SizeDirectory;
                if (dir != null)
                {
                    // Search the tree for the item with the right name
                    var nodes = selectedFolder.Nodes.Find(dir.FullName, false);
                    if (nodes.Length == 1)
                    {
                        // If we found one, select it
                        _treeView.SelectedNode = nodes[0];
                        nodes[0].EnsureVisible();
                    }
                }
            }
        }

        /// <summary>
        /// Mirror the widths of list view columns in the status labels of its status strip
        /// </summary>
        private void _listView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.ColumnIndex >= _columnStatusLabels.Length)
                throw new ArgumentOutOfRangeException($"No status label for column index {e.ColumnIndex}");

            // Set the width of the status label to the width of the column
            _columnStatusLabels[e.ColumnIndex].Width = _listView.Columns[e.ColumnIndex].Width;
        }

        /// <summary>
        /// Allow the user to choose a folder to open and parse
        /// </summary>
        private async void _openButton_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.UseDescriptionForTitle = true;
                dlg.Description = "Choose a folder to open";
                dlg.SelectedPath = _lastOpenedPath;

                if (dlg.ShowDialog(this) == DialogResult.OK && dlg.SelectedPath != null)
                {
                    _lastOpenedPath = dlg.SelectedPath;
                    await LoadSizesAsync(dlg.SelectedPath);
                }
            }
        }

        /// <summary>
        /// Signal the current loading task that it should terminate
        /// </summary>
        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _cancelled = true;
        }

        private void OpenInExplorer(string path) => Process.Start("explorer.exe", $"\"{path}\"");
        private void RevealInExplorer(string path) => Process.Start("explorer.exe", $"/select,\"{path}\"");

        /// <summary>
        /// "Reveal" a directory in Explorer, i.e. show its parent with
        /// this directory selected
        /// </summary>
        private void _showDirInExplorerMenuItem_Click(object sender, EventArgs e)
        {
            if (_treeView.SelectedNode != null)
                RevealInExplorer(SelectedTreeDirectory.FullName);
        }

        /// <summary>
        /// "Open" a directory in Explorer, i.e. show its contents
        /// </summary>
        private void _openDirInExplorerMenuItem_Click(object sender, EventArgs e)
        {
            if (_treeView.SelectedNode != null)
                OpenInExplorer(SelectedTreeDirectory.FullName);
        }

        /// <summary>
        /// Reveal an item in Explorer
        /// </summary>
        private void showItemInExplorerMenuItem_Click(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count > 0)
                RevealInExplorer(SelectedListItem.FullName);
        }

        /// <summary>
        /// Open an item directory or reveal a file in Explorer
        /// </summary>
        private void openItemInExplorerMenuItem_Click(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count > 0)
            {
                var item = SelectedListItem;
                if (item is SizeDirectory)
                    OpenInExplorer(item.FullName);
                else
                    RevealInExplorer(item.FullName);
            }
        }

        /// <summary>
        /// Go to parent folder
        /// </summary>
        private void _upToolButton_Click(object sender, EventArgs e)
        {
            GotoParentDirectory();
        }

        private void GotoParentDirectory()
        {
            var node = _treeView.SelectedNode;
            if (node != null && node.Level > 0)
                _treeView.SelectedNode = node.Parent;
        }

        /// <summary>
        /// Refresh, i.e. reload the current folder structure
        /// </summary>
        private async void _refreshToolButton_Click(object sender, EventArgs e)
        {
            await LoadSizesAsync(_lastOpenedPath);
        }

        // Atualiza os nodes da TreeView
        private List<TreeNode> RefreshSelectedNode(string[] dirs)
        {
            List<TreeNode> result = new List<TreeNode>();
            var selectedNode = GetSelectedNodeTree();
            if ((_treeView.Nodes.Count < 0) || (selectedNode == null))
            {
                return result;
            }

            selectedNode.Nodes.Clear();

            foreach (string dir in dirs)
            {
                result.Add(PrepareNode(dir));
            }

            return result;
        }

        // M�todo recursivo para atualiza��o de pastas na treeView
        private TreeNode PrepareNode(string dir)
        {

            SizeDirectory newSizeDir = SizeDirectory.FromPath(dir);
            TreeNode newNode = new TreeNode(Path.GetFileName(newSizeDir.FullName)) { Name = newSizeDir.FullName };

            newNode.Text = newSizeDir.Name;
            newNode.ImageKey = ICON_FOLDER;
            newNode.SelectedImageKey = ICON_FOLDER;
            newNode.Tag = newSizeDir;

            string[] subDirs = Directory.GetDirectories(dir);

            foreach (string subDir in subDirs)
            {
                SizeDirectory newSizeSubDir = SizeDirectory.FromPath(subDir);
                newNode.Nodes.Add(PrepareNode(subDir));
            }
            return newNode;
        }

        private void _refreshCurrentFolder_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = GetSelectedNodeTree();
            _listView.Items.Clear();
            string[] dirs = Directory.GetDirectories(selectedNode.FullPath);
            List<TreeNode> updatedDirs = RefreshSelectedNode(dirs);

            // adicionando pastas
            ((SizeDirectory)selectedNode.Tag).Directories.Clear();
            foreach (TreeNode node in updatedDirs)
            {
                AddFolderToList(node, selectedNode);
            }

            // adicionando arquivos
            string[] updatedFiles = Directory.GetFiles(selectedNode.FullPath);
            SizeDirectory parentSizeDirectory = SizeDirectory.FromPath(selectedNode.FullPath) ;
            foreach (var file in updatedFiles)
            {
                SizeFile sizeItem = new SizeFile(file, selectedNode.Parent.FullPath);
                ListViewItem fileItem = new ListViewItem(new[] {
                        sizeItem.Name,
                        NiceSize(sizeItem.SizeInBytes),
                        PercentageOf(sizeItem.SizeInBytes, parentSizeDirectory.SizeInBytes),
                        0.ToString("#,,0"),
                        0.ToString("#,,0"),
                         sizeItem.SizeInBytes.ToString("#,,0"),
                        sizeItem.LastModified.ToString("yyyy-MM-dd HH:mm:ss")
                    },
                sizeItem.Exception != null ? ICON_ERROR : ICON_FILE);
                fileItem.Name = sizeItem.FullName;
                fileItem.Tag = file;
                _listView.Items.Add(fileItem);
            }

            _treeView.Sort();
        }

        /// <summary>
        /// Prepare the list view context menu. There can be no list item selected
        /// and then we need to disable some menu items
        /// </summary>
        private void _listViewContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var hasSelectedItem = _listView.SelectedItems.Count == 1;
            var hasFolderSelected = false;
            if (hasSelectedItem)
            {
                hasFolderSelected = SelectedListItem is SizeDirectory;
                _updateMenuItem.Visible = false;
                _separatorUpdate.Visible = false;
                _newFileMenuItem.Visible = false;
                _newFolderMenuItem.Visible = false;
                _separatorDelete.Visible = true;
                _deleteMenuItem.Visible = true;
                _renameMenuItem.Visible = true;
                _copyMenuItem.Visible = true;
                _cutMenuItem.Visible = true;
                _pasteMenuItem.Visible = false;
            } else {
                _updateMenuItem.Visible = true;
                _separatorUpdate.Visible = true;
                _newFileMenuItem.Visible = true;
                _newFolderMenuItem.Visible = true;
                _deleteMenuItem.Visible = false;
                _separatorDelete.Visible = false;
                _renameMenuItem.Visible = false;
                _copyMenuItem.Visible = false;
                _cutMenuItem.Visible = false;
                if (this._copiedItem != null) { _pasteMenuItem.Visible = true; }
            }
        }

        public void _newFileMenuItem_Click(object sender, EventArgs e)
        {
            var node = GetSelectedNodeTree();
            string selectedFolder = node.FullPath;
            FormNewFile formNewFile = new FormNewFile(selectedFolder);
            formNewFile.FileCreated += (sender, e) => AddFileToList(e, node);
            formNewFile.Show();
        }
        public void _deleteMenuItem_Click(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0) {
                return;
            }

            SizeItem sizeItem = (SizeItem)_listView.SelectedItems[0].Tag;
            string selectedItem = sizeItem.FullName;

            DialogResult result = MessageBox.Show($"Deseja realmente excluir este item? \n{selectedItem}", "Confirma��o", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {

                // remove o item da listview
                foreach (ListViewItem item in _listView.Items)
                {
                    if (item.Name == selectedItem)
                    {
                        _listView.Items.Remove(item);
                        break;
                    }
                }

                if (Directory.Exists(selectedItem))
                {
                    SizeDirectory sizeDirectory = (SizeDirectory)sizeItem;
                    TreeNode[] treeToRemove = _treeView.Nodes.Find(selectedItem, true);
                    SizeDirectory parentSizeDir = (SizeDirectory)_treeView.SelectedNode.Tag;
                    parentSizeDir.Directories.Remove(sizeDirectory);
                    treeToRemove[0].Remove();
                    // remove diret�rio
                    Directory.Delete(selectedItem, true);
                }
                else if (File.Exists(selectedItem))
                {
                    SizeFile sizeFile = (SizeFile)sizeItem;
                    SizeDirectory parentSizeDir = (SizeDirectory)_treeView.SelectedNode.Tag;
                    parentSizeDir.Files.Remove(sizeFile);
                    // Fecha todos os handles associados ao arquivo
                    File.WriteAllText(selectedItem, String.Empty);
                    // remove arquivo
                    File.Delete(selectedItem);
                }
            }
        }
        public void _viewMenuItem_Click(object sender, EventArgs e) {}

        private void ResetViewChecks()
        {
            _viewBigIconMenuItem.Checked = false;
            _viewSmallIconMenuItem.Checked = false;
            _viewListIconMenuItem.Checked = false;
            _viewDetailsIconMenuItem.Checked = false;
        }

        public void _viewListIconMenuItem_Click(object sender, EventArgs e) {
            ResetViewChecks();
            _viewListIconMenuItem.Checked = true;
            _listView.View = View.SmallIcon;
        }
        public void _viewDetailsIconMenuItem_Click(object sender, EventArgs e)
        {
            ResetViewChecks();
            _viewDetailsIconMenuItem.Checked = true;
            _listView.View = View.Details;
        }

        public void _viewBigIconMenuItem_Click(object sender, EventArgs e) {
            ResetViewChecks();
            _viewBigIconMenuItem.Checked = true;
            _listView.View = View.LargeIcon;
        }

        public void _viewSmallIconMenuItem_Click(object sender, EventArgs e) {
            ResetViewChecks();
            _viewSmallIconMenuItem.Checked = true;
            _listView.View = View.SmallIcon;
        }

        public void _copyMenuItem_Click(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                return;
            }

            SizeItem sizeItem = (SizeItem)_listView.SelectedItems[0].Tag;
            SizeDirectory parentSizeDir = (SizeDirectory)_treeView.SelectedNode.Tag;
            _copiedItem = new CopiedItem(_treeView.SelectedNode, parentSizeDir, _listView.SelectedItems[0], sizeItem, false);
        }

        public void _cutMenuItem_Click(object sender, EventArgs e)
        {
            SizeItem sizeItem = (SizeItem)_listView.SelectedItems[0].Tag;
            SizeDirectory parentSizeDir = (SizeDirectory)_treeView.SelectedNode.Tag;
            _copiedItem = new CopiedItem(_treeView.SelectedNode, parentSizeDir, _listView.SelectedItems[0], sizeItem, true);
        }

        public void _pasteMenuItem_Click(object sender, EventArgs e)
        {
            if (this._copiedItem == null)
            {
                return;
            }

            TreeNode destinationTreeNode = _treeView.SelectedNode;
            SizeDirectory destinationSizeDirectory = (SizeDirectory)_treeView.SelectedNode.Tag;

            if (this._copiedItem.IsDirectory())
            {
                DirectoryInfo dirInfo = new DirectoryInfo(this._copiedItem.copiedListViewItem.Name);
                if (this._copiedItem.isMoveItem)
                {
                    dirInfo.MoveTo(destinationSizeDirectory.FullName);
                } else {
                    // fazer o foreach
                }
            } else {
                var newDestinationName = Path.Combine(destinationSizeDirectory.FullName, this._copiedItem.copiedSizeItem.Name);
                if (this._copiedItem.isMoveItem)
                {
                    File.Move(this._copiedItem.copiedListViewItem.Name, newDestinationName);
                } else {
                    File.Copy(this._copiedItem.copiedListViewItem.Name, newDestinationName);
                }
            }
        }

        public void _renameMenuItem_Click(object sender, EventArgs e)
        {
        }

        // Adiciona arquivo � listView
        public void AddFileToList(object sender, TreeNode selectedNode)
        {
            string itemFullName = "";
            string itemName = "";
            string rootPath = "";

            if (sender is ItemCreatedEventArgs)
            {
                ItemCreatedEventArgs fileCreated = (ItemCreatedEventArgs)sender;
                itemFullName = fileCreated.ItemFullName;
                itemName = fileCreated.ItemName;
                rootPath = fileCreated.RootPath;
            }
            else if (sender is TreeNode)
            {
                TreeNode folderCreated = (TreeNode)sender;
                itemFullName = folderCreated.FullPath;
                itemName = folderCreated.Text;
            }

            var parentNode = GetSelectedNodeTree();
            var parentDir = (SizeDirectory)parentNode.Tag;
            
            SizeFile file = new SizeFile(itemFullName, rootPath);

            var newListChild = new ListViewItem(new[] {
                    itemName,
                    NiceSize(0),
                    PercentageOf(0, parentDir.SizeInBytes),
                    null,
                    0.ToString("#,,0"),
                    0.ToString("#,,0"),
                    0.ToString("yyyy-MM-dd HH:mm:ss") });

            newListChild.Name = itemFullName;
            newListChild.ImageKey = ICON_FILE;
            newListChild.Tag = file;

            _listView.Items.Add(newListChild);
            parentDir.Files.Add(file);
        }

        // Adiciona ite do tipo 'diret�rio' � listView
        public void AddFolderToList(object sender, TreeNode parentNode)
        {
            SizeDirectory parentDir = (SizeDirectory)parentNode.Tag;
            TreeNode newNode = new TreeNode();
            string itemFullName = "";
            string itemName = "";

            if (sender is ItemCreatedEventArgs) {
                ItemCreatedEventArgs folderCreated = (ItemCreatedEventArgs)sender;
                itemFullName = folderCreated.ItemFullName;
                itemName = folderCreated.ItemName;
                newNode = new TreeNode(Path.GetFileName(itemFullName)) { Name = itemFullName };
            }
            else if (sender is TreeNode)
            {
                TreeNode folderCreated = (TreeNode)sender;
                SizeDirectory sizeDir = (SizeDirectory)folderCreated.Tag;
                itemFullName = sizeDir.FullName;
                itemName = sizeDir.Name;
                newNode = folderCreated;
            }

            newNode.ImageKey = ICON_FOLDER;
            newNode.SelectedImageKey = ICON_FOLDER_OPEN;
            newNode.EnsureVisible();

            var newListChild = new ListViewItem(new[] {
                    itemName,
                    NiceSize(0),
                    PercentageOf(0, parentDir.SizeInBytes),
                    null,
                    0.ToString("#,,0"),
                    0.ToString("#,,0"),
                    0.ToString("yyyy-MM-dd HH:mm:ss") } );

            newListChild.Name = itemFullName;
            newListChild.ImageKey = ICON_FOLDER;

            SizeDirectory newSizeDirectory = SizeDirectory.FromPath(itemFullName);
            parentDir.Directories.Add(newSizeDirectory);
            
            newListChild.Tag = newSizeDirectory;
            _listView.Items.Add(newListChild);

            newNode.Tag = newSizeDirectory;
            parentNode.Nodes.Add(newNode);
        }

        public void _newFolderMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = GetSelectedNodeTree();
            string selectedFolder = node.FullPath;
            FormNewFolder formNewFolder = new FormNewFolder(selectedFolder);
            formNewFolder.FolderCreated += (sender, e) => AddFolderToList(e, node);
            formNewFolder.Show();
        }

        public TreeNode GetSelectedNodeTree() {
            TreeNode node = null;
            if (_treeView.SelectedNode == null)
            {
                node = _treeView.Nodes.Find(_lastCurrentOpenedPath, true).FirstOrDefault();
                if (node == null)
                {
                    return node;
                }
            }
            else
            {
                node = _treeView.SelectedNode;
            }

            return node;
        }

        /// <summary>
        /// Open the tool home page
        /// </summary>
        private void _updateAvailableButton_Click(object sender, EventArgs e)
        {
            try
            {
                var pi = new ProcessStartInfo($"https://www.mobzystems.com/Tools/{ToolName}");
                pi.UseShellExecute = true;
                Process.Start(pi);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "Could not open web page", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}