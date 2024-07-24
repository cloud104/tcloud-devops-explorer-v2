using Microsoft.VisualBasic.FileIO;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using System.Windows.Forms;
using System.Xml.Linq;
using TCLoudExplorer;
using TCLoudExplorer.@class;
using TCLoudExplorer.Properties;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TCloudExplorer
{
    public partial class MainForm : Form
    {
        private CopiedItem _copiedItem = null;

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
        private const string TYPE_FOLDER = "folder";
        private const string TYPE_FILE = "file";
        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss";
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
            //if (Environment.GetCommandLineArgs().Length > 1)
           // {
                string path = @"c:\Clean-Folder\";
                //Environment.GetCommandLineArgs()[1];
                await LoadSizesAsync(path);
                // Save this path for Refresh and Open
                _lastOpenedPath = Path.GetFullPath(path);
            //}
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
                        subdir.LastModified.ToString(DATE_FORMAT)
                    },
                    subdir.Exception != null ? ICON_ERROR : ICON_FOLDER);

                    // Definir o Name do ListViewItem com o nome do subdiretório
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
                        file.LastModified.ToString(DATE_FORMAT)
                    },
                    file.Exception != null ? ICON_ERROR : ICON_FILE);

                    fileItem.ImageKey = GetImageExtension(file);

                    // Definir o Name do ListViewItem com o nome do subdiretório
                    fileItem.Name = file.FullName;

                    // Definir a propriedade Tag para armazenar o objeto subdir
                    fileItem.Tag = file;

                    // Adicionar o item ao ListView
                    _listView.Items.Add(fileItem);
                }

                _upToolButton.Enabled = e.Node.Level > 0;
            }
        }
        
        // Resgata a extensão do arquivo, insere o ícone de acordo com o S.O. e retorna a extensão
        // para ser inserida no listViewItem
        private string GetImageExtension(SizeItem file)
        {
            if (file is not SizeItem) return null;

            string extension = "";
            if (Path.HasExtension(file.FullName))
            {
                extension = Path.GetExtension(file.FullName);
                if (!_imageList.Images.ContainsKey(extension))
                {
                    Icon fileIcon = GetSystemIcon.GetIconByFileName(file.FullName);
                    _imageList.Images.Add(extension, fileIcon);
                }
            }

            return extension;
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
            _treeView.SelectedNode = e.Node;
        }

        /// <summary>
        /// "Open" a directory when it's double-clicked in the list view
        /// </summary>
        private void _listView_DoubleClick(object sender, EventArgs e)
        {
            if (this._listView.SelectedItems.Count > 1)
                return;

            SizeItem sizeItem = (SizeItem)this._listView.SelectedItems[0].Tag;
            if (sizeItem is SizeFile)
            {
                OpenItemListView();
            } else if (sizeItem is SizeDirectory)
            {
                OpenSelectedFolder();
            }
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

        // Método recursivo para atualização de pastas na treeView
        private TreeNode PrepareNode(string dir)
        {

            SizeDirectory newSizeDir = SizeDirectory.FromPath(dir);
            TreeNode newNode = new TreeNode(Path.GetFullPath(newSizeDir.FullName)) { Name = Path.GetFullPath(newSizeDir.FullName) };

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
            RefreshCurrentFolder();
        }
        public void RefreshCurrentFolder()
        {
            RefreshCurrentFolder(null);
        }

        public void RefreshCurrentFolder(TreeNode selectedNode)
        {
            if (selectedNode is null)
            {
                selectedNode = GetSelectedNodeTree();
            } else
            {
                selectedNode.Nodes.Clear();
            }
            _listView.Items.Clear();
            string fullPath = selectedNode.FullPath;
            string[] dirs = Directory.GetDirectories(fullPath);
            List<TreeNode> updatedDirs = RefreshSelectedNode(dirs);
            selectedNode.Nodes.AddRange(updatedDirs.ToArray());

            // adicionando pastas
            ((SizeDirectory)selectedNode.Tag).Directories.Clear();
            foreach (TreeNode node in selectedNode.Nodes)
            {
                AddFolderToList(node, selectedNode);
            }

            // adicionando arquivos
            string[] updatedFiles = Directory.GetFiles(fullPath);
            SizeDirectory parentSizeDirectory = SizeDirectory.FromPath(fullPath);
            var sizeParentDir = (SizeDirectory)selectedNode.Tag;
            sizeParentDir.Files.Clear();
            foreach (var file in updatedFiles)
            {
                SizeFile sizeFile = new SizeFile(file, fullPath);
                ListViewItem fileItem = new ListViewItem(new[] {
                        sizeFile.Name,
                        NiceSize(sizeFile.SizeInBytes),
                        PercentageOf(sizeFile.SizeInBytes, parentSizeDirectory.SizeInBytes),
                        0.ToString("#,,0"),
                        0.ToString("#,,0"),
                        sizeFile.SizeInBytes.ToString("#,,0"),
                        sizeFile.LastModified.ToString(DATE_FORMAT)
                    });
                fileItem.Name = sizeFile.FullName;
                fileItem.Tag = sizeFile;
                fileItem.ImageKey = ICON_FILE;
                fileItem.ImageKey = GetImageExtension(sizeFile);
                _listView.Items.Add(fileItem);
                sizeParentDir.Files.Add(sizeFile);
            }

            _treeView.Sort();
        }

        /// <summary>
        /// Prepare the list view context menu. There can be no list item selected
        /// and then we need to disable some menu items
        /// </summary>
        private void _listViewContextMenu_Opening(object sender, EventArgs e)
        {
            var hasSelectedItem = _listView.SelectedItems.Count >= 1;
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
                _openItemMenuItem.Visible = true;
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
                _openItemMenuItem.Visible = false;
                if (this._copiedItem != null) {
                    _pasteMenuItem.Visible = true;
                } else
                {
                    _pasteMenuItem.Visible = false;
                }
            }
        }

        public void _newFileMenuItem_Click(object sender, EventArgs e)
        {
            string newName = "";
            newName = Microsoft.VisualBasic.Interaction.InputBox("Entre com o nome do novo arquivo:", "Novo Arquivo", newName);

            if (string.IsNullOrEmpty(newName))
            {
                return;
            }

            TreeNode selectedFolder = GetSelectedNodeTree();
            string newNameFullPath = Path.Combine(selectedFolder.FullPath, newName);

            if (!Utils.IsValidFileName(newName))
            {
                MessageBox.Show("O nome do arquivo não pode ser vazio ou conter caracteres especiais:\r\n" + "\t\\/:*?\"<>|,", "Nome Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(newNameFullPath))
            {
                MessageBox.Show("Existe um arquivo com o mesmo nome no caminho atual！", "Arquivo Já Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!Path.HasExtension(newName))
            {
                MessageBox.Show("Não é possível criar um arquivo sem extensão!", "Formato Incorreto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                File.Create(newNameFullPath);
                RefreshCurrentFolder(selectedFolder);
            }
        }
        public void _deleteMenuItem_Click(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0) {
                return;
            }

            DialogResult result = MessageBox.Show("Deseja realmente excluir o(s) item(s) selecionado(s)?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                return;
            }

            var selectedItems = _listView.SelectedItems.Cast<ListViewItem>().ToList();

            foreach (var item in selectedItems)
            {
                SizeItem sizeItem = (SizeItem)item.Tag;
                DeleteItem(sizeItem);
            }
        }

        public void DeleteItem(SizeItem sizeItem)
        {
            string selectedItem = sizeItem.FullName;

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
                // remove diretório
                DeleteFolder(selectedItem);
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

            var selectedNode = GetSelectedNodeTree();
            SizeDirectory parentSizeDir = (SizeDirectory)selectedNode.Tag;
            var coll = _listView.SelectedItems.Cast<ListViewItem>().ToList();
            _copiedItem = new CopiedItem(selectedNode, parentSizeDir, coll, false);
        }

        public void _openItemMenuItem_Click(object sender, EventArgs e)
        {
            OpenItemListView();
        }

        public void OpenItemListView()
        {
            if (_listView.SelectedItems.Count == 0)
            {
                return;
            }

            if (_listView.SelectedItems[0].Tag is SizeFile)
            {
                SizeItem sizeItem = (SizeItem)_listView.SelectedItems[0].Tag;
                if (sizeItem is SizeFile)
                {
                    Process.Start(new ProcessStartInfo(sizeItem.FullName) { UseShellExecute = true });
                }
            }
            else if (_listView.SelectedItems[0].Tag is SizeDirectory)
            {
                OpenSelectedFolder();
            }
        }

        public void _cutMenuItem_Click(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 0)
            {
                return;
            }

            var selectedNode = GetSelectedNodeTree();
            SizeItem sizeItem = (SizeItem)selectedNode.Tag;
            SizeDirectory parentSizeDir = (SizeDirectory)selectedNode.Tag;
            var coll = _listView.SelectedItems.Cast<ListViewItem>().ToList();
            _copiedItem = new CopiedItem(selectedNode, parentSizeDir, coll, true);
        }

        public void _pasteMenuItem_Click(object sender, EventArgs e)
        {
            if (this._copiedItem == null)
            {
                return;
            }

            TreeNode destinationTreeNode = GetSelectedNodeTree();
            SizeDirectory destinationSizeDirectory = (SizeDirectory)destinationTreeNode.Tag;
            bool replaceDestination = false;
            DialogResult result = DialogResult.None;
            TreeNode parentSourceFullName = this._copiedItem.parentSource;

            foreach (var listViewItem in this._copiedItem.copiedListViewItem)
            {
                // se for diretórios
                if (listViewItem.Tag is SizeDirectory)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(listViewItem.Name);
                    SizeDirectory sizeDirectory = (SizeDirectory)listViewItem.Tag;
                    string fullDestinationName = Path.Combine(destinationSizeDirectory.FullName, listViewItem.Text);

                    if (Directory.Exists(fullDestinationName) && result.Equals(DialogResult.None)) { 
                        result = MessageBox.Show("Existem conteúdos iguais na pasta de destino. Deseja substituir o conteúdo pelo copiado?", "Conteúdo Existente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result.Equals(DialogResult.No) || (result.Equals(DialogResult.None)))
                        {
                            return;
                        } else
                        {
                            replaceDestination = true;
                        }
                    }
                    CopySubFoldersAndFiles(listViewItem.Name, destinationSizeDirectory.FullName, this._copiedItem.isMoveItem, replaceDestination);
                    // se for mover, deleta os diretórios e seu conteúdo
                    if (this._copiedItem.isMoveItem)
                    {
                        DeleteFolder(listViewItem.Name);
                    }
                }
                // se for arquivos
                else if (listViewItem.Tag is SizeItem)
                {
                    SizeItem sizeItem = (SizeItem)listViewItem.Tag;
                    var newDestinationName = Path.Combine(destinationSizeDirectory.FullName, listViewItem.Text);

                    if (File.Exists(newDestinationName) && result.Equals(DialogResult.None))
                    {
                        result = MessageBox.Show("Existem conteúdos iguais na pasta de destino. Deseja substituir o conteúdo pelo copiado?", "Conteúdo Existente", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result.Equals(DialogResult.No) || (result.Equals(DialogResult.None)))
                        {
                            return;
                        }
                        else
                        {
                            replaceDestination = true;
                        }
                    }

                    CopyFile(sizeItem.FullName, newDestinationName, this._copiedItem.isMoveItem, replaceDestination);
                }
            }

            if (this._copiedItem.isMoveItem)
            {
                RefreshCurrentFolder(parentSourceFullName);
                destinationTreeNode = GetSelectedNodeTree();
                RefreshCurrentFolder(destinationTreeNode);
            }
            else
            {
                RefreshCurrentFolder();
            }
            destinationTreeNode.EnsureVisible();
            this._copiedItem = null;
        }

        private void DeleteFolder(string target)
        {
            string[] files = Directory.GetFiles(target);
            string[] dirs = Directory.GetDirectories(target);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteFolder(dir);
            }

            Directory.Delete(target, false);
        }

        // Método que faz a copia de diretorios ou arquivos recursivamente
        private void CopySubFoldersAndFiles(string sourceFolder, string destinationFolder, bool move, bool replace)
        {
            string[] files = Directory.GetFiles(sourceFolder);
            string[] subdirectories = Directory.GetDirectories(sourceFolder);
            string directoryName = Path.GetFileName(sourceFolder);
            string destinationPath = Path.Combine(destinationFolder, directoryName);

            if (Directory.Exists(destinationPath))
            {
                if (replace)
                {
                    DeleteFolder(destinationPath);
                }
            }

            // cria o diretorio de destino
            Directory.CreateDirectory(destinationPath);

            // se for arquivo
            foreach (string file in files)
            {
                string fileSourceName = Path.GetFileName(file);
                string subFileDestName = Path.Combine(destinationFolder, directoryName);
                string fileDestName = Path.Combine(subFileDestName, fileSourceName);
                CopyFile(file, fileDestName, move, replace);
            }

            // verifica sub diretorios e faz a recursão
            foreach (string subdirectory in subdirectories)
            {
                string subSubDestName = Path.Combine(destinationFolder, directoryName);
                CopySubFoldersAndFiles(subdirectory, subSubDestName, move, replace);
            }
        }

        public void CopyFile(string sourceFile, string destinationFile, bool move, bool replace)
        {
            string fileName = Path.GetFileName(sourceFile);

            if (replace && File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }

            if (move)
            {
                File.Move(sourceFile, destinationFile);
            }
            else
            {
                File.Copy(sourceFile, destinationFile);
            }
        }

        public void _renameMenuItem_Click(object sender, EventArgs e)
        {
            if (_listView.SelectedItems.Count == 1)
            {
                ListViewItem selectedItem = _listView.SelectedItems[0];
                string currentName = selectedItem.Text;
                string newName = Microsoft.VisualBasic.Interaction.InputBox("Entre com o novo nome:", "Rename", currentName);
                if (string.IsNullOrEmpty(newName)) {
                MessageBox.Show("O nome do arquivo não pode ser nulo ou conter caracteres especiais:\r\n" + "\t\\/:*?\"<>|,", "Nome Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!Utils.IsValidFileName(newName))
                {
                    MessageBox.Show("O nome do arquivo não pode ser nulo ou conter caracteres especiais:\r\n" + "\t\\/:*?\"<>|,", "Nome Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                // se for diretório
                if (selectedItem.Tag is SizeDirectory)
                {
                    SizeDirectory sizeDirectory = (SizeDirectory)selectedItem.Tag;
                    string newPath = Path.Combine(Path.GetDirectoryName(sizeDirectory.FullName), newName);
                    if (Directory.Exists(newPath))
                    {
                        MessageBox.Show("O novo nome já existe no diretório atual!", "Nome Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    // renomeia o diretório
                    RenameDirectory(sizeDirectory.FullName, newName);
                }
                // se for arquivo
                else if (selectedItem.Tag is SizeFile)
                {
                    SizeFile sizeFile = (SizeFile)selectedItem.Tag;
                    string newPath = Path.Combine(Path.GetDirectoryName(sizeFile.FullName), newName);
                    if (File.Exists(newPath)) {
                        MessageBox.Show("O novo nome já existe no diretório atual!", "Nome Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    if (!Path.HasExtension(newName))
                    {
                        MessageBox.Show("Não é possível criar um arquivo sem extensão!", "Formato Incorreto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    File.Move(sizeFile.FullName, newPath);
                }
                else
                {
                    MessageBox.Show("Ocorreu um erro ao renomear o item selecionado. Entre em contato com o suporte ou tente novamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                RefreshCurrentFolder();
            } else if (_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("Selecione apenas um item para renomear", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        // Adiciona arquivo à listView
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

            ListViewItem newListChild = CreateListViewItem(itemName, itemFullName, parentDir, TYPE_FILE);

            newListChild.Tag = file;
            newListChild.ImageKey = GetImageExtension(file);
            _listView.Items.Add(newListChild);
            parentDir.Files.Add(file);
        }

        // Adiciona ite do tipo 'diretório' à listView
        public void AddFolderToList(object sender, TreeNode parentNode)
        {
            SizeDirectory parentDir = (SizeDirectory)parentNode.Tag;
            TreeNode newNode = new TreeNode();
            string itemFullName = "";
            string itemName = "";

            if (sender is TreeNode)
            {
                TreeNode senderTree = (TreeNode)sender;
                parentNode.Nodes.RemoveByKey(senderTree.Name);
                SizeDirectory sizeDir = (SizeDirectory)senderTree.Tag;
                itemFullName = sizeDir.FullName;
                itemName = sizeDir.Name;
                newNode = senderTree;
            }

            newNode.ImageKey = ICON_FOLDER;
            newNode.SelectedImageKey = ICON_FOLDER_OPEN;
            newNode.EnsureVisible();
            var newListChild = CreateListViewItem(itemName, itemFullName, parentDir, TYPE_FOLDER);
            newNode.Tag = (SizeDirectory)newListChild.Tag;
            _listView.Items.Add(newListChild);
            parentNode.Nodes.Add(newNode);
        }

        public ListViewItem CreateListViewItem(string itemName, string itemFullName, SizeDirectory parentDir, string type)
        {
            ListViewItem newListChild = new ListViewItem();
            newListChild.Name = itemFullName;
            newListChild.Text = itemName;
            string sizeInBytes = "0";
            string percentageOf = "0";
            string totDirectoryCount = "0";
            string totFileCount = "0";
            string lastModified = "0";

            if (type.Equals(TYPE_FOLDER))
            {

                newListChild.ImageKey = ICON_FOLDER;
                SizeDirectory newSizeDirectory = SizeDirectory.FromPath(itemFullName);
                sizeInBytes = NiceSize(newSizeDirectory.SizeInBytes);
                percentageOf = PercentageOf(0, parentDir.SizeInBytes);
                totDirectoryCount = newSizeDirectory.TotalDirectoryCount.ToString("#,,0");
                totFileCount = newSizeDirectory.TotalFileCount.ToString("#,,0");
                lastModified = newSizeDirectory.LastModified.ToString(DATE_FORMAT);
                parentDir.Directories.Add(newSizeDirectory);
                newListChild.Tag = newSizeDirectory;
            }
            else if (type.Equals(TYPE_FILE))
            {
                SizeFile newSizeFile = new SizeFile(itemFullName, Path.GetDirectoryName(itemFullName));
                sizeInBytes = NiceSize(newSizeFile.SizeInBytes);
                percentageOf = PercentageOf(0, parentDir.SizeInBytes);
                totDirectoryCount = 0.ToString("#,,0");
                totFileCount = newSizeFile.SizeInBytes.ToString("#,,0");
                lastModified = newSizeFile.LastModified.ToString(DATE_FORMAT);
                newListChild.Tag = newSizeFile;
                string imageKey = GetImageExtension(newSizeFile);
                newListChild.ImageKey = string.IsNullOrEmpty(imageKey) ? ICON_FILE : imageKey;
            }

            newListChild.SubItems.Add(sizeInBytes);
            newListChild.SubItems.Add(percentageOf);
            newListChild.SubItems.Add(totDirectoryCount);
            newListChild.SubItems.Add(totFileCount);
            newListChild.SubItems.Add(sizeInBytes);
            newListChild.SubItems.Add(lastModified);

            return newListChild;
        }

        public void _newFolderMenuItem_Click(object sender, EventArgs e)
        {
            string newName = "";
            newName = Microsoft.VisualBasic.Interaction.InputBox("Entre com o nome do novo diretório:", "Novo Diretório", newName);

            if (string.IsNullOrEmpty(newName))
            {
                return;
            }

            TreeNode selectedFolder = GetSelectedNodeTree();
            string newNameFullPath = Path.Combine(selectedFolder.FullPath, newName);
            if (!Utils.IsValidFileName(newName))
            {
                MessageBox.Show("O nome do diretório não pode ser nulo ou conter caracteres especiais:\r\n" + "\t\\/:*?\"<>|,", "Nome Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            } else if (Directory.Exists(newNameFullPath))
            {
                MessageBox.Show("Existe um diretório com o mesmo nome no caminho atual！", "Arquivo Já Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Directory.CreateDirectory(newNameFullPath);
            TreeNode newTreeNodeFolder = new TreeNode(Path.GetFileName(newNameFullPath)) { Name = Path.GetFullPath(newNameFullPath) };
            SizeDirectory sizeDirectory = SizeDirectory.FromPath(newNameFullPath);
            newTreeNodeFolder.Tag = sizeDirectory;
            selectedFolder.Nodes.Add(newTreeNodeFolder);
            AddFolderToList(newTreeNodeFolder, selectedFolder);

            /*/
            TreeNode node = GetSelectedNodeTree();
            string selectedFolder = node.FullPath;
            FormNewFolder formNewFolder = new FormNewFolder(selectedFolder);
            formNewFolder.FolderCreated += (sender, e) => AddFolderToList(e, node);
            formNewFolder.Show();
            */
        }

        public TreeNode GetSelectedNodeTree() {
            TreeNode node = null;
            if (_treeView.SelectedNode == null)
            {
                node = _treeView.Nodes.Find(_lastCurrentOpenedPath, true).FirstOrDefault();
                if ((node is null) || (string.IsNullOrEmpty(node.FullPath)))
                {
                    node = new TreeNode(Path.GetFullPath(_lastCurrentOpenedPath)) { Name = Path.GetFullPath(_lastCurrentOpenedPath) };
                }
            }
            else
            {
                return _treeView.SelectedNode;
            }

            if (node.Tag == null)
            {
                node.Tag = SizeDirectory.FromPath(_lastCurrentOpenedPath);
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

        public void RenameDirectory(string currentPath, string newName)
        {
            string parentDirectory = Path.GetDirectoryName(currentPath);
            string newPath = Path.Combine(parentDirectory, newName);

            if (Directory.Exists(currentPath))
            {
                Directory.Move(currentPath, newPath);
            }
        }
    }
}