using Microsoft.Reporting.WinForms;
using PhoneCare.Data;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare.Forms.BaoCao
{
    public partial class frmHoaDon : Form
    {
        private readonly int _idDonHang;

        /// <summary>
        /// Khởi tạo đối tượng frmHoaDon.
        /// </summary>
        public frmHoaDon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Khởi tạo đối tượng frmHoaDon.
        /// </summary>
        public frmHoaDon(int idDonHang) : this()
        {
            _idDonHang = idDonHang;
        }

        /// <summary>
        /// Khởi tạo và tải dữ liệu khi biểu mẫu frmHoaDon_Load được hiển thị.
        /// </summary>
        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            if (_idDonHang <= 0)
            {
                MessageBox.Show("Không xác định được đơn hàng cần in hóa đơn.");
                Close();
                return;
            }

            LoadHoaDon();
        }

        /// <summary>
        /// Tải dữ liệu đơn hàng và hiển thị báo cáo hóa đơn.
        /// </summary>
        private void LoadHoaDon()
        {
            using (var db = new PhoneCareDbContext())
            {
                var donHang = db.DonHangs
                    .Include(x => x.NhanVien)
                    .Include(x => x.CoSoCuaHang)
                    .FirstOrDefault(x => x.Id == _idDonHang && x.IsDeleted == false);

                if (donHang == null)
                {
                    MessageBox.Show("Không tìm thấy đơn hàng.");
                    Close();
                    return;
                }

                var dichVus = db.DichVus
                    .Where(x => x.IdDonHang == _idDonHang && x.IsDeleted == false)
                    .OrderBy(x => x.Id)
                    .ToList();

                var table = TaoBangHoaDon();
                var tongTien = dichVus.Sum(x => x.DonGia);
                var nguoiLap = Class.CurrentUser.FullName;
                var nguoiNhanMay = donHang.NhanVien != null ? donHang.NhanVien.FullName : nguoiLap;
                var coSo = donHang.CoSoCuaHang;

                if (dichVus.Count == 0)
                {
                    AddHoaDonRow(table, donHang, coSo, nguoiLap, nguoiNhanMay, tongTien, 0, string.Empty, 0);
                }
                else
                {
                    for (int i = 0; i < dichVus.Count; i++)
                    {
                        AddHoaDonRow(
                            table,
                            donHang,
                            coSo,
                            nguoiLap,
                            nguoiNhanMay,
                            tongTien,
                            i + 1,
                            dichVus[i].TenDichVu,
                            dichVus[i].DonGia
                        );
                    }
                }

                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("HoaDonDataSet", table));
                reportViewer1.LocalReport.ReportEmbeddedResource = "PhoneCare.BaoCao.HoaDon.rdlc";
                reportViewer1.RefreshReport();
            }
        }

        /// <summary>
        /// Tạo cấu trúc bảng dữ liệu dùng để hiển thị hóa đơn.
        /// </summary>
        private DataTable TaoBangHoaDon()
        {
            var table = new DataTable("HoaDonDataSet");

            table.Columns.Add("MaPhieu", typeof(string));
            table.Columns.Add("ThoiGian", typeof(string));
            table.Columns.Add("NguoiLap", typeof(string));
            table.Columns.Add("NguoiThu", typeof(string));
            table.Columns.Add("LoaiDichVu", typeof(string));
            table.Columns.Add("TenKH", typeof(string));
            table.Columns.Add("DiaChi", typeof(string));
            table.Columns.Add("SoDT", typeof(string));
            table.Columns.Add("LoaiMay", typeof(string));
            table.Columns.Add("Mau", typeof(string));
            table.Columns.Add("IMEI", typeof(string));
            table.Columns.Add("Password", typeof(string));
            table.Columns.Add("TongTien", typeof(decimal));
            table.Columns.Add("NguoiNhanMay", typeof(string));
            table.Columns.Add("STT", typeof(int));
            table.Columns.Add("TenDichVu", typeof(string));
            table.Columns.Add("DonGia", typeof(decimal));
            table.Columns.Add("DiaChiCuaHang", typeof(string));
            table.Columns.Add("DienThoaiCuaHang", typeof(string));

            return table;
        }

        /// <summary>
        /// Thêm một dòng dữ liệu vào bảng hóa đơn.
        /// </summary>
        private void AddHoaDonRow(
            DataTable table,
            Models.DonHang donHang,
            Models.CoSoCuaHang coSo,
            string nguoiLap,
            string nguoiNhanMay,
            decimal tongTien,
            int stt,
            string tenDichVu,
            decimal donGia)
        {
            table.Rows.Add(
                "#" + donHang.Id,
                DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                nguoiLap,
                nguoiLap,
                donHang.LoaiDichVu,
                donHang.TenKH,
                donHang.DiaChi,
                donHang.SoDT,
                donHang.LoaiMay,
                donHang.Mau,
                donHang.IMEI,
                string.Empty,
                tongTien,
                nguoiNhanMay,
                stt,
                tenDichVu,
                donGia,
                coSo != null ? coSo.Address : string.Empty,
                coSo != null ? (string.IsNullOrWhiteSpace(coSo.Hotline) ? coSo.HomePhone : coSo.Hotline) : string.Empty
            );
        }
    }
}
