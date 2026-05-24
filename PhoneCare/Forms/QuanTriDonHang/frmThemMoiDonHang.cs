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
            bool isEdit = _id.HasValue;

            ConfigureDichVuGrid();
            dgvDichVu.DataSource = _dsDichVu;
            LoadKyThuat();
            LoadTinhTrang();
            LoadLevel();

            groupBox4.Visible = isEdit;
            btnInHoaDon.Visible = isEdit;
            btnInPhieuNhan.Visible = isEdit;
            mnuThemDichVu.Visible = isEdit;
            mnuSuaDichVu.Visible = isEdit;
            mnuXoaDichVu.Visible = isEdit;
            ApplyFormModeLayout(isEdit);

            if (isEdit)
            {
                LoadDichVu();
                this.Text = "Chỉnh sửa đơn hàng";
                LoadDataForEdit();
            }
            else
            {
                this.Text = "Thêm mới đơn hàng";
                lblTongTien.Text = "0 VND";
            }
        }

        private void ApplyFormModeLayout(bool isEdit)
        {
            if (isEdit)
            {
                groupBox4.Visible = true;
                groupBox5.Top = 574;
                btnDong.Left = 350;
                btnLuu.Left = 817;
                ClientSize = new System.Drawing.Size(984, 735);
                return;
            }

            groupBox4.Visible = false;
            groupBox5.Top = txtTinhTrangMay.Bottom + 18;
            btnDong.Left = btnInHoaDon.Left;
            btnLuu.Left = btnInPhieuNhan.Left;
            ClientSize = new System.Drawing.Size(ClientSize.Width, groupBox5.Bottom + 16);
        }

        private void ConfigureDichVuGrid()
        {
            dgvDichVu.AutoGenerateColumns = false;
            dgvDichVu.AllowUserToAddRows = false;
            dgvDichVu.RowHeadersWidth = 35;
            dgvDichVu.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            if (dgvDichVu.Columns.Count > 0) return;

            dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Id",
                HeaderText = "ID",
                DataPropertyName = "Id",
                FillWeight = 12
            });

            dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenDichVu",
                HeaderText = "Dịch vụ",
                DataPropertyName = "TenDichVu",
                FillWeight = 68
            });

            dgvDichVu.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonGia",
                HeaderText = "Giá",
                DataPropertyName = "DonGia",
                DefaultCellStyle = { Format = "N0" },
                FillWeight = 20
            });
        }

        private void LoadLevel()
        {
            cbLevel.Items.AddRange(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
        }
        private void LoadKyThuat()
        {
            cbKyThuat.Items.AddRange(new string[] { "Kỹ thuật 1", "Kỹ thuật 2", "Kỹ thuật 3", "Kỹ thuật 4", "Kỹ thuật 5" });
        }

        public enum RepairStatus
        {
            ChoSua = 1,
            DangSua = 2,
            KhongSuaDuoc = 3,
            KhachKhongSua = 4,
            DaTraKhach = 5
        }

        private void LoadTinhTrang()
        {
            var list = new[]
            {
                new { Text = "Chờ sửa", Value = (int)RepairStatus.ChoSua },
                new { Text = "Đang sửa", Value = (int)RepairStatus.DangSua },
                new { Text = "Không sửa được", Value = (int)RepairStatus.KhongSuaDuoc },
                new { Text = "Khách không sửa", Value = (int)RepairStatus.KhachKhongSua },
                new { Text = "Đã trả khách", Value = (int)RepairStatus.DaTraKhach }
            };

            cbTinhTrang.DataSource = list;
            cbTinhTrang.DisplayMember = "Text";
            cbTinhTrang.ValueMember = "Value";
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
                    _donHang.IMEI = Convert.ToInt32(txtIMEI.Text);
                    _donHang.Mau = txtMau.Text;
                    _donHang.Password = txtPassword.Text;

                    _donHang.Level = Convert.ToInt32(cbLevel.SelectedItem);
                    _donHang.LoaiKyThuat = cbKyThuat.Text;

                    _donHang.TinhTrang = (int)cbTinhTrang.SelectedValue;
                    _donHang.TinhTrangMay = txtTinhTrangMay.Text;

                    _donHang.LoaiDichVu = GetLoaiDichVu();

                    _donHang.NgayNhan = DateTime.Now;
                    _donHang.IdNguoiNhan = Class.CurrentUser.Id;

                    _donHang.DateModify = DateTime.Now;
                    _donHang.UserModify = Class.CurrentUser.Id;
                    _donHang.IsDeleted = false;

                    _donHang.DichVus = _dsDichVu;
                    _donHang.IdCoSo = Class.CurrentUser.CoSoCuaHangId;

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
                //try
                //{
                _donHang.TenKH = txtTenKH.Text;
                _donHang.SoDT = txtSDT.Text;
                _donHang.DiaChi = txtDiaChi.Text;

                _donHang.LoaiMay = txtLoaiMay.Text;
                _donHang.IMEI = Convert.ToInt32(txtIMEI.Text);
                _donHang.Mau = txtMau.Text;
                _donHang.Password = txtPassword.Text;

                _donHang.Level = Convert.ToInt32(cbLevel.SelectedItem);
                _donHang.LoaiKyThuat = cbKyThuat.Text;

                _donHang.TinhTrang = (int)cbTinhTrang.SelectedValue;
                _donHang.TinhTrangMay = txtTinhTrangMay.Text;

                _donHang.LoaiDichVu = GetLoaiDichVu();

                _donHang.NgayNhan = DateTime.Now;
                _donHang.IdNguoiNhan = Class.CurrentUser.Id;

                _donHang.DateCreated = DateTime.Now;
                _donHang.UserCreated = Class.CurrentUser.Id;
                _donHang.IsDeleted = false;

                _donHang.DichVus = _dsDichVu;
                _donHang.IdCoSo = Class.CurrentUser.CoSoCuaHangId;

                _context.DonHangs.Add(_donHang);
                _context.SaveChanges();

                MessageBox.Show("Lưu thành công!");
                ClearForm();
                _parentForm.LoadData();
                this.Close();
                //}
                //catch (Exception ex)
                //{
                //    MessageBox.Show(ex.Message);
                //}
            }
        }

        private void mnuThemDichVu_Click(object sender, EventArgs e)
        {
            frmDichVu f = new frmDichVu(this, idDonHang: _id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }

        public void LoadDichVu()
        {
            _dsDichVu = new BindingList<DichVu>(
                _context.DichVus
               .Where(x => x.IdDonHang == _id && x.IsDeleted == false)
               .ToList()
            );
            ConfigureDichVuGrid();
            dgvDichVu.DataSource = null;
            dgvDichVu.DataSource = _dsDichVu;

            TinhTongTien();
        }

        private void mnuSuaDichVu_Click(object sender, EventArgs e)
        {
            if (dgvDichVu.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvDichVu.CurrentRow.Cells["Id"].Value);
            frmDichVu f = new frmDichVu(this, id: id);
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
                "Xóa dịch vụ",
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

        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            if (!_id.HasValue)
            {
                MessageBox.Show("Vui lòng lưu đơn hàng trước khi in hóa đơn.");
                return;
            }

            var form = new PhoneCare.Forms.BaoCao.frmHoaDon(_id.Value);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }

        private void btnInPhieuNhan_Click(object sender, EventArgs e)
        {
            if (!_id.HasValue)
            {
                MessageBox.Show("Vui lòng lưu đơn hàng trước khi in phiếu nhận.");
                return;
            }

            var form = new PhoneCare.Forms.BaoCao.frmPhieuNhanMay(_id.Value);
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }

        private void LoadDataForEdit()
        {
            using (var db = new PhoneCareDbContext())
            {
                var coso = db.DonHangs.FirstOrDefault(x => x.Id == _id);

                if (coso == null) return;
                //txtID.Text = coso.Id.ToString();
                //txtCode.Text = coso.Code;
                //txtName.Text = coso.Name;
                //txtAddress.Text = coso.Address;
                //txtHomePhone.Text = coso.HomePhone;
                //txtHotline.Text = coso.Hotline;
                txtTenKH.Text = coso.TenKH;
                txtSDT.Text = coso.SoDT;
                txtDiaChi.Text = coso.DiaChi;

                txtLoaiMay.Text = coso.LoaiMay;
                txtIMEI.Text = coso.IMEI.ToString();
                txtMau.Text = coso.Mau;
                txtPassword.Text = coso.Password;

                cbLevel.SelectedItem = coso.Level.ToString();
                if (cbKyThuat.Items.Contains(coso.LoaiKyThuat))
                {
                    cbKyThuat.SelectedItem = coso.LoaiKyThuat;
                }
                else
                {
                    cbKyThuat.Text = coso.LoaiKyThuat;
                }

                cbTinhTrang.SelectedValue = coso.TinhTrang;
                txtTinhTrangMay.Text = coso.TinhTrangMay;

                chkBaoHanh.Checked = coso.LoaiDichVu.Contains("Bảo hành");
                chkSuaChua.Checked = coso.LoaiDichVu.Contains("Sửa chữa");
                chkDichVu.Checked = coso.LoaiDichVu.Contains("Dịch vụ");
                chkCaiDat.Checked = coso.LoaiDichVu.Contains("Cài đặt");
            }
        }

        private void btnDong_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
