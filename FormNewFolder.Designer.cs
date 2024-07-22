namespace TCloudExplorer
{
    partial class FormNewFolder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _btnCancelNewFolder = new Button();
            _btnOkNewFolder = new Button();
            _txtNewFolder = new TextBox();
            label1 = new Label();
            SuspendLayout();
            // 
            // _btnCancelNewFolder
            // 
            _btnCancelNewFolder.Location = new Point(16, 108);
            _btnCancelNewFolder.Margin = new Padding(4);
            _btnCancelNewFolder.Name = "_btnCancelNewFolder";
            _btnCancelNewFolder.Size = new Size(127, 41);
            _btnCancelNewFolder.TabIndex = 7;
            _btnCancelNewFolder.Text = "Cancelar";
            _btnCancelNewFolder.UseVisualStyleBackColor = true;
            _btnCancelNewFolder.Click += _btnCancelNewFolder_Click;
            // 
            // _btnOkNewFolder
            // 
            _btnOkNewFolder.Location = new Point(204, 108);
            _btnOkNewFolder.Margin = new Padding(4);
            _btnOkNewFolder.Name = "_btnOkNewFolder";
            _btnOkNewFolder.Size = new Size(131, 41);
            _btnOkNewFolder.TabIndex = 6;
            _btnOkNewFolder.Text = "OK";
            _btnOkNewFolder.UseVisualStyleBackColor = true;
            _btnOkNewFolder.Click += _btnOkNewFolder_Click;
            // 
            // _txtNewFolder
            // 
            _txtNewFolder.Location = new Point(16, 54);
            _txtNewFolder.Margin = new Padding(4);
            _txtNewFolder.Name = "_txtNewFolder";
            _txtNewFolder.Size = new Size(317, 35);
            _txtNewFolder.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 18);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(168, 30);
            label1.TabIndex = 4;
            label1.Text = "Criar nova pasta:";
            // 
            // FormNewFolder
            // 
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(364, 172);
            Controls.Add(_btnCancelNewFolder);
            Controls.Add(_btnOkNewFolder);
            Controls.Add(_txtNewFolder);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormNewFolder";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormNewFolder";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button _btnCancelNewFolder;
        private Button _btnOkNewFolder;
        private TextBox _txtNewFolder;
        private Label label1;
    }
}