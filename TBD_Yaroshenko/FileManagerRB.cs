using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;

namespace TBD_Yaroshenko
{
    public partial class FileManagerRB : Form
    {
        // Поля класу
        private string _username;
        private string _userRole;
        private string _connectionString = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;
        private string _dataFolderPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\Data";
        private List<FileItem> _allFiles = new List<FileItem>();
        private Dictionary<string, Process> _runningProcesses = new Dictionary<string, Process>();
        private string _currentFilePath;
        private Image _currentImage;
        private Dictionary<string, FileAccessTimer> _accessTimers = new Dictionary<string, FileAccessTimer>();
        private HashSet<string> _expiredFiles = new HashSet<string>();
        private HashSet<string> _shownExpiredFiles = new HashSet<string>();




        // Клас для зберігання інформації про файл
        public class FileItem
        {
            public string Name { get; set; }
            public string ConfidentialityLevel { get; set; }
            public string FilePath { get; set; }
        }

        // Клас для зберігання прав доступу
        public class FileAccessInfo
        {
            public bool CanRead { get; set; }
            public bool CanWrite { get; set; }
            public bool CanExecute { get; set; }
            public int DurationSeconds { get; set; }
            public bool IsExpired { get; set; }

            // Додаємо час початку доступу
            public DateTime AccessStartTime { get; set; }

            // Властивість для перевірки часу
            public bool CheckIfExpired()
            {
                if (DurationSeconds <= 0) return false;
                IsExpired = (DateTime.Now - AccessStartTime).TotalSeconds >= DurationSeconds;
                return IsExpired;
            }
        }

        // Клас для керування таймером доступу
        public class FileAccessTimer
        {
            public string FileName { get; set; }
            public DateTime StartTime { get; set; }
            public int DurationSeconds { get; set; }

            public void Stop()
            {
                // Логіка зупинки таймера
            }
        }

        public FileManagerRB(string username, string role)
        {
            InitializeComponent();
            _username = username;
            _userRole = role;

            InitializeFileAccessData();
            LoadAvailableFiles();
            InitializeTimer();
        }

        private void InitializeFileAccessData()
        {
            // Ініціалізація даних про доступ до файлів
            // (можна завантажити з бази даних)
        }

        private void InitializeTimer()
        {
            accessTimer.Interval = 1000;
            accessTimer.Tick += accessTimer_Tick;
        }

        private void LoadAvailableFiles()
        {
            listBoxFiles.Items.Clear();

            try
            {
                var files = Directory.GetFiles(_dataFolderPath)
                    .Select(f => Path.GetFileName(f))
                    .Where(f => !string.IsNullOrEmpty(f));

                foreach (var file in files)
                {
                    if (HasFileAccess(file))
                    {
                        listBoxFiles.Items.Add(file);
                        _allFiles.Add(new FileItem
                        {
                            Name = file,
                            FilePath = Path.Combine(_dataFolderPath, file),
                            ConfidentialityLevel = GetFileConfidentialityLevel(file)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження файлів: {ex.Message}", "Помилка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetFileConfidentialityLevel(string fileName)
        {
            // Отримати рівень конфіденційності з бази даних
            return "Public"; // Заглушка
        }

        private bool HasFileAccess(string fileName)
        {
            // Адмін має доступ до всіх файлів
            if (_userRole == "Admin") return true;

            // Перевірка прав доступу для інших ролей
            var accessInfo = GetCurrentFileAccessInfo(fileName);
            return accessInfo != null && (accessInfo.CanRead || accessInfo.CanWrite || accessInfo.CanExecute);
        }

        private FileAccessInfo GetCurrentFileAccessInfo(string fileName)
        {
            // First check if this file is locally marked as expired
            if (_expiredFiles.Contains(fileName))
            {
                return new FileAccessInfo
                {
                    CanRead = false,
                    CanWrite = false,
                    CanExecute = false,
                    IsExpired = true
                };
            }

            try
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    string query = @"
                SELECT CAN_READ, CAN_WRITE, CAN_EXECUTE, DURATION_SECONDS
                FROM FILE_ACCESS_RB
                WHERE FILE_NAME = @fileName AND ROLE = @role";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@fileName", fileName);
                    cmd.Parameters.AddWithValue("@role", _userRole);
                    con.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new FileAccessInfo
                            {
                                CanRead = Convert.ToBoolean(reader["CAN_READ"]),
                                CanWrite = Convert.ToBoolean(reader["CAN_WRITE"]),
                                CanExecute = Convert.ToBoolean(reader["CAN_EXECUTE"]),
                                DurationSeconds = Convert.ToInt32(reader["DURATION_SECONDS"]),
                                AccessStartTime = DateTime.Now
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Помилка отримання прав доступу: {ex.Message}");
            }

            return null;
        }

        private void OpenSelectedFile()
        {
            if (listBoxFiles.SelectedItem == null) return;

            string selectedFileName = listBoxFiles.SelectedItem.ToString();
            var file = _allFiles.FirstOrDefault(f => f.Name == selectedFileName);

            if (file == null || !File.Exists(file.FilePath))
            {
                MessageBox.Show("Файл не знайдено", "Помилка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var accessInfo = GetCurrentFileAccessInfo(selectedFileName);

            if (accessInfo != null && accessInfo.IsExpired)
            {
                MessageBox.Show("Доступ до цього файлу закінчився", "Помилка доступу",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _currentFilePath = file.FilePath;
            string fileExtension = Path.GetExtension(_currentFilePath).ToLower();

            try
            {

                textBoxFileContent.Visible = false;
                pictureBox.Visible = false;
                btnSave.Visible = false;
                buttonRotate.Visible = false;

                switch (fileExtension)
                {
                    case ".txt":
                        if (accessInfo?.CanRead == true || _userRole == "Admin")
                        {
                            textBoxFileContent.Text = File.ReadAllText(_currentFilePath);
                            textBoxFileContent.Visible = true;
                            textBoxFileContent.ReadOnly = !(accessInfo?.CanWrite == true || _userRole == "Admin");
                            btnSave.Visible = accessInfo?.CanWrite == true || _userRole == "Admin";

                            if (accessInfo?.DurationSeconds > 0 && _userRole != "Admin")
                            {
                                StartAccessTimer(selectedFileName, accessInfo.DurationSeconds);
                            }
                        }
                        else
                        {
                            MessageBox.Show("У вас немає прав для читання цього файлу", "Помилка доступу",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                    case ".gif":
                        if (accessInfo?.CanRead == true || _userRole == "Admin")
                        {
                            pictureBox.Visible = true;
                            _currentImage?.Dispose();
                            _currentImage = Image.FromFile(_currentFilePath);
                            pictureBox.Image = _currentImage;
                            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            buttonRotate.Visible = accessInfo?.CanWrite == true || _userRole == "Admin";
                            btnSave.Visible = accessInfo?.CanWrite == true || _userRole == "Admin";

                            if (accessInfo?.DurationSeconds > 0 && _userRole != "Admin")
                            {
                                StartAccessTimer(selectedFileName, accessInfo.DurationSeconds);
                            }
                        }
                        else
                        {
                            MessageBox.Show("У вас немає прав для перегляду цього зображення", "Помилка доступу",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    case ".exe":
                    case ".lnk":
                        if (accessInfo?.CanExecute == true || _userRole == "Admin")
                        {
                            try
                            {
                                var process = Process.Start(new ProcessStartInfo(_currentFilePath)
                                {
                                    UseShellExecute = true
                                });

                                if (process != null)
                                {
                                    _runningProcesses[selectedFileName] = process;
                                    process.EnableRaisingEvents = true;
                                    process.Exited += (sender, e) =>
                                    {
                                        _runningProcesses.Remove(selectedFileName);
                                    };

                                    if (_userRole != "Admin" && accessInfo?.DurationSeconds > 0)
                                    {
                                        StartAccessTimer(selectedFileName, accessInfo.DurationSeconds);
                                    }
                                }
                            }
                            catch (Win32Exception winEx)
                            {
                                MessageBox.Show($"Помилка запуску програми: {winEx.Message}", "Помилка",
                                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("У вас немає прав для запуску цього файлу", "Помилка доступу",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    default:
                        MessageBox.Show("Непідтримуваний тип файлу", "Помилка",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Доступ до файлу заборонено", "Помилка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StartAccessTimer(string fileName, int durationSeconds)
        {
            if (_accessTimers.ContainsKey(fileName))
            {
                _accessTimers[fileName].Stop();
                _accessTimers.Remove(fileName);
            }

            var timerInfo = new FileAccessTimer
            {
                FileName = fileName,
                StartTime = DateTime.Now,
                DurationSeconds = durationSeconds
            };

            _accessTimers[fileName] = timerInfo;
            accessTimer.Start();
            UpdateAccessTimeLabel();
        }


        private void BlockFileAccess(string fileName)
        {
            // Just track expiration locally instead of modifying the database
            _expiredFiles.Add(fileName);

            // Optional: Show message to user
            if (!_shownExpiredFiles.Contains(fileName))
            {
                MessageBox.Show($"Доступ до файлу {fileName} закінчився",
                              "Інформація",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                _shownExpiredFiles.Add(fileName);
            }
        }

        private void UpdateAccessTimeLabel()
        {
            if (listBoxFiles.SelectedItem == null) return;

            string selectedFile = listBoxFiles.SelectedItem.ToString();
            if (_accessTimers.TryGetValue(selectedFile, out var timerInfo))
            {
                var remaining = timerInfo.DurationSeconds - (DateTime.Now - timerInfo.StartTime).TotalSeconds;
                labelAccessTime.Text = $"Доступ закінчиться через: {Math.Max(0, (int)remaining)} сек";
            }
            else if (_userRole == "Admin")
            {
                labelAccessTime.Text = "Доступ без обмежень";
            }
            else
            {
                labelAccessTime.Text = "Доступ не обмежений за часом";
            }
        }

        private void CloseCurrentFile()
        {
            textBoxFileContent.Visible = false;
            pictureBox.Visible = false;
            btnSave.Visible = false;
            buttonRotate.Visible = false;

            if (_currentImage != null)
            {
                _currentImage.Dispose();
                _currentImage = null;
                pictureBox.Image = null;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath)) return;

            try
            {
                string fileExtension = Path.GetExtension(_currentFilePath).ToLower();

                if (fileExtension == ".txt")
                {
                    File.WriteAllText(_currentFilePath, textBoxFileContent.Text);
                    MessageBox.Show("Файл успішно збережено", "Успіх",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (fileExtension == ".png" || fileExtension == ".jpg" ||
                         fileExtension == ".jpeg" || fileExtension == ".gif")
                {
                    _currentImage.Save(_currentFilePath);
                    MessageBox.Show("Зображення успішно збережено", "Успіх",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (_currentImage != null)
            {
                _currentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox.Image = _currentImage;
                pictureBox.Refresh();
            }
        }

        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpenSelectedFile();
        }

        private void accessTimer_Tick(object sender, EventArgs e)
        {
            bool anyTimerRunning = false;
            List<string> filesToBlock = new List<string>();

            foreach (var timerInfo in _accessTimers.Values.ToList())
            {
                var elapsed = DateTime.Now - timerInfo.StartTime;
                if (elapsed.TotalSeconds >= timerInfo.DurationSeconds)
                {
                    filesToBlock.Add(timerInfo.FileName);

                    // Закриваємо файл, якщо він відкритий
                    if (listBoxFiles.SelectedItem?.ToString() == timerInfo.FileName)
                    {
                        CloseCurrentFile();
                    }

                    // Закриваємо процес для .exe файлів
                    if (_runningProcesses.TryGetValue(timerInfo.FileName, out var process))
                    {
                        try
                        {
                            if (!process.HasExited)
                            {
                                process.Kill();
                                process.WaitForExit(1000);
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Помилка закриття процесу: {ex.Message}");
                        }
                        finally
                        {
                            _runningProcesses.Remove(timerInfo.FileName);
                        }
                    }
                }
                else
                {
                    anyTimerRunning = true;
                }
            }

            // Блокуємо доступ до файлів
            foreach (var fileName in filesToBlock)
            {
                if (_accessTimers.ContainsKey(fileName))
                {
                    _accessTimers[fileName].Stop();
                    _accessTimers.Remove(fileName);

                    // Оновлюємо статус файлу в базі даних
                    BlockFileAccess(fileName);

                    // Показуємо повідомлення
                    MessageBox.Show($"Доступ до файлу {fileName} закінчився",
                                  "Інформація",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }
            }

            UpdateAccessTimeLabel();

            if (!anyTimerRunning)
            {
                accessTimer.Stop();
            }
        }

        public void ResetFileAccess(string fileName)
        {
            _expiredFiles.Remove(fileName);
            _shownExpiredFiles.Remove(fileName);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            foreach (var process in _runningProcesses.Values)
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                }
                catch { }
            }

            _currentImage?.Dispose();
            base.OnFormClosing(e);
        }
    }
}