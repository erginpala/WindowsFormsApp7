using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class frmGiris : Form
    {
        public frmGiris()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnGirisYap_Click(object sender, EventArgs e)
        {
            string kullaniciAdi = txtKullaniciAdi.Text;
            string sifre = txtSifre.Text;

            // Kullanıcı adı ve şifreyi kontrol et (şimdilik elle)
            if (kullaniciAdi == "admin" && sifre == "1234")
            {
                // Giriş başarılıysa:

                // 1. Ana formu (Form1) oluştur
                Form1 anaForm = new Form1();

                // 2. Ana formu göster
                anaForm.Show();

                // 3. Bu giriş formunu gizle
                this.Hide();
            }
            else
            {
                // Giriş hatalıysa:
                MessageBox.Show("Hatalı kullanıcı adı veya şifre!", "Giriş Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
