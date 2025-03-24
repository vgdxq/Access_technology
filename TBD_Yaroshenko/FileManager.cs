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
        private string currentFilePath = string.Empty; // Поточний шлях до файлу
        private Bitmap currentImage; // Поточне зображення для перегляду
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString; // Рядок підключення до БД
        private string userSecurityLevel; // Рівень безпеки користувача
        private string accessControlType; // Тип контролю доступу
        private string _username; // Ім'я поточного користувача
        private DateTime? accessStartTime = null; // Час початку доступу
        private int? accessDurationSeconds = null; // Тривалість доступу в секундах
        private Dictionary<string, FileAccessInfo> _fileAccessDict = new Dictionary<string, FileAccessInfo>(); // Словник для зберігання інформації про доступ
        private Dictionary<string, Process> _runningProcesses = new Dictionary<string, Process>(); // Словник для відстеження запущених процесів
        private HashSet<string> blockedFiles = new HashSet<string>(); // Множина заблокованих файлів

        public FileManager(string username, string accessControlType)
        {
            InitializeComponent();
            this.FormClosing += FileManager_FormClosing;

            // Приховуємо елементи інтерфейсу на початку
            buttonRotate.Visible = false;
            btnSave.Visible = false;
            pictureBox.Visible = false;
            textBoxFileContent.Visible = false;

            // Налаштування таймера для контролю часу доступу
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

            // Налаштування інтерфейсу в залежності від типу контролю доступу
            labelAccessTime.Visible = (accessControlType != "Mandatory");

            if (accessControlType != "Mandatory")
            {
                accessTimer.Start();
            }

            // Отримання рівня конфіденційності користувача з БД
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

            // Синхронізація файлів з базою даних
            string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
            SyncFilesWithDatabase(folderPath);

            // Перевірка доступу для дискреційного контролю
            if (accessControlType == "Discretionary" && !CheckUserHasAccess())
            {
                MessageBox.Show("No one has given you access to the files yet. Contact the resource owners.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
                return;
            }
        }

        // Обробник закриття форми
        private void FileManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Звільнення ресурсів
            if (currentImage != null)
            {
                currentImage.Dispose();
                currentImage = null;
            }

            // Зупинка таймера
            if (accessTimer != null)
            {
                accessTimer.Stop();
                accessTimer.Dispose();
            }
        }

        // Обробник події таймера для контролю часу доступу
        private void AccessTimer_Tick(object sender, EventArgs e)
        {
            if (accessControlType == "Mandatory") return;

            if (!accessStartTime.HasValue || !accessDurationSeconds.HasValue)
            {
                labelAccessTime.Text = "Access: unlimited";
                return;
            }

            TimeSpan elapsed = DateTime.Now - accessStartTime.Value;
            int remaining = accessDurationSeconds.Value - (int)elapsed.TotalSeconds;

            if (remaining > 0)
            {
                labelAccessTime.Text = $"Time left: {remaining} sec.";
            }
            else
            {
                accessTimer.Stop();
                labelAccessTime.Text = "Access closed!";

                CloseRunningProcesses();
                MessageBox.Show("Time expired for current file", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                HideFileContent();

                if (!string.IsNullOrEmpty(currentFilePath))
                {
                    string fileName = Path.GetFileName(currentFilePath);
                    blockedFiles.Add(fileName);
                }
            }
        }

        // Метод для закриття запущених процесів
        private void CloseRunningProcesses()
        {
            var processesToClose = _runningProcesses.Values.ToList();

            foreach (var process in processesToClose)
            {
                try
                {
                    if (process != null && !process.HasExited)
                    {
                        bool closedGracefully = false;
                        var notepadProcesses = Process.GetProcessesByName("notepad++");
                        foreach (Process p in notepadProcesses)
                        {
                            try
                            {
                                if (p.Id == process.Id)
                                {
                                    closedGracefully = p.CloseMainWindow();
                                    if (!p.WaitForExit(2000))
                                    {
                                        p.Kill();
                                    }
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error closing process {p.Id}: {ex.Message}");
                            }
                        }

                        if (!closedGracefully && !process.HasExited)
                        {
                            process.Kill();
                        }
                    }
                }
                catch (InvalidOperationException ex)
                {
                    Debug.WriteLine($"Process already finished: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error closing process: {ex.Message}");
                }
                finally
                {
                    try
                    {
                        process?.Dispose();
                    }
                    catch { }
                }
            }

            _runningProcesses.Clear();
        }

        // Перевірка наявності доступу до файлів
        private bool CheckUserHasAccess()
        {
            var accessibleFiles = new List<string>();
            string query = @"SELECT FILE_NAME FROM USER_FILE_ACCESS 
                   WHERE USERNAME = @username 
                   AND (CAN_READ = 1 OR CAN_EXECUTE = 1)";

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

        // Синхронізація файлів з базою даних
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

        // Визначення рівня конфіденційності файлу за його ім'ям
        private string DetermineConfidentialityLevel(string fileName)
        {
            if (fileName.Contains("logo")) return "Unclassified";
            else if (fileName.Contains("readme")) return "FOUO";
            else if (fileName.Contains("data")) return "Confidential";
            else if (fileName.Contains("config")) return "Secret";
            else if (fileName.Contains("app")) return "Top Secret";
            else return "Administrative";
        }

        // Завантаження файлів для мандатного контролю доступу
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

        // Завантаження файлів для дискреційного контролю доступу
        private void LoadFilesDiscretionary(string folderPath)
        {
            if (isLoading) return;
            isLoading = true;

            listBoxFiles.Items.Clear();
            var accessibleFiles = new List<string>();
            string query = @"SELECT FILE_NAME 
                FROM USER_FILE_ACCESS 
                WHERE USERNAME = @username 
                AND (
                    (FILE_NAME LIKE '%.exe' OR FILE_NAME LIKE '%.lnk') AND CAN_EXECUTE = 1
                    OR 
                    (NOT (FILE_NAME LIKE '%.exe' OR FILE_NAME LIKE '%.lnk') AND CAN_READ = 1)
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

        // Перевірка дозволу доступу за рівнем конфіденційності
        private bool IsAccessAllowed(string fileConfidentialityLevel)
        {
            var securityLevels = new List<string> { "Top Secret", "Secret", "Confidential", "FOUO", "Unclassified" };
            int userLevelIndex = securityLevels.IndexOf(userSecurityLevel);
            int fileLevelIndex = securityLevels.IndexOf(fileConfidentialityLevel);
            return userLevelIndex <= fileLevelIndex;
        }

        // Обробник завантаження форми
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

        // Обробник вибору файлу у списку
        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            accessTimer.Stop();

            if (listBoxFiles.SelectedIndex == -1) return;

            string selectedFile = listBoxFiles.SelectedItem.ToString();
            if (blockedFiles.Contains(selectedFile))
            {
                MessageBox.Show("Access to this file has expired", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                listBoxFiles.SelectedIndex = -1;
                return;
            }

            string folderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
            currentFilePath = Path.Combine(folderPath, selectedFile);
            string fileExtension = Path.GetExtension(currentFilePath).ToLower();
            bool isExecutable = fileExtension == ".exe" || fileExtension == ".lnk";

            if (accessControlType == "Discretionary")
            {
                if (!GetAccessDuration(selectedFile))
                {
                    MessageBox.Show("Access denied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (accessDurationSeconds.HasValue)
                {
                    accessStartTime = DateTime.Now;
                    accessTimer.Start();
                }
                else
                {
                    labelAccessTime.Text = "Access: unlimited";
                }

                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();
                    string query = isExecutable
                        ? "SELECT CAN_EXECUTE, CAN_WRITE FROM USER_FILE_ACCESS WHERE USERNAME=@user AND FILE_NAME=@file"
                        : "SELECT CAN_READ, CAN_WRITE FROM USER_FILE_ACCESS WHERE USERNAME=@user AND FILE_NAME=@file";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@user", _username);
                        cmd.Parameters.AddWithValue("@file", selectedFile);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool hasAccess = isExecutable
                                    ? reader.GetBoolean(0)
                                    : reader.GetBoolean(0);
                                bool canWrite = reader.GetBoolean(1);

                                if (hasAccess)
                                {
                                    OpenFile(canWrite, isExecutable);
                                }
                                else
                                {
                                    MessageBox.Show("Access denied", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                OpenFile(true, isExecutable);
            }
        }

        // Відкриття файлу
        private void OpenFile(bool canWrite, bool isExecutable)
        {
            string fileName = Path.GetFileName(currentFilePath);
            if (blockedFiles.Contains(fileName))
            {
                MessageBox.Show("Access to this file has expired", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        MessageBox.Show("Access time expired", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        HideFileContent();
                        return;
                    }
                }
            }

            try
            {
                string fileExtension = Path.GetExtension(currentFilePath).ToLower();

                // Сховати всі елементи інтерфейсу
                textBoxFileContent.Visible = false;
                pictureBox.Visible = false;
                buttonRotate.Visible = false;
                btnSave.Visible = false;

                switch (fileExtension)
                {
                    case ".txt":
                        textBoxFileContent.Text = File.ReadAllText(currentFilePath);
                        textBoxFileContent.Visible = true;
                        textBoxFileContent.ReadOnly = !canWrite;
                        btnSave.Visible = canWrite;
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
                        buttonRotate.Visible = canWrite;
                        btnSave.Visible = canWrite;
                        break;

                    case ".exe":
                    case ".lnk":
                        if (isExecutable)
                        {
                            try
                            {
                                var startInfo = new ProcessStartInfo
                                {
                                    FileName = currentFilePath,
                                    UseShellExecute = true,
                                    Verb = "open"
                                };

                                var process = Process.Start(startInfo);
                                _runningProcesses[fileName] = process;
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Failed to open shortcut: {ex.Message}", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        break;

                    default:
                        Process.Start(new ProcessStartInfo(currentFilePath) { UseShellExecute = true });
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Приховування вмісту файлу
        private void HideFileContent()
        {
            textBoxFileContent.Visible = false;
            pictureBox.Visible = false;
            buttonRotate.Visible = false;
            btnSave.Visible = false;
        }

        // Отримання тривалості доступу до файлу
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
                        int duration = (result == DBNull.Value) ? 0 : Convert.ToInt32(result);
                        accessDurationSeconds = duration == 0 ? null : duration;
                        if (accessDurationSeconds.HasValue)
                            _fileAccessDict[fileName] = new FileAccessInfo { AccessStartTime = DateTime.Now, DurationSeconds = accessDurationSeconds.Value };
                        return true;
                    }
                    return false;
                }
            }
        }
        // Обробник кнопки обертання зображення
        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (currentImage != null)
            {
                currentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox.Image = currentImage;
                pictureBox.Invalidate();
            }
        }

        // Обробник кнопки збереження змін
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
                        MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (currentImage != null && fileExtension is ".png" or ".jpg" or ".jpeg" or ".gif")
                    {
                        currentImage.Save(currentFilePath, System.Drawing.Imaging.ImageFormat.Png);
                        MessageBox.Show("Changes saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to save changes: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No changes to save or file not selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Клас для зберігання інформації про доступ до файлу
        public class FileAccessInfo
        {
            public DateTime AccessStartTime { get; set; }
            public int DurationSeconds { get; set; }
            public bool IsExpired { get; set; }
        }
    }
}