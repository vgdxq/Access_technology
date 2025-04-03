using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


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

        // Клас для зберігання прав доступу до файлів
        public class FileAccessInfo
        {
            public bool CanRead { get; set; }
            public bool CanWrite { get; set; }
            public bool CanExecute { get; set; }
            public int DurationSeconds { get; set; }
            public bool IsExpired { get; set; }
            public DateTime AccessStartTime { get; set; }

            // Метод для перевірки чи закінчився час доступу
            public bool CheckIfExpired()
            {
                if (DurationSeconds <= 0) return false;
                IsExpired = (DateTime.Now - AccessStartTime).TotalSeconds >= DurationSeconds;
                return IsExpired;
            }
        }

        // Клас для керування таймером доступу до файлу
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

        // Ініціалізація даних про доступ до файлів
        private void InitializeFileAccessData()
        {
            // Можна завантажити дані з бази даних
        }

        // Ініціалізація таймера для контролю часу доступу
        private void InitializeTimer()
        {
            accessTimer.Interval = 1000;
            accessTimer.Tick += accessTimer_Tick;
        }

        // Завантаження доступних файлів для поточного користувача
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
                MessageBox.Show($"Error loading files: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Отримання рівня конфіденційності файлу
        private string GetFileConfidentialityLevel(string fileName)
        {
            // Заглушка - реальна реалізація має отримувати дані з БД
            return "Public";
        }

        // Перевірка чи має поточний користувач доступ до файлу
        private bool HasFileAccess(string fileName)
        {
            // Адміністратор має доступ до всіх файлів
            if (_userRole == "Admin") return true;

            // Перевірка прав доступу для інших ролей
            var accessInfo = GetCurrentFileAccessInfo(fileName);
            return accessInfo != null && (accessInfo.CanRead || accessInfo.CanWrite || accessInfo.CanExecute);
        }

        // Отримання інформації про поточні права доступу до файлу
        private FileAccessInfo GetCurrentFileAccessInfo(string fileName)
        {
            // Спочатку перевіряємо чи файл вже позначений як протермінований
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
                Debug.WriteLine($"Error getting access rights: {ex.Message}");
            }

            return null;
        }

        // Відкриття обраного файлу з перевіркою прав доступу
        private void OpenSelectedFile()
        {
            if (listBoxFiles.SelectedItem == null) return;

            string selectedFileName = listBoxFiles.SelectedItem.ToString();
            var file = _allFiles.FirstOrDefault(f => f.Name == selectedFileName);

            if (file == null || !File.Exists(file.FilePath))
            {
                MessageBox.Show("File not found", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var accessInfo = GetCurrentFileAccessInfo(selectedFileName);

            if (accessInfo != null && accessInfo.IsExpired)
            {
                MessageBox.Show("Access to this file has expired", "Access Error",
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
                            MessageBox.Show("You don't have permission to read this file", "Access Error",
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
                            MessageBox.Show("You don't have permission to view this image", "Access Error",
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
                                MessageBox.Show($"Error starting program: {winEx.Message}", "Error",
                                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("You don't have permission to execute this file", "Access Error",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        break;

                    default:
                        MessageBox.Show("Unsupported file type", "Error",
                                      MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access to the file is denied", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Запуск таймера для контролю часу доступу до файлу
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

        // Блокування доступу до файлу після закінчення часу
        private void BlockFileAccess(string fileName)
        {
            // Додаємо файл до списку протермінованих (якщо ще не додано)
            if (_expiredFiles.Add(fileName))
            {
                // Показуємо повідомлення тільки при першому додаванні
                this.Invoke((MethodInvoker)(() =>
                {
                    MessageBox.Show($"Access to file {fileName} has expired",
                                  "Access Expired",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                }));
            }
        }

        // Оновлення відображення часу доступу
        private void UpdateAccessTimeLabel()
        {
            if (listBoxFiles.SelectedItem == null) return;

            string selectedFile = listBoxFiles.SelectedItem.ToString();
            if (_accessTimers.TryGetValue(selectedFile, out var timerInfo))
            {
                var remaining = timerInfo.DurationSeconds - (DateTime.Now - timerInfo.StartTime).TotalSeconds;
                labelAccessTime.Text = $"Access expires in: {Math.Max(0, (int)remaining)} sec";
            }
            else if (_userRole == "Admin")
            {
                labelAccessTime.Text = "Unlimited access";
            }
            else
            {
                labelAccessTime.Text = "No time limit for access";
            }
        }

        // Закриття поточного файлу
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

        // Обробник події збереження файлу
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_currentFilePath)) return;

            try
            {
                string fileExtension = Path.GetExtension(_currentFilePath).ToLower();

                if (fileExtension == ".txt")
                {
                    File.WriteAllText(_currentFilePath, textBoxFileContent.Text);
                    MessageBox.Show("File saved successfully", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (fileExtension == ".png" || fileExtension == ".jpg" ||
                         fileExtension == ".jpeg" || fileExtension == ".gif")
                {
                    _currentImage.Save(_currentFilePath);
                    MessageBox.Show("Image saved successfully", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving file: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник події обертання зображення
        private void buttonRotate_Click(object sender, EventArgs e)
        {
            if (_currentImage != null)
            {
                _currentImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pictureBox.Image = _currentImage;
                pictureBox.Refresh();
            }
        }

        // Обробник зміни вибраного файлу у списку
        private void listBoxFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            OpenSelectedFile();
        }

        // Обробник події таймера для контролю часу доступу
        private void accessTimer_Tick(object sender, EventArgs e)
        {
            bool anyTimerRunning = false;
            var currentTime = DateTime.Now;

            foreach (var timerInfo in _accessTimers.Values.ToList())
            {
                if ((currentTime - timerInfo.StartTime).TotalSeconds >= timerInfo.DurationSeconds)
                {
                    // Блокуємо доступ
                    BlockFileAccess(timerInfo.FileName);

                    // Закриваємо файл, якщо він відкритий
                    if (listBoxFiles.SelectedItem?.ToString() == timerInfo.FileName)
                    {
                        CloseCurrentFile();
                    }

                    // Закриваємо процеси
                    if (_runningProcesses.TryGetValue(timerInfo.FileName, out var process))
                    {
                        try
                        {
                            if (!process.HasExited) process.Kill();
                        }
                        catch { /* Ігноруємо помилки */ }
                        finally
                        {
                            _runningProcesses.Remove(timerInfo.FileName);
                        }
                    }

                    // Видаляємо таймер
                    _accessTimers.Remove(timerInfo.FileName);
                }
                else
                {
                    anyTimerRunning = true;
                }
            }

            UpdateAccessTimeLabel();

            if (!anyTimerRunning)
            {
                accessTimer.Stop();
            }
        }

        // Скидання обмежень доступу до файлу
        public void ResetFileAccess(string fileName)
        {
            _expiredFiles.Remove(fileName);
            _shownExpiredFiles.Remove(fileName);
        }

        // Обробник закриття форми
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