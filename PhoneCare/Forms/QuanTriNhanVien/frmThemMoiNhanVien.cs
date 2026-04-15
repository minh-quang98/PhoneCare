using PhoneCare.Data;
using PhoneCare.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PhoneCare.Forms.QuanTriNhanVien
{
    public partial class frmThemMoiNhanVien : Form
    {
        public frmThemMoiNhanVien()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
           
        }

        private void frmThemMoiNhanVien_Load(object sender, EventArgs e)
        {
            LoadCoSo();
        }

        private void LoadCoSo()
        {
            using (var db = new PhoneCareDbContext())
            {
                cboCoSoLamViec.DataSource = db.CoSoCuaHangs.ToList();
                cboCoSoLamViec.DisplayMember = "TenCoSo"; // tên hiển thị
                cboCoSoLamViec.ValueMember = "Id";        // giá trị
            }
        }
    }
}
