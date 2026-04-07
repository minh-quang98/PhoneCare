using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneCare
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static class Session
        {
            public static bool IsLoggedIn = false;

        }

        private void UpdateMenu()
        {
            if (Session.IsLoggedIn)
            {
                mnuAuthen.Visible = false;
            } else
            {
                mnuAuthen.Visible = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateMenu();
        }

        private void mnuLogIn_Click(object sender, EventArgs e)
        {
            frmDangNhap f = new frmDangNhap();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuQuanTriNhanVien_Click(object sender, EventArgs e)
        {
            Forms.QuanTriNhanVien.frmQuanTriNhanVien f = new Forms.QuanTriNhanVien.frmQuanTriNhanVien();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }

        private void mnuChangePassword_Click(object sender, EventArgs e)
        {
            Forms.frmDoiMatKhau f = new Forms.frmDoiMatKhau();
            f.StartPosition = FormStartPosition.CenterScreen;
            f.Show();
        }
    }
}
