using PhoneCare.Data;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare.Forms.QuanTriDonHang
{
    public partial class frmDanhSachDonHang : Form
    {
        private PhoneCareDbContext _context = new PhoneCareDbContext();
        private int _page = 1;
        private int _pageSize = 10;
        private int _total = 0;
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
            cbPageSize.Items.AddRange(new object[] { 10, 30, 50, 100 });
            cbPageSize.SelectedIndex = 0;

            cbTieuChi.Items.AddRange(new object[] {
                "ID","Tên KH","SĐT","IMEI","Kỹ thuật"
            });

            cbTrangThai.Items.AddRange(new object[] {
                "Chờ sửa","Đang sửa","Đã sửa","Không sửa được","Khách không sửa","Đã trả khách"
            });

            cbCoSo.DataSource = _context.CoSoCuaHangs
                .Where(x => !x.IsDeleted)
                .ToList();
            cbCoSo.DisplayMember = "Name";
            cbCoSo.ValueMember = "Id";
            cbCoSo.SelectedIndex = -1;
        }

        public void LoadData()
        {
            var query = _context.DonHangs
            .Include("NhanVien")
            .Include("CoSoCuaHang")
            .Where(x => !x.IsDeleted)
            .AsQueryable();

            // Search
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
                }
            }

            // Trạng thái
            if (cbTrangThai.SelectedIndex >= 0)
            {
                string status = cbTrangThai.Text;
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
            DateTime to = dtTo.Value.Date;
            query = query.Where(x => x.NgayNhan >= from && x.NgayNhan <= to);

            int total = query.Count();

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
                DichVu = string.Join(",", x.DichVus.Select(d => d.TenDichVu)),
                x.NgayNhan,
                NguoiNhan = x.NhanVien != null ? x.NhanVien.FullName : "",
                x.LoaiKyThuat,
                x.TinhTrang,
                x.Level
            }).ToList();

            dgvDonHang.DataSource = data;
            _total = total;
            lblPaging.Text = $"{((_page - 1) * _pageSize) + 1} - {((_page - 1) * _pageSize) + data.Count} / {total}";
            lblTotal.Text = $"Tổng: {data.Count}";
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            _page = 1;
            LoadData();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtKeyword.Clear();
            cbTrangThai.SelectedIndex = -1;
            cbCoSo.SelectedIndex = -1;
            LoadData();
        }

        private void cbPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pageSize = int.Parse(cbPageSize.SelectedItem.ToString());
            _page = 1;
            LoadData();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
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
            _page = _total / _pageSize;
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
            } else
            {
                MessageBox.Show("Không tìm thấy dữ liệu!");
            }
        }
    }
}
