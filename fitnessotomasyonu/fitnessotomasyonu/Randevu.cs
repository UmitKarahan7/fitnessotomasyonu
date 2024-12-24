using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;

namespace fitnessotomasyonu
{
    public partial class Randevu : Form
    {
        private IFirebaseClient firebaseClient; // Firebase istemcisi için değişken tanımı
        string userid = "";
        string username = "";
        public Randevu()
        {
            InitializeComponent();
            InitializeFirebase();
            LoadRandevuData(); // Load data on form load
        }
        public void uid(string uid, string name)
        {
            userid = uid;
            username = name;
        }
        private void InitializeFirebase()
        {
            // Firebase bağlantı ayarları
            IFirebaseConfig config = new FirebaseConfig
            {
                AuthSecret = "rzY0DHXmYzFVk41DeiJJNxo4YIgs4bJs0bBocYWT",
                BasePath = "https://fitness-d349f-default-rtdb.europe-west1.firebasedatabase.app/"
            };

            firebaseClient = new FireSharp.FirebaseClient(config); // Firebase istemcisini başlat
            if (firebaseClient == null)
            {
                MessageBox.Show("Firebase bağlantısı sağlanamadı!");
            }
        }

        private async void Randevubtn_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || string.IsNullOrEmpty(RandevuDateTimePicker.Text) || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun!");
                return;
            }

            // Randevu bilgilerini oluşturma
            var randevu = new
            {
                Egitmenismi = comboBox1.SelectedItem.ToString(),
                Rid = Guid.NewGuid().ToString(),
                uyeismi = username,
                tarih = RandevuDateTimePicker.Value.ToString("yyyy-MM-dd"),
                saat = comboBox2.SelectedItem.ToString()
            };

            try
            {
                // Check if the user already has an appointment at the same date and time
                FirebaseResponse response = await firebaseClient.GetAsync("randevu");
                if (response.Body != "null")
                {
                    Dictionary<string, RandevuModel> randevular = response.ResultAs<Dictionary<string, RandevuModel>>();
                    bool isDuplicate = randevular.Values.Any(r => r.uyeismi == username && r.tarih == randevu.tarih && r.saat == randevu.saat);

                    if (isDuplicate)
                    {
                        MessageBox.Show("Bu tarih ve saatte zaten bir randevunuz var!");
                        return;
                    }
                }

                // Veritabanına randevu bilgilerini gönderme
                await firebaseClient.SetAsync($"randevu/{randevu.Rid}", randevu);
                MessageBox.Show("Randevu başarıyla alındı!");
                LoadRandevuData(); // Reload data after adding a new appointment
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private async void LoadRandevuData()
        {
            try
            {
                FirebaseResponse response = await firebaseClient.GetAsync("randevu");
                if (response.Body != "null")
                {
                    Dictionary<string, RandevuModel> randevular = response.ResultAs<Dictionary<string, RandevuModel>>();
                    dataGridView1.DataSource = randevular.Values.ToList();
                }
                else
                {
                    MessageBox.Show("Hiç randevu yok!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void Randevu_Load(object sender, EventArgs e)
        {

        }

       
    }

    public class RandevuModel
    {
        public string Egitmenismi { get; set; }
        public string uyeismi { get; set; }
        public string tarih { get; set; }
        public string saat { get; set; }
    }
}
