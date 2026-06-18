using PhoneCare.Data;
using System;
using System.Linq;
using System.Windows.Forms;

namespace PhoneCare.Forms.QuanTriNhanVien
{
    public partial class frmQuanTriNhanVien : Form
    {
        public frmQuanTriNhanVien()
        {
            InitializeComponent();
        }

        public void LoadNhanVien()
        {
            using (var db = new PhoneCareDbContext())
            {
                var list = db.NhanViens
                             .Where(x => !x.IsDeleted)
                             .Select(x => new
                             {
                                 x.Id,
                                 x.UserName,
                                 x.FullName,
                                 x.Email,
                                 x.Phone,
                                 TypeEmployee = x.LoaiNhanVien,
                                 WorkPlaceName = x.CoSoCuaHang.Name
                             })
                             .ToList();
                dgvNhanVien.DataSource = list;
            }
        }

        private void frmQuanTriNhanVien_Load(object sender, EventArgs e)
        {
            dgvNhanVien.AutoGenerateColumns = true;
            LoadNhanVien();
            dgvNhanVien.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void ctmThemMoi_Click(object sender, EventArgs e)
        {
            var f = new frmThemMoiNhanVien(this);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.ShowDialog(this);
        }

        private void ctmChinhSua_Click(object sender, EventArgs e)
        {
            if (dgvNhanVien.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvNhanVien.CurrentRow.Cells["Id"].Value);
            var f = new frmThemMoiNhanVien(this, id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog(this);
        }

        private void ctmXoa_Click(object sender, EventArgs e)
        {
            if (dgvNhanVien.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvNhanVien.CurrentRow.Cells["Id"].Value);

            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa nhân viên này không?",
                "Xác nhận Xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.No) return;

            using (var db = new PhoneCareDbContext())
            {
                var nhanvien = db.NhanViens.FirstOrDefault(x => x.Id == id && !x.IsDeleted);

                if (nhanvien == null)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu!");
                    return;
                }

                nhanvien.IsDeleted = true;
                nhanvien.DateModify = DateTime.Now;
                nhanvien.UserModify = Class.CurrentUser.Id;

                db.SaveChanges();
            }

            MessageBox.Show("Xóa nhân viên thành công!");
            LoadNhanVien();
        }
    }
}
