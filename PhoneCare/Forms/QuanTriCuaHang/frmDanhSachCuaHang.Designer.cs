namespace PhoneCare.Forms.QuanTriCuaHang
{
    partial class frmDanhSachCuaHang
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
            this.dgvCoSo = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuThemMoi = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCapNhat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuXoa = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCoSo)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvCoSo
            // 
            this.dgvCoSo.AllowUserToDeleteRows = false;
            this.dgvCoSo.ContextMenuStrip = this.contextMenuStrip1;
            this.dgvCoSo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCoSo.Location = new System.Drawing.Point(0, 0);
            this.dgvCoSo.Name = "dgvCoSo";
            this.dgvCoSo.Size = new System.Drawing.Size(800, 450);
            this.dgvCoSo.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuThemMoi,
            this.mnuCapNhat,
            this.mnuXoa});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 92);
            // 
            // mnuThemMoi
            // 
            this.mnuThemMoi.Name = "mnuThemMoi";
            this.mnuThemMoi.Size = new System.Drawing.Size(181, 22);
            this.mnuThemMoi.Text = "Thêm mới cửa hàng";
            this.mnuThemMoi.Click += new System.EventHandler(this.mnuThemMoi_Click);
            // 
            // mnuCapNhat
            // 
            this.mnuCapNhat.Name = "mnuCapNhat";
            this.mnuCapNhat.Size = new System.Drawing.Size(181, 22);
            this.mnuCapNhat.Text = "Cập nhật cửa hàng";
            this.mnuCapNhat.Click += new System.EventHandler(this.mnuCapNhat_Click);
            // 
            // mnuXoa
            // 
            this.mnuXoa.Name = "mnuXoa";
            this.mnuXoa.Size = new System.Drawing.Size(181, 22);
            this.mnuXoa.Text = "Xóa cửa hàng";
            this.mnuXoa.Click += new System.EventHandler(this.mnuXoa_Click);
            // 
            // frmDanhSachCuaHang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dgvCoSo);
            this.Name = "frmDanhSachCuaHang";
            this.Text = "frmDanhSachCuaHang";
            this.Load += new System.EventHandler(this.frmDanhSachCuaHang_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCoSo)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCoSo;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuThemMoi;
        private System.Windows.Forms.ToolStripMenuItem mnuCapNhat;
        private System.Windows.Forms.ToolStripMenuItem mnuXoa;
    }
}