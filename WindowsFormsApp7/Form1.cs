using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data; // DataTable için gerekli
using System.Data.SqlClient; // SQL Server işlemleri için gerekli
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp7
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=OtomasyonDB;Integrated Security=True;";
        private int seciliCalisanID = 0;

        public Form1()
        {
            InitializeComponent();

            // !!!!!!!!!! ÇÖZÜM BU SATIRDIR !!!!!!!!!
            // Tasarım ekranına (Properties penceresine) HİÇ GEREK KALMADAN,
            // 'Ekle' butonunu (adı btnEkle) koda (btnEkle_Click metoduna) bağlıyoruz.
            this.btnEkle.Click += new System.EventHandler(this.btnEkle_Click);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CalisanListele();
        }

        private void CalisanListele()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Calisanlar";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    dgvCalisanlar.DataSource = dataTable;

                    if (dgvCalisanlar.Columns["ID"] != null)
                    {
                        dgvCalisanlar.Columns["ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Veriler listelenirken bir hata oluştu: " + ex.Message);
            }
        }

        // 'Ekle' Butonunun Kodu (Bu kod zaten vardı, şimdi üstteki satır sayesinde çalışacak)
        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAdSoyad.Text) || string.IsNullOrWhiteSpace(txtMaas.Text))
            {
                MessageBox.Show("Ad Soyad ve Maaş alanları boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Calisanlar (AdSoyad, Departman, Maas) VALUES (@adSoyad, @departman, @maas)";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@adSoyad", txtAdSoyad.Text);
                        command.Parameters.AddWithValue("@departman", txtDepartman.Text);
                        command.Parameters.AddWithValue("@maas", Convert.ToDecimal(txtMaas.Text));

                        int etkilenenSatirSayisi = command.ExecuteNonQuery();

                        if (etkilenenSatirSayisi > 0)
                        {
                            MessageBox.Show("Yeni çalışan başarıyla eklendi.");
                            CalisanListele();
                            AlanlariTemizle();
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Maaş alanına lütfen geçerli bir sayı giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ekleme işlemi sırasında bir hata oluştu: " + ex.Message);
            }
        }

        // Temizleme Metodu
        private void AlanlariTemizle()
        {
            txtAdSoyad.Text = "";
            txtDepartman.Text = "";
            txtMaas.Text = "";
            seciliCalisanID = 0;
        }

        // Güncelleme (Boş)
        private void btnGuncelle_Click(object sender, EventArgs e)
        {
            // 1. Bir çalışan seçili mi diye kontrol etmemiz lazım.
            // (Eğer seciliCalisanID 0 ise, listeden kimse seçilmemiş demektir)
            if (seciliCalisanID == 0)
            {
                MessageBox.Show("Lütfen güncellemek için bir çalışan seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Metottan çık, güncelleme yapma
            }

            // 2. Textbox'ların boş olmadığını da kontrol edelim
            if (string.IsNullOrWhiteSpace(txtAdSoyad.Text) || string.IsNullOrWhiteSpace(txtMaas.Text))
            {
                MessageBox.Show("Ad Soyad ve Maaş alanları boş bırakılamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // UPDATE sorgusu
                    string query = "UPDATE Calisanlar SET AdSoyad = @adSoyad, Departman = @departman, Maas = @maas WHERE ID = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parametreleri Textbox'lardan ve global değişkenden al
                        command.Parameters.AddWithValue("@adSoyad", txtAdSoyad.Text);
                        command.Parameters.AddWithValue("@departman", txtDepartman.Text);
                        command.Parameters.AddWithValue("@maas", Convert.ToDecimal(txtMaas.Text));
                        command.Parameters.AddWithValue("@id", seciliCalisanID); // WHERE koşulu için ID'yi ver

                        int etkilenenSatir = command.ExecuteNonQuery();

                        if (etkilenenSatir > 0)
                        {
                            MessageBox.Show("Çalışan bilgileri başarıyla güncellendi.");

                            // İşlem bitince listeyi yenile
                            CalisanListele();

                            // Textbox'ları ve seçimi temizle
                            AlanlariTemizle();
                        }
                    }
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Maaş alanına lütfen geçerli bir sayı giriniz.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Güncelleme işlemi sırasında bir hata oluştu: " + ex.Message);
            }
        }

        // Silme (Boş)
        private void btnSil_Click(object sender, EventArgs e)
        {
            // 1. Bir çalışan seçili mi diye kontrol et (seciliCalisanID == 0)
            if (seciliCalisanID == 0)
            {
                MessageBox.Show("Lütfen silmek için bir çalışan seçin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Metottan çık, silme yapma
            }

            // 2. Silme işlemi kritik olduğu için kullanıcıdan ONAY alalım.
            DialogResult onay = MessageBox.Show(
                "Seçili çalışanı silmek istediğinize emin misiniz? Bu işlem geri alınamaz.",
                "Silme Onayı",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            // 3. Eğer kullanıcı "Evet" (Yes) butonuna basarsa silme işlemine devam et
            if (onay == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        // DELETE sorgusu
                        string query = "DELETE FROM Calisanlar WHERE ID = @id";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            // Parametre olarak sadece ID yeterli
                            command.Parameters.AddWithValue("@id", seciliCalisanID);

                            int etkilenenSatir = command.ExecuteNonQuery();

                            if (etkilenenSatir > 0)
                            {
                                MessageBox.Show("Çalışan başarıyla silindi.");

                                // İşlem bitince listeyi yenile
                                CalisanListele();

                                // Textbox'ları ve seçimi temizle
                                AlanlariTemizle();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Silme işlemi sırasında bir hata oluştu: " + ex.Message);
                }
            }
            // Eğer kullanıcı "Hayır" derse, hiçbir şey yapma.
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Ana form (Form1) kapatıldığında, tüm uygulamayı kapat.
            Application.Exit();
        }

        private void dgvCalisanlar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Eğer tıklanan yer satır başlığı (header) değilse ve geçerli bir satırsa
            if (e.RowIndex >= 0)
            {
                try
                {
                    // Tıklanan satırı al
                    DataGridViewRow row = dgvCalisanlar.Rows[e.RowIndex];

                    // Verileri TextBox'lara doldur
                    // ÖNEMLİ: 'seciliCalisanID' değişkenini burada saklıyoruz
                    seciliCalisanID = Convert.ToInt32(row.Cells["ID"].Value);

                    txtAdSoyad.Text = row.Cells["AdSoyad"].Value.ToString();
                    txtDepartman.Text = row.Cells["Departman"].Value.ToString();
                    txtMaas.Text = row.Cells["Maas"].Value.ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Satır verileri alınırken bir hata oluştu: " + ex.Message);
                }
            }
        }
    }
}