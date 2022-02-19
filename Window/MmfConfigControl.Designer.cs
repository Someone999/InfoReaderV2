namespace InfoReader.Window
{
    partial class MmfConfigControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_mmfNameTag = new System.Windows.Forms.Label();
            this.chkBox_enabled = new System.Windows.Forms.CheckBox();
            this.txBox_mmfName = new System.Windows.Forms.TextBox();
            this.lst_status = new System.Windows.Forms.CheckedListBox();
            this.lb_statusTag = new System.Windows.Forms.Label();
            this.lb_modeTag = new System.Windows.Forms.Label();
            this.lst_mode = new System.Windows.Forms.CheckedListBox();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_configFormat = new System.Windows.Forms.Button();
            this.txBox_interval = new System.Windows.Forms.TextBox();
            this.lb_intervalTag = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lb_mmfNameTag
            // 
            this.lb_mmfNameTag.AutoSize = true;
            this.lb_mmfNameTag.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lb_mmfNameTag.Location = new System.Drawing.Point(3, 5);
            this.lb_mmfNameTag.Name = "lb_mmfNameTag";
            this.lb_mmfNameTag.Size = new System.Drawing.Size(116, 17);
            this.lb_mmfNameTag.TabIndex = 0;
            this.lb_mmfNameTag.Text = "MmfNameElement";
            // 
            // chkBox_enabled
            // 
            this.chkBox_enabled.AutoSize = true;
            this.chkBox_enabled.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.chkBox_enabled.Location = new System.Drawing.Point(331, 5);
            this.chkBox_enabled.Name = "chkBox_enabled";
            this.chkBox_enabled.Size = new System.Drawing.Size(120, 21);
            this.chkBox_enabled.TabIndex = 2;
            this.chkBox_enabled.Text = "EnabledElement";
            this.chkBox_enabled.UseVisualStyleBackColor = true;
            // 
            // txBox_mmfName
            // 
            this.txBox_mmfName.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txBox_mmfName.Location = new System.Drawing.Point(125, 3);
            this.txBox_mmfName.Name = "txBox_mmfName";
            this.txBox_mmfName.Size = new System.Drawing.Size(199, 23);
            this.txBox_mmfName.TabIndex = 3;
            // 
            // lst_status
            // 
            this.lst_status.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lst_status.FormattingEnabled = true;
            this.lst_status.Location = new System.Drawing.Point(3, 65);
            this.lst_status.Name = "lst_status";
            this.lst_status.Size = new System.Drawing.Size(216, 58);
            this.lst_status.TabIndex = 4;
            this.lst_status.Leave += new System.EventHandler(this.lst_status_Leave);
            // 
            // lb_statusTag
            // 
            this.lb_statusTag.AutoSize = true;
            this.lb_statusTag.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lb_statusTag.Location = new System.Drawing.Point(3, 33);
            this.lb_statusTag.Name = "lb_statusTag";
            this.lb_statusTag.Size = new System.Drawing.Size(89, 17);
            this.lb_statusTag.TabIndex = 5;
            this.lb_statusTag.Text = "StatusElement";
            // 
            // lb_modeTag
            // 
            this.lb_modeTag.AutoSize = true;
            this.lb_modeTag.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lb_modeTag.Location = new System.Drawing.Point(222, 33);
            this.lb_modeTag.Name = "lb_modeTag";
            this.lb_modeTag.Size = new System.Drawing.Size(89, 17);
            this.lb_modeTag.TabIndex = 6;
            this.lb_modeTag.Text = "ModeElement";
            // 
            // lst_mode
            // 
            this.lst_mode.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lst_mode.FormattingEnabled = true;
            this.lst_mode.Location = new System.Drawing.Point(225, 65);
            this.lst_mode.Name = "lst_mode";
            this.lst_mode.Size = new System.Drawing.Size(216, 58);
            this.lst_mode.TabIndex = 7;
            this.lst_mode.Leave += new System.EventHandler(this.lst_mode_Leave);
            // 
            // btn_save
            // 
            this.btn_save.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_save.Location = new System.Drawing.Point(191, 145);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(120, 47);
            this.btn_save.TabIndex = 8;
            this.btn_save.Text = "SaveElement";
            this.btn_save.UseVisualStyleBackColor = true;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_configFormat
            // 
            this.btn_configFormat.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_configFormat.Location = new System.Drawing.Point(314, 145);
            this.btn_configFormat.Name = "btn_configFormat";
            this.btn_configFormat.Size = new System.Drawing.Size(127, 47);
            this.btn_configFormat.TabIndex = 9;
            this.btn_configFormat.Text = "FormatEditElement";
            this.btn_configFormat.UseVisualStyleBackColor = true;
            this.btn_configFormat.Click += new System.EventHandler(this.btn_configFormat_Click);
            // 
            // txBox_interval
            // 
            this.txBox_interval.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.txBox_interval.Location = new System.Drawing.Point(6, 165);
            this.txBox_interval.Name = "txBox_interval";
            this.txBox_interval.Size = new System.Drawing.Size(100, 23);
            this.txBox_interval.TabIndex = 10;
            this.txBox_interval.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txBox_interval_KeyPress);
            // 
            // lb_intervalTag
            // 
            this.lb_intervalTag.AutoSize = true;
            this.lb_intervalTag.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.lb_intervalTag.Location = new System.Drawing.Point(7, 140);
            this.lb_intervalTag.Name = "lb_intervalTag";
            this.lb_intervalTag.Size = new System.Drawing.Size(99, 17);
            this.lb_intervalTag.TabIndex = 11;
            this.lb_intervalTag.Text = "InvervalElement";
            // 
            // MmfConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.lb_intervalTag);
            this.Controls.Add(this.txBox_interval);
            this.Controls.Add(this.btn_configFormat);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.lst_mode);
            this.Controls.Add(this.lb_modeTag);
            this.Controls.Add(this.lb_statusTag);
            this.Controls.Add(this.lst_status);
            this.Controls.Add(this.txBox_mmfName);
            this.Controls.Add(this.chkBox_enabled);
            this.Controls.Add(this.lb_mmfNameTag);
            this.Name = "MmfConfigControl";
            this.Size = new System.Drawing.Size(443, 197);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_mmfNameTag;
        private System.Windows.Forms.CheckBox chkBox_enabled;
        private System.Windows.Forms.TextBox txBox_mmfName;
        private System.Windows.Forms.CheckedListBox lst_status;
        private System.Windows.Forms.Label lb_statusTag;
        private System.Windows.Forms.Label lb_modeTag;
        private System.Windows.Forms.CheckedListBox lst_mode;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_configFormat;
        private System.Windows.Forms.TextBox txBox_interval;
        private System.Windows.Forms.Label lb_intervalTag;
    }
}
