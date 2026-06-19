using PhoneCare.Class;
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
        private readonly PhoneCareDbContext _context = new PhoneCareDbContext();
        private BindingList<DichVu> _dsDichVu = new BindingList<DichVu>();
        private DonHang _donHang = new DonHang();
        private readonly frmDanhSachDonHang _parentForm;
        private readonly int? _id;

        /// <summary>
        /// Khởi tạo đối tượng frmThemMoiDonHang.
        /// </summary>
        public frmThemMoiDonHang(frmDanhSachDonHang parentForm, int? id = null)
        {
            InitializeComponent();
            _parentForm = parentForm;
            _id = id;
            FormClosed += (sender, args) => _context.Dispose();
        }

        /// <summary>
        /// Khởi tạo và tải dữ liệu khi biểu mẫu frmThemMoiDonHang_Load được hiển thị.
        /// </summary>
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
                Text = "Chỉnh sửa đơn hàng";
                LoadDataForEdit();
                LoadDichVu();
                ApplyServicePermission();
            }
            else
            {
                Text = "Thêm mới đơn hàng";
                lblTongTien.Text = "0 VND";
            }
        }

        /// <summary>
        /// Điều chỉnh bố cục và trạng thái điều khiển theo chế độ thêm mới hoặc chỉnh sửa.
        /// </summary>
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

        /// <summary>
        /// Thiết lập cột và cách hiển thị cho bảng danh sách dịch vụ.
        /// </summary>
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

        /// <summary>
        /// Tải danh sách mức độ hoặc loại kỹ thuật lên biểu mẫu.
        /// </summary>
        private void LoadLevel()
        {
            cbLevel.Items.Clear();
            cbLevel.Items.AddRange(new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" });
        }

        /// <summary>
        /// Tải danh sách kỹ thuật viên có thể phân công.
        /// </summary>
        private void LoadKyThuat()
        {
            cbKyThuat.Items.Clear();
            var technicians = _context.NhanViens
                .Where(x => !x.IsDeleted && !x.KhoaTaiKhoan && x.LoaiNhanVien == PermissionService.KyThuat)
                .OrderBy(x => x.FullName)
                .Select(x => x.FullName)
                .ToList();

            cbKyThuat.Items.AddRange(technicians.Cast<object>().ToArray());
        }

        /// <summary>
        /// Tải danh sách trạng thái sửa chữa lên biểu mẫu.
        /// </summary>
        private void LoadTinhTrang()
        {
            var list = new[]
            {
                new { Text = "Chờ sửa", Value = (int)RepairStatus.ChoSua },
                new { Text = "Đang sửa", Value = (int)RepairStatus.DangSua },
                new { Text = "Đã sửa", Value = (int)RepairStatus.DaSua },
                new { Text = "Không sửa được", Value = (int)RepairStatus.KhongSuaDuoc },
                new { Text = "Khách không sửa", Value = (int)RepairStatus.KhachKhongSua },
                new { Text = "Đã trả khách", Value = (int)RepairStatus.DaTraKhach }
            };

            cbTinhTrang.DataSource = list;
            cbTinhTrang.DisplayMember = "Text";
            cbTinhTrang.ValueMember = "Value";
        }

        /// <summary>
        /// Tính và hiển thị tổng tiền của các dịch vụ trong đơn hàng.
        /// </summary>
        private void TinhTongTien()
        {
            decimal tong = _dsDichVu.Sum(x => x.DonGia);
            lblTongTien.Text = tong.ToString("N0") + " VND";
        }

        /// <summary>
        /// Xác định loại dịch vụ đang được chọn trên biểu mẫu.
        /// </summary>
        private string GetLoaiDichVu()
        {
            var list = new List<string>();

            if (chkBaoHanh.Checked) list.Add("Bảo hành");
            if (chkSuaChua.Checked) list.Add("Sửa chữa");
            if (chkDichVu.Checked) list.Add("Dịch vụ");
            if (chkCaiDat.Checked) list.Add("Cài đặt");

            return string.Join(", ", list);
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnLuu_Click.
        /// </summary>
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

                if (!RepairStatusHelper.CanEditOrder(_donHang.TinhTrang))
                {
                    MessageBox.Show("Không thể sửa đơn hàng ở trạng thái hiện tại.");
                    return;
                }
            }

            try
            {
                MapFormToDonHang(_donHang);

                if (_id.HasValue)
                {
                    _donHang.DateModify = DateTime.Now;
                    _donHang.UserModify = Class.CurrentUser.Id;
                }
                else
                {
                    _donHang.NgayNhan = DateTime.Now;
                    _donHang.IdNguoiNhan = Class.CurrentUser.Id;
                    _donHang.DateCreated = DateTime.Now;
                    _donHang.UserCreated = Class.CurrentUser.Id;
                    _donHang.IsDeleted = false;
                    _donHang.IdCoSo = Class.CurrentUser.CoSoCuaHangId;
                    _context.DonHangs.Add(_donHang);
                }

                _context.SaveChanges();

                MessageBox.Show("Lưu thành công!");
                ClearForm();
                _parentForm.LoadData();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Sao chép dữ liệu từ biểu mẫu vào entity đơn hàng.
        /// </summary>
        private void MapFormToDonHang(DonHang donHang)
        {
            donHang.TenKH = txtTenKH.Text.Trim();
            donHang.SoDT = txtSDT.Text.Trim();
            donHang.DiaChi = txtDiaChi.Text.Trim();
            donHang.LoaiMay = txtLoaiMay.Text.Trim();
            donHang.IMEI = txtIMEI.Text.Trim();
            donHang.Mau = txtMau.Text.Trim();
            donHang.Password = txtPassword.Text;
            donHang.Level = Convert.ToInt32(cbLevel.SelectedItem);
            donHang.LoaiKyThuat = cbKyThuat.Text;
            donHang.TinhTrang = (int)cbTinhTrang.SelectedValue;
            donHang.TinhTrangMay = txtTinhTrangMay.Text.Trim();
            donHang.LoaiDichVu = GetLoaiDichVu();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuThemDichVu_Click.
        /// </summary>
        private void mnuThemDichVu_Click(object sender, EventArgs e)
        {
            if (!CanModifyServices()) return;

            var f = new frmDichVu(this, idDonHang: _id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);
        }

        /// <summary>
        /// Tải và hiển thị danh sách dịch vụ của đơn hàng.
        /// </summary>
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

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuSuaDichVu_Click.
        /// </summary>
        private void mnuSuaDichVu_Click(object sender, EventArgs e)
        {
            if (dgvDichVu.CurrentRow == null) return;
            if (!CanModifyServices()) return;

            int id = Convert.ToInt32(dgvDichVu.CurrentRow.Cells["Id"].Value);
            var f = new frmDichVu(this, id: id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);
        }

        /// <summary>
        /// Xóa các thông báo lỗi kiểm tra dữ liệu trên biểu mẫu.
        /// </summary>
        private void ClearError()
        {
            errorProvider1.SetError(txtTenKH, "");
            errorProvider1.SetError(txtSDT, "");
            errorProvider1.SetError(txtLoaiMay, "");
            errorProvider1.SetError(txtIMEI, "");
            errorProvider1.SetError(txtTinhTrangMay, "");
            errorProvider1.SetError(cbKyThuat, "");
            errorProvider1.SetError(cbTinhTrang, "");
            errorProvider1.SetError(cbLevel, "");
        }

        /// <summary>
        /// Kiểm tra dữ liệu nhập trên biểu mẫu và hiển thị lỗi tương ứng.
        /// </summary>
        private bool ValidateInput()
        {
            bool valid = true;
            ClearError();

            if (string.IsNullOrWhiteSpace(txtTenKH.Text))
            {
                errorProvider1.SetError(txtTenKH, "Tên khách hàng không được để trống");
                valid = false;
            }

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

            if (string.IsNullOrWhiteSpace(txtLoaiMay.Text))
            {
                errorProvider1.SetError(txtLoaiMay, "Loại máy không được để trống");
                valid = false;
            }

            string imei = txtIMEI.Text.Trim();
            if (string.IsNullOrWhiteSpace(imei))
            {
                errorProvider1.SetError(txtIMEI, "IMEI không được để trống");
                valid = false;
            }
            else if (!imei.All(char.IsDigit) || imei.Length < 14 || imei.Length > 17)
            {
                errorProvider1.SetError(txtIMEI, "IMEI phải là dãy số từ 14 đến 17 chữ số");
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(txtTinhTrangMay.Text))
            {
                errorProvider1.SetError(txtTinhTrangMay, "Tình trạng máy không được để trống");
                valid = false;
            }

            if (cbKyThuat.SelectedIndex < 0 && string.IsNullOrWhiteSpace(cbKyThuat.Text))
            {
                errorProvider1.SetError(cbKyThuat, "Vui lòng chọn kỹ thuật viên");
                valid = false;
            }

            if (cbTinhTrang.SelectedIndex < 0)
            {
                errorProvider1.SetError(cbTinhTrang, "Vui lòng chọn trạng thái");
                valid = false;
            }

            if (cbLevel.SelectedIndex < 0)
            {
                errorProvider1.SetError(cbLevel, "Vui lòng chọn level");
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Đưa các trường nhập liệu trên biểu mẫu về trạng thái ban đầu.
        /// </summary>
        private void ClearForm()
        {
            txtTenKH.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            txtLoaiMay.Clear();
            txtIMEI.Clear();
            txtMau.Clear();
            txtPassword.Clear();
            chkBaoHanh.Checked = false;
            chkSuaChua.Checked = false;
            chkDichVu.Checked = false;
            chkCaiDat.Checked = false;
            cbLevel.SelectedIndex = -1;
            txtTinhTrangMay.Clear();
            _dsDichVu.Clear();
            cbKyThuat.SelectedIndex = -1;
            cbTinhTrang.SelectedIndex = -1;
            lblTongTien.Text = "0 VNĐ";
            errorProvider1.Clear();
            txtTenKH.Focus();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuXoaDichVu_Click.
        /// </summary>
        private void mnuXoaDichVu_Click(object sender, EventArgs e)
        {
            if (dgvDichVu.CurrentRow == null) return;
            if (!CanModifyServices()) return;

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

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnInHoaDon_Click.
        /// </summary>
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

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnInPhieuNhan_Click.
        /// </summary>
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

        /// <summary>
        /// Tải dữ liệu hiện có lên biểu mẫu để chỉnh sửa.
        /// </summary>
        private void LoadDataForEdit()
        {
            _donHang = _context.DonHangs.FirstOrDefault(x => x.Id == _id && !x.IsDeleted);

            if (_donHang == null) return;

            txtTenKH.Text = _donHang.TenKH;
            txtSDT.Text = _donHang.SoDT;
            txtDiaChi.Text = _donHang.DiaChi;
            txtLoaiMay.Text = _donHang.LoaiMay;
            txtIMEI.Text = _donHang.IMEI;
            txtMau.Text = _donHang.Mau;
            txtPassword.Text = _donHang.Password;
            cbLevel.SelectedItem = _donHang.Level.ToString();

            if (cbKyThuat.Items.Contains(_donHang.LoaiKyThuat))
            {
                cbKyThuat.SelectedItem = _donHang.LoaiKyThuat;
            }
            else
            {
                cbKyThuat.Text = _donHang.LoaiKyThuat;
            }

            cbTinhTrang.SelectedValue = _donHang.TinhTrang;
            txtTinhTrangMay.Text = _donHang.TinhTrangMay;

            string loaiDichVu = _donHang.LoaiDichVu ?? string.Empty;
            chkBaoHanh.Checked = loaiDichVu.Contains("Bảo hành");
            chkSuaChua.Checked = loaiDichVu.Contains("Sửa chữa");
            chkDichVu.Checked = loaiDichVu.Contains("Dịch vụ");
            chkCaiDat.Checked = loaiDichVu.Contains("Cài đặt");
        }

        /// <summary>
        /// Kiểm tra đơn hàng và người dùng hiện tại có cho phép thay đổi dịch vụ hay không.
        /// </summary>
        public bool CanModifyServices()
        {
            if (!PermissionService.CanManageServices())
            {
                MessageBox.Show("Bạn không có quyền cập nhật dịch vụ.");
                return false;
            }

            int status = _donHang != null ? _donHang.TinhTrang : 0;
            if (!RepairStatusHelper.CanEditOrder(status))
            {
                MessageBox.Show("Không thể cập nhật dịch vụ ở trạng thái hiện tại.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Áp dụng quyền thao tác dịch vụ lên các điều khiển của giao diện.
        /// </summary>
        private void ApplyServicePermission()
        {
            bool enabled = PermissionService.CanManageServices() && RepairStatusHelper.CanEditOrder(_donHang.TinhTrang);
            mnuThemDichVu.Enabled = enabled;
            mnuSuaDichVu.Enabled = enabled;
            mnuXoaDichVu.Enabled = enabled;
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnDong_Click.
        /// </summary>
        private void btnDong_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
