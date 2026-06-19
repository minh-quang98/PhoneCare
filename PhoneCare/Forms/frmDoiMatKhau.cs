using PhoneCare.Class;
using PhoneCare.Data;
using PhoneCare.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare.Forms
{
    public partial class frmDoiMatKhau : Form
    {
        private readonly int _userId;
        private bool _showPassword = false;

        /// <summary>
        /// Khởi tạo đối tượng frmDoiMatKhau.
        /// </summary>
        public frmDoiMatKhau()
        {
            InitializeComponent();
            _userId = Class.CurrentUser.Id;
        }

        /// <summary>
        /// Kiểm tra dữ liệu nhập trên biểu mẫu và hiển thị lỗi tương ứng.
        /// </summary>
        private bool ValidateInput()
        {
            bool validate = true;
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(txtOldPassword.Text))
            {
                errorProvider1.SetError(txtOldPassword, "Vui lòng nhập mật khẩu cũ!");
                validate = false;
            }

            if (string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                errorProvider1.SetError(txtNewPassword, "Vui lòng nhập mật khẩu mới!");
                validate = false;
            }

            if (txtNewPassword.Text != txtReNewPassword.Text)
            {
                errorProvider1.SetError(txtReNewPassword, "Mật khẩu nhập lại không khớp!");
                validate = false;
            }

            return validate;
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnSave_Click.
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            using (var context = new PhoneCareDbContext())
            {
                var user = context.Set<NhanVien>()
                                  .FirstOrDefault(x => x.Id == _userId && !x.IsDeleted);

                if (user == null)
                {
                    MessageBox.Show("Không tìm thấy tài khoản");
                    return;
                }

                if (!IsPasswordValid(user.Password, txtOldPassword.Text))
                {
                    errorProvider1.SetError(txtOldPassword, "Mật khẩu cũ không đúng!");
                    return;
                }

                user.Password = PasswordHasher.Hash(txtNewPassword.Text);
                user.DateModify = DateTime.Now;
                user.UserModify = _userId;

                context.SaveChanges();

                MessageBox.Show("Đổi mật khẩu thành công");
                Close();
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

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnCancel_Click.
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnShowHide_Click.
        /// </summary>
        private void btnShowHide_Click(object sender, EventArgs e)
        {
            if (_showPassword)
            {
                txtNewPassword.PasswordChar = '*';
                txtOldPassword.PasswordChar = '*';
                txtReNewPassword.PasswordChar = '*';
                _showPassword = false;
            }
            else
            {
                txtNewPassword.PasswordChar = '\0';
                txtOldPassword.PasswordChar = '\0';
                txtReNewPassword.PasswordChar = '\0';
                _showPassword = true;
            }
        }
    }
}
