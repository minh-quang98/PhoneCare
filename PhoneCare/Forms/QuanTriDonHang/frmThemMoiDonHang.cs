using PhoneCare.Data;
using PhoneCare.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare.Forms.QuanTriDonHang
{
    public partial class frmThemMoiDonHang : Form
    {
        private PhoneCareDbContext _context = new PhoneCareDbContext();
        private BindingList<DichVu> _dsDichVu = new BindingList<DichVu>();
        private DonHang _donHang = new DonHang();
        private frmDanhSachDonHang _parentForm;
        private int? _id;

        public frmThemMoiDonHang(frmDanhSachDonHang parentForm, int? id = null)
        {
            InitializeComponent();
            _parentForm = parentForm;
            _id = id;
        }

        private void frmThemMoiDonHang_Load(object sender, EventArgs e)
        {
            dgvDichVu.AutoGenerateColumns = false;
            dgvDichVu.DataSource = _dsDichVu;
            LoadKyThuat();
            LoadTrangThai();
            LoadLevel();
            LoadDichVu();

            lblTongTien.Text = "0 VND";
        }
        private void LoadLevel()
        {
            cbLevel.Items.AddRange(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
        }
        private void LoadKyThuat()
        {
            cbKyThuat.Items.AddRange(new string[] { "Kỹ thuật 1", "Kỹ thuật 2", "Kỹ thuật 3", "Kỹ thuật 4", "Kỹ thuật 5" });
        }

        private void LoadTrangThai()
        {
            cbTinhTrang.Items.AddRange(new string[]
            {
                "Chờ sửa",
                "Đang sửa",
                "Chờ sửa",
                "Không sửa được",
                "Khách không sửa", 
                "Đã trả khách",
            });
        }

        private void TinhTongTien()
        {
            decimal tong = _dsDichVu.Sum(x => x.DonGia);

            lblTongTien.Text = tong.ToString("N0") + " VND";
        }

        private string GetLoaiDichVu()
        {
            List<string> list = new List<string>();

            if (chkBaoHanh.Checked) list.Add("Bảo hành");
            if (chkSuaChua.Checked) list.Add("Sửa chữa");
            if (chkDichVu.Checked) list.Add("Dịch vụ");
            if (chkCaiDat.Checked) list.Add("Cài đặt");

            return string.Join(", ", list);
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            if (_id.HasValue)
            {
                _donHang = _context.DonHangs.Find(_id.Value);
                if (_donHang == null)
                {
                    MessageBox.Show("Đơn hàng không tồn tại");
                    return;
                }
                try
                {
                    _donHang.TenKH = txtTenKH.Text;
                    _donHang.SoDT = txtSDT.Text;
                    _donHang.DiaChi = txtDiaChi.Text;

                    _donHang.LoaiMay = txtLoaiMay.Text;
                    _donHang.IMEI = txtIMEI.Text;
                    _donHang.Mau = txtMau.Text;
                    _donHang.Password = txtPassword.Text;

                    _donHang.Level = (int)cbLevel.SelectedItem;
                    _donHang.LoaiKyThuat = (string)cbKyThuat.SelectedValue;

                    _donHang.TinhTrang = cbTinhTrang.Text;
                    _donHang.TinhTrangMay = txtTinhTrangMay.Text;

                    _donHang.LoaiDichVu = GetLoaiDichVu();

                    _donHang.NgayNhan = DateTime.Now;
                    _donHang.IdNguoiNhan = Class.CurrentUser.Id;

                    _donHang.DateModify = DateTime.Now;
                    _donHang.UserModify = Class.CurrentUser.Id;
                    _donHang.IsDeleted = false;

                    _donHang.DichVus = _dsDichVu;

                    _context.SaveChanges();

                    MessageBox.Show("Lưu thành công!");
                    ClearForm();
                    _parentForm.LoadData();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    _donHang.TenKH = txtTenKH.Text;
                    _donHang.SoDT = txtSDT.Text;
                    _donHang.DiaChi = txtDiaChi.Text;

                    _donHang.LoaiMay = txtLoaiMay.Text;
                    _donHang.IMEI = txtIMEI.Text;
                    _donHang.Mau = txtMau.Text;
                    _donHang.Password = txtPassword.Text;

                    _donHang.Level = (int)cbLevel.SelectedItem;
                    _donHang.LoaiKyThuat = (string)cbKyThuat.SelectedValue;

                    _donHang.TinhTrang = cbTinhTrang.Text;
                    _donHang.TinhTrangMay = txtTinhTrangMay.Text;

                    _donHang.LoaiDichVu = GetLoaiDichVu();

                    _donHang.NgayNhan = DateTime.Now;
                    _donHang.IdNguoiNhan = Class.CurrentUser.Id;

                    _donHang.DateCreated = DateTime.Now;
                    _donHang.UserCreated = Class.CurrentUser.Id;
                    _donHang.IsDeleted = false;

                    _donHang.DichVus = _dsDichVu;

                    _context.DonHangs.Add(_donHang);
                    _context.SaveChanges();

                    MessageBox.Show("Lưu thành công!");
                    ClearForm();
                    _parentForm.LoadData();
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void mnuThemDichVu_Click(object sender, EventArgs e)
        {
            frmDichVu f = new frmDichVu(this);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }

        public void LoadDichVu()
        {
            _dsDichVu = new BindingList<DichVu>(_context.DichVus.Where(x => x.IdDonHang == _donHang.Id).ToList());
            dgvDichVu.DataSource = _dsDichVu;
            TinhTongTien();
        }

        private void mnuSuaDichVu_Click(object sender, EventArgs e)
        {
            if (dgvDichVu.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvDichVu.CurrentRow.Cells["Id"].Value);
            frmDichVu f = new frmDichVu(this, id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }

        private void ClearError()
        {
            errorProvider1.SetError(txtTenKH, "");
            errorProvider1.SetError(txtSDT, "");
            errorProvider1.SetError(txtLoaiMay, "");
            errorProvider1.SetError(txtIMEI, "");
            errorProvider1.SetError(txtTinhTrangMay, "");
            errorProvider1.SetError(cbKyThuat, "");
            errorProvider1.SetError(cbTinhTrang, "");
        }

        private bool ValidateInput()
        {
            bool valid = true;
            ClearError();

            // 1. Tên khách hàng
            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                errorProvider1.SetError(txtTenKH, "Tên khách hàng không được để trống");
                valid = false;
            }

            // 2. SĐT
            if (string.IsNullOrWhiteSpace(txtSDT.Text))
            {
                errorProvider1.SetError(txtSDT, "Số điện thoại không được để trống");
                valid = false;
            }
            else if (!txtSDT.Text.All(char.IsDigit))
            {
                errorProvider1.SetError(txtSDT, "Số điện thoại chỉ được nhập số");
                valid = false;
            }


            // 3. Loại máy
            if (string.IsNullOrWhiteSpace(txtLoaiMay.Text))
            {
                errorProvider1.SetError(txtLoaiMay, "Loại máy không được để trống");
                valid = false;
            }

            // 4. IMEI
            if (string.IsNullOrWhiteSpace(txtIMEI.Text))
            {
                errorProvider1.SetError(txtIMEI, "IMEI không được để trống");
                valid = false;
            }

            // 5. Tình trạng máy
            if (string.IsNullOrWhiteSpace(txtTinhTrangMay.Text))
            {
                errorProvider1.SetError(txtTinhTrangMay, "Tình trạng máy không được để trống");
                valid = false;
            }

            // 6. Chọn kỹ thuật
            if (cbKyThuat.SelectedIndex < 0)
            {
                errorProvider1.SetError(cbKyThuat, "Vui lòng chọn kỹ thuật viên");
                valid = false;
            }

            // 7. Trạng thái
            if (cbTinhTrang.SelectedIndex < 0)
            {
                errorProvider1.SetError(cbTinhTrang, "Vui lòng chọn trạng thái");
                valid = false;
            }

            // 8. Loại dịch vụ (checkbox)
            if (!chkBaoHanh.Checked && !chkSuaChua.Checked && !chkDichVu.Checked && !chkCaiDat.Checked)
            {
                errorProvider1.SetError(chkBaoHanh, "Chọn ít nhất 1 loại dịch vụ");
                valid = false;
            }

            // 9. Danh sách dịch vụ (grid)
            if (_dsDichVu == null || _dsDichVu.Count == 0)
            {
                errorProvider1.SetError(dgvDichVu, "Phải có ít nhất 1 dịch vụ");
                valid = false;
            }

            return valid;
        }

        private void ClearForm()
        {
            // ===== Thông tin khách hàng =====
            txtTenKH.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();

            // ===== Thông tin máy =====
            txtLoaiMay.Clear();
            txtIMEI.Clear();
            txtMau.Clear();
            txtPassword.Clear();

            // ===== Loại dịch vụ =====
            chkBaoHanh.Checked = false;
            chkSuaChua.Checked = false;
            chkDichVu.Checked = false;
            chkCaiDat.Checked = false;

            cbLevel.SelectedIndex = -1;

            // ===== Tình trạng =====
            txtTinhTrangMay.Clear();

            // ===== Danh sách dịch vụ =====
            _dsDichVu.Clear(); // BindingList → grid auto clear

            // ===== Dropdown thao tác =====
            cbKyThuat.SelectedIndex = -1;
            cbTinhTrang.SelectedIndex = -1;

            // ===== Tổng tiền =====
            lblTongTien.Text = "0 VNĐ";

            // ===== Clear validation =====
            errorProvider1.Clear();

            // ===== Focus lại =====
            txtTenKH.Focus();
        }

        private void mnuXoaDichVu_Click(object sender, EventArgs e)
        {
            if (dgvDichVu.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvDichVu.CurrentRow.Cells["Id"].Value);

            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa dịch vụ này không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.No) return;

            var dichVu = _context.DichVus.Find(id);
            if (dichVu == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu!");
                return;
            }

            try
            {
                dichVu.IsDeleted = true;
                dichVu.DateModify = DateTime.Now;
                dichVu.UserModify = Class.CurrentUser.Id;

                _context.SaveChanges();
                LoadDichVu();
                MessageBox.Show("Xóa dịch vụ thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
