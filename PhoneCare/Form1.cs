using PhoneCare.Class;
using System;
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
                mnuQuanTriNhanVien.Visible = PermissionService.CanManageEmployees();
                mnuQuanLyCuaHang.Visible = PermissionService.CanManageStores();
                mnuQuanTri.Visible = mnuQuanTriNhanVien.Visible || mnuQuanLyCuaHang.Visible;
                mnuDonHang.Visible = PermissionService.CanViewOrders();
                mnuTroGiup.Visible = true;
            }
            else
            {
                mnuLogIn.Visible = true;
                mnuChangePassword.Visible = false;
                mnuDangXuat.Visible = false;
                mnuQuanTriNhanVien.Visible = false;
                mnuQuanLyCuaHang.Visible = false;
                mnuQuanTri.Visible = false;
                mnuDonHang.Visible = false;
                mnuTroGiup.Visible = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateMenu();
            BeginInvoke(new Action(ShowLoginIfNeeded));
        }

        private void mnuLogIn_Click(object sender, EventArgs e)
        {
            var f = new frmDangNhap(this);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog(this);
        }

        private void ShowLoginIfNeeded()
        {
            if (Class.CurrentUser.Id != 0) return;

            using (var f = new frmDangNhap(this))
            {
                f.StartPosition = FormStartPosition.CenterScreen;

                if (f.ShowDialog(this) != DialogResult.OK || Class.CurrentUser.Id == 0)
                {
                    Close();
                    return;
                }
            }

            UpdateMenu();
        }

        private void mnuQuanTriNhanVien_Click(object sender, EventArgs e)
        {
            if (!PermissionService.CanManageEmployees())
            {
                MessageBox.Show("Bạn không có quyền quản lý nhân viên.");
                return;
            }

            var f = new Forms.QuanTriNhanVien.frmQuanTriNhanVien();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuChangePassword_Click(object sender, EventArgs e)
        {
            var f = new Forms.frmDoiMatKhau();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuQuanLyCuaHang_Click(object sender, EventArgs e)
        {
            if (!PermissionService.CanManageStores())
            {
                MessageBox.Show("Bạn không có quyền quản lý cửa hàng.");
                return;
            }

            var f = new Forms.QuanTriCuaHang.frmDanhSachCuaHang();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuDonHang_Click(object sender, EventArgs e)
        {
            if (!PermissionService.CanViewOrders())
            {
                MessageBox.Show("Bạn không có quyền xem đơn hàng.");
                return;
            }

            var f = new Forms.QuanTriDonHang.frmDanhSachDonHang();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuTroGiup_Click(object sender, EventArgs e)
        {
            using (var f = new Forms.frmThongTinHoTro())
            {
                f.ShowDialog(this);
            }
        }
    }
}
