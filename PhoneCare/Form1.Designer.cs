namespace PhoneCare
{
    partial class Form1
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuAuthen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLogIn = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChangePassword = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDangXuat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuQuanTri = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuQuanTriNhanVien = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuQuanLyCuaHang = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDonHang = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTroGiup = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAuthen,
            this.mnuQuanTri,
            this.mnuDonHang,
            this.mnuTroGiup});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(685, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // mnuAuthen
            // 
            this.mnuAuthen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLogIn,
            this.mnuChangePassword,
            this.mnuDangXuat});
            this.mnuAuthen.Name = "mnuAuthen";
            this.mnuAuthen.Size = new System.Drawing.Size(70, 20);
            this.mnuAuthen.Text = "Tài khoản";
            // 
            // mnuLogIn
            // 
            this.mnuLogIn.Name = "mnuLogIn";
            this.mnuLogIn.Size = new System.Drawing.Size(145, 22);
            this.mnuLogIn.Text = "Đăng nhập";
            this.mnuLogIn.Click += new System.EventHandler(this.mnuLogIn_Click);
            // 
            // mnuChangePassword
            // 
            this.mnuChangePassword.Name = "mnuChangePassword";
            this.mnuChangePassword.Size = new System.Drawing.Size(145, 22);
            this.mnuChangePassword.Text = "Đổi mật khẩu";
            this.mnuChangePassword.Click += new System.EventHandler(this.mnuChangePassword_Click);
            // 
            // mnuDangXuat
            // 
            this.mnuDangXuat.Name = "mnuDangXuat";
            this.mnuDangXuat.Size = new System.Drawing.Size(145, 22);
            this.mnuDangXuat.Text = "Đăng Xuất";
            // 
            // mnuQuanTri
            // 
            this.mnuQuanTri.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuQuanTriNhanVien,
            this.mnuQuanLyCuaHang});
            this.mnuQuanTri.Name = "mnuQuanTri";
            this.mnuQuanTri.Size = new System.Drawing.Size(62, 20);
            this.mnuQuanTri.Text = "Quản trị";
            // 
            // mnuQuanTriNhanVien
            // 
            this.mnuQuanTriNhanVien.Name = "mnuQuanTriNhanVien";
            this.mnuQuanTriNhanVien.Size = new System.Drawing.Size(170, 22);
            this.mnuQuanTriNhanVien.Text = "Quán lý nhân viên";
            this.mnuQuanTriNhanVien.Click += new System.EventHandler(this.mnuQuanTriNhanVien_Click);
            // 
            // mnuQuanLyCuaHang
            // 
            this.mnuQuanLyCuaHang.Name = "mnuQuanLyCuaHang";
            this.mnuQuanLyCuaHang.Size = new System.Drawing.Size(170, 22);
            this.mnuQuanLyCuaHang.Text = "Quản lý cửa hàng";
            this.mnuQuanLyCuaHang.Click += new System.EventHandler(this.mnuQuanLyCuaHang_Click);
            // 
            // mnuDonHang
            // 
            this.mnuDonHang.Name = "mnuDonHang";
            this.mnuDonHang.Size = new System.Drawing.Size(71, 20);
            this.mnuDonHang.Text = "Đơn hàng";
            this.mnuDonHang.Click += new System.EventHandler(this.mnuDonHang_Click);
            // 
            // mnuTroGiup
            // 
            this.mnuTroGiup.Name = "mnuTroGiup";
            this.mnuTroGiup.Size = new System.Drawing.Size(63, 20);
            this.mnuTroGiup.Text = "Trợ giúp";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phone Care";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuAuthen;
        private System.Windows.Forms.ToolStripMenuItem mnuLogIn;
        private System.Windows.Forms.ToolStripMenuItem mnuChangePassword;
        private System.Windows.Forms.ToolStripMenuItem mnuQuanTri;
        private System.Windows.Forms.ToolStripMenuItem mnuQuanTriNhanVien;
        private System.Windows.Forms.ToolStripMenuItem mnuDonHang;
        private System.Windows.Forms.ToolStripMenuItem mnuTroGiup;
        private System.Windows.Forms.ToolStripMenuItem mnuQuanLyCuaHang;
        private System.Windows.Forms.ToolStripMenuItem mnuDangXuat;
    }
}

