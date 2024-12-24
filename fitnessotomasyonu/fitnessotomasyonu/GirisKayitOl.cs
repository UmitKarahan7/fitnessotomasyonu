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
using System.Reflection.Emit;


namespace fitnessotomasyonu
{
    public partial class GirisKayitOl : Form

    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "rzY0DHXmYzFVk41DeiJJNxo4YIgs4bJs0bBocYWT",
            BasePath = "https://fitness-d349f-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        IFirebaseClient client;
        string userID = "";
        string username = "";

        public GirisKayitOl()
        {
            InitializeComponent();
            client = new FireSharp.FirebaseClient(config);
        }

        private async void Giris_Click(object sender, EventArgs e)
        {
            // Kullanıcıdan alınan telefon ve şifre
            string telefon = GirisTelTb.Text.Trim();
            string sifre = GirisSifreTb.Text.Trim();

            // Giriş bilgilerinin boş olup olmadığını kontrol et
            if (string.IsNullOrEmpty(telefon) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Telefon ve şifre alanları boş bırakılamaz!");
                return;
            }

            try
            {
                // Firebase'den verileri çek
                FirebaseResponse response = await client.GetAsync("users");
                var users = response.ResultAs<Dictionary<string, Dictionary<string, string>>>();

                // Kullanıcıların arasında arama yap
                foreach (var user in users)
                {
                    if (user.Value["Utel"] == telefon && user.Value["Usifre"] == sifre)
                    {
                        // Kullanıcı bulundu
                        string uid = user.Value["Uid"]; // UID bilgisi alınıyor
                        username = user.Value["Uad"];

                        if (user.Value.ContainsKey("Ulvl") && user.Value["Ulvl"] == "2") // Yönetici kontrolü
                        {
                            MessageBox.Show("Giriş başarılı! Yönetici olarak giriş yaptınız.");
                            AdminFormu1 adminForm = new AdminFormu1();
                            adminForm.Show();
                            this.Close();

                        }
                        else
                        {
                            // Giriş başarılı, UID'yi AnaForm'a geç
                            MessageBox.Show($"Giriş başarılı! Hoş geldiniz, {user.Value["Uad"]}.");

                            // AnaForm'a UID'yi parametre olarak geçir
                            AnaForm anaForm = new AnaForm();
                            anaForm.useridal(uid, username);
                            anaForm.butongizle();
                            this.Close();
                            anaForm.Show();




                        }
                        return; // Kullanıcı bulunduğu için döngüyü sonlandır
                    }
                }

                // Kullanıcı bulunamazsa
                MessageBox.Show("Hatalı telefon veya şifre!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Hata: {ex.Message}");
            }
        }

        private void UyeOl_Click(object sender, EventArgs e)
        {
            GirisYapPanel.Visible = false;
            UyeOlPanel.Visible = !UyeOlPanel.Visible;
        }

        private void GirisKayitOl_Load(object sender, EventArgs e)
        {
            UyeOlPanel.Visible = false;
            GirisSifreTb.PasswordChar = '*';

            GirisTelTb.KeyPress += GirisTelTb_KeyPress;
            GirisSifreTb.KeyPress += GirisSifreTb_KeyPress;
            GirisSifreTb.TextChanged += GirisSifreTb_TextChanged;


            TxtAd.KeyPress += new KeyPressEventHandler(TxtAd_KeyPress);
            TxtSoyad.KeyPress += new KeyPressEventHandler(TxtSoyad_KeyPress);
            TxtYas.KeyPress += new KeyPressEventHandler(TxtYas_KeyPress);
            TxtTelefon.KeyPress += new KeyPressEventHandler(TxtTelefon_KeyPress);
            TxtSifre.KeyPress += new KeyPressEventHandler(TxtSifre_KeyPress);

        }

        private async void UyeOl1_Click(object sender, EventArgs e)
        {
            if (TxtAd.Text == ""
                || TxtSoyad.Text == ""
                || TxtYas.Text == ""
                || TxtTelefon.Text == ""
                || TxtSifre.Text == ""
                || CbCinsiyet.Text == ""
                || CbUyelik.Text == "")
            {
                MessageBox.Show("Bütün bilgileri doldurunuz!");
            }
            else
            {
                try
                {
                    string telefon = TxtTelefon.Text;

                    // Check if the phone number already exists
                    var existingUsers = await client.GetAsync("users");
                    if (existingUsers != null && existingUsers.Body.Contains(telefon))
                    {
                        MessageBox.Show("Bu telefon numarasıyla zaten bir kullanıcı var!");
                        return;
                    }

                    // Rastgele UID üretimi
                    string generatedUid = Guid.NewGuid().ToString("N").Substring(0, 8);

                    // Üye olma tarihi
                    string uyeOlmaTarihi = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    var user = new
                    {
                        Uad = TxtAd.Text,
                        Usoyad = TxtSoyad.Text,
                        Uyas = Convert.ToInt32(TxtYas.Text),
                        Utel = TxtTelefon.Text,
                        Usifre = TxtSifre.Text,
                        Ucinsiyet = CbCinsiyet.SelectedItem?.ToString(),
                        Uuyelik = CbUyelik.SelectedItem?.ToString(),
                        Uid = generatedUid,
                        UyeOlmaTarihi = uyeOlmaTarihi
                    };

                    // UID'yi anahtar olarak kullanarak veri ekleme
                    var firebasePath = $"users/{generatedUid}";
                    await client.SetAsync(firebasePath, user);

                    MessageBox.Show($"Kullanıcı başarıyla eklendi. UID: {generatedUid}");

                    // Giriş sayfasına yönlendirme
                    UyeOlPanel.Visible = false;
                    GirisYapPanel.Visible = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            UyeOlPanel.Visible = false;
            GirisYapPanel.Visible = true;
        }

        private void btnGizleGoster_Click(object sender, EventArgs e)
        {
            if (GirisSifreTb.PasswordChar == '*')
            {
                GirisSifreTb.PasswordChar = '\0';
                btnGizleGoster.Text = "Gizle";
            }
            else
            {
                GirisSifreTb.PasswordChar = '*';
                btnGizleGoster.Text = "Göster";
            }
        }

        private void Giris_ClientSizeChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
            AnaForm anaForm = new AnaForm();
            anaForm.ShowDialog();

        }

        private void GirisSifreTb_TextChanged(object sender, EventArgs e)
        {

            // Şifre 8 karakterden fazla olmamalıdır
            if (GirisSifreTb.Text.Length > 8)
            {
                GirisSifreTb.Text = GirisSifreTb.Text.Substring(0, 8); // Şifreyi 8 karaktere kısıtla
                MessageBox.Show("Şifre 8 karakterden fazla olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
        private void GirisTelTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakam girilmesine izin veriyoruz
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }

            // Telefon numarasının tam olarak 10 karakter uzunluğunda olup olmadığını kontrol et
            if (GirisTelTb.Text.Length >= 10 && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Telefon numarası 10 haneli olmalıdır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void GirisSifreTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '.' || e.KeyChar == '_') && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Şifre yalnızca harf, rakam, nokta (.) ve alt çizgi (_) içerebilir!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (TxtSifre.Text.Length == 8 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Şifre 8 karakterden uzun olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void TxtAd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Ad yalnızca harflerden oluşabilir!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void TxtSoyad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Soyad yalnızca harflerden oluşabilir!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TxtYas_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Yaş yalnızca rakamlardan oluşabilir
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
                return;
            }

            // Yaş 0 ile başlamaz
            if (TxtYas.Text.Length == 0 && e.KeyChar == '0')
            {
                e.Handled = true;
                MessageBox.Show("Yaş 0 ile başlamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Yaş yalnızca 2 haneli olabilir
            if (TxtYas.Text.Length >= 2 && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Yaş yalnızca 2 haneli olabilir!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TxtTelefon_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Sadece rakam girilmesine izin veriyoruz
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }

            // Telefon numarasının tam olarak 10 karakter uzunluğunda olup olmadığını kontrol et
            if (TxtTelefon.Text.Length >= 10 && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Telefon numarası 10 haneli olmalıdır!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void TxtSifre_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '.' || e.KeyChar == '_') && e.KeyChar != (char)8)
            {
                e.Handled = true;
                MessageBox.Show("Şifre yalnızca harf, rakam, nokta (.) ve alt çizgi (_) içerebilir!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            if (TxtSifre.Text.Length == 8 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
                MessageBox.Show("Şifre 8 karakterden uzun olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void TxtSifre_TextChanged(object sender, EventArgs e)
        {
            // Şifre 8 karakterden fazla olmamalıdır
            if (TxtSifre.Text.Length > 8)
            {
                TxtSifre.Text = TxtSifre.Text.Substring(0, 8); // Şifreyi 8 karaktere kısıtla
                MessageBox.Show("Şifre 8 karakterden fazla olamaz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void UyeOlPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
