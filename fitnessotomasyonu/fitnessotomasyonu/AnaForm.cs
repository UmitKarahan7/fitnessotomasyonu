using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Response;
using FireSharp.Config;
using FireSharp.Interfaces;
using static fitnessotomasyonu.Veri;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace fitnessotomasyonu
{
    


    public partial class AnaForm : Form
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "rzY0DHXmYzFVk41DeiJJNxo4YIgs4bJs0bBocYWT",
            BasePath = "https://fitness-d349f-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        string uid1 = "";
        string username = "";
        public void useridal(string uid,string usernam)
        {
            uid1 = uid;
            username = usernam;
        }
        public void butongizle()
        {
            pictureBox4.Visible = false;
            pictureBox2.Visible = true;
            button4.Visible = true;
            Egitmenler.Visible = true;
        }
        public AnaForm()
        {
            InitializeComponent();
         
            

            client = new FireSharp.FirebaseClient(config);
            if (client == null)
            {
                MessageBox.Show("Firebase bağlantısı başarısız");
            }
         

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void AnaFormPB_Click(object sender, EventArgs e)
        {

        }

     
       
        private void exitlabel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void ResetPictureBox_Click(object sender, EventArgs e)
        {
            this.Hide();
            AnaForm anaForm = new AnaForm();
            anaForm.ShowDialog();
            this.Close();
        }

        private void YOHBtn_Click(object sender, EventArgs e)
        {
            panel3.Visible = !panel3.Visible;
            panel2.Visible = false;
            panel4.Visible = false;
            panel6.Visible = false;
        }

        private void AnaForm_Load(object sender, EventArgs e)
        {
          
           
        }

        private void UyelikSecenekleriBtn_Click(object sender, EventArgs e)
        {
            panel2.Visible = !panel2.Visible;
            panel3.Visible =false;
            panel6.Visible=false;
           
                
        }

        private void btnGizleGoster_Click(object sender, EventArgs e)
        {
         
        }

        private void TxtTelefon_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            GirisKayitOl go = new GirisKayitOl();

            this.Hide();
            go.Show();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Panel görünürlüğünü değiştir
                panel4.Visible = !panel4.Visible;

                // Firebase'den kullanıcıyı çek
                FirebaseResponse response = await client.GetAsync($"users/{uid1}");
                if (response.Body != "null")
                {
                    // Kullanıcı verilerini çözümle
                    var user = response.ResultAs<Dictionary<string, string>>();
                    if (user != null && user.ContainsKey("Uad")) // Uad, uname karşılığı
                    {
                        textBox1.Text = user["Uad"]; // Kullanıcı adını TextBox'a yazdır
                        textBox2.Text = user["Usoyad"]; // Kullanıcı adını TextBox'a yazdır
                        textBox3.Text = user["Usifre"]; // Kullanıcı adını TextBox'a yazdır
                        textBox4.Text = user["Utel"]; // Kullanıcı adını TextBox'a yazdır
                        textBox5.Text = user["Ucinsiyet"]; // Kullanıcı adını TextBox'a yazdır
                        textBox6.Text = user["Uuyelik"]; // Kullanıcı adını TextBox'a yazdır

                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı adı bulunamadı!");
                    }
                }
                else
                {
                    MessageBox.Show("Kullanıcı bulunamadı!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            // TextBox'ların boş olup olmadığını kontrol et
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox2.Text) ||
                string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Tüm alanları doldurmanız gerekiyor.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  // İşlemi durdur
            }

            var user = new
            {
                Uad = textBox1.Text,
                Usoyad = textBox2.Text,
                Utel = textBox4.Text,
                Usifre = textBox3.Text,
            };

            // UID'yi anahtar olarak kullanarak veri güncelleme
            var firebasePath = $"users/{uid1}";

            // UpdateAsync ile sadece belirtilen alanları güncelleyebilirsiniz
            await client.UpdateAsync(firebasePath, user);

            MessageBox.Show($"Kullanıcı başarıyla güncellendi. UID: {uid1}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AnaForm form = new AnaForm();
            this.Close();
            form.Show();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            // UID'yi anahtar olarak kullanarak veri silme
            var firebasePath = $"users/{uid1}";  // UID'ye göre kullanıcı verisini hedefliyoruz

            try
            {
                // Firebase veritabanından ilgili kullanıcıyı sil
                FirebaseResponse response = await client.DeleteAsync(firebasePath);

                // Eğer response.StatusCode 200 ise işlem başarılı
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    MessageBox.Show($"UID: {uid1} başarıyla silindi.");
                    this.Close();
                    Form form = new AnaForm();
                    form.Show();
                }
                else
                {
                    MessageBox.Show($"Silme işlemi başarısız. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Bir hata oluştu: {ex.Message}");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
           panel6.Visible = !panel6.Visible;
            panel4.Visible = false;
            panel3.Visible = false;
            panel2.Visible = false;
        }

        private void Egitmenler_Click(object sender, EventArgs e)
        {
            Randevu et = new Randevu();
            et.uid(uid1,username);
            et.ShowDialog();
            
        }

        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void PBilgileriBtn_Click(object sender, EventArgs e)
        {
            panel4.Visible=!panel4.Visible;
            panel2.Visible=false;
            panel3.Visible=false;
            panel6.Visible=false;
        }

        private void HesaplaBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Verileri al
                int yas = int.Parse(textBox7.Text);
                double boy = double.Parse(textBox12.Text); // cm cinsinden
                double kilo = double.Parse(textBox8.Text);
                double boyunCevresi = double.Parse(textBox13.Text);
                double kalcaCevresi = double.Parse(textBox9.Text);
                double belCevresi = double.Parse(textBox11.Text);
                string cinsiyet = comboBox1.SelectedItem.ToString();

                // BMI hesapla
                double boyMetre = boy / 100; // Boyu metreye çevir
                double bmi = kilo / (boyMetre * boyMetre);

                // Yağ oranı hesapla
                double yagOrani;
                if (cinsiyet == "Erkek")
                {
                    yagOrani = (1.20 * bmi) + (0.23 * yas) - 16.2;
                }
                else if (cinsiyet == "Kadın")
                {
                    yagOrani = (1.20 * bmi) + (0.23 * yas) - 5.4;
                }
                else
                {
                    MessageBox.Show("Cinsiyet seçimi hatalı!");
                    return;
                }

                // Sonucu yazdır
                SonucTb.Text = $"%{yagOrani:F2}"; // Sonucu yüzde olarak formatla
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hesaplama sırasında bir hata oluştu: " + ex.Message);
            }
        }
    }
}

