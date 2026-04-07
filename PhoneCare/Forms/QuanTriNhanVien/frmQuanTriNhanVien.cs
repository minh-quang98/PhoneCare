using PhoneCare.Data;
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
                             .Where(x => x.IsDelete == 0) // nếu có soft delete
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
            Forms.QuanTriNhanVien.frmThemMoiNhanVien f = new Forms.QuanTriNhanVien.frmThemMoiNhanVien();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }
    }
}
