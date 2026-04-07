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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.thêmMớiCửaHàngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cậpNhậtCửaHàngToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(800, 450);
            this.dataGridView1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.thêmMớiCửaHàngToolStripMenuItem,
            this.cậpNhậtCửaHàngToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 48);
            // 
            // thêmMớiCửaHàngToolStripMenuItem
            // 
            this.thêmMớiCửaHàngToolStripMenuItem.Name = "thêmMớiCửaHàngToolStripMenuItem";
            this.thêmMớiCửaHàngToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.thêmMớiCửaHàngToolStripMenuItem.Text = "Thêm mới cửa hàng";
            // 
            // cậpNhậtCửaHàngToolStripMenuItem
            // 
            this.cậpNhậtCửaHàngToolStripMenuItem.Name = "cậpNhậtCửaHàngToolStripMenuItem";
            this.cậpNhậtCửaHàngToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.cậpNhậtCửaHàngToolStripMenuItem.Text = "Cập nhật cửa hàng";
            // 
            // frmDanhSachCuaHang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.dataGridView1);
            this.Name = "frmDanhSachCuaHang";
            this.Text = "frmDanhSachCuaHang";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem thêmMớiCửaHàngToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cậpNhậtCửaHàngToolStripMenuItem;
    }
}