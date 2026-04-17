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
using System.Xml.Linq;

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

        private void btnSave_Click(object sender, EventArgs e)
        {
           
        }

        private void frmThemMoiNhanVien_Load(object sender, EventArgs e)
        {
            LoadCoSo();
        }

        private void LoadCoSo()
        {
            using (var db = new PhoneCareDbContext())
            {
                cboCoSoLamViec.DataSource = db.CoSoCuaHangs.ToList();
                cboCoSoLamViec.DisplayMember = "TenCoSo"; // tên hiển thị
                cboCoSoLamViec.ValueMember = "Id";        // giá trị
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

        }
    }
}
