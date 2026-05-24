using PhoneCare.Data;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static PhoneCare.Forms.QuanTriDonHang.frmThemMoiDonHang;

namespace PhoneCare.Forms.QuanTriDonHang
{
    public partial class frmDanhSachDonHang : Form
    {
        private PhoneCareDbContext _context = new PhoneCareDbContext();
        private int _page = 1;
        private int _pageSize = 10;
        private int _total = 0;
        private bool _isLoadingStaticData = false;
        public frmDanhSachDonHang()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            frmThemMoiDonHang f = new frmThemMoiDonHang(this);
            f.ShowDialog();
        }

        private void frmDanhSachDonHang_Load(object sender, EventArgs e)
        {
            LoadStaticData();
            LoadData();
        }


        private void LoadStaticData()
        {
            _isLoadingStaticData = true;

            cbPageSize.Items.Clear();
            cbPageSize.Items.AddRange(new object[] { 10, 30, 50, 100 });
            cbPageSize.SelectedIndex = 0;

            cbTieuChi.Items.Clear();
            cbTieuChi.Items.AddRange(new object[] {
                "ID","Tên KH","SĐT","IMEI","Kỹ thuật"
            });
            cbTieuChi.SelectedIndex = 0;

            var list = new[]
            {
                new { Text = "Chờ sửa", Value = (int)RepairStatus.ChoSua },
                new { Text = "Đang sửa", Value = (int)RepairStatus.DangSua },
                new { Text = "Không sửa được", Value = (int)RepairStatus.KhongSuaDuoc },
                new { Text = "Khách không sửa", Value = (int)RepairStatus.KhachKhongSua },
                new { Text = "Đã trả khách", Value = (int)RepairStatus.DaTraKhach }
            };

            cbTrangThai.DataSource = list;
            cbTrangThai.DisplayMember = "Text";
            cbTrangThai.ValueMember = "Value";
            cbTrangThai.SelectedIndex = -1;

            cbCoSo.DataSource = _context.CoSoCuaHangs
                .Where(x => !x.IsDeleted)
                .ToList();
            cbCoSo.DisplayMember = "Name";
            cbCoSo.ValueMember = "Id";
            cbCoSo.SelectedIndex = -1;

            _isLoadingStaticData = false;
        }

        public void LoadData()
        {
            var query = BuildFilteredQuery();

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
                TinhTrang = GetTinhTrangText(x.TinhTrang),
                x.Level
            }).ToList();

            dgvDonHang.DataSource = data;
            _total = total;
            UpdatePagingInfo(data.Count, total);
        }

        private IQueryable<Models.DonHang> BuildFilteredQuery()
        {
            var query = _context.DonHangs
                .Include("NhanVien")
                .Include("CoSoCuaHang")
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
                        if (int.TryParse(keyword, out int imei))
                            query = query.Where(x => x.IMEI == imei);
                        break;
                    case "Kỹ thuật":
                        query = query.Where(x => x.LoaiKyThuat.Contains(keyword));
                        break;
                }
            }

            // Trạng thái
            if (cbTrangThai.SelectedIndex >= 0)
            {
                int status = (int)cbTrangThai.SelectedValue;
                query = query.Where(x => x.TinhTrang == status);
            }

            // Cơ sở
            if (cbCoSo.SelectedIndex >= 0)
            {
                int cosoId = (int)cbCoSo.SelectedValue;
                query = query.Where(x => x.IdCoSo == cosoId);
            }

            // Date
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date.AddDays(1);

            query = query.Where(x => x.NgayNhan >= from && x.NgayNhan < to);

            return query;
        }

        private int GetLastPage(int total)
        {
            if (total <= 0) return 1;

            return (int)Math.Ceiling(total / (double)_pageSize);
        }

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

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            _page = 1;
            LoadData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtKeyword.Clear();
            cbTieuChi.SelectedIndex = 0;
            cbTrangThai.SelectedIndex = -1;
            cbCoSo.SelectedIndex = -1;
            _page = 1;
            LoadData();
        }

        private void cbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_isLoadingStaticData || cbPageSize.SelectedItem == null) return;

            _pageSize = int.Parse(cbPageSize.SelectedItem.ToString());
            _page = 1;
            LoadData();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_page >= GetLastPage(_total)) return;

            _page++;
            LoadData();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page--;
                LoadData();
            }
        }

        private void btnToTop_Click(object sender, EventArgs e)
        {
            if (_page > 1)
            {
                _page = 1;
                LoadData();
            }
        }

        private void btnToBottom_Click(object sender, EventArgs e)
        {
            _page = GetLastPage(_total);
            LoadData();
        }

        private void mnuThemDonHang_Click(object sender, EventArgs e)
        {
            frmThemMoiDonHang f = new frmThemMoiDonHang(this);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }

        private void mnuSuaDonHang_Click(object sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;
            int id = Convert.ToInt32(dgvDonHang.CurrentRow.Cells["Id"].Value);
            frmThemMoiDonHang f = new frmThemMoiDonHang(this, id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }

        private void mnuXoaDonHang_Click(object sender, EventArgs e)
        {
            if (dgvDonHang.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvDonHang.CurrentRow.Cells["Id"].Value);
            var result = MessageBox.Show("Bạn có chắc muốn xóa đơn hàng này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No) { return; }

            var donHang = _context.DonHangs.FirstOrDefault(x => x.Id == id);
            if (donHang != null)
            {
                donHang.IsDeleted = true;
                donHang.DateModify = DateTime.Now;
                donHang.UserModify = Class.CurrentUser.Id;

                _context.SaveChanges();

                MessageBox.Show("Xóa thành công!");
                LoadData();
            }
            else
            {
                MessageBox.Show("Không tìm thấy dữ liệu!");
            }
        }

        private void btnXuatExcel_Click(object sender, EventArgs e)
        {
            var data = BuildFilteredQuery()
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
                dialog.Filter = "Excel CSV (*.csv)|*.csv";
                dialog.FileName = $"DanhSachDonHang_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (dialog.ShowDialog(this) != DialogResult.OK) return;

                using (var writer = new StreamWriter(dialog.FileName, false, new UTF8Encoding(true)))
                {
                    WriteCsvRow(writer, new[]
                    {
                        "STT",
                        "ID",
                        "Tên KH",
                        "SĐT",
                        "Loại máy",
                        "IMEI",
                        "Ngày nhận",
                        "Người nhận",
                        "Kỹ thuật",
                        "Trạng thái",
                        "Level",
                        "Cơ sở"
                    });

                    for (int i = 0; i < data.Count; i++)
                    {
                        var item = data[i];
                        WriteCsvRow(writer, new[]
                        {
                            (i + 1).ToString(),
                            item.Id.ToString(),
                            item.TenKH,
                            item.SoDT,
                            item.LoaiMay,
                            item.IMEI.ToString(),
                            item.NgayNhan.HasValue ? item.NgayNhan.Value.ToString("dd/MM/yyyy HH:mm") : "",
                            item.NhanVien != null ? item.NhanVien.FullName : "",
                            item.LoaiKyThuat,
                            GetTinhTrangText(item.TinhTrang),
                            item.Level.ToString(),
                            item.CoSoCuaHang != null ? item.CoSoCuaHang.Name : ""
                        });
                    }
                }

                MessageBox.Show("Xuất Excel thành công!");
            }
        }

        private void WriteCsvRow(StreamWriter writer, string[] values)
        {
            writer.WriteLine(string.Join(",", values.Select(EscapeCsvValue)));
        }

        private string EscapeCsvValue(string value)
        {
            value = value ?? "";
            value = value.Replace("\"", "\"\"");

            return $"\"{value}\"";
        }

        private string GetTinhTrangText(int value)
        {
            switch ((RepairStatus)value)
            {
                case RepairStatus.ChoSua:
                    return "Chờ sửa";
                case RepairStatus.DangSua:
                    return "Đang sửa";
                case RepairStatus.KhongSuaDuoc:
                    return "Không sửa được";
                case RepairStatus.KhachKhongSua:
                    return "Khách không sửa";
                case RepairStatus.DaTraKhach:
                    return "Đã trả khách";
                default:
                    return value.ToString();
            }
        }
    }
}
