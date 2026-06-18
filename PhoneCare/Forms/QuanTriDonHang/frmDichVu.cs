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
            if (_id.HasValue)
            {
                LoadDataForEdit();
                return;
            }

            Text = "Thêm dịch vụ";
            btnLuu.Text = "Lưu lại";
            if (_idDonHang.HasValue)
            {
                lblMaPhieu.Text = _idDonHang.ToString();
            }
            else
            {
                lblMaPhieu.Text = "";
            }
        }

        private void LoadDataForEdit()
        {
            using (var db = new PhoneCareDbContext())
            {
                var dichVu = db.DichVus.FirstOrDefault(x => x.Id == _id.Value && x.IsDeleted == false);
                if (dichVu == null)
                {
                    MessageBox.Show("Không tìm thấy dịch vụ cần chỉnh sửa.");
                    Close();
                    return;
                }

                _idDonHang = dichVu.IdDonHang;
                Text = "Chỉnh sửa dịch vụ";
                btnLuu.Text = "Cập nhật";
                lblMaPhieu.Text = dichVu.IdDonHang.ToString();
                txtDichVu.Text = dichVu.TenDichVu;
                txtBaoGia.Text = dichVu.DonGia.ToString("0");
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            if (!_parentForm.CanModifyServices()) return;

            using (var db = new PhoneCareDbContext())
            {
                if (_id.HasValue)
                {
                    var dichVu = db.DichVus.Find(_id.Value);
                    if (dichVu == null || dichVu.IsDeleted)
                    {
                        MessageBox.Show("Không tìm thấy dịch vụ cần cập nhật.");
                        return;
                    }

                    dichVu.TenDichVu = txtDichVu.Text.Trim();
                    dichVu.DonGia = decimal.Parse(txtBaoGia.Text);
                    dichVu.DateModify = DateTime.Now;
                    dichVu.UserModify = Class.CurrentUser.Id;
                    db.SaveChanges();
                    MessageBox.Show("Cập nhật dịch vụ thành công!");
                }
                else
                {
                    if (!_idDonHang.HasValue)
                    {
                        MessageBox.Show("Không xác định được đơn hàng để thêm dịch vụ.");
                        return;
                    }

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
                    MessageBox.Show("Thêm dịch vụ thành công!");
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
            else
            {
                errorProvider1.SetError(txtDichVu, "");
            }

            if (string.IsNullOrWhiteSpace(txtBaoGia.Text))
            {
                errorProvider1.SetError(txtBaoGia, "Báo giá không được để trống!");
                validate = false;
            }
            else
            {
                errorProvider1.SetError(txtBaoGia, "");
            }

            return validate;
        }
    }
}
