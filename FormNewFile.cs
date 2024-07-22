using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TCLoudExplorer.@class;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TCloudExplorer
{
    public partial class FormNewFile : Form
    {
        private string selectedFolder = "";
        public event EventHandler<ItemCreatedEventArgs> FileCreated;

        public FormNewFile()
        {
            InitializeComponent();
        }

        public FormNewFile(string selectedFolder)
        {
            InitializeComponent();
            this.selectedFolder = selectedFolder;
        }

        private void _btnCancelNewFile_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void _btnOkNewFile_Click(object sender, EventArgs e)
        {
            string newFileName = _txtNewFile.Text;
            string newFilePath = Path.Combine(this.selectedFolder, newFileName);

            if (!Utils.IsValidFileName(newFileName))
            {
                MessageBox.Show("O nome do arquivo não pode ser nulo ou conter caracteres especiais:\r\n" + "\t\\/:*?\"<>|,", "Nome Inválido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (File.Exists(newFilePath))
            {
                MessageBox.Show("Existe um arquivo com o mesmo nome no caminho atual！", "Arquivo Já Existente", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (!Path.HasExtension(newFileName))
            {
                MessageBox.Show("Não é possível criar um arquivo sem extensão!", "Formato Incorreto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                using (FileStream fs = File.Create(newFilePath)) {}
                this.Close();
                FileCreated?.Invoke(this, new ItemCreatedEventArgs(newFileName, this.selectedFolder));
            }
        }
    }
}
