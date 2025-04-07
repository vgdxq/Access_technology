using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TBD_Yaroshenko
{
    public partial class CaptchaForm : Form
    {
        // Оголошення елементів управління
        private PictureBox pictureBoxCaptcha;
        private TextBox textBoxCaptcha;
        private Button buttonVerify;
        private Button buttonRefresh;
        private Label labelInstruction;
        private Label labelAttempts;
        private Label labelError; // Додано
        private Label labelAttemptsInfo; // Додано

        private string captchaText;
        private Random random;

        public CaptchaForm()
        {
            random = new Random();
            InitializeComponents();
            GenerateNewCaptcha();
        }

        private void InitializeComponents()
        {
            // Налаштування основної форми
            this.Text = "Підтвердження безпеки";
            this.ClientSize = new Size(300, 180);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.Font = new Font("Segoe UI", 9);

            // Створення та налаштування PictureBox для капчі
            pictureBoxCaptcha = new PictureBox
            {
                Location = new Point(20, 50),
                Size = new Size(200, 50),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Створення TextBox для введення капчі
            textBoxCaptcha = new TextBox
            {
                Location = new Point(20, 110),
                Size = new Size(150, 20)
            };

            // Створення кнопки підтвердження
            buttonVerify = new Button
            {
                Text = "Confirm",
                Location = new Point(180, 110),
                Size = new Size(90, 23),
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonVerify.FlatAppearance.BorderSize = 0;
            buttonVerify.Click += buttonVerify_Click;

            // Створення кнопки оновлення капчі
            buttonRefresh = new Button
            {
                Text = "Update",
                Location = new Point(230, 50),
                Size = new Size(60, 23),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            buttonRefresh.FlatAppearance.BorderSize = 0;
            buttonRefresh.Click += buttonRefresh_Click;

            // Створення Label з інструкцією
            labelInstruction = new Label
            {
                Text = "Enter the characters from the image:",
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Створення Label для відображення спроб
            labelAttempts = new Label
            {
                ForeColor = Color.Red,
                Location = new Point(20, 140),
                Size = new Size(260, 20)
            };

            // Створення Label для помилок
            labelError = new Label
            {
                ForeColor = Color.Red,
                Location = new Point(20, 140),
                Size = new Size(260, 20),
                Visible = false
            };

            // Створення Label для інформації про спроби
            labelAttemptsInfo = new Label
            {
                Location = new Point(20, 160),
                Size = new Size(200, 20),
                ForeColor = Color.Red,
                Visible = false
            };

            // Додавання елементів на форму
            this.Controls.Add(pictureBoxCaptcha);
            this.Controls.Add(textBoxCaptcha);
            this.Controls.Add(buttonVerify);
            this.Controls.Add(buttonRefresh);
            this.Controls.Add(labelInstruction);
            this.Controls.Add(labelAttempts);
            this.Controls.Add(labelError);
            this.Controls.Add(labelAttemptsInfo);
        }

        public void SetAttemptsInfo(int currentAttempts, int maxAttempts)
        {
            labelAttemptsInfo.Text = $"Attempts: {currentAttempts + 1}/{maxAttempts}";
            labelAttemptsInfo.Visible = true;
        }

        private void GenerateNewCaptcha()
        {
            captchaText = GenerateRandomString(6); // Генеруємо 6 випадкових символів

            // Створення зображення капчі
            Bitmap bmp = new Bitmap(pictureBoxCaptcha.Width, pictureBoxCaptcha.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);

                // Додаємо шум
                for (int i = 0; i < 50; i++)
                {
                    int x = random.Next(bmp.Width);
                    int y = random.Next(bmp.Height);
                    bmp.SetPixel(x, y, Color.FromArgb(
                        random.Next(150, 255),
                        random.Next(150, 255),
                        random.Next(150, 255)));
                }

                // Малюємо текст з викривленням
                for (int i = 0; i < captchaText.Length; i++)
                {
                    Font font = new Font("Arial", random.Next(18, 22),
                        FontStyle.Bold | FontStyle.Italic);

                    float x = 10 + i * 30;
                    float y = random.Next(5, 15);

                    g.DrawString(captchaText[i].ToString(), font,
                        Brushes.Black, x, y);
                }

                // Додаємо лінії для додаткового захисту
                for (int i = 0; i < 5; i++)
                {
                    g.DrawLine(new Pen(Color.FromArgb(
                        random.Next(150, 255),
                        random.Next(150, 255),
                        random.Next(150, 255))),
                        random.Next(bmp.Width), random.Next(bmp.Height),
                        random.Next(bmp.Width), random.Next(bmp.Height));
                }
            }

            pictureBoxCaptcha.Image = bmp;
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void buttonVerify_Click(object sender, EventArgs e)
        {
            if (textBoxCaptcha.Text.Equals(captchaText, StringComparison.OrdinalIgnoreCase))
            {
                // Показуємо сповіщення про успіх
                MessageBox.Show("Captcha entered correctly!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                textBoxCaptcha.BackColor = Color.LightPink;
                labelError.Text = "Invalid captcha! Please try again.";
                labelError.Visible = true;

                // Оновлюємо капчу
                GenerateNewCaptcha();
                textBoxCaptcha.Text = "";
                textBoxCaptcha.Focus();
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            GenerateNewCaptcha();
            textBoxCaptcha.Focus();
        }
    }
}