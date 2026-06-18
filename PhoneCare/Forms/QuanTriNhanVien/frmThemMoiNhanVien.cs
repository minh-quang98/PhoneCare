using PhoneCare.Class;
using PhoneCare.Data;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare.Forms.QuanTriNhanVien
{
    public partial class frmThemMoiNhanVien : Form
    {
        private readonly frmQuanTriNhanVien _parentForm;
        private readonly int? _id = null;

        public frmThemMoiNhanVien(frmQuanTriNhanVien parent, int? id = null)
        {
            InitializeComponent();
            _parentForm = parent;
            _id = id;
        }

        private string GetTypeEmployee()
        {
            if (rbtAdmin.Checked) return PermissionService.Admin;
            if (rbtAdminCS.Checked) return PermissionService.AdminCoSo;
            if (rbtSale.Checked) return PermissionService.Sale;
            if (rbtKyThuat.Checked) return PermissionService.KyThuat;
            return PermissionService.Marketing;
        }

        private void SetTypeEmployee(string type)
        {
            switch (type)
            {
                case PermissionService.Admin:
                    rbtAdmin.Checked = true;
                    break;
                case PermissionService.AdminCoSo:
                    rbtAdminCS.Checked = true;
                    break;
                case PermissionService.Sale:
                    rbtSale.Checked = true;
                    break;
                case PermissionService.KyThuat:
                    rbtKyThuat.Checked = true;
                    break;
                case PermissionService.Marketing:
                    rbtMarketting.Checked = true;
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
                    var nhanvien = db.NhanViens.FirstOrDefault(x => x.Id == _id && !x.IsDeleted);
                    if (nhanvien == null)
                    {
                        MessageBox.Show("Không tìm thấy nhân viên!");
                        return;
                    }

                    nhanvien.UserName = txtTaiKhoan.Text.Trim();
                    if (!string.IsNullOrWhiteSpace(txtMatKhau.Text))
                    {
                        nhanvien.Password = PasswordHasher.Hash(txtMatKhau.Text.Trim());
                    }

                    nhanvien.FullName = txtHoTen.Text.Trim();
                    nhanvien.NickName = txtNickName.Text.Trim();
                    nhanvien.Email = txtEmail.Text.Trim();
                    nhanvien.Phone = txtSDT.Text.Trim();
                    nhanvien.IdCoSoLamViec = Convert.ToInt32(cboCoSoLamViec.SelectedValue);
                    nhanvien.KhoaTaiKhoan = chkKhoaTaiKhoan.Checked;
                    nhanvien.LoaiNhanVien = GetTypeEmployee();
                    nhanvien.DateModify = DateTime.Now;
                    nhanvien.UserModify = Class.CurrentUser.Id;
                    nhanvien.IsDeleted = false;

                    db.SaveChanges();
                    MessageBox.Show("Cập nhật thành công!");
                }
                else
                {
                    if (db.NhanViens.Any(x => x.UserName == txtTaiKhoan.Text.Trim() && !x.IsDeleted))
                    {
                        errorProvider1.SetError(txtTaiKhoan, $"Tài khoản {txtTaiKhoan.Text} đã tồn tại!");
                        return;
                    }

                    db.NhanViens.Add(new Models.NhanVien
                    {
                        UserName = txtTaiKhoan.Text.Trim(),
                        Password = PasswordHasher.Hash(txtMatKhau.Text.Trim()),
                        FullName = txtHoTen.Text.Trim(),
                        NickName = txtNickName.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Phone = txtSDT.Text.Trim(),
                        IdCoSoLamViec = Convert.ToInt32(cboCoSoLamViec.SelectedValue),
                        KhoaTaiKhoan = chkKhoaTaiKhoan.Checked,
                        LoaiNhanVien = GetTypeEmployee(),
                        DateCreated = DateTime.Now,
                        UserCreated = Class.CurrentUser.Id,
                        IsDeleted = false
                    });

                    db.SaveChanges();
                    MessageBox.Show("Thêm nhân viên thành công!");
                }
            }

            ClearForm();
            _parentForm.LoadNhanVien();
            Close();
        }

        private void frmThemMoiNhanVien_Load(object sender, EventArgs e)
        {
            LoadCoSo();
            if (_id.HasValue)
            {
                Text = "Chỉnh sửa nhân viên";
                LoadDataForEdit();
            }
            else
            {
                Text = "Thêm mới nhân viên";
            }
        }

        private void LoadDataForEdit()
        {
            using (var db = new PhoneCareDbContext())
            {
                var nhanvien = db.NhanViens.FirstOrDefault(x => x.Id == _id && !x.IsDeleted);

                if (nhanvien == null) return;
                txtTaiKhoan.Text = nhanvien.UserName;
                txtMatKhau.Clear();
                txtHoTen.Text = nhanvien.FullName;
                txtNickName.Text = nhanvien.NickName;
                txtEmail.Text = nhanvien.Email;
                txtSDT.Text = nhanvien.Phone;
                chkKhoaTaiKhoan.Checked = nhanvien.KhoaTaiKhoan;
                cboCoSoLamViec.SelectedValue = nhanvien.IdCoSoLamViec;
                SetTypeEmployee(nhanvien.LoaiNhanVien);
            }
        }

        private void LoadCoSo()
        {
            using (var db = new PhoneCareDbContext())
            {
                cboCoSoLamViec.DataSource = db.CoSoCuaHangs.Where(x => !x.IsDeleted).ToList();
                cboCoSoLamViec.DisplayMember = "Name";
                cboCoSoLamViec.ValueMember = "Id";
            }
        }

        private bool ValidateInput()
        {
            bool validate = true;
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(txtTaiKhoan.Text))
            {
                errorProvider1.SetError(txtTaiKhoan, "Tài khoản không được để trống!");
                validate = false;
            }
            if (!_id.HasValue && string.IsNullOrWhiteSpace(txtMatKhau.Text))
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
            Close();
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
