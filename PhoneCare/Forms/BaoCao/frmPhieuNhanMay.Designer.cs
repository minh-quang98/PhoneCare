namespace PhoneCare.Forms.BaoCao
{
    partial class frmPhieuNhanMay
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "PhoneCare.BaoCao.PhieuNhanMay.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.ServerReport.BearerToken = null;
            this.reportViewer1.Size = new System.Drawing.Size(384, 561);
            this.reportViewer1.TabIndex = 0;
            // 
            // frmPhieuNhanMay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 561);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmPhieuNhanMay";
            this.Text = "In phiếu nhận";
            this.Load += new System.EventHandler(this.frmPhieuNhanMay_Load);
            this.ResumeLayout(false);

        }

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}
