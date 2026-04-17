using PhoneCare.Data;
using PhoneCare.Forms.QuanTriCuaHang;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneCare.Forms.QuanTriNhanVien
{
    public partial class frmQuanTriNhanVien : Form
    {
        public frmQuanTriNhanVien()
        {
            InitializeComponent();
        }

        private void LoadNhanVien()
        {
            using (var db = new PhoneCareDbContext())
            {
                var list = db.NhanViens
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
                Console.WriteLine("kiem tra list: "+ list.Count());
                dgvNhanVien.DataSource = list;
            }
        }

        private void frmQuanTriNhanVien_Load(object sender, EventArgs e)
        {
            dgvNhanVien.AutoGenerateColumns = true;
            LoadNhanVien();
        }

        private void ctmThemMoi_Click(object sender, EventArgs e)
        {
            frmThemMoiNhanVien f = new frmThemMoiNhanVien(this);
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void ctmChinhSua_Click(object sender, EventArgs e)
        {
            if (dgvNhanVien.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvNhanVien.CurrentRow.Cells["Id"].Value);
            frmThemMoiNhanVien f = new frmThemMoiNhanVien(this, id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }

        private void ctmXoa_Click(object sender, EventArgs e)
        {
            if (dgvNhanVien.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvNhanVien.CurrentRow.Cells["Id"].Value);

            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa cửa hàng này không?",
                "Xác nhận Xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.No) return;

            using (var db = new PhoneCareDbContext())
            {
                var nhanvien = db.NhanViens.FirstOrDefault(x => x.Id == id);

                if (nhanvien == null)
                {
                    MessageBox.Show("Không tìm thấy dữ liệu!");
                    return;
                }

                nhanvien.IsDeleted = true;
                nhanvien.DateModify = DateTime.Now;
                nhanvien.UserModify = 1;

                db.SaveChanges();
            }

            MessageBox.Show("Xóa thành công!");

            LoadNhanVien();
        }
    }
}
