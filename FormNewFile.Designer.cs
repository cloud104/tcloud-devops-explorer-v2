namespace TCloudExplorer
{
    partial class FormNewFile
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
            label1 = new Label();
            _txtNewFile = new TextBox();
            _btnOkNewFile = new Button();
            _btnCancelNewFile = new Button();
            SuspendLayout();

            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 20);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(188, 30);
            label1.TabIndex = 0;
            label1.Text = "Criar novo arquivo:";
            label1.Click += label1_Click;
            // 
            // _txtNewFile
            // 
            _txtNewFile.Location = new Point(20, 56);
            _txtNewFile.Margin = new Padding(4);
            _txtNewFile.Name = "_txtNewFile";
            _txtNewFile.Size = new Size(317, 35);
            _txtNewFile.TabIndex = 1;
            // 
            // _btnOkNewFile
            // 
            _btnOkNewFile.Location = new Point(208, 110);
            _btnOkNewFile.Margin = new Padding(4);
            _btnOkNewFile.Name = "_btnOkNewFile";
            _btnOkNewFile.Size = new Size(131, 41);
            _btnOkNewFile.TabIndex = 2;
            _btnOkNewFile.Text = "OK";
            _btnOkNewFile.UseVisualStyleBackColor = true;
            _btnOkNewFile.Click += _btnOkNewFile_Click;
            // 
            // _btnCancelNewFile
            // 
            _btnCancelNewFile.Location = new Point(20, 110);
            _btnCancelNewFile.Margin = new Padding(4);
            _btnCancelNewFile.Name = "_btnCancelNewFile";
            _btnCancelNewFile.Size = new Size(127, 41);
            _btnCancelNewFile.TabIndex = 3;
            _btnCancelNewFile.Text = "Cancelar";
            _btnCancelNewFile.UseVisualStyleBackColor = true;
            _btnCancelNewFile.Click += _btnCancelNewFile_Click;
            // 
            // FormNewFile
            // 
            AcceptButton = _btnOkNewFile;
            AutoScaleDimensions = new SizeF(12F, 30F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(353, 170);
            Controls.Add(_btnCancelNewFile);
            Controls.Add(_btnOkNewFile);
            Controls.Add(_txtNewFile);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(4);
            Name = "FormNewFile";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Novo arquivo";
            Load += FormNewFile_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox _txtNewFile;
        private Button _btnOkNewFile;
        private Button _btnCancelNewFile;
    }
}