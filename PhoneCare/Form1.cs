using PhoneCare.Class;
using System;
using System.Windows.Forms;

namespace PhoneCare
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Khởi tạo cửa sổ chính của ứng dụng.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cập nhật trạng thái menu theo người dùng và quyền hiện tại.
        /// </summary>
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

        /// <summary>
        /// Khởi tạo và tải dữ liệu khi biểu mẫu Form1_Load được hiển thị.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateMenu();
            BeginInvoke(new Action(ShowLoginIfNeeded));
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuLogIn_Click.
        /// </summary>
        private void mnuLogIn_Click(object sender, EventArgs e)
        {
            var f = new frmDangNhap(this);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog(this);
        }

        /// <summary>
        /// Hiển thị màn hình đăng nhập khi chưa có phiên người dùng.
        /// </summary>
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

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuQuanTriNhanVien_Click.
        /// </summary>
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

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuChangePassword_Click.
        /// </summary>
        private void mnuChangePassword_Click(object sender, EventArgs e)
        {
            var f = new Forms.frmDoiMatKhau();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuQuanLyCuaHang_Click.
        /// </summary>
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

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuDonHang_Click.
        /// </summary>
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

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuTroGiup_Click.
        /// </summary>
        private void mnuTroGiup_Click(object sender, EventArgs e)
        {
            using (var f = new Forms.frmThongTinHoTro())
            {
                f.ShowDialog(this);
            }
        }
    }
}
