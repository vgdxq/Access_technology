using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace TBD_Yaroshenko
{
    public partial class FileManager : Form
    {
        private string currentFilePath = string.Empty;
        private Bitmap currentImage;
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;
        private string userRole; // Додано поле для зберігання ролі користувача

        // Конструктор з параметром для передачі ролі користувача
        public FileManager(string role)
        {
            InitializeComponent();
            buttonRotate.Visible = false;
            btnSave.Visible = false;
            pictureBox.Visible = false;
            userRole = role; // Зберігаємо роль користувача
        }

        private void LoadFiles(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                listBoxFiles.Items.Clear();

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    // Отримуємо список файлів з бази даних
                    string selectQuery = "SELECT FILE_NAME, CONFIDENTIALITY_LEVEL FROM FILE_ACCESS";
                    SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                    SqlDataReader reader = selectCmd.ExecuteReader();

                    // Збираємо всі файли з бази даних
                    var dbFiles = new System.Collections.Generic.List<(string FileName, string ConfidentialityLevel)>();
                    while (reader.Read())
                    {
                        dbFiles.Add((reader["FILE_NAME"].ToString(), reader["CONFIDENTIALITY_LEVEL"].ToString()));
                    }
                    reader.Close();

                    // Видаляємо файли з бази даних, яких немає в папці
                    foreach (var dbFile in dbFiles)
                    {
                        if (!files.Any(f => Path.GetFileName(f) == dbFile.FileName))
                        {
                            string deleteQuery = "DELETE FROM FILE_ACCESS WHERE FILE_NAME = @FileName";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, con))
                            {
                                deleteCmd.Parameters.AddWithValue("@FileName", dbFile.FileName);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }
                    }

                    // Додаємо нові файли до бази даних і фільтруємо їх за роллю користувача
                    foreach (string file in files)
                    {
                        string fileName = Path.GetFileName(file);

                        // Перевіряємо, чи файл вже є в базі даних
                        string checkQuery = "SELECT COUNT(*) FROM FILE_ACCESS WHERE FILE_NAME = @FileName";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@FileName", fileName);
                            int fileExists = (int)checkCmd.ExecuteScalar();

                            // Якщо файлу немає в базі даних, додаємо його
                            if (fileExists == 0)
                            {
                                string insertQuery = "INSERT INTO FILE_ACCESS (FILE_NAME, CONFIDENTIALITY_LEVEL) VALUES (@FileName, @ConfidentialityLevel)";
                                using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                                {
                                    insertCmd.Parameters.AddWithValue("@FileName", fileName);
                                    insertCmd.Parameters.AddWithValue("@ConfidentialityLevel", "Secret"); // Рівень конфіденційності за замовчуванням
                                    insertCmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // Отримуємо рівень конфіденційності файлу з бази даних
                        string confidentialityQuery = "SELECT CONFIDENTIALITY_LEVEL FROM FILE_ACCESS WHERE FILE_NAME = @FileName";
                        using (SqlCommand confidentialityCmd = new SqlCommand(confidentialityQuery, con))
                        {
                            confidentialityCmd.Parameters.AddWithValue("@FileName", fileName);
                            string confidentialityLevel = confidentialityCmd.ExecuteScalar()?.ToString();

                            // Фільтруємо файли відповідно до ролі користувача
                            if (IsFileAccessAllowed(confidentialityLevel))
                            {
                                listBoxFiles.Items.Add(fileName);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Папка не знайдена!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для перевірки доступу до файлу на основі ролі користувача
        private bool IsFileAccessAllowed(string confidentialityLevel)
        {
            switch (userRole)
            {
                case "Admin":
                    return true; // Адміністратор бачить всі файли
                case "Developer":
                    return confidentialityLevel == "Confidential" || confidentialityLevel == "Public";
                case "User":
                    return confidentialityLevel == "Public";
                default:
                    return false; // Якщо роль не визначена, доступ заборонений
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
                        pictureBox.Visible = true;
                        currentImage?.Dispose();
                        using (var tempImage = Image.FromFile(currentFilePath))
                        {
                            currentImage = new Bitmap(tempImage);
                        }

                        pictureBox.Image = currentImage;
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        buttonRotate.Visible = true;
                        btnSave.Visible = true;
                    }
                    else
                    {
                        pictureBox.Visible = false;
                        buttonRotate.Visible = false;
                        btnSave.Visible = false;
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
                pictureBox.Visible = false;
                buttonRotate.Visible = false;
                btnSave.Visible = false;
                currentImage?.Dispose();
                currentImage = null;
                pictureBox.Image = null;
            }
        }

        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
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