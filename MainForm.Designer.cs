using System.Collections;
using TCLoudExplorer.Properties;

namespace TCloudExplorer
{
  partial class MainForm
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

        public class TreeNodeComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                TreeNode nodeX = x as TreeNode;
                TreeNode nodeY = y as TreeNode;

                return string.Compare(nodeX.Text, nodeY.Text);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            ColumnHeader _nameColumn;
            ColumnHeader _sizeColumn;
            ColumnHeader _percentageColumn;
            ColumnHeader _directoriesColumn;
            ColumnHeader _filesColumn;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            _splitContainer = new SplitContainer();
            _treeView = new TreeView();
            _treeViewContextMenu = new ContextMenuStrip(components);
            _treeStatusStrip = new StatusStrip();
            _treeStatusLabel = new ToolStripStatusLabel();
            _listStatusStrip = new StatusStrip();
            _listNameStatusLabel = new ToolStripStatusLabel();
            _listSizeStatusLabel = new ToolStripStatusLabel();
            _listPercentageStatusLabel = new ToolStripStatusLabel();
            _listFoldersStatusLabel = new ToolStripStatusLabel();
            _listFilesStatusLabel = new ToolStripStatusLabel();
            _listBytesStatusLabel = new ToolStripStatusLabel();
            _listLastModifiedStatusLabel = new ToolStripStatusLabel();
            _listView = new ListView();
            _bytesColumn = new ColumnHeader();
            _lastModifiedColumn = new ColumnHeader();
            _listViewContextMenu = new ContextMenuStrip(components);
            _openFolderMenuItem = new ToolStripMenuItem();
            _openItemMenuItem = new ToolStripMenuItem();
            _updateMenuItem = new ToolStripMenuItem();
            _separatorUpdate = new ToolStripSeparator();
            _copyMenuItem = new ToolStripMenuItem();
            _pasteMenuItem = new ToolStripMenuItem();
            _cutMenuItem = new ToolStripMenuItem();
            _newFileMenuItem = new ToolStripMenuItem();
            _newFolderMenuItem = new ToolStripMenuItem();
            _separatorDelete = new ToolStripSeparator();
            _deleteMenuItem = new ToolStripMenuItem();
            _separatorView = new ToolStripSeparator();
            _renameMenuItem = new ToolStripMenuItem();
            _viewMenuItem = new ToolStripMenuItem();
            _viewSmallIconMenuItem = new ToolStripMenuItem();
            _viewBigIconMenuItem = new ToolStripMenuItem();
            _viewListIconMenuItem = new ToolStripMenuItem();
            _viewDetailsIconMenuItem = new ToolStripMenuItem();
            _panelLoading = new Panel();
            _progressBarLoading = new ProgressBar();
            _lblInitLoading = new Label();
            _showDirInExplorerMenuItem = new ToolStripMenuItem();
            _openDirInExplorerMenuItem = new ToolStripMenuItem();
            _showItemInExplorerMenuItem = new ToolStripMenuItem();
            _openItemInExplorerMenuItem = new ToolStripMenuItem();
            _topStatusStrip = new StatusStrip();
            _refreshToolButton = new ToolStripDropDownButton();
            _upToolButton = new ToolStripDropDownButton();
            toolStripDropDownButton1 = new ToolStripDropDownButton();
            _loadingStatusStrip = new StatusStrip();
            _cancelButton = new ToolStripDropDownButton();
            _loadingLabel = new ToolStripStatusLabel();
            _nameColumn = new ColumnHeader();
            _sizeColumn = new ColumnHeader();
            _percentageColumn = new ColumnHeader();
            _directoriesColumn = new ColumnHeader();
            _filesColumn = new ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)_splitContainer).BeginInit();
            _splitContainer.Panel1.SuspendLayout();
            _splitContainer.Panel2.SuspendLayout();
            _splitContainer.SuspendLayout();
            _treeStatusStrip.SuspendLayout();
            _listStatusStrip.SuspendLayout();
            _listViewContextMenu.SuspendLayout();
            _panelLoading.SuspendLayout();
            _topStatusStrip.SuspendLayout();
            _loadingStatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // _nameColumn
            // 
            _nameColumn.Text = "Name";
            _nameColumn.Width = 200;
            // 
            // _sizeColumn
            // 
            _sizeColumn.Text = "Size";
            _sizeColumn.TextAlign = HorizontalAlignment.Right;
            _sizeColumn.Width = 100;
            // 
            // _percentageColumn
            // 
            _percentageColumn.Text = "%";
            _percentageColumn.TextAlign = HorizontalAlignment.Right;
            _percentageColumn.Width = 75;
            // 
            // _directoriesColumn
            // 
            _directoriesColumn.Text = "Folders";
            _directoriesColumn.TextAlign = HorizontalAlignment.Right;
            _directoriesColumn.Width = 100;
            // 
            // _filesColumn
            // 
            _filesColumn.Text = "Files";
            _filesColumn.TextAlign = HorizontalAlignment.Right;
            _filesColumn.Width = 100;
            // 
            // _splitContainer
            // 
            _splitContainer.Dock = DockStyle.Fill;
            _splitContainer.Location = new Point(0, 90);
            _splitContainer.Margin = new Padding(5);
            _splitContainer.Name = "_splitContainer";
            // 
            // _splitContainer.Panel1
            // 
            _splitContainer.Panel1.Controls.Add(_treeView);
            _splitContainer.Panel1.Controls.Add(_treeStatusStrip);
            // 
            // _splitContainer.Panel2
            // 
            _splitContainer.Panel2.Controls.Add(_listStatusStrip);
            _splitContainer.Panel2.Controls.Add(_listView);
            _splitContainer.Size = new Size(2305, 987);
            _splitContainer.SplitterDistance = 757;
            _splitContainer.SplitterWidth = 7;
            _splitContainer.TabIndex = 0;
            // 
            // _treeView
            // 
            _treeView.ContextMenuStrip = _treeViewContextMenu;
            _treeView.Dock = DockStyle.Fill;
            _treeView.FullRowSelect = true;
            _treeView.HideSelection = false;
            _treeView.Location = new Point(0, 0);
            _treeView.Margin = new Padding(10, 16, 10, 16);
            _treeView.Name = "_treeView";
            _treeView.Size = new Size(757, 948);
            _treeView.Sorted = true;
            _treeView.TabIndex = 0;
            _treeView.AfterSelect += _treeView_AfterSelect;
            _treeView.NodeMouseClick += _treeView_NodeMouseClick;
            // 
            // _treeViewContextMenu
            // 
            _treeViewContextMenu.ImageScalingSize = new Size(32, 32);
            _treeViewContextMenu.Name = "_treeViewContextMenu";
            _treeViewContextMenu.Size = new Size(61, 4);
            _treeViewContextMenu.Opening += _treeViewContextMenu_Opening;
            // 
            // _treeStatusStrip
            // 
            _treeStatusStrip.ImageScalingSize = new Size(28, 28);
            _treeStatusStrip.Items.AddRange(new ToolStripItem[] { _treeStatusLabel });
            _treeStatusStrip.Location = new Point(0, 948);
            _treeStatusStrip.Name = "_treeStatusStrip";
            _treeStatusStrip.Padding = new Padding(2, 0, 47, 0);
            _treeStatusStrip.Size = new Size(757, 39);
            _treeStatusStrip.SizingGrip = false;
            _treeStatusStrip.TabIndex = 1;
            _treeStatusStrip.Text = "statusStrip1";
            // 
            // _treeStatusLabel
            // 
            _treeStatusLabel.Name = "_treeStatusLabel";
            _treeStatusLabel.Size = new Size(708, 30);
            _treeStatusLabel.Spring = true;
            _treeStatusLabel.Text = "Open a folder to get started";
            _treeStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _listStatusStrip
            // 
            _listStatusStrip.ImageScalingSize = new Size(28, 28);
            _listStatusStrip.Items.AddRange(new ToolStripItem[] { _listNameStatusLabel, _listSizeStatusLabel, _listPercentageStatusLabel, _listFoldersStatusLabel, _listFilesStatusLabel, _listBytesStatusLabel, _listLastModifiedStatusLabel });
            _listStatusStrip.Location = new Point(0, 948);
            _listStatusStrip.Name = "_listStatusStrip";
            _listStatusStrip.Padding = new Padding(2, 0, 47, 0);
            _listStatusStrip.Size = new Size(1541, 39);
            _listStatusStrip.TabIndex = 1;
            _listStatusStrip.Text = "statusStrip2";
            // 
            // _listNameStatusLabel
            // 
            _listNameStatusLabel.AutoSize = false;
            _listNameStatusLabel.Name = "_listNameStatusLabel";
            _listNameStatusLabel.Size = new Size(37, 30);
            _listNameStatusLabel.Text = "name";
            _listNameStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _listSizeStatusLabel
            // 
            _listSizeStatusLabel.AutoSize = false;
            _listSizeStatusLabel.Name = "_listSizeStatusLabel";
            _listSizeStatusLabel.Size = new Size(26, 30);
            _listSizeStatusLabel.Text = "size";
            _listSizeStatusLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // _listPercentageStatusLabel
            // 
            _listPercentageStatusLabel.AutoSize = false;
            _listPercentageStatusLabel.Name = "_listPercentageStatusLabel";
            _listPercentageStatusLabel.Size = new Size(66, 30);
            _listPercentageStatusLabel.Text = "percentage";
            _listPercentageStatusLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // _listFoldersStatusLabel
            // 
            _listFoldersStatusLabel.AutoSize = false;
            _listFoldersStatusLabel.Name = "_listFoldersStatusLabel";
            _listFoldersStatusLabel.Size = new Size(43, 30);
            _listFoldersStatusLabel.Text = "folders";
            _listFoldersStatusLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // _listFilesStatusLabel
            // 
            _listFilesStatusLabel.AutoSize = false;
            _listFilesStatusLabel.Name = "_listFilesStatusLabel";
            _listFilesStatusLabel.Size = new Size(28, 30);
            _listFilesStatusLabel.Text = "files";
            _listFilesStatusLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // _listBytesStatusLabel
            // 
            _listBytesStatusLabel.AutoSize = false;
            _listBytesStatusLabel.Name = "_listBytesStatusLabel";
            _listBytesStatusLabel.Size = new Size(35, 30);
            _listBytesStatusLabel.Text = "bytes";
            _listBytesStatusLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // _listLastModifiedStatusLabel
            // 
            _listLastModifiedStatusLabel.Name = "_listLastModifiedStatusLabel";
            _listLastModifiedStatusLabel.Size = new Size(133, 30);
            _listLastModifiedStatusLabel.Text = "last modified";
            // 
            // _listView
            // 
            _listView.Columns.AddRange(new ColumnHeader[] { _nameColumn, _sizeColumn, _percentageColumn, _directoriesColumn, _filesColumn, _bytesColumn, _lastModifiedColumn });
            _listView.ContextMenuStrip = _listViewContextMenu;
            _listView.Dock = DockStyle.Fill;
            _listView.FullRowSelect = true;
            _listView.HideSelection = true;
            _listView.Location = new Point(0, 0);
            _listView.Margin = new Padding(10, 16, 10, 16);
            _listView.Name = "_listView";
            _listView.Size = new Size(1541, 987);
            _listView.TabIndex = 0;
            _listView.UseCompatibleStateImageBehavior = false;
            _listView.View = View.Details;
            _listView.ColumnWidthChanged += _listView_ColumnWidthChanged;
            _listView.DoubleClick += _listView_DoubleClick;
            _listView.KeyPress += _listView_KeyPress;
            // 
            // _bytesColumn
            // 
            _bytesColumn.Text = "Bytes";
            _bytesColumn.TextAlign = HorizontalAlignment.Right;
            _bytesColumn.Width = 150;
            // 
            // _lastModifiedColumn
            // 
            _lastModifiedColumn.Text = "Last modified";
            _lastModifiedColumn.Width = 150;
            // 
            // _listViewContextMenu
            // 
            _listViewContextMenu.ImageScalingSize = new Size(32, 32);
            _listViewContextMenu.Items.AddRange(new ToolStripItem[] { _openFolderMenuItem, _openItemMenuItem, _updateMenuItem, _separatorUpdate, _copyMenuItem, _pasteMenuItem, _cutMenuItem, _newFileMenuItem, _newFolderMenuItem, _separatorDelete, _deleteMenuItem, _separatorView, _renameMenuItem, _viewMenuItem });
            _listViewContextMenu.Name = "_treeViewContextMenu";
            _listViewContextMenu.Size = new Size(295, 418);
            _listViewContextMenu.Opening += _listViewContextMenu_Opening;
            // 
            // _openFolderMenuItem
            // 
            _openFolderMenuItem.Name = "_openFolderMenuItem";
            _openFolderMenuItem.Size = new Size(294, 36);
            _openFolderMenuItem.Text = "&Abrir Diretório";
            _openFolderMenuItem.Visible = false;
            _openFolderMenuItem.Click += _openFolderMenuItem_Click;
            // 
            // _openItemMenuItem
            // 
            _openItemMenuItem.Name = "_openItemMenuItem";
            _openItemMenuItem.Size = new Size(294, 36);
            _openItemMenuItem.Text = "&Abrir";
            _openItemMenuItem.Visible = false;
            _openItemMenuItem.Click += _openItemMenuItem_Click;
            // 
            // _updateMenuItem
            // 
            _updateMenuItem.Name = "_updateMenuItem";
            _updateMenuItem.ShortcutKeys = Keys.F5;
            _updateMenuItem.Size = new Size(294, 36);
            _updateMenuItem.Text = "&Atualizar";
            _updateMenuItem.Click += _refreshCurrentFolder_Click;
            // 
            // _separatorUpdate
            // 
            _separatorUpdate.Name = "_separatorUpdate";
            _separatorUpdate.Size = new Size(291, 6);
            // 
            // _copyMenuItem
            // 
            _copyMenuItem.Name = "_copyMenuItem";
            _copyMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            _copyMenuItem.Size = new Size(294, 36);
            _copyMenuItem.Text = "&Copiar";
            _copyMenuItem.Click += _copyMenuItem_Click;
            // 
            // _pasteMenuItem
            // 
            _pasteMenuItem.Name = "_pasteMenuItem";
            _pasteMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            _pasteMenuItem.Size = new Size(294, 36);
            _pasteMenuItem.Text = "&Colar";
            _pasteMenuItem.Visible = false;
            _pasteMenuItem.Click += _pasteMenuItem_Click;
            // 
            // _cutMenuItem
            // 
            _cutMenuItem.Name = "_cutMenuItem";
            _cutMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            _cutMenuItem.Size = new Size(294, 36);
            _cutMenuItem.Text = "&Recortar";
            _cutMenuItem.Click += _cutMenuItem_Click;
            // 
            // _newFileMenuItem
            // 
            _newFileMenuItem.Image = (Image)resources.GetObject("_newFileMenuItem.Image");
            _newFileMenuItem.ImageScaling = ToolStripItemImageScaling.None;
            _newFileMenuItem.Name = "_newFileMenuItem";
            _newFileMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            _newFileMenuItem.Size = new Size(294, 36);
            _newFileMenuItem.Text = "&Novo arquivo";
            _newFileMenuItem.Click += _newFileMenuItem_Click;
            // 
            // _newFolderMenuItem
            // 
            _newFolderMenuItem.Image = (Image)resources.GetObject("_newFolderMenuItem.Image");
            _newFolderMenuItem.ImageScaling = ToolStripItemImageScaling.None;
            _newFolderMenuItem.Name = "_newFolderMenuItem";
            _newFolderMenuItem.ShortcutKeys = Keys.Control | Keys.D;
            _newFolderMenuItem.Size = new Size(294, 36);
            _newFolderMenuItem.Text = "&Novo diretório";
            _newFolderMenuItem.Click += _newFolderMenuItem_Click;
            // 
            // _separatorDelete
            // 
            _separatorDelete.Name = "_separatorDelete";
            _separatorDelete.Size = new Size(291, 6);
            // 
            // _deleteMenuItem
            // 
            _deleteMenuItem.Image = (Image)resources.GetObject("_deleteMenuItem.Image");
            _deleteMenuItem.ImageScaling = ToolStripItemImageScaling.None;
            _deleteMenuItem.Name = "_deleteMenuItem";
            _deleteMenuItem.ShortcutKeys = Keys.Delete;
            _deleteMenuItem.Size = new Size(294, 36);
            _deleteMenuItem.Text = "&Excluir";
            _deleteMenuItem.Click += _deleteMenuItem_Click;
            // 
            // _separatorView
            // 
            _separatorView.Name = "_separatorView";
            _separatorView.Size = new Size(291, 6);
            // 
            // _renameMenuItem
            // 
            _renameMenuItem.Name = "_renameMenuItem";
            _renameMenuItem.ShortcutKeys = Keys.F2;
            _renameMenuItem.Size = new Size(294, 36);
            _renameMenuItem.Text = "&Renomear";
            _renameMenuItem.Click += _renameMenuItem_Click;
            // 
            // _viewMenuItem
            // 
            _viewMenuItem.DropDownItems.AddRange(new ToolStripItem[] { _viewSmallIconMenuItem, _viewBigIconMenuItem, _viewListIconMenuItem, _viewDetailsIconMenuItem });
            _viewMenuItem.Name = "_viewMenuItem";
            _viewMenuItem.Size = new Size(294, 36);
            _viewMenuItem.Text = "&Visualizar";
            _viewMenuItem.Click += _viewMenuItem_Click;
            // 
            // _viewSmallIconMenuItem
            // 
            _viewSmallIconMenuItem.Name = "_viewSmallIconMenuItem";
            _viewSmallIconMenuItem.Size = new Size(308, 40);
            _viewSmallIconMenuItem.Text = "&Arquivos Pequenos";
            _viewSmallIconMenuItem.Click += _viewSmallIconMenuItem_Click;
            // 
            // _viewBigIconMenuItem
            // 
            _viewBigIconMenuItem.Name = "_viewBigIconMenuItem";
            _viewBigIconMenuItem.Size = new Size(308, 40);
            _viewBigIconMenuItem.Text = "&Arquivos Grandes";
            _viewBigIconMenuItem.Click += _viewBigIconMenuItem_Click;
            // 
            // _viewListIconMenuItem
            // 
            _viewListIconMenuItem.Name = "_viewListIconMenuItem";
            _viewListIconMenuItem.Size = new Size(308, 40);
            _viewListIconMenuItem.Text = "&Lista";
            _viewListIconMenuItem.Click += _viewListIconMenuItem_Click;
            // 
            // _viewDetailsIconMenuItem
            // 
            _viewDetailsIconMenuItem.Name = "_viewDetailsIconMenuItem";
            _viewDetailsIconMenuItem.Size = new Size(308, 40);
            _viewDetailsIconMenuItem.Text = "&Detalhes";
            _viewDetailsIconMenuItem.Click += _viewDetailsIconMenuItem_Click;
            // 
            // _panelLoading
            // 
            _panelLoading.BackColor = SystemColors.Menu;
            _panelLoading.BackgroundImageLayout = ImageLayout.None;
            _panelLoading.BorderStyle = BorderStyle.FixedSingle;
            _panelLoading.Controls.Add(_progressBarLoading);
            _panelLoading.Controls.Add(_lblInitLoading);
            _panelLoading.Location = new Point(528, 278);
            _panelLoading.Name = "_panelLoading";
            _panelLoading.Size = new Size(681, 189);
            _panelLoading.TabIndex = 5;
            // 
            // _progressBarLoading
            // 
            _progressBarLoading.Location = new Point(22, 124);
            _progressBarLoading.Name = "_progressBarLoading";
            _progressBarLoading.Size = new Size(636, 40);
            _progressBarLoading.TabIndex = 2;
            _progressBarLoading.Visible = false;
            // 
            // _lblInitLoading
            // 
            _lblInitLoading.AutoSize = true;
            _lblInitLoading.ImageAlign = ContentAlignment.MiddleLeft;
            _lblInitLoading.Location = new Point(22, 51);
            _lblInitLoading.Name = "_lblInitLoading";
            _lblInitLoading.Size = new Size(0, 30);
            _lblInitLoading.TabIndex = 1;
            _lblInitLoading.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // _showDirInExplorerMenuItem
            // 
            _showDirInExplorerMenuItem.Name = "_showDirInExplorerMenuItem";
            _showDirInExplorerMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
            _showDirInExplorerMenuItem.Size = new Size(329, 32);
            _showDirInExplorerMenuItem.Text = "Show in &Explorer";
            _showDirInExplorerMenuItem.Click += _showDirInExplorerMenuItem_Click;
            // 
            // _openDirInExplorerMenuItem
            // 
            _openDirInExplorerMenuItem.Name = "_openDirInExplorerMenuItem";
            _openDirInExplorerMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.O;
            _openDirInExplorerMenuItem.Size = new Size(329, 32);
            _openDirInExplorerMenuItem.Text = "&Open in Explorer";
            _openDirInExplorerMenuItem.Click += _openDirInExplorerMenuItem_Click;
            // 
            // _showItemInExplorerMenuItem
            // 
            _showItemInExplorerMenuItem.Name = "_showItemInExplorerMenuItem";
            _showItemInExplorerMenuItem.Size = new Size(216, 32);
            _showItemInExplorerMenuItem.Text = "Show in &Explorer";
            _showItemInExplorerMenuItem.Click += showItemInExplorerMenuItem_Click;
            // 
            // _openItemInExplorerMenuItem
            // 
            _openItemInExplorerMenuItem.Name = "_openItemInExplorerMenuItem";
            _openItemInExplorerMenuItem.Size = new Size(216, 32);
            _openItemInExplorerMenuItem.Text = "&Open in Explorer";
            _openItemInExplorerMenuItem.Click += openItemInExplorerMenuItem_Click;
            // 
            // _topStatusStrip
            // 
            _topStatusStrip.Dock = DockStyle.Top;
            _topStatusStrip.ImageScalingSize = new Size(28, 28);
            _topStatusStrip.Items.AddRange(new ToolStripItem[] { _refreshToolButton, _upToolButton, toolStripDropDownButton1 });
            _topStatusStrip.Location = new Point(0, 0);
            _topStatusStrip.Name = "_topStatusStrip";
            _topStatusStrip.Padding = new Padding(2, 0, 23, 0);
            _topStatusStrip.ShowItemToolTips = true;
            _topStatusStrip.Size = new Size(2305, 45);
            _topStatusStrip.SizingGrip = false;
            _topStatusStrip.TabIndex = 2;
            // 
            // _refreshToolButton
            // 
            _refreshToolButton.Enabled = false;
            _refreshToolButton.Image = Resources.refresh;
            _refreshToolButton.ImageTransparentColor = Color.Magenta;
            _refreshToolButton.Margin = new Padding(0, 3, 0, 0);
            _refreshToolButton.Name = "_refreshToolButton";
            _refreshToolButton.Padding = new Padding(4);
            _refreshToolButton.ShowDropDownArrow = false;
            _refreshToolButton.Size = new Size(135, 42);
            _refreshToolButton.Text = "Atualizar";
            _refreshToolButton.ToolTipText = "Refresh this folder";
            _refreshToolButton.Click += _refreshToolButton_Click;
            // 
            // _upToolButton
            // 
            _upToolButton.Alignment = ToolStripItemAlignment.Right;
            _upToolButton.Enabled = false;
            _upToolButton.Image = (Image)resources.GetObject("_upToolButton.Image");
            _upToolButton.ImageTransparentColor = Color.Magenta;
            _upToolButton.Margin = new Padding(0, 3, 0, 0);
            _upToolButton.Name = "_upToolButton";
            _upToolButton.Overflow = ToolStripItemOverflow.Never;
            _upToolButton.Padding = new Padding(4);
            _upToolButton.ShowDropDownArrow = false;
            _upToolButton.Size = new Size(166, 42);
            _upToolButton.Text = "Pasta Acima";
            _upToolButton.ToolTipText = "Go to parent folder";
            _upToolButton.Click += _upToolButton_Click;
            // 
            // toolStripDropDownButton1
            // 
            toolStripDropDownButton1.Image = Resources.search;
            toolStripDropDownButton1.ImageTransparentColor = Color.Magenta;
            toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            toolStripDropDownButton1.Size = new Size(149, 41);
            toolStripDropDownButton1.Text = "Pesquisar";
            toolStripDropDownButton1.Click += toolStripDropDownButton1_Click;
            // 
            // _loadingStatusStrip
            // 
            _loadingStatusStrip.Dock = DockStyle.Top;
            _loadingStatusStrip.ImageScalingSize = new Size(28, 28);
            _loadingStatusStrip.Items.AddRange(new ToolStripItem[] { _cancelButton, _loadingLabel });
            _loadingStatusStrip.Location = new Point(0, 45);
            _loadingStatusStrip.Name = "_loadingStatusStrip";
            _loadingStatusStrip.Padding = new Padding(2, 0, 23, 0);
            _loadingStatusStrip.ShowItemToolTips = true;
            _loadingStatusStrip.Size = new Size(2305, 45);
            _loadingStatusStrip.SizingGrip = false;
            _loadingStatusStrip.TabIndex = 4;
            _loadingStatusStrip.Text = "Loading...";
            // 
            // _cancelButton
            // 
            _cancelButton.Image = Resources.pause;
            _cancelButton.ImageTransparentColor = Color.Magenta;
            _cancelButton.Margin = new Padding(0, 3, 0, 0);
            _cancelButton.Name = "_cancelButton";
            _cancelButton.Padding = new Padding(4);
            _cancelButton.ShowDropDownArrow = false;
            _cancelButton.Size = new Size(114, 42);
            _cancelButton.Text = "Pausar";
            _cancelButton.ToolTipText = "Pausar load de pastas...";
            _cancelButton.Click += _cancelButton_Click;
            // 
            // _loadingLabel
            // 
            _loadingLabel.Name = "_loadingLabel";
            _loadingLabel.Size = new Size(2166, 36);
            _loadingLabel.Spring = true;
            _loadingLabel.Text = "...";
            _loadingLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(168F, 168F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(2305, 1077);
            Controls.Add(_panelLoading);
            Controls.Add(_splitContainer);
            Controls.Add(_loadingStatusStrip);
            Controls.Add(_topStatusStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5);
            Name = "MainForm";
            Text = "TCloud Explorer";
            Load += Form_Load;
            _splitContainer.Panel1.ResumeLayout(false);
            _splitContainer.Panel1.PerformLayout();
            _splitContainer.Panel2.ResumeLayout(false);
            _splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)_splitContainer).EndInit();
            _splitContainer.ResumeLayout(false);
            _treeStatusStrip.ResumeLayout(false);
            _treeStatusStrip.PerformLayout();
            _listStatusStrip.ResumeLayout(false);
            _listStatusStrip.PerformLayout();
            _listViewContextMenu.ResumeLayout(false);
            _panelLoading.ResumeLayout(false);
            _panelLoading.PerformLayout();
            _topStatusStrip.ResumeLayout(false);
            _topStatusStrip.PerformLayout();
            _loadingStatusStrip.ResumeLayout(false);
            _loadingStatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private SplitContainer _splitContainer;
    private TreeView _treeView;
    private ContextMenuStrip _treeViewContextMenu;
    private ToolStripMenuItem _showDirInExplorerMenuItem;
    private ToolStripMenuItem _openDirInExplorerMenuItem;
    /** Begin Devops Code **/
    private ToolStripMenuItem _newFileMenuItem;
    private ToolStripMenuItem _newFolderMenuItem;
    private ToolStripMenuItem _deleteMenuItem;
    private ToolStripSeparator _separatorDelete;
    private ToolStripSeparator _separatorUpdate;
    private ToolStripSeparator _separatorView;
    private ToolStripMenuItem _updateMenuItem;
    private ToolStripMenuItem _openFolderMenuItem;
    private ToolStripMenuItem _copyMenuItem;
    private ToolStripMenuItem _openItemMenuItem;
    private ToolStripMenuItem _cutMenuItem;
    private ToolStripMenuItem _pasteMenuItem;
    private ToolStripMenuItem _renameMenuItem;
    private ToolStripMenuItem _viewMenuItem;
   /** Sub Menus **/
    private ToolStripMenuItem _viewListIconMenuItem;
    private ToolStripMenuItem _viewDetailsIconMenuItem;
    private ToolStripMenuItem _viewBigIconMenuItem;
    private ToolStripMenuItem _viewSmallIconMenuItem;
    /** END Devops Code **/
    private ContextMenuStrip _listViewContextMenu;
    private ToolStripMenuItem _showItemInExplorerMenuItem;
    private ToolStripMenuItem _openItemInExplorerMenuItem;
    private StatusStrip _treeStatusStrip;
    private StatusStrip _listStatusStrip;
    private ToolStripStatusLabel _treeStatusLabel;
    private ToolStripStatusLabel _listNameStatusLabel;
    private ToolStripStatusLabel _listSizeStatusLabel;
    private ToolStripStatusLabel _listPercentageStatusLabel;
    private ToolStripStatusLabel _listFoldersStatusLabel;
    private ToolStripStatusLabel _listFilesStatusLabel;
    private ToolStripStatusLabel _listBytesStatusLabel;
    private StatusStrip _topStatusStrip;
    private ToolStripDropDownButton _refreshToolButton;
    private StatusStrip _loadingStatusStrip;
    private ToolStripDropDownButton _cancelButton;
    private ToolStripStatusLabel _loadingLabel;
    private ToolStripDropDownButton _upToolButton;
    private ToolStripStatusLabel _listLastModifiedStatusLabel;
    public ListView _listView;
    private ColumnHeader _bytesColumn;
    private ColumnHeader _lastModifiedColumn;
    private ToolStripDropDownButton toolStripDropDownButton1;
    private Panel _panelLoading;
    private Label _lblInitLoading;
    private ProgressBar _progressBarLoading;
    }
}