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

namespace PhoneCare.Forms.QuanTriCuaHang
{
    public partial class frmDanhSachCuaHang : Form
    {
        public frmDanhSachCuaHang()
        {
            InitializeComponent();
        }

        private void mnuThemMoi_Click(object sender, EventArgs e)
        {
            frmThemMoiCuaHang f = new frmThemMoiCuaHang(this);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }

        private void frmDanhSachCuaHang_Load(object sender, EventArgs e)
        {
            LoadCoSo();
            dgvCoSo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        public void LoadCoSo()
        {
            using (var db = new PhoneCareDbContext())
            {
                var list = db.CoSoCuaHangs
                             .Where(x => !x.IsDeleted)
                             .Select(x => new
                             {
                                 x.Id,
                                 x.Code,
                                 x.Name,
                                 x.Address,
                                 x.HomePhone,
                                 x.Hotline
                             })
                             .ToList();

                dgvCoSo.DataSource = list;
            }
        }

        private void mnuCapNhat_Click(object sender, EventArgs e)
        {
            if (dgvCoSo.CurrentRow == null) return;

            int id = Convert.ToInt32(dgvCoSo.CurrentRow.Cells["Id"].Value);
            frmThemMoiCuaHang f = new frmThemMoiCuaHang(this, id);
            f.StartPosition = FormStartPosition.CenterParent;
            f.ShowDialog();
        }
    }
}
