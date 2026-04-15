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

namespace PhoneCare.Forms.QuanTriCuaHang
{
    
    public partial class frmThemMoiCuaHang : Form
    {
        private Forms.QuanTriCuaHang.frmDanhSachCuaHang _parentForm;
        private int? _id = null;

        public frmThemMoiCuaHang(Forms.QuanTriCuaHang.frmDanhSachCuaHang parent, int? id = null)
        {
            InitializeComponent();
            _parentForm = parent;
            _id = id;
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Vui lòng nhập mã cơ sở");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập tên cơ sở");
                return false;
            }

            return true;
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            using (var db = new PhoneCareDbContext())
            {
                if (_id.HasValue)
                {
                    var coso = db.CoSoCuaHangs.FirstOrDefault(x => x.Id == _id);

                    if (coso == null) return;

                    coso.Code = txtCode.Text.Trim();
                    coso.Name = txtName.Text.Trim();
                    coso.Address = txtAddress.Text.Trim();
                    coso.HomePhone = txtHomePhone.Text.Trim();
                    coso.Hotline = txtHotline.Text.Trim();

                    coso.DateModify = DateTime.Now;
                    coso.UserModify = 1; // TODO: lấy user login sau

                    db.SaveChanges();

                    MessageBox.Show("Cập nhật thành công!");
                } else
                {
                    if (db.CoSoCuaHangs.Any(x => x.Code == txtCode.Text.Trim()))
                    {
                        MessageBox.Show("Code đã tồn tại!");
                        return;
                    }
                    db.CoSoCuaHangs.Add(new CoSoCuaHang
                    {
                        Code = txtCode.Text.Trim(),
                        Name = txtName.Text.Trim(),
                        Address = txtAddress.Text.Trim(),
                        HomePhone = txtHomePhone.Text.Trim(),
                        Hotline = txtHotline.Text.Trim(),
                        IsDeleted = false,
                        DateCreated = DateTime.Now,
                        UserCreated = 1
                    });

                    db.SaveChanges();
                    MessageBox.Show("Thêm thành công!");
                }
            }

            ClearForm();
            // 🔥 Reload form danh sách
            _parentForm.LoadCoSo();

            // 🔥 Đóng form hiện tại
            this.Close();
        }

        private void ClearForm()
        {
            txtCode.Clear();
            txtName.Clear();
            txtAddress.Clear();
            txtHomePhone.Clear();
            txtHotline.Clear();

            txtCode.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            ClearForm();
            this.Close();
        }

        private void frmThemMoiCuaHang_Load(object sender, EventArgs e)
        {
            if (_id.HasValue)
            {
                this.Text = "Chỉnh sửa cửa hàng";
                LoadDataForEdit();
            }
            else
            {
                this.Text = "Thêm mới cửa hàng";
            }
        }

        private void LoadDataForEdit()
        {
            using (var db = new PhoneCareDbContext())
            {
                var coso = db.CoSoCuaHangs.FirstOrDefault(x => x.Id == _id);

                if (coso == null) return;
                txtID.Text = coso.Id.ToString();
                txtCode.Text = coso.Code;
                txtName.Text = coso.Name;
                txtAddress.Text = coso.Address;
                txtHomePhone.Text = coso.HomePhone;
                txtHotline.Text = coso.Hotline;
            }
        }
    }
}
