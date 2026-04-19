using PhoneCare.Data;
using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace PhoneCare.Forms.QuanTriNhanVien
{
    public partial class frmThemMoiNhanVien : Form
    {
        private frmQuanTriNhanVien _parentForm;
        private int? _id = null;
        public frmThemMoiNhanVien(frmQuanTriNhanVien parent, int? id = null)
        {
            InitializeComponent();
            _parentForm = parent;
            _id = id;
        }

        private string GetTypeEmployee()
        {
            if (rbtAdmin.Checked) return "Admin";
            if (rbtAdminCS.Checked) return "AdminCS";
            if (rbtSale.Checked) return "Sale";
            if (rbtKyThuat.Checked) return "Kỹ thuật";
            return "Marketing";
        }

        private void SetTypeEmployee(string type)
        {
            switch (type)
            {
                case "Admin":
                    rbtAdmin.Checked = true;
                    break;
                case "AdminCS":
                    rbtAdminCS.Checked = true;
                    break;
                case "Sale":
                    rbtSale.Checked = true;
                    break;
                case "Kỹ thuật":
                    rbtKyThuat.Checked = true;
                    break;
                case "Marketing":
                    rbtMarketting.Checked = true;
                    break;
                default:
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            using (var db = new PhoneCareDbContext())
            {
                if (_id.HasValue)
                {
                    var nhanvien = db.NhanViens.FirstOrDefault(x => x.Id == _id);
                    if (nhanvien == null)
                    {
                        MessageBox.Show("Không tìm thấy nhân viên!");
                        return;
                    }

                    nhanvien.UserName = txtTaiKhoan.Text.Trim();
                    nhanvien.Password = txtMatKhau.Text.Trim();
                    nhanvien.FullName = txtHoTen.Text.Trim();
                    nhanvien.NickName = txtNickName.Text.Trim();
                    nhanvien.Email = txtEmail.Text.Trim();
                    nhanvien.Phone = txtSDT.Text.Trim();
                    nhanvien.IdCoSoLamViec = Convert.ToInt32(cboCoSoLamViec.SelectedValue);
                    nhanvien.KhoaTaiKhoan = chkKhoaTaiKhoan.Checked;
                    nhanvien.LoaiNhanVien = this.GetTypeEmployee();
                    nhanvien.DateModify = DateTime.Now;
                    nhanvien.UserModify = 1;
                    nhanvien.IsDeleted = false;

                    db.SaveChanges();

                    MessageBox.Show("Cập nhật thành công!");
                } else
                {
                    if (db.NhanViens.Any(x => x.UserName == txtTaiKhoan.Text.Trim()))
                    {
                        errorProvider1.SetError(txtTaiKhoan, $"Tài khoản {txtTaiKhoan.Text} đã tồn tại!");
                        return;
                    }
                    db.NhanViens.Add(new Models.NhanVien {
                        UserName = txtTaiKhoan.Text.Trim(),
                        Password = txtMatKhau.Text.Trim(),
                        FullName = txtHoTen.Text.Trim(),
                        NickName = txtNickName.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Phone = txtSDT.Text.Trim(),
                        IdCoSoLamViec = Convert.ToInt32(cboCoSoLamViec.SelectedValue),
                        KhoaTaiKhoan = chkKhoaTaiKhoan.Checked,
                        LoaiNhanVien = this.GetTypeEmployee(),
                        DateCreated = DateTime.Now,
                        UserCreated = 1,
                        IsDeleted = false
                    });

                    db.SaveChanges();
                    MessageBox.Show("Thêm nhân viên thành công!");
                }
            }
            ClearForm();
            _parentForm.LoadNhanVien();

            this.Close();
            
        }

        private void frmThemMoiNhanVien_Load(object sender, EventArgs e)
        {
            LoadCoSo();
            if (_id.HasValue)
            {
                this.Text = "Chỉnh sửa nhân viên";
                LoadDataForEdit();
            }
            else
            {
                this.Text = "Thêm mới nhân viên";
            }
        }

        private void LoadDataForEdit()
        {
            using (var db = new PhoneCareDbContext())
            {
                var nhanvien = db.NhanViens.FirstOrDefault(x => x.Id == _id);

                if (nhanvien == null) return;
                txtTaiKhoan.Text = nhanvien.UserName;
                txtMatKhau.Text = nhanvien.Password;
                txtHoTen.Text = nhanvien.FullName;
                txtNickName.Text = nhanvien.NickName;
                txtEmail.Text = nhanvien.Email;
                txtSDT.Text = nhanvien.Phone;
                chkKhoaTaiKhoan.Checked = nhanvien.KhoaTaiKhoan;
                cboCoSoLamViec.SelectedValue = nhanvien.IdCoSoLamViec;
                this.SetTypeEmployee(nhanvien.LoaiNhanVien);
            }
        }

        private void LoadCoSo()
        {
            using (var db = new PhoneCareDbContext())
            {
                cboCoSoLamViec.DataSource = db.CoSoCuaHangs.ToList();
                cboCoSoLamViec.DisplayMember = "Name"; // tên hiển thị
                cboCoSoLamViec.ValueMember = "Id";     // giá trị
            }
        }

        private bool ValidateInput()
        {
            bool validate = true;
            if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                errorProvider1.SetError(txtTaiKhoan, "Tài khoản không được để trống!");
                validate = false;
            }
            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                errorProvider1.SetError(txtMatKhau, "Mật khẩu không được để trống!");
                validate = false;
            }
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                errorProvider1.SetError(txtHoTen, "Họ và tên không được để trống!");
                validate = false;
            }
            return validate;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            this.Close();
        }

        private void ClearForm()
        {
            txtTaiKhoan.Clear();
            txtMatKhau.Clear();
            txtHoTen.Clear();
            txtNickName.Clear();
            txtEmail.Clear();
            txtSDT.Clear();

        }
    }
}
