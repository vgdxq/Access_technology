using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Configuration;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace TBD_Yaroshenko
{
    public partial class FileManager : Form
    {
        private string currentFilePath = string.Empty;
        private Bitmap currentImage;
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;
        private string userSecurityLevel;

        public FileManager(string username)
        {
            InitializeComponent();
            buttonRotate.Visible = false;
            btnSave.Visible = false;
            pictureBox.Visible = false;

     
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Ім'я користувача не вказано.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Отримуємо рівень конфіденційності користувача з бази даних
            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "SELECT SECURITY_LEVEL FROM LOGIN_TBL WHERE USERNAME = @Username";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 50)).Value = username;

                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            userSecurityLevel = result.ToString();
                        }
                        else
                        {
                            userSecurityLevel = "Administrative"; // Значення за замовчуванням
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Помилка бази даних: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Синхронізуємо файли з базою даних
            string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
            SyncFilesWithDatabase(folderPath);

            // Завантажуємо файли для відображення
            LoadFiles(folderPath);
        }

        // синхронізації файлів з папки з таблицею FILE_ACCESS
        private void SyncFilesWithDatabase(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Папка не знайдена!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Отримуємо список файлів у папці
            string[] files = Directory.GetFiles(folderPath);

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();

                // Додаємо файли до таблиці FILE_ACCESS
                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);

                    // Перевіряємо, чи файл вже існує в базі даних
                    string checkQuery = "SELECT COUNT(*) FROM FILE_ACCESS WHERE FILE_NAME = @FileName";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 100)).Value = fileName;
                        int count = (int)checkCmd.ExecuteScalar();

                        // Якщо файл не існує в базі, додаємо його
                        if (count == 0)
                        {
                            // Визначаємо рівень конфіденційності для нового файлу
                            string confidentialityLevel = DetermineConfidentialityLevel(fileName);

                            string insertQuery = "INSERT INTO FILE_ACCESS (FILE_NAME, CONFIDENTIALITY_LEVEL) VALUES (@FileName, @ConfidentialityLevel)";
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                            {
                                insertCmd.Parameters.Add(new SqlParameter("@FileName", SqlDbType.VarChar, 100)).Value = fileName;
                                insertCmd.Parameters.Add(new SqlParameter("@ConfidentialityLevel", SqlDbType.VarChar, 20)).Value = confidentialityLevel;
                                insertCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        // визначення рівня конфіденційності файлу
        private string DetermineConfidentialityLevel(string fileName)
        {
            // Приклад: визначаємо рівень конфіденційності на основі імені файлу
            if (fileName.Contains("logo"))
            {
                return "Unclassified"; // Не таємно
            }
            else if (fileName.Contains("readme"))
            {
                return "FOUO"; // Для службового використання
            }
            else if (fileName.Contains("data"))
            {
                return "Confidential"; // Таємно
            }
            else if (fileName.Contains("config"))
            {
                return "Secret"; // Цілком таємно
            }
            else if (fileName.Contains("app"))
            {
                return "Top Secret"; // Особливої таємності
            }
            else
            {
                return "Administrative"; // За замовчуванням
            }
        }

        // завантаження файлів з папки
        private void LoadFiles(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                listBoxFiles.Items.Clear();

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string selectQuery = "SELECT FILE_NAME, CONFIDENTIALITY_LEVEL FROM FILE_ACCESS";
                    SqlCommand selectCmd = new SqlCommand(selectQuery, con);
                    SqlDataReader reader = selectCmd.ExecuteReader();

                    var dbFiles = new List<(string FileName, string ConfidentialityLevel)>();
                    while (reader.Read())
                    {
                        dbFiles.Add((reader["FILE_NAME"].ToString(), reader["CONFIDENTIALITY_LEVEL"].ToString()));
                    }
                    reader.Close();

                    // Додаємо файли до списку, якщо користувач має доступ
                    foreach (var dbFile in dbFiles)
                    {
                        if (files.Any(f => Path.GetFileName(f) == dbFile.FileName) && IsAccessAllowed(dbFile.ConfidentialityLevel))
                        {
                            listBoxFiles.Items.Add(dbFile.FileName);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Папка не знайдена!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Метод для перевірки доступу до файлу
        private bool IsAccessAllowed(string fileConfidentialityLevel)
        {
            // Рівні конфіденційності від найвищого до найнижчого
            var securityLevels = new List<string> { "Top Secret", "Secret", "Confidential", "FOUO", "Unclassified" };

            int userLevelIndex = securityLevels.IndexOf(userSecurityLevel);
            int fileLevelIndex = securityLevels.IndexOf(fileConfidentialityLevel);

            // Користувач може отримати доступ, якщо його рівень вищий або дорівнює рівню файлу
            return userLevelIndex <= fileLevelIndex;
        }

        // завантаження форми
        private void FileManager_Load(object sender, EventArgs e)
        {
            string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
            LoadFiles(folderPath);
        }

        //  вибор файлу
        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxFiles.SelectedIndex != -1)
            {
                string selectedFile = listBoxFiles.SelectedItem.ToString();
                string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
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
        }

        // обертання зображення
        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                currentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox.Image = currentImage;
                pictureBox.Invalidate();
            }
        }

        // збереження зображення
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
    }
}