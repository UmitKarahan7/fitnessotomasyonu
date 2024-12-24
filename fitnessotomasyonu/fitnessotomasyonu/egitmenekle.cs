using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace fitnessotomasyonu
{
    public partial class egitmenekle : Form
    {
        private readonly IFirebaseClient client;



        public egitmenekle()
        {
            InitializeComponent();
            client = new FireSharp.FirebaseClient(new FirebaseConfig
            {
                AuthSecret = "rzY0DHXmYzFVk41DeiJJNxo4YIgs4bJs0bBocYWT",
                BasePath = "https://fitness-d349f-default-rtdb.europe-west1.firebasedatabase.app/"
            });

            if (client == null)
                MessageBox.Show("Bağlantı kurulamadı!");
        }

        private void egitmenekle_Load(object sender, EventArgs e)
        {

            AdTb.KeyPress += new KeyPressEventHandler(AdTb_KeyPress);
            SoyadTb.KeyPress += new KeyPressEventHandler(SoyadTb_KeyPress);
            YasTb.KeyPress += new KeyPressEventHandler(YasTb_KeyPress);
            TelTb.KeyPress += new KeyPressEventHandler(TelTb_KeyPress);

            SetupDataGridView();
            LoadEgitmenler();
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
        }

        private void SetupDataGridView()
        {
            dataGridView1.Columns.Clear();
            var columns = new[] { "ID", "Eğitmen Adı", "Eğitmen Soyadı", "Yaş", "Telefon", "Uzmanlık", "Çalışma Saatleri", "Cinsiyet", "Tarih" };
            foreach (var column in columns)
                dataGridView1.Columns.Add(column.Replace(" ", ""), column);
            dataGridView1.Columns["ID"].Visible = true;

        }

        private async void LoadEgitmenler()
        {
            try
            {
                var response = await client.GetAsync("egitmenler");
                if (response.Body == "null")
                {
                    MessageBox.Show("Veritabanında eğitmen bulunamadı.");
                    return;
                }

                var egitmenler = response.ResultAs<Dictionary<string, Dictionary<string, dynamic>>>();

                dataGridView1.Rows.Clear(); // Tabloyu temizle

                foreach (var egitmen in egitmenler)
                {
                    string id = egitmen.Key; // Firebase'in oluşturduğu benzersiz ID
                    var egitmenData = egitmen.Value;

                    dataGridView1.Rows.Add(
                        id, // Firebase ID'yi ilk sütuna ekle
                        egitmenData["Ead"],
                        egitmenData["Esoyad"],
                        egitmenData["Eyas"],
                        egitmenData["Etel"],
                        egitmenData["Euzmanlik"],
                        egitmenData["Ecalisma"],
                        egitmenData["Ecinsiyet"],
                        egitmenData["IseAlimTarihi"]
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void ClearFields()
        {
            AdTb.Clear();
            SoyadTb.Clear();
            YasTb.Clear();
            TelTb.Clear();
            UzmanlikCb.SelectedIndex = -1;
            RandevuTb.Clear();
            CinsiyetCb.SelectedIndex = -1;
        }

        private async void EekleTb_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AdTb.Text) || string.IsNullOrWhiteSpace(SoyadTb.Text))
            {
                MessageBox.Show("Eksik Bilgi Girdiniz!");
                return;
            }

            try
            {
                var egitmen = new
                {
                    Ead = AdTb.Text,
                    Esoyad = SoyadTb.Text,
                    Eyas = int.Parse(YasTb.Text),
                    Etel = TelTb.Text,
                    Euzmanlik = UzmanlikCb.Text,
                    Ecalisma = RandevuTb.Text,
                    Ecinsiyet = CinsiyetCb.SelectedItem?.ToString(),
                    IseAlimTarihi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                // Firebase'e veri ekleme ve benzersiz ID alma
                var response = await client.PushAsync("egitmenler/", egitmen);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Eğitmen başarıyla eklendi!");
                    ClearFields();
                    LoadEgitmenler(); // Yeni veriyi yüklemek için tabloyu güncelle
                }
                else
                {
                    MessageBox.Show($"Kayıt eklenirken bir hata oluştu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private async void SiralaBtn_Click(object sender, EventArgs e)
        {
            try

            {

                // Firebase'den Verileri Çek

                FirebaseResponse response = await client.GetAsync("egitmenler");

                if (response.Body == "null")

                {

                    MessageBox.Show("Veritabanında eğitmen bulunamadı.");

                    return;

                }



                // Veriyi Dictionary'e Dönüştür

                var egitmenler = response.ResultAs<Dictionary<string, Dictionary<string, dynamic>>>();



                if (egitmenler == null || egitmenler.Count == 0)

                {

                    MessageBox.Show("Sıralama için yeterli veri yok!");

                    return;

                }



                // Veriyi Listeye Dönüştür (ID'yi de dahil ederek)

                var egitmenListesi = egitmenler.Select(x => new Dictionary<string, dynamic>(x.Value)

        {

            { "FirebaseID", x.Key } // Firebase'in oluşturduğu ID'yi ekle

        }).ToList();



                // ComboBox'taki Seçime Göre Sıralama

                string selectedOption = siralacb.SelectedItem?.ToString() ?? "";

                switch (selectedOption)

                {

                    case "Tümü":

                        break; // Filtre veya sıralama yok

                    case "Yaş (Büyükten Küçüğe)":

                        egitmenListesi.Sort((x, y) => Convert.ToInt32(y["Eyas"]).CompareTo(Convert.ToInt32(x["Eyas"])));

                        break;

                    case "Yaş (Küçükten Büyüğe)":

                        egitmenListesi.Sort((x, y) => Convert.ToInt32(x["Eyas"]).CompareTo(Convert.ToInt32(y["Eyas"])));

                        break;

                    case "(A-Z)":

                        egitmenListesi.Sort((x, y) => string.Compare(x["Ead"], y["Ead"], StringComparison.Ordinal));

                        break;

                    case "(Z-A)":

                        egitmenListesi.Sort((x, y) => string.Compare(y["Ead"], x["Ead"], StringComparison.Ordinal));

                        break;

                    case "Cinsiyet (Kadın)":

                        egitmenListesi = egitmenListesi.Where(x => x["Ecinsiyet"].ToString() == "Kadın").ToList();

                        break;

                    case "Cinsiyet (Erkek)":

                        egitmenListesi = egitmenListesi.Where(x => x["Ecinsiyet"].ToString() == "Erkek").ToList();

                        break;

                    case "Tarih(Eski)":

                        egitmenListesi = egitmenListesi.OrderBy(x => DateTime.Parse(x["IseAlimTarihi"])).ToList();

                        break;

                    case "Tarih(Yeni)":

                        egitmenListesi = egitmenListesi.OrderByDescending(x => DateTime.Parse(x["IseAlimTarihi"])).ToList();

                        break;

                    default:

                        MessageBox.Show("Lütfen geçerli bir sıralama seçeneği seçin!");

                        return;

                }



                // DataGridView'i Temizle ve Yeni Sıralanmış Veriyi Ekle

                dataGridView1.Rows.Clear();

                foreach (var egitmen in egitmenListesi)

                {

                    dataGridView1.Rows.Add(

                        egitmen["FirebaseID"], // Firebase ID

                        egitmen["Ead"],

                        egitmen["Esoyad"],

                        egitmen["Eyas"],

                        egitmen["Etel"],

                        egitmen["Euzmanlik"],

                        egitmen["Ecalisma"],

                        egitmen["Ecinsiyet"],

                        egitmen["IseAlimTarihi"]

                    );

                }

            }

            catch (Exception ex)

            {

                MessageBox.Show($"Hata: {ex.Message}");

            }


        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private async void silbtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Lütfen bir satır seçin!");
                    return;
                }

                string selectedID = dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString();

                var confirmResult = MessageBox.Show("Eğitmeni silmek istediğinizden emin misiniz?", "Silme Onayı", MessageBoxButtons.YesNo);
                if (confirmResult == DialogResult.No)
                    return;

                var response = await client.DeleteAsync($"egitmenler/{selectedID}");
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show("Kayıt başarıyla silindi!");
                    LoadEgitmenler();
                }
                else
                {
                    MessageBox.Show($"Kayıt silinirken bir hata oluştu: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void exitlabel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            this.Close();
            AdminFormu1 adminFormu1 = new AdminFormu1();
            adminFormu1.Show();

        }

        private void siralacb_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            this.Hide();
            egitmenekle egitmenekle = new egitmenekle();
            egitmenekle.Show();
            this.Close();
        }
        private void AdTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf ve backspace karakterine izin ver
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void SoyadTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece harf ve backspace karakterine izin ver
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }
        private void YasTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Yaş yalnızca rakamlardan oluşabilir
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
                return;
            }

            // Yaş 0 ile başlamaz
            if (YasTb.Text.Length == 0 && e.KeyChar == '0')
            {
                e.Handled = true;
                MessageBox.Show("Yaş 0 ile başlamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Yaş yalnızca 2 haneli olabilir
            if (YasTb.Text.Length >= 2 && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Yaş yalnızca 2 haneli olabilir!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TelTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakam girilmesine izin veriyoruz
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }

            // Telefon numarasının tam olarak 10 karakter uzunluğunda olup olmadığını kontrol et
            if (TelTb.Text.Length >= 10 && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Telefon numarası 10 haneli olmalıdır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
