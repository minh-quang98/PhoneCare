using PhoneCare.Data;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare
{
    public partial class frmDangNhap : Form
    {
        private Form1 _parentForm;
        public frmDangNhap(Form1 parentForm)
        {
            InitializeComponent();
            _parentForm = parentForm;
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            HandleDangNhap();
        }

        private void btnDangNhap_KeyPress(object sender, KeyPressEventArgs e)
        {
            HandleDangNhap();
        }

        private void HandleDangNhap()
        {
            using (var db = new PhoneCareDbContext())
            {
                var user = db.NhanViens.FirstOrDefault(x => x.UserName == txtTenDangNhap.Text.Trim()
                                                        && x.Password == txtMatKhau.Text.Trim()
                                                        && !x.IsDeleted
                                                        && !x.KhoaTaiKhoan);

                if (user != null)
                {
                    Class.CurrentUser.Id = user.Id;
                    Class.CurrentUser.UserName = user.UserName;
                    Class.CurrentUser.FullName = user.FullName;

                    MessageBox.Show("Đăng nhập thành công!");

                    _parentForm.UpdateMenu();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu!");
                }
            }
        }
    }
}
