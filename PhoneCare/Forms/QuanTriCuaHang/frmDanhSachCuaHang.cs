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
            frmThemMoiCuaHang f = new frmThemMoiCuaHang();
            f.StartPosition = FormStartPosition.CenterParent;
            f.Show();
        }
    }
}
