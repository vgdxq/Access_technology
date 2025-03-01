using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace TBD_Yaroshenko
{
    public partial class FileManager : Form
    {
        private string currentFilePath = string.Empty;
        private Bitmap currentImage;  // Використовуємо Bitmap для редагування зображення

        public FileManager()
        {
            InitializeComponent();

            // За замовчуванням кнопки та pictureBox не видимі
            buttonRotate.Visible = false;
            btnSave.Visible = false;
            pictureBox.Visible = false;
        }

        private void LoadFiles(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                listBoxFiles.Items.Clear();

                foreach (string file in files)
                {
                    listBoxFiles.Items.Add(Path.GetFileName(file));
                }
            }
            else
            {
                MessageBox.Show("Папка не знайдена!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FileManager_Load(object sender, EventArgs e)
        {
            string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\Data";
            LoadFiles(folderPath);
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedIndex != -1)
            {
                string selectedFile = listBoxFiles.SelectedItem.ToString();
                string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\Data";
                currentFilePath = Path.Combine(folderPath, selectedFile);

                try
                {
                    string fileExtension = Path.GetExtension(currentFilePath).ToLower();

                    if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".gif")
                    {
                        // Показуємо pictureBox
                        pictureBox.Visible = true;

                        // Закриваємо попереднє зображення, якщо воно є
                        currentImage?.Dispose();

                        // Завантажуємо нове зображення в Bitmap (копія, щоб уникнути блокування файлу)
                        using (var tempImage = Image.FromFile(currentFilePath))
                        {
                            currentImage = new Bitmap(tempImage);
                        }

                        pictureBox.Image = currentImage;
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                        // Показуємо кнопки для роботи з зображенням
                        buttonRotate.Visible = true;
                        btnSave.Visible = true;
                    }
                    else
                    {
                        // Ховаємо pictureBox та кнопки, якщо файл не є зображенням
                        pictureBox.Visible = false;
                        buttonRotate.Visible = false;
                        btnSave.Visible = false;

                        // Закриваємо попереднє зображення, якщо воно є
                        currentImage?.Dispose();
                        currentImage = null;
                        pictureBox.Image = null;

                        if (fileExtension == ".txt")
                        {
                            Process.Start("notepad.exe", currentFilePath);
                        }
                        else if (fileExtension == ".exe")
                        {
                            Process.Start(currentFilePath);
                        }
                        else
                        {
                            Process.Start(new ProcessStartInfo(currentFilePath) { UseShellExecute = true });
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося відкрити файл: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Якщо нічого не обрано, ховаємо pictureBox та кнопки
                pictureBox.Visible = false;
                buttonRotate.Visible = false;
                btnSave.Visible = false;

                // Закриваємо попереднє зображення, якщо воно є
                currentImage?.Dispose();
                currentImage = null;
                pictureBox.Image = null;
            }
        }

        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                // Повертаємо зображення
                currentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox.Image = currentImage;
                pictureBox.Invalidate();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (currentImage != null && !string.IsNullOrEmpty(currentFilePath))
            {
                try
                {
                    // Зберігаємо зображення у файл
                    currentImage.Save(currentFilePath, System.Drawing.Imaging.ImageFormat.Png);
                    MessageBox.Show("Зміни збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося зберегти зміни: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Немає змін для збереження або файл не вибраний.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}