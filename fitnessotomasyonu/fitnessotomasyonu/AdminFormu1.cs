using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace fitnessotomasyonu
{
    public partial class AdminFormu1 : Form
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "rzY0DHXmYzFVk41DeiJJNxo4bJs0bBocYWT",
            BasePath = "https://fitness-d349f-default-rtdb.europe-west1.firebasedatabase.app/"
        };

        IFirebaseClient client;
        CurrencyManager cm;

        public AdminFormu1()
        {
            InitializeComponent();
            client = new FireSharp.FirebaseClient(config);
          
        }

        private void AdminFormu1_Load(object sender, EventArgs e)
        {
            
            // DataGridView Ayarları
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            VerileriGetir();
        }

        private void VerileriGetir()
        {
            try
            {
                FirebaseResponse response = client.Get("users");

                if (response.Body == "null" || string.IsNullOrEmpty(response.Body))
                {
                    MessageBox.Show("Veri bulunamadı!");
                    return;
                }

                // Gelen JSON'u ayrıştır ve Dictionary olarak al
                var data = response.ResultAs<Dictionary<string, Dictionary<string, string>>>();

                if (data == null)
                {
                    MessageBox.Show("Veri bulunamadı!");
                    return;
                }

                // DataTable oluştur
                DataTable dt = new DataTable();

                // Sütunları istenen sırada ekle
                
                dt.Columns.Add("ID"); // Benzersiz ID sütunu
                dt.Columns.Add("Uad");
                dt.Columns.Add("Usoyad");
                dt.Columns.Add("Uyas");
                dt.Columns.Add("Utel");
                dt.Columns.Add("Usifre");
                dt.Columns.Add("Ucinsiyet");
                dt.Columns.Add("Uuyelik");
                dt.Columns.Add("UyeOlmaTarihi");

                // Verileri DataTable'e ekle
                foreach (var item in data)
                {
                    DataRow row = dt.NewRow();
                    row["ID"] = item.Key; // Firebase'deki benzersiz ID'yi ekle

                    row["Uad"] = item.Value.ContainsKey("Uad") ? item.Value["Uad"] : "";
                    row["Usoyad"] = item.Value.ContainsKey("Usoyad") ? item.Value["Usoyad"] : "";
                    row["Uyas"] = item.Value.ContainsKey("Uyas") ? item.Value["Uyas"] : "";
                    row["Utel"] = item.Value.ContainsKey("Utel") ? item.Value["Utel"] : "";
                    row["Usifre"] = item.Value.ContainsKey("Usifre") ? item.Value["Usifre"] : "";
                    row["Ucinsiyet"] = item.Value.ContainsKey("Ucinsiyet") ? item.Value["Ucinsiyet"] : "";
                    row["Uuyelik"] = item.Value.ContainsKey("Uuyelik") ? item.Value["Uuyelik"] : "";
                    row["UyeOlmaTarihi"] = item.Value.ContainsKey("UyeOlmaTarihi") ? item.Value["UyeOlmaTarihi"] : "";

                    dt.Rows.Add(row);
                }

                // DataGridView'e bağla
                dataGridView1.DataSource = dt;

                // ID sütununu gizle (isterseniz)
                //dataGridView1.Columns["ID"].Visible = true;

                // CurrencyManager'i ayarla
                cm = (CurrencyManager)BindingContext[dataGridView1.DataSource];
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void silbtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Seçili satırın ID'sini al
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen bir satır seçin!");
                    return;
                }

                string selectedID = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();

                // Firebase'den sil ve kontrol
                if (client.Delete("users/" + selectedID).StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Kayıt başarıyla silindi!");
                    VerileriGetir(); // Tabloyu yenile
                }
                else
                {
                    MessageBox.Show("Kayıt silinirken bir hata oluştu.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (cm.Position < cm.Count - 1)
            {
                cm.Position += 1;
                dataGridView1.Rows[cm.Position].Selected = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (cm.Position > 0)
            {
                cm.Position -= 1;
                dataGridView1.Rows[cm.Position].Selected = true;
            }
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            AdminFormu1 adminF1 = new AdminFormu1();
            adminF1.ShowDialog();
            this.Close();
        }

        private void label11_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Hücre tıklama işlemleri
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
            GirisKayitOl girisKayitOl = new GirisKayitOl();
            girisKayitOl.Show();
        }

        private void SiralaBtn_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource == null)
            {
                MessageBox.Show("Sıralama veya filtreleme için veri bulunamadı!");
                return;
            }

            // DataSource'u DataTable olarak al
            DataTable dt = dataGridView1.DataSource as DataTable;
            if (dt == null)
            {
                MessageBox.Show("Veri kaynağı bir DataTable değil!");
                return;
            }

            // Seçili seçenek kontrolü
            string selectedOption = siralacb.SelectedItem != null ? siralacb.SelectedItem.ToString() : "";

            // DataView üzerinden işlem yap
            DataView dv = dt.DefaultView;

            try
            {
                switch (selectedOption)
                {
                    case "Tümü":
                        dv.RowFilter = ""; // Filtreyi temizle
                        dv.Sort = ""; // Sıralamayı temizle
                        break;

                    case "Erkek":
                        dv.RowFilter = "Ucinsiyet = 'Erkek'";
                        break;

                    case "Kadın":
                        dv.RowFilter = "Ucinsiyet = 'Kadın'";
                        break;

                    case "Altın Üyelik":
                        dv.RowFilter = "Uuyelik = 'Altın Üyelik'";
                        break;

                    case "Gümüş Üyelik":
                        dv.RowFilter = "Uuyelik = 'Gümüş Üyelik'";
                        break;

                    case "Tarihe Göre(Eski)":
                        dv.Sort = "UyeOlmaTarihi ASC";
                        break;

                    case "Tarihe Göre(Yeni)":
                        dv.Sort = "UyeOlmaTarihi DESC";
                        break;

                    case "A-Z":
                        dv.Sort = "Uad ASC";
                        break;

                    case "Z-A":
                        dv.Sort = "Uad DESC";
                        break;

                    default:
                        MessageBox.Show("Geçersiz seçim!");
                        return;
                }

                // DataGridView'i güncellemeye gerek yok, DataView doğrudan bağlı
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
            egitmenekle egitmenekle = new egitmenekle();
            egitmenekle.ShowDialog();

        }
    }
}
