namespace InfoReader.Window
{
    partial class FormatEditor
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
            this.btn_save = new System.Windows.Forms.Button();
            this.txBox_formatEditor = new ICSharpCode.TextEditor.TextEditorControl();
            this.SuspendLayout();
            // 
            // btn_save
            // 
            this.btn_save.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_save.Location = new System.Drawing.Point(510, 381);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(194, 57);
            this.btn_save.TabIndex = 1;
            this.btn_save.Text = "SaveElement";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // txBox_formatEditor
            // 
            this.txBox_formatEditor.IsReadOnly = false;
            this.txBox_formatEditor.Location = new System.Drawing.Point(13, 13);
            this.txBox_formatEditor.Name = "txBox_formatEditor";
            this.txBox_formatEditor.ShowLineNumbers = false;
            this.txBox_formatEditor.Size = new System.Drawing.Size(691, 362);
            this.txBox_formatEditor.TabIndex = 2;
            // 
            // FormatEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 450);
            this.Controls.Add(this.txBox_formatEditor);
            this.Controls.Add(this.btn_save);
            this.Name = "FormatEditor";
            this.Text = "FormatEditor";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btn_save;
        private ICSharpCode.TextEditor.TextEditorControl txBox_formatEditor;
    }
}