using PhoneCare.Class;
using PhoneCare.Data;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare
{
    public partial class frmDangNhap : Form
    {
        private readonly Form1 _parentForm;
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Khởi tạo đối tượng frmDangNhap.
        /// </summary>
        public frmDangNhap(Form1 parentForm)
        {
            InitializeComponent();
            _parentForm = parentForm;
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnDangNhap_Click.
        /// </summary>
        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            HandleDangNhap();
        }

        /// <summary>
        /// Xác thực thông tin đăng nhập, cập nhật trạng thái khóa và mở phiên người dùng.
        /// </summary>
        private void HandleDangNhap()
        {
            using (var db = new PhoneCareDbContext())
            {
                string userName = txtTenDangNhap.Text.Trim();
                string password = txtMatKhau.Text;
                var now = DateTime.Now;

                var user = db.NhanViens.FirstOrDefault(x => x.UserName == userName && !x.IsDeleted);

                if (user != null && user.LockoutEndAt.HasValue && user.LockoutEndAt.Value > now)
                {
                    MessageBox.Show($"Tài khoản đang bị khóa tạm thời đến {user.LockoutEndAt.Value:HH:mm:ss}.");
                    return;
                }

                if (user != null && !user.KhoaTaiKhoan && IsPasswordValid(user.Password, password))
                {
                    if (!PasswordHasher.IsHashed(user.Password))
                    {
                        user.Password = PasswordHasher.Hash(password);
                    }

                    user.FailedLoginCount = 0;
                    user.LockoutEndAt = null;
                    user.LastFailedLoginAt = null;
                    db.SaveChanges();

                    Class.CurrentUser.Id = user.Id;
                    Class.CurrentUser.UserName = user.UserName;
                    Class.CurrentUser.FullName = user.FullName;
                    Class.CurrentUser.CoSoCuaHangId = user.IdCoSoLamViec;
                    Class.CurrentUser.LoaiNhanVien = user.LoaiNhanVien;

                    MessageBox.Show("Đăng nhập thành công!");

                    _parentForm.UpdateMenu();
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    if (user != null && !user.KhoaTaiKhoan)
                    {
                        user.FailedLoginCount += 1;
                        user.LastFailedLoginAt = now;
                        if (user.FailedLoginCount >= MaxFailedAttempts)
                        {
                            user.LockoutEndAt = now.Add(LockoutDuration);
                        }

                        db.SaveChanges();
                    }

                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
                }
            }
        }

        /// <summary>
        /// Xác thực mật khẩu nhập vào và hỗ trợ dữ liệu mật khẩu cũ chưa được băm.
        /// </summary>
        private bool IsPasswordValid(string storedPassword, string password)
        {
            if (PasswordHasher.IsHashed(storedPassword))
            {
                return PasswordHasher.Verify(password, storedPassword);
            }

            return storedPassword == password;
        }
    }
}
