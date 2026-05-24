using PhoneCare.Data;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare.Forms.QuanTriDonHang
{

    public partial class frmDichVu : Form
    {
        private frmThemMoiDonHang _parentForm;
        private int? _id = null;
        private int? _idDonHang = null;
        public frmDichVu(frmThemMoiDonHang parentForm, int? idDonHang = null, int? id = null)
        {
            InitializeComponent();
            _parentForm = parentForm;
            _id = id;
            _idDonHang = idDonHang;
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            ClearForm();
            this.Close();
        }

        private void txtBaoGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtBaoGia_TextChanged(object sender, EventArgs e)
        {
            string raw = txtBaoGia.Text;
            string onlyNumber = new string(raw.Where(char.IsDigit).ToArray());
            if (raw != onlyNumber)
            {
                txtBaoGia.Text = onlyNumber;
                txtBaoGia.SelectionStart = txtBaoGia.Text.Length;
            }

            if (string.IsNullOrWhiteSpace(txtBaoGia.Text))
            {
                lblSoTien.Text = "0 VNĐ";
                return;
            }

            if (decimal.TryParse(txtBaoGia.Text, out decimal value))
            {
                lblSoTien.Text = value.ToString("N0") + " VNĐ";
            }
        }

        private void ClearForm()
        {
            txtBaoGia.Clear();
            txtDichVu.Clear();
            lblSoTien.Text = "0 VNĐ";
            lblMaPhieu.Text = "";
        }

        private void frmDichVu_Load(object sender, EventArgs e)
        {
            if (_idDonHang.HasValue)
            {
                lblMaPhieu.Text = _idDonHang.ToString();
            }
            else
            {
                lblMaPhieu.Text = "";
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            using (var db = new PhoneCareDbContext())
            {
                if (_id.HasValue)
                {
                    var dichVu = db.DichVus.Find(_id.Value);
                    if (dichVu != null)
                    {
                        dichVu.TenDichVu = txtDichVu.Text.Trim();
                        dichVu.DonGia = decimal.Parse(txtBaoGia.Text);
                        dichVu.DateModify = DateTime.Now;
                        dichVu.UserModify = Class.CurrentUser.Id;
                        dichVu.IsDeleted = false;
                        db.SaveChanges();
                    }
                    MessageBox.Show($"Đã lưu DV: {dichVu.TenDichVu}, IdDonHang = {dichVu.IdDonHang}");
                }
                else
                {
                    var dichVu = new Models.DichVu
                    {
                        TenDichVu = txtDichVu.Text.Trim(),
                        DonGia = decimal.Parse(txtBaoGia.Text),
                        IdDonHang = _idDonHang.Value,
                        DateCreated = DateTime.Now,
                        UserCreated = Class.CurrentUser.Id,
                        IsDeleted = false,
                    };
                    db.DichVus.Add(dichVu);
                    db.SaveChanges();
                    MessageBox.Show($"Đã lưu DV: {dichVu.TenDichVu}, IdDonHang = {dichVu.IdDonHang}");
                }
            }

            _parentForm.LoadDichVu();
            ClearForm();
            this.Close();
        }

        private bool ValidateInput()
        {
            bool validate = true;
            if (string.IsNullOrWhiteSpace(txtDichVu.Text))
            {
                errorProvider1.SetError(txtDichVu, "Tên dịch vụ không được để trống!");
                validate = false;
            }
            return validate;
        }
    }
}
