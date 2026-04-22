using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneCare
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void UpdateMenu()
        {
            if (Class.CurrentUser.Id != 0)
            {
                mnuLogIn.Visible = false;
                mnuChangePassword.Visible = true;
                mnuDangXuat.Visible = true;
                mnuQuanTri.Visible = true;
                mnuDonHang.Visible = true;
                mnuTroGiup.Visible = true;
            } else
            {
                mnuLogIn.Visible = true;
                mnuChangePassword.Visible = false;
                mnuDangXuat.Visible = false;
                mnuQuanTri.Visible = false;
                mnuDonHang.Visible = false;
                mnuTroGiup.Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateMenu();
        }

        private void mnuLogIn_Click(object sender, EventArgs e)
        {
            frmDangNhap f = new frmDangNhap(this);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuQuanTriNhanVien_Click(object sender, EventArgs e)
        {
            Forms.QuanTriNhanVien.frmQuanTriNhanVien f = new Forms.QuanTriNhanVien.frmQuanTriNhanVien();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuChangePassword_Click(object sender, EventArgs e)
        {
            Forms.frmDoiMatKhau f = new Forms.frmDoiMatKhau();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuQuanLyCuaHang_Click(object sender, EventArgs e)
        {
            Forms.QuanTriCuaHang.frmDanhSachCuaHang f = new Forms.QuanTriCuaHang.frmDanhSachCuaHang();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuDonHang_Click(object sender, EventArgs e)
        {
            Forms.QuanTriDonHang.frmDanhSachDonHang f = new Forms.QuanTriDonHang.frmDanhSachDonHang();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }
    }
}
