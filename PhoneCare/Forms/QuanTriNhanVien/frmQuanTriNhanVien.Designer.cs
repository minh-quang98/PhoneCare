namespace PhoneCare.Forms.QuanTriNhanVien
{
    partial class frmQuanTriNhanVien
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
            this.components = new System.ComponentModel.Container();
            this.dgvNhanVien = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctmThemMoi = new System.Windows.Forms.ToolStripMenuItem();
            this.ctmChinhSua = new System.Windows.Forms.ToolStripMenuItem();
            this.ctmXoa = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNhanVien)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvNhanVien
            // 
            this.dgvNhanVien.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvNhanVien.ContextMenuStrip = this.contextMenuStrip1;
            this.dgvNhanVien.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvNhanVien.Location = new System.Drawing.Point(0, 0);
            this.dgvNhanVien.Name = "dgvNhanVien";
            this.dgvNhanVien.Size = new System.Drawing.Size(800, 450);
            this.dgvNhanVien.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctmThemMoi,
            this.ctmChinhSua,
            this.ctmXoa});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(185, 92);
            // 
            // ctmThemMoi
            // 
            this.ctmThemMoi.Name = "ctmThemMoi";
            this.ctmThemMoi.Size = new System.Drawing.Size(184, 22);
            this.ctmThemMoi.Text = "Thêm mới nhân viên";
            this.ctmThemMoi.Click += new System.EventHandler(this.ctmThemMoi_Click);
            // 
            // ctmChinhSua
            // 
            this.ctmChinhSua.Name = "ctmChinhSua";
            this.ctmChinhSua.Size = new System.Drawing.Size(184, 22);
            this.ctmChinhSua.Text = "Chỉnh sửa nhân viên";
            this.ctmChinhSua.Click += new System.EventHandler(this.ctmChinhSua_Click);
            // 
            // ctmXoa
            // 
            this.ctmXoa.Name = "ctmXoa";
            this.ctmXoa.Size = new System.Drawing.Size(184, 22);
            this.ctmXoa.Text = "Xóa nhân viên";
            this.ctmXoa.Click += new System.EventHandler(this.ctmXoa_Click);
            // 
            // frmQuanTriNhanVien
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvNhanVien);
            this.Name = "frmQuanTriNhanVien";
            this.Text = "Quản trị nhân viên";
            this.Load += new System.EventHandler(this.frmQuanTriNhanVien_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvNhanVien)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvNhanVien;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ctmThemMoi;
        private System.Windows.Forms.ToolStripMenuItem ctmChinhSua;
        private System.Windows.Forms.ToolStripMenuItem ctmXoa;
    }
}