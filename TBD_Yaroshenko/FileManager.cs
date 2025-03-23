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
        private string accessControlType;
        private string _username;
        private DateTime? accessStartTime = null; // Час початку доступу
        private int? accessDurationSeconds = null; // Тривалість доступу в секундах
        private Dictionary<string, FileAccessInfo> _fileAccessDict = new Dictionary<string, FileAccessInfo>();
        private HashSet<string> blockedFiles = new HashSet<string>();

        public FileManager(string username, string accessControlType)
        {
            InitializeComponent();
            buttonRotate.Visible = false;
            btnSave.Visible = false;
            pictureBox.Visible = false;
            textBoxFileContent.Visible = false;
         

            accessTimer.Interval = 1000;
            accessTimer.Tick += AccessTimer_Tick;
            accessTimer.Start();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("No username specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _username = username;
            this.accessControlType = accessControlType;

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
                        userSecurityLevel = result != null ? result.ToString() : "Administrative";
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Синхронізуємо файли з базою даних
            string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
            SyncFilesWithDatabase(folderPath);

            // Перевіряємо доступні файли для користувача
            if (accessControlType == "Discretionary" && !CheckUserHasAccess())
            {
                MessageBox.Show("No one has given you access to the files yet. Contact the resource owners.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }
        }

        private void AccessTimer_Tick(object sender, EventArgs e)
        {
            if (accessControlType != "Discretionary" || !accessStartTime.HasValue)
            {
                labelAccessTime.Text = "Доступ: необмежений";
                return;
            }

            TimeSpan elapsed = DateTime.Now - accessStartTime.Value;
            int remaining = accessDurationSeconds.Value - (int)elapsed.TotalSeconds;

            if (remaining > 0)
            {
                labelAccessTime.Text = $"Залишилось: {remaining} сек.";
            }
            else
            {
                accessTimer.Stop();
                labelAccessTime.Text = "Доступ закрито!";
                MessageBox.Show("Час вичерпано для поточного файлу", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Приховуємо вміст файлу
                HideFileContent();

                // Додаємо файл до списку заблокованих
                if (!string.IsNullOrEmpty(currentFilePath))
                {
                    string fileName = Path.GetFileName(currentFilePath);
                    blockedFiles.Add(fileName);
                }
            }
        }

        private bool CheckUserHasAccess()
        {
            var accessibleFiles = new List<string>();
            string query = "SELECT FILE_NAME FROM USER_FILE_ACCESS WHERE USERNAME = @username AND CAN_READ = 1";

            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", _username);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accessibleFiles.Add(reader.GetString(0));
                    }
                }
            }
            return accessibleFiles.Count > 0;
        }

        private void SyncFilesWithDatabase(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                MessageBox.Show("Folder not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] files = Directory.GetFiles(folderPath);
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                foreach (string filePath in files)
                {
                    string fileName = Path.GetFileName(filePath);
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    string checkQuery = "SELECT COUNT(*) FROM FILE_ACCESS WHERE FILE_NAME = @FileName";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@FileName", fileName);
                        int count = (int)checkCmd.ExecuteScalar();

                        if (count == 0)
                        {
                            string confidentialityLevel = DetermineConfidentialityLevel(fileName);
                            string insertQuery = "INSERT INTO FILE_ACCESS (FILE_NAME, CONFIDENTIALITY_LEVEL) VALUES (@FileName, @ConfidentialityLevel)";
                            using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                            {
                                insertCmd.Parameters.AddWithValue("@FileName", fileName);
                                insertCmd.Parameters.AddWithValue("@ConfidentialityLevel", confidentialityLevel);
                                insertCmd.ExecuteNonQuery();
                            }

                            bool canExecute = fileExtension == ".exe" || fileExtension == ".lnk";
                            string insertAccessQuery = @"
                                INSERT INTO USER_FILE_ACCESS (USERNAME, FILE_NAME, CAN_READ, CAN_WRITE, OWN, CAN_EXECUTE)
                                VALUES (@Username, @FileName, @CanRead, @CanWrite, @Own, @CanExecute)";
                            using (SqlCommand insertAccessCmd = new SqlCommand(insertAccessQuery, con))
                            {
                                insertAccessCmd.Parameters.AddWithValue("@Username", "Admin");
                                insertAccessCmd.Parameters.AddWithValue("@FileName", fileName);
                                insertAccessCmd.Parameters.AddWithValue("@CanRead", 1);
                                insertAccessCmd.Parameters.AddWithValue("@CanWrite", 1);
                                insertAccessCmd.Parameters.AddWithValue("@Own", 1);
                                insertAccessCmd.Parameters.AddWithValue("@CanExecute", canExecute ? 1 : 0);
                                insertAccessCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        private string DetermineConfidentialityLevel(string fileName)
        {
            if (fileName.Contains("logo")) return "Unclassified";
            else if (fileName.Contains("readme")) return "FOUO";
            else if (fileName.Contains("data")) return "Confidential";
            else if (fileName.Contains("config")) return "Secret";
            else if (fileName.Contains("app")) return "Top Secret";
            else return "Administrative";
        }

        private void LoadFilesMandatory(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                string[] files = Directory.GetFiles(folderPath);
                listBoxFiles.Items.Clear();

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string selectQuery = "SELECT FILE_NAME, CONFIDENTIALITY_LEVEL FROM FILE_ACCESS";
                    using (SqlCommand selectCmd = new SqlCommand(selectQuery, con))
                    {
                        using (SqlDataReader reader = selectCmd.ExecuteReader())
                        {
                            var dbFiles = new List<(string FileName, string ConfidentialityLevel)>();
                            while (reader.Read())
                            {
                                dbFiles.Add((reader["FILE_NAME"].ToString(), reader["CONFIDENTIALITY_LEVEL"].ToString()));
                            }

                            foreach (var dbFile in dbFiles)
                            {
                                if (files.Any(f => Path.GetFileName(f) == dbFile.FileName) && IsAccessAllowed(dbFile.ConfidentialityLevel))
                                {
                                    listBoxFiles.Items.Add(dbFile.FileName);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Folder not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool isLoading = false;

        private void LoadFilesDiscretionary(string folderPath)
        {
            if (isLoading) return;
            isLoading = true;

            listBoxFiles.Items.Clear();
            var accessibleFiles = new List<string>();
            string query = @"
        SELECT FILE_NAME 
        FROM USER_FILE_ACCESS 
        WHERE USERNAME = @username 
        AND (
            ( -- Для .exe та .lnk перевіряємо CAN_EXECUTE
                (FILE_NAME LIKE '%.exe' OR FILE_NAME LIKE '%.lnk') 
                AND CAN_EXECUTE = 1
            )
            OR 
            ( -- Для інших файлів перевіряємо CAN_READ
                FILE_NAME NOT LIKE '%.exe' 
                AND FILE_NAME NOT LIKE '%.lnk' 
                AND CAN_READ = 1
            )
        )";


            using (SqlConnection conn = new SqlConnection(cs))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", _username);
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        accessibleFiles.Add(reader.GetString(0));
                    }
                }
            }

            if (accessibleFiles.Count == 0)
            {
                MessageBox.Show("No one has given you access to the files yet. Contact the resource owners.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                listBoxFiles.Items.AddRange(accessibleFiles.ToArray());
                MessageBox.Show($"User: {_username}, Available files: {string.Join(", ", accessibleFiles)}");
            }

            isLoading = false;
        }

        private bool IsAccessAllowed(string fileConfidentialityLevel)
        {
            var securityLevels = new List<string> { "Top Secret", "Secret", "Confidential", "FOUO", "Unclassified" };
            int userLevelIndex = securityLevels.IndexOf(userSecurityLevel);
            int fileLevelIndex = securityLevels.IndexOf(fileConfidentialityLevel);
            return userLevelIndex <= fileLevelIndex;
        }

        private void FileManager_Load(object sender, EventArgs e)
        {
            string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
            if (accessControlType == "Role-based")
            {
                MessageBox.Show("Unknown access delimitation type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (accessControlType == "Discretionary")
            {
                LoadFilesDiscretionary(folderPath);
            }
            else
            {
                LoadFilesMandatory(folderPath);
            }
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            accessTimer.Stop();

            if (listBoxFiles.SelectedIndex == -1) return;

            string selectedFile = listBoxFiles.SelectedItem.ToString();
            if (blockedFiles.Contains(selectedFile))
            {
                MessageBox.Show("Доступ до цього файлу вичерпано", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                listBoxFiles.SelectedIndex = -1;
                return;
            }

            string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
            currentFilePath = Path.Combine(folderPath, selectedFile);

            if (accessControlType == "Discretionary")
            {
                if (!GetAccessDuration(selectedFile))
                {
                    MessageBox.Show("Доступ заборонено", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (accessDurationSeconds.HasValue)
                {
                    accessStartTime = DateTime.Now;
                    accessTimer.Start();
                }
                else
                {
                    labelAccessTime.Text = "Доступ: необмежений";
                }

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = "SELECT CAN_READ, CAN_WRITE FROM USER_FILE_ACCESS WHERE USERNAME=@user AND FILE_NAME=@file";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@user", _username);
                        cmd.Parameters.AddWithValue("@file", selectedFile);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool canRead = reader.GetBoolean(0);
                                bool canWrite = reader.GetBoolean(1);
                                if (canRead) OpenFile(canWrite);
                            }
                        }
                    }
                }
            }
            else
            {
                OpenFile(true);
            }
        }

        private void OpenFile(bool canWrite)
        {
            string fileName = Path.GetFileName(currentFilePath);
            if (blockedFiles.Contains(fileName))
            {
                MessageBox.Show("Доступ до цього файлу вичерпано", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_fileAccessDict.ContainsKey(fileName))
            {
                var accessInfo = _fileAccessDict[fileName];
                if (accessInfo.DurationSeconds != -1)
                {
                    TimeSpan elapsed = DateTime.Now - accessInfo.AccessStartTime;
                    if (elapsed.TotalSeconds > accessInfo.DurationSeconds)
                    {
                        accessInfo.IsExpired = true;
                        MessageBox.Show("Час доступу вичерпано", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        HideFileContent();
                        return;
                    }
                }
            }

            try
            {
                string fileExtension = Path.GetExtension(currentFilePath).ToLower();

                // Сховати всі елементи інтерфейсу перед відображенням нового вмісту
                textBoxFileContent.Visible = false;
                pictureBox.Visible = false;
                buttonRotate.Visible = false;
                btnSave.Visible = false;

                switch (fileExtension)
                {
                    case ".txt":
                        textBoxFileContent.Text = File.ReadAllText(currentFilePath);
                        textBoxFileContent.Visible = true;
                        if (canWrite)
                        {
                            textBoxFileContent.ReadOnly = false; // Дозволяємо редагування
                            btnSave.Visible = true; // Показуємо кнопку збереження
                        }
                        else
                        {
                            textBoxFileContent.ReadOnly = true; // Лише для читання
                        }
                        break;

                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                        pictureBox.Visible = true;
                        currentImage?.Dispose();
                        using (var tempImage = Image.FromFile(currentFilePath))
                        {
                            currentImage = new Bitmap(tempImage);
                        }
                        pictureBox.Image = currentImage;
                        pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        buttonRotate.Visible = true;
                        btnSave.Visible = canWrite;
                        break;

                    case ".exe":
                        Process.Start(currentFilePath);
                        break;

                    default:
                        Process.Start(new ProcessStartInfo(currentFilePath) { UseShellExecute = true });
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка відкриття файлу: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HideFileContent()
        {
            textBoxFileContent.Visible = false;
            pictureBox.Visible = false;
            buttonRotate.Visible = false;
            btnSave.Visible = false;
        }

        private bool GetAccessDuration(string fileName)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string query = "SELECT DURATION_SECONDS FROM USER_FILE_ACCESS WHERE USERNAME = @Username AND FILE_NAME = @FileName";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Username", _username);
                    cmd.Parameters.AddWithValue("@FileName", fileName);

                    var result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        accessDurationSeconds = (result == DBNull.Value) ? null : Convert.ToInt32(result);
                        if (accessDurationSeconds.HasValue) _fileAccessDict[fileName] = new FileAccessInfo { AccessStartTime = DateTime.Now, DurationSeconds = accessDurationSeconds.Value };
                        return true;
                    }
                    return false;
                }
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
            if (!string.IsNullOrEmpty(currentFilePath))
            {
                try
                {
                    string fileExtension = Path.GetExtension(currentFilePath).ToLower();
                    if (fileExtension == ".txt")
                    {
                        File.WriteAllText(currentFilePath, textBoxFileContent.Text);
                        MessageBox.Show("Зміни збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (currentImage != null && fileExtension is ".png" or ".jpg" or ".jpeg" or ".gif")
                    {
                        currentImage.Save(currentFilePath, System.Drawing.Imaging.ImageFormat.Png);
                        MessageBox.Show("Зміни збережено!", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не вдалося зберегти зміни: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Немає змін для збереження або файл не вибрано.", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public class FileAccessInfo
        {
            public DateTime AccessStartTime { get; set; }
            public int DurationSeconds { get; set; }
            public bool IsExpired { get; set; }
        }
    }
}