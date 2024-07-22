using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using TCloudExplorer;
using TCLoudExplorer;
using TCLoudExplorer.@class;

namespace TCloudExplorer
{
    public partial class FormNewFolder : Form
    {
        private String selectedFolder;
        public EventHandler<ItemCreatedEventArgs> FolderCreated;

        private class IconsIndexes
        {
            public const int FixedDrive = 0;
            public const int CDRom = 1;
            public const int RemovableDisk = 2;
            public const int Folder = 3;
            public const int RecentFiles = 4;
        }

        public FormNewFolder(String selectedFolder)
        {
            InitializeComponent();
            this.selectedFolder = selectedFolder;
        }

        private void _btnCancelNewFolder_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _btnOkNewFolder_Click(object sender, EventArgs e)
        {
            string newFolderName = _txtNewFolder.Text;
            string newFolderFullPath = Path.Combine(this.selectedFolder, newFolderName);

            if (!Utils.IsValidFileName(newFolderName))
            {
                MessageBox.Show("O nome do diretório não pode ser nulo ou conter caracteres especiais:\r\n" + "\t\\/:*?\"<>|,", "Nome Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (Directory.Exists(newFolderFullPath))
            {
                MessageBox.Show("Existe um diretório com o mesmo nome no caminho atual！", "Arquivo Já Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Directory.CreateDirectory(newFolderFullPath);
                this.Close();
                FolderCreated?.Invoke(this, new ItemCreatedEventArgs(newFolderName, this.selectedFolder));
            }
        }
    }
}
