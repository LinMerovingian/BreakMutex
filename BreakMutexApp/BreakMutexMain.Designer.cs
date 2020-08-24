namespace BreakMutexApp
{
    partial class BreakMutexMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
			this.label1 = new System.Windows.Forms.Label();
			this.GridProcessList = new System.Windows.Forms.DataGridView();
			this.btnReload = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.btnBreak = new System.Windows.Forms.Button();
			this.txtMutexName = new System.Windows.Forms.TextBox();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.label3 = new System.Windows.Forms.Label();
			this.txtSearchName = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.GridProcessList)).BeginInit();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 17);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "Process List";
			// 
			// GridProcessList
			// 
			this.GridProcessList.AllowUserToAddRows = false;
			this.GridProcessList.AllowUserToDeleteRows = false;
			this.GridProcessList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.GridProcessList.Location = new System.Drawing.Point(12, 41);
			this.GridProcessList.MultiSelect = false;
			this.GridProcessList.Name = "GridProcessList";
			this.GridProcessList.ReadOnly = true;
			this.GridProcessList.RowHeadersVisible = false;
			this.GridProcessList.RowTemplate.Height = 21;
			this.GridProcessList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.GridProcessList.Size = new System.Drawing.Size(703, 318);
			this.GridProcessList.TabIndex = 4;
			// 
			// btnReload
			// 
			this.btnReload.Location = new System.Drawing.Point(640, 12);
			this.btnReload.Name = "btnReload";
			this.btnReload.Size = new System.Drawing.Size(75, 23);
			this.btnReload.TabIndex = 3;
			this.btnReload.Text = "Reload";
			this.btnReload.UseVisualStyleBackColor = true;
			this.btnReload.Click += new System.EventHandler(this.BtnReload_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label2.Location = new System.Drawing.Point(215, 370);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "Mutex name";
			// 
			// btnBreak
			// 
			this.btnBreak.Location = new System.Drawing.Point(640, 365);
			this.btnBreak.Name = "btnBreak";
			this.btnBreak.Size = new System.Drawing.Size(75, 23);
			this.btnBreak.TabIndex = 7;
			this.btnBreak.Text = "Break";
			this.btnBreak.UseVisualStyleBackColor = true;
			this.btnBreak.Click += new System.EventHandler(this.BtnBreak_Click);
			// 
			// txtMutexName
			// 
			this.txtMutexName.Location = new System.Drawing.Point(288, 367);
			this.txtMutexName.Name = "txtMutexName";
			this.txtMutexName.Size = new System.Drawing.Size(346, 19);
			this.txtMutexName.TabIndex = 6;
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
			this.statusStrip1.Location = new System.Drawing.Point(0, 394);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(727, 22);
			this.statusStrip1.SizingGrip = false;
			this.statusStrip1.TabIndex = 8;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// lblStatus
			// 
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(118, 17);
			this.lblStatus.Text = "toolStripStatusLabel1";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.label3.Location = new System.Drawing.Point(167, 17);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(115, 12);
			this.label3.TabIndex = 1;
			this.label3.Text = "Search process name";
			// 
			// txtSearchName
			// 
			this.txtSearchName.Location = new System.Drawing.Point(288, 14);
			this.txtSearchName.Name = "txtSearchName";
			this.txtSearchName.Size = new System.Drawing.Size(346, 19);
			this.txtSearchName.TabIndex = 2;
			// 
			// BreakMutexMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(727, 416);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.txtSearchName);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.txtMutexName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.btnBreak);
			this.Controls.Add(this.btnReload);
			this.Controls.Add(this.GridProcessList);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Margin = new System.Windows.Forms.Padding(1, 2, 1, 2);
			this.MaximizeBox = false;
			this.Name = "BreakMutexMain";
			this.Text = "BreakMutex";
			this.Load += new System.EventHandler(this.BreakMutexMain_Load);
			((System.ComponentModel.ISupportInitialize)(this.GridProcessList)).EndInit();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView GridProcessList;
        private System.Windows.Forms.Button btnReload;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBreak;
        private System.Windows.Forms.TextBox txtMutexName;
		private System.Windows.Forms.StatusStrip statusStrip1;
		private System.Windows.Forms.ToolStripStatusLabel lblStatus;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtSearchName;
	}
}

