using System;
using System.Windows.Forms;

namespace PhoneCare.Forms
{
    public partial class frmThongTinHoTro : Form
    {
        public frmThongTinHoTro()
        {
            InitializeComponent();
        }

        private void frmThongTinHoTro_Load(object sender, EventArgs e)
        {
            lblTitle.Text = "Phone Care - Hỗ trợ";
            lblSupport.Text = "Quy trình: tiếp nhận máy, cập nhật sửa chữa, thêm dịch vụ, in phiếu/hóa đơn.";
            lblEmail.Text = "Email hỗ trợ: minhquang10998@gmail.com";
            lblHotline.Text = "Hotline: 0877217317";
            lblVersion.Text = "Phiên bản: 1.0";
        }
    }
}
