using PhoneCare.Class;
using PhoneCare.Data;
using PhoneCare.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare.Forms.QuanTriDonHang
{
    public partial class frmDanhSachDonHang : Form
    {
        private int _page = 1;
        private int _pageSize = 10;
        private int _total = 0;
        private bool _isLoadingStaticData = false;

        /// <summary>
        /// Khởi tạo đối tượng frmDanhSachDonHang.
        /// </summary>
        public frmDanhSachDonHang()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnAdd_Click.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            OpenCreateForm();
        }

        /// <summary>
        /// Khởi tạo và tải dữ liệu khi biểu mẫu frmDanhSachDonHang_Load được hiển thị.
        /// </summary>
        private void frmDanhSachDonHang_Load(object sender, EventArgs e)
        {
            LoadStaticData();
            LoadData();
        }

        /// <summary>
        /// Tải các danh mục tĩnh dùng cho bộ lọc đơn hàng.
        /// </summary>
        private void LoadStaticData()
        {
            _isLoadingStaticData = true;

            cbPageSize.Items.Clear();
            cbPageSize.Items.AddRange(new object[] { 10, 30, 50, 100 });
            cbPageSize.SelectedIndex = 0;

            cbTieuChi.Items.Clear();
            cbTieuChi.Items.AddRange(new object[] {
                "ID","Tên KH","SĐT","IMEI","Kỹ thuật","Loại máy"
            });
            cbTieuChi.SelectedIndex = 0;

            cbTrangThai.DataSource = GetStatusList();
            cbTrangThai.DisplayMember = "Text";
            cbTrangThai.ValueMember = "Value";
            cbTrangThai.SelectedIndex = -1;

            using (var context = new PhoneCareDbContext())
            {
                cbCoSo.DataSource = context.CoSoCuaHangs
                    .Where(x => !x.IsDeleted)
                    .ToList();
            }

            cbCoSo.DisplayMember = "Name";
            cbCoSo.ValueMember = "Id";
            cbCoSo.SelectedIndex = -1;

            dtFrom.ShowCheckBox = true;
            dtTo.ShowCheckBox = true;
            dtFrom.Checked = false;
            dtTo.Checked = false;

            _isLoadingStaticData = false;
        }

        /// <summary>
        /// Tải danh sách dữ liệu theo bộ lọc và trang hiện tại lên giao diện.
        /// </summary>
        public void LoadData()
        {
            using (var context = new PhoneCareDbContext())
            {
                var query = BuildFilteredQuery(context);

                int total = query.Count();
                int lastPage = GetLastPage(total);
                if (_page > lastPage)
                {
                    _page = lastPage;
                }

                var data = query
                    .OrderByDescending(x => x.Id)
                    .Skip((_page - 1) * _pageSize)
                    .Take(_pageSize)
                    .ToList()
                    .Select(x => new
                    {
                        x.Id,
                        x.TenKH,
                        x.SoDT,
                        x.LoaiMay,
                        x.IMEI,
                        x.NgayNhan,
                        NguoiNhan = x.NhanVien != null ? x.NhanVien.FullName : "",
                        x.LoaiKyThuat,
                        TinhTrang = RepairStatusHelper.GetText(x.TinhTrang),
                        x.Level
                    }).ToList();

                dgvDonHang.DataSource = data;
                _total = total;
                UpdatePagingInfo(data.Count, total);
            }
        }

        /// <summary>
        /// Tạo truy vấn đơn hàng theo các điều kiện lọc được cung cấp.
        /// </summary>
        private IQueryable<DonHang> BuildFilteredQuery(PhoneCareDbContext context)
        {
            var query = context.DonHangs
                .Include(x => x.NhanVien)
                .Include(x => x.CoSoCuaHang)
                .Where(x => !x.IsDeleted)
                .AsQueryable();

            string keyword = txtKeyword.Text.Trim();
            string tieuChi = cbTieuChi.Text;

            if (!string.IsNullOrEmpty(keyword))
            {
                switch (tieuChi)
                {
                    case "ID":
                        if (int.TryParse(keyword, out int id))
                            query = query.Where(x => x.Id == id);
                        break;
                    case "Tên KH":
                        query = query.Where(x => x.TenKH.Contains(keyword));
                        break;
                    case "SĐT":
                        query = query.Where(x => x.SoDT.Contains(keyword));
                        break;
                    case "IMEI":
                        query = query.Where(x => x.IMEI.Contains(keyword));
                        break;
                    case "Kỹ thuật":
                        query = query.Where(x => x.LoaiKyThuat.Contains(keyword));
                        break;
                    case "Loại máy":
                        query = query.Where(x => x.LoaiMay.Contains(keyword));
                        break;
                }
            }

            if (cbTrangThai.SelectedIndex >= 0)
            {
                int status = (int)cbTrangThai.SelectedValue;
                query = query.Where(x => x.TinhTrang == status);
            }

            if (cbCoSo.SelectedIndex >= 0)
            {
                int cosoId = (int)cbCoSo.SelectedValue;
                query = query.Where(x => x.IdCoSo == cosoId);
            }

            if (dtFrom.Checked)
            {
                DateTime from = dtFrom.Value.Date;
                query = query.Where(x => x.NgayNhan >= from);
            }

            if (dtTo.Checked)
            {
                DateTime to = dtTo.Value.Date.AddDays(1);
                query = query.Where(x => x.NgayNhan < to);
            }

            return query;
        }

        /// <summary>
        /// Tính số trang cuối cùng từ tổng số bản ghi và kích thước trang.
        /// </summary>
        private int GetLastPage(int total)
        {
            if (total <= 0) return 1;
            return (int)Math.Ceiling(total / (double)_pageSize);
        }

        /// <summary>
        /// Cập nhật thông tin và trạng thái điều khiển phân trang.
        /// </summary>
        private void UpdatePagingInfo(int rowCount, int total)
        {
            if (total == 0)
            {
                lblPaging.Text = "0 - 0 / 0";
            }
            else
            {
                int from = ((_page - 1) * _pageSize) + 1;
                int to = from + rowCount - 1;
                lblPaging.Text = $"{from} - {to} / {total}";
            }

            lblTotal.Text = $"Tổng: {total}";

            int lastPage = GetLastPage(total);
            btnToTop.Enabled = _page > 1;
            btnPrev.Enabled = _page > 1;
            btnNext.Enabled = _page < lastPage;
            btnToBottom.Enabled = _page < lastPage;
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnTimKiem_Click.
        /// </summary>
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            _page = 1;
            LoadData();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnRefresh_Click.
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtKeyword.Clear();
            cbTieuChi.SelectedIndex = 0;
            cbTrangThai.SelectedIndex = -1;
            cbCoSo.SelectedIndex = -1;
            dtFrom.Checked = false;
            dtTo.Checked = false;
            _page = 1;
            LoadData();
        }

        /// <summary>
        /// Cập nhật dữ liệu khi lựa chọn trên điều khiển thay đổi.
        /// </summary>
        private void cbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoadingStaticData || cbPageSize.SelectedItem == null) return;

            _pageSize = int.Parse(cbPageSize.SelectedItem.ToString());
            _page = 1;
            LoadData();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnNext_Click.
        /// </summary>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_page >= GetLastPage(_total)) return;
            _page++;
            LoadData();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnPrev_Click.
        /// </summary>
        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page--;
                LoadData();
            }
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnToTop_Click.
        /// </summary>
        private void btnToTop_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page = 1;
                LoadData();
            }
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnToBottom_Click.
        /// </summary>
        private void btnToBottom_Click(object sender, EventArgs e)
        {
            _page = GetLastPage(_total);
            LoadData();
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuThemDonHang_Click.
        /// </summary>
        private void mnuThemDonHang_Click(object sender, EventArgs e)
        {
            OpenCreateForm();
        }

        /// <summary>
        /// Mở biểu mẫu tạo đơn hàng mới và làm mới danh sách sau khi đóng.
        /// </summary>
        private void OpenCreateForm()
        {
            if (!PermissionService.CanEditOrders())
            {
                MessageBox.Show("Bạn không có quyền thêm đơn hàng.");
                return;
            }

            var f = new frmThemMoiDonHang(this);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuSuaDonHang_Click.
        /// </summary>
        private void mnuSuaDonHang_Click(object sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;
            if (!PermissionService.CanEditOrders())
            {
                MessageBox.Show("Bạn không có quyền sửa đơn hàng.");
                return;
            }

            int id = Convert.ToInt32(dgvDonHang.CurrentRow.Cells["Id"].Value);
            using (var context = new PhoneCareDbContext())
            {
                var donHang = context.DonHangs.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
                if (donHang == null)
                {
                    MessageBox.Show("Không tìm thấy đơn hàng.");
                    return;
                }

                if (!RepairStatusHelper.CanEditOrder(donHang.TinhTrang))
                {
                    MessageBox.Show("Không thể sửa đơn hàng ở trạng thái hiện tại.");
                    return;
                }
            }

            var f = new frmThemMoiDonHang(this, id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh mnuXoaDonHang_Click.
        /// </summary>
        private void mnuXoaDonHang_Click(object sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvDonHang.CurrentRow.Cells["Id"].Value);
            var result = MessageBox.Show("Bạn có chắc muốn xóa đơn hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No) return;

            using (var context = new PhoneCareDbContext())
            {
                var donHang = context.DonHangs.FirstOrDefault(x => x.Id == id && !x.IsDeleted);
                if (donHang != null)
                {
                    donHang.IsDeleted = true;
                    donHang.DateModify = DateTime.Now;
                    donHang.UserModify = Class.CurrentUser.Id;

                    context.SaveChanges();

                    MessageBox.Show("Xóa thành công!");
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Không tìm thấy dữ liệu!");
                }
            }
        }

        /// <summary>
        /// Xử lý sự kiện nhấn nút hoặc mục lệnh btnXuatExcel_Click.
        /// </summary>
        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            using (var context = new PhoneCareDbContext())
            {
                var data = BuildFilteredQuery(context)
                    .OrderByDescending(x => x.Id)
                    .ToList();

                if (data.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu để xuất Excel.");
                    return;
                }

                using (var dialog = new SaveFileDialog())
                {
                    dialog.Title = "Xuất danh sách đơn hàng";
                    dialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
                    dialog.FileName = $"DanhSachDonHang_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    if (dialog.ShowDialog(this) != DialogResult.OK) return;

                    var headers = new List<string>
                    {
                        "STT", "ID", "Tên KH", "SĐT", "Loại máy", "IMEI", "Ngày nhận",
                        "Người nhận", "Kỹ thuật", "Trạng thái", "Level", "Cơ sở"
                    };

                    var rows = data.Select((item, index) => (IList<string>)new List<string>
                    {
                        (index + 1).ToString(),
                        item.Id.ToString(),
                        item.TenKH,
                        item.SoDT,
                        item.LoaiMay,
                        item.IMEI,
                        item.NgayNhan.HasValue ? item.NgayNhan.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        item.NhanVien != null ? item.NhanVien.FullName : "",
                        item.LoaiKyThuat,
                        RepairStatusHelper.GetText(item.TinhTrang),
                        item.Level.ToString(),
                        item.CoSoCuaHang != null ? item.CoSoCuaHang.Name : ""
                    }).ToList();

                    ExcelExporter.Export(dialog.FileName, "Danh sách đơn hàng", headers, rows);
                    MessageBox.Show("Xuất Excel thành công!");
                }
            }
        }

        /// <summary>
        /// Tạo danh sách trạng thái dùng cho bộ lọc đơn hàng.
        /// </summary>
        private object[] GetStatusList()
        {
            return new[]
            {
                new { Text = "Chờ sửa", Value = (int)RepairStatus.ChoSua },
                new { Text = "Đang sửa", Value = (int)RepairStatus.DangSua },
                new { Text = "Đã sửa", Value = (int)RepairStatus.DaSua },
                new { Text = "Không sửa được", Value = (int)RepairStatus.KhongSuaDuoc },
                new { Text = "Khách không sửa", Value = (int)RepairStatus.KhachKhongSua },
                new { Text = "Đã trả khách", Value = (int)RepairStatus.DaTraKhach }
            };
        }
    }
}
