using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using PhoneCare.Data;
using PhoneCare.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneCare.Forms
{
    public partial class frmDoiMatKhau : Form
    {
        private int _userId;
        private bool _showPassword = false;
        public frmDoiMatKhau()
        {
            InitializeComponent();
            _userId = Class.CurrentUser.Id;
        }

        private bool ValidateInput()
        {
            bool validate = true;
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

                // ❗ So sánh mật khẩu cũ
                if (user.Password != txtOldPassword.Text)
                {
                    errorProvider1.SetError(txtOldPassword, "Mật khẩu cũ không đúng!");
                    return;
                }

                // Update mật khẩu mới
                user.Password = txtNewPassword.Text;
                user.DateModify = DateTime.Now;
                user.UserModify = _userId;

                context.SaveChanges();

                MessageBox.Show("Đổi mật khẩu thành công");
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnShowHide_Click(object sender, EventArgs e)
        {
            if (_showPassword)
            {
                txtNewPassword.PasswordChar = '*';
                txtOldPassword.PasswordChar = '*';
                txtReNewPassword.PasswordChar = '*';
                _showPassword = false;
            } else
            {
                txtNewPassword.PasswordChar = '\0';
                txtOldPassword.PasswordChar = '\0';
                txtReNewPassword.PasswordChar = '\0';
                _showPassword = true;
            }
        }
    }
}
