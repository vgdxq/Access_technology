using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Configuration;
using System.Text;
using System.Collections.Concurrent;
using Microsoft.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace TBD_Yaroshenko
{
    public partial class radioNoInfo : Form
    {
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString; // Рядок підключення до БД
        private string complexPattern = "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$"; // Патерн складності паролю


        public radioNoInfo()
        {
            InitializeComponent();
            // Додаємо варіанти контролю доступу у комбобокс
            comboBoxAccessControl.Items.AddRange(new string[] { "Mandatory", "Discretionary", "Role-Based" });
            comboBoxAccessControl.SelectedIndex = 0; // Встановлюємо значення за замовчуванням


        }

        // Обробник кнопки довідки
        private void довідкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Performed by: Yaroshenko Iryna, group B-125-21-3-B");
        }

        // Метод для визначення складності паролю
        private string GetPasswordComplexity(string password)
        {
            return Regex.IsMatch(password, complexPattern) ? "High" : "Low";
        }

        // Метод для збереження історії паролів
        private void SavePasswordHistory(string username, string password)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "INSERT INTO PASSWORD_HISTORY (USERNAME, OLD_PASSWORD, CHANGE_DATE) VALUES (@username, @password, GETDATE())", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 50)).Value = username;
                    cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar, 100)).Value = password;
                    con.Open();
                    cmd.ExecuteNonQuery();
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
        }

        // Метод для перевірки чи пароль був використаний раніше
        private bool IsPasswordInHistory(string username, string password)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM (SELECT TOP 3 OLD_PASSWORD FROM PASSWORD_HISTORY WHERE USERNAME = @username ORDER BY CHANGE_DATE DESC) AS LastPasswords WHERE OLD_PASSWORD = @password", con))
                {
                    cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 50)).Value = username;
                    cmd.Parameters.Add(new SqlParameter("@password", SqlDbType.VarChar, 100)).Value = password;
                    con.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Обробник зміни стану чекбоксу для показу паролю
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        // Обробник зміни стану чекбоксу для перевірки складності паролю
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The current password does not meet the new complexity requirements", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Обробник кнопки входу
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields for authorization", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string accessControlType = comboBoxAccessControl.SelectedItem?.ToString() ?? "Undefined";

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                string query = "SELECT * FROM LOGIN_TBL WHERE USERNAME = @user AND PASS = @pass";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@user", textBox1.Text);
                cmd.Parameters.AddWithValue("@pass", textBox2.Text);
                var dr = cmd.ExecuteReader();

                if (dr.HasRows)
                {
                    dr.Read();
                    string securityLevel = dr["SECURITY_LEVEL"]?.ToString() ?? "Unclassified"; // Змінили ім'я змінної
                    string userRole = dr["ROLE"]?.ToString() ?? "User"; // Використовуємо інше ім'я змінної

                    dr.Close();
                    string updateQuery = "UPDATE LOGIN_TBL SET ACCESS_CONTROL_TYPE = @accessControlType WHERE USERNAME = @user";
                    var updateCmd = new SqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@accessControlType", accessControlType);
                    updateCmd.Parameters.AddWithValue("@user", textBox1.Text);
                    updateCmd.ExecuteNonQuery();

                    MessageBox.Show($"Login successful! Security Level: {securityLevel}, Access Control Type: {accessControlType}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    if (accessControlType == "Discretionary")
                    {
                        MainWind_Discretionary mainWindDiscretionary = new MainWind_Discretionary(textBox1.Text, accessControlType);
                        mainWindDiscretionary.Show();
                    }
                    else if (accessControlType == "Role-Based")
                    {
                        MainWind_RoleBased mainWindRoleBased = new MainWind_RoleBased(textBox1.Text, userRole); // Використовуємо userRole
                        mainWindRoleBased.Show();
                    }
                    else
                    {
                        MainWind mainWind = new MainWind(textBox1.Text);
                        mainWind.Show();
                    }

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Authorization error. Incorrect login or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Обробник кнопки реєстрації
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The password does not meet the complexity requirements", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var con = new SqlConnection(cs))
                using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM LOGIN_TBL WHERE USERNAME = @user", con))
                {
                    checkCmd.Parameters.Add(new SqlParameter("@user", SqlDbType.VarChar, 50)).Value = textBox1.Text;
                    con.Open();
                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        MessageBox.Show("User already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Отримуємо вибраний тип контролю доступу
                    string accessControlType = comboBoxAccessControl.SelectedItem?.ToString() ?? "Undefined";

                    using (var insertCmd = new SqlCommand(
                        "INSERT INTO LOGIN_TBL (USERNAME, PASS, PASSWORD_COMPLEXITY, SECURITY_LEVEL, ACCESS_CONTROL_TYPE) VALUES (@user, @pass, @complexity, 'Unclassified', @accessControlType)", con))
                    {
                        insertCmd.Parameters.Add(new SqlParameter("@user", SqlDbType.VarChar, 50)).Value = textBox1.Text;
                        insertCmd.Parameters.Add(new SqlParameter("@pass", SqlDbType.VarChar, 100)).Value = textBox2.Text;
                        insertCmd.Parameters.Add(new SqlParameter("@complexity", SqlDbType.VarChar, 10)).Value = GetPasswordComplexity(textBox2.Text);
                        insertCmd.Parameters.Add(new SqlParameter("@accessControlType", SqlDbType.VarChar, 20)).Value = accessControlType;
                        insertCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("User added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        }

        // Обробник кнопки зміни паролю
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The new password does not meet complexity requirements", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (IsPasswordInHistory(textBox1.Text, textBox2.Text))
            {
                MessageBox.Show("Cannot use one of the last three passwords", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var con = new SqlConnection(cs))
                {
                    // Визначаємо складність нового пароля
                    string newPasswordComplexity = GetPasswordComplexity(textBox2.Text);

                    // Оновлюємо пароль і складність пароля
                    using (var cmd = new SqlCommand("UPDATE LOGIN_TBL SET PASS = @newPass, PASSWORD_COMPLEXITY = @complexity WHERE USERNAME = @user", con))
                    {
                        cmd.Parameters.Add(new SqlParameter("@user", SqlDbType.VarChar, 50)).Value = textBox1.Text;
                        cmd.Parameters.Add(new SqlParameter("@newPass", SqlDbType.VarChar, 100)).Value = textBox2.Text;
                        cmd.Parameters.Add(new SqlParameter("@complexity", SqlDbType.VarChar, 10)).Value = newPasswordComplexity;
                        con.Open();
                        if (cmd.ExecuteNonQuery() > 0)
                        {
                            SavePasswordHistory(textBox1.Text, textBox2.Text);
                            MessageBox.Show("Password successfully updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Обробник завантаження форми
        }
        #region Brute Force

        private bool isBruteForceRunning = false;
        private DateTime bruteForceStartTime;
        private string currentUsername;
        private string foundPassword;
        private int bruteForceAttempts;
        private long totalIterations;
        private long currentIteration;

        private void btnStartBrute_Click_1(object sender, EventArgs e)
        {
            if (isBruteForceRunning) return;
            txtBruteLog.Clear();

            var txtBruteUser = this.txtBruteUser;
            var numLength = this.numLength;
            var chkExactLength = this.chkExactLength;
            var lblStatus = this.lblBruteStatus;
            var progressBar = this.progressBarBrute;

            if (string.IsNullOrWhiteSpace(txtBruteUser?.Text))
            {
                MessageBox.Show("Please enter username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentUsername = txtBruteUser.Text;
            int targetLength = chkNoInfo.Checked ? 0 : (int)numLength.Value; // Використовуємо CheckBox замість RadioButton
            bool exactLength = chkExactLength.Checked && !chkNoInfo.Checked; // Вимкнення точної довжини у режимі без інформації

            // Формуємо набір символів для перебору
            var charSets = new List<string>();
            if (chkNoInfo.Checked ||
        (!chkLowerLatin.Checked && !chkUpperLatin.Checked && !chkDigits.Checked &&
         !chkSpecial.Checked && !chkCyrillic.Checked))
            {
                // Режим без інформації - всі символи
                charSets.Add("abcdefghijklmnopqrstuvwxyz");
                charSets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                charSets.Add("0123456789");
                charSets.Add("!@#$%^&*()_+-=[]{}|;:,.<>?");
                charSets.Add("абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ");
            }
            else
            {
                // Режим з інформацією - тільки вибрані символи
                if (this.chkLowerLatin.Checked)
                    charSets.Add("abcdefghijklmnopqrstuvwxyz");
                if (this.chkUpperLatin.Checked)
                    charSets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                if (this.chkDigits.Checked)
                    charSets.Add("0123456789");
                if (this.chkSpecial.Checked)
                    charSets.Add("!@#$%^&*()_+-=[]{}|;:,.<>?");
                if (this.chkCyrillic.Checked)
                    charSets.Add("абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ");
            }

            if (charSets.Count == 0)
            {
                MessageBox.Show("Please select at least one character set", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string chars = string.Join("", charSets);
            totalIterations = (long)Math.Pow(chars.Length, targetLength > 0 ? targetLength : 1); // Запобігаємо помилці при targetLength = 0
            currentIteration = 0;

            if (targetLength > 8 || chkNoInfo.Checked)
            {
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.Maximum = 100;
            }
            else
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
            }
            progressBar.Value = 0;
            lblProgress.Text = "0%";

            isBruteForceRunning = true;
            bruteForceAttempts = 0;
            foundPassword = null;
            bruteForceStartTime = DateTime.Now;

            this.btnStartBrute.Enabled = false;
            this.btnStopBrute.Enabled = true;
            lblStatus.Text = "Status: In progress...";

            Task.Run(() =>
            {
                try
                {
                    if (exactLength && !chkNoInfo.Checked)
                    {
                        BruteForceExactLength(currentUsername, chars, targetLength);
                    }
                    else
                    {
                        BruteForceVariableLength(currentUsername, chars, targetLength);
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }

                this.Invoke((MethodInvoker)delegate
                {
                    isBruteForceRunning = false;
                    btnStartBrute.Enabled = true;
                    btnStopBrute.Enabled = false;
                    progressBar.Value = progressBar.Maximum;
                    progressBar.Style = ProgressBarStyle.Continuous;

                    if (foundPassword != null)
                    {
                        lblStatus.Text = $"Status: Found! Password: {foundPassword}";
                        lblProgress.Text = "100%";
                        MessageBox.Show($"Password found: {foundPassword}\nTime: {(DateTime.Now - bruteForceStartTime).TotalSeconds:F2} sec\nAttempts: {bruteForceAttempts}",
                        "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblStatus.Text = "Status: Not Found";
                        lblProgress.Text = "Completed";
                        MessageBox.Show("Password not found with the specified parameters",
                        "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                });
            });
        }

        private void chkNoInfo_CheckedChanged_1(object sender, EventArgs e)
        {
            // Вмикаємо/вимикаємо інші елементи керування при зміні стану CheckBox
            bool noInfoMode = chkNoInfo.Checked;

            numLength.Enabled = !noInfoMode;
            chkExactLength.Enabled = !noInfoMode;
            chkLowerLatin.Enabled = !noInfoMode;
            chkUpperLatin.Enabled = !noInfoMode;
            chkDigits.Enabled = !noInfoMode;
            chkSpecial.Enabled = !noInfoMode;
            chkCyrillic.Enabled = !noInfoMode;
        }


        private void btnStopBrute_Click_1(object sender, EventArgs e)
        {
            isBruteForceRunning = false;
            this.btnStopBrute.Enabled = false;
            this.lblBruteStatus.Text = "Status: Stopped by user";
            this.progressBarBrute.Value = 0;
            this.progressBarBrute.Style = ProgressBarStyle.Continuous;
            this.lblProgress.Text = "Stopped";
        }

        private void BruteForceExactLength(string username, string chars, int length)
        {
            var indices = new int[length];
            var password = new char[length];

            while (isBruteForceRunning && foundPassword == null)
            {
                // Генеруємо пароль
                for (int i = 0; i < length; i++)
                {
                    password[i] = chars[indices[i]];
                }

                bruteForceAttempts++;
                currentIteration++;

                if (bruteForceAttempts % 10000 == 0)
                {
                    UpdateBruteLog(new string(password));
                    UpdateProgressBar();
                }

                if (CheckPassword(username, new string(password)))
                {
                    foundPassword = new string(password);
                    UpdateBruteLog($"Password found: {foundPassword}");
                    break;
                }

                // Оновлюємо індекси
                for (int i = length - 1; i >= 0; i--)
                {
                    if (indices[i] < chars.Length - 1)
                    {
                        indices[i]++;
                        break;
                    }
                    indices[i] = 0;
                }
            }
        }

        private void BruteForceVariableLength(string username, string chars, int approximateLength)
        {
            int[] lengths;

            if (approximateLength == 0)
            {
                lengths = Enumerable.Range(1, 25).ToArray();
            }
            else
            {
                lengths = new[] { approximateLength - 1, approximateLength, approximateLength + 1 };
            }

            foreach (int len in lengths)
            {
                if (!isBruteForceRunning) break;
                BruteForceRecursive(username, chars, len, "");
                if (foundPassword != null) break;
            }
        }

        private void BruteForceRecursive(string username, string chars, int length, string current)
        {
            if (!isBruteForceRunning || foundPassword != null) return;

            if (current.Length == length)
            {
                bruteForceAttempts++;
                currentIteration++;

                if (bruteForceAttempts % 1000 == 0)
                {
                    UpdateBruteLog(current);
                    UpdateProgressBar();
                }

                if (CheckPassword(username, current))
                {
                    foundPassword = current;
                    UpdateBruteLog($"Password found: {current}");
                }
            }
            else
            {
                Parallel.ForEach(chars, (c, state) =>
                {
                    if (!isBruteForceRunning || foundPassword != null)
                    {
                        state.Break();
                        return;
                    }
                    BruteForceRecursive(username, chars, length, current + c);
                });
            }
        }

        private void UpdateBruteLog(string attempt)
        {
            if (txtBruteLog.InvokeRequired)
            {
                txtBruteLog.BeginInvoke(new Action<string>(UpdateBruteLog), attempt);
            }
            else
            {
                txtBruteLog.AppendText($"{DateTime.Now:HH:mm:ss}: {attempt}\n");
                txtBruteLog.ScrollToCaret();
            }
        }
        private void UpdateProgressBar()
        {
            if (progressBarBrute.InvokeRequired)
            {
                progressBarBrute.Invoke(new Action(UpdateProgressBar));
            }
            else
            {
                if (totalIterations > 0)
                {
                    int progress = (int)((currentIteration * 100) / totalIterations);
                    progressBarBrute.Value = Math.Min(progress, 100);
                }
            }
        }

        private bool CheckPassword(string username, string password)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM LOGIN_TBL WHERE USERNAME = @user AND PASS = @pass", con))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    con.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Dictionary Attack

        private bool isDictionaryAttackRunning = false;
        private string dictionaryPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\rockyou.txt";
        private string englishDictionaryPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\english_words.txt";
        private List<string> dictionary;
        private HashSet<string> englishWords = new HashSet<string>();

        // Завантаження англійського словника
        private async Task LoadEnglishDictionaryAsync()
        {
            try
            {
                if (!File.Exists(englishDictionaryPath))
                {
                    string directory = Path.GetDirectoryName(englishDictionaryPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                    string[] defaultWords = { "hello", "world", "testing" };
                    await File.WriteAllLinesAsync(englishDictionaryPath, defaultWords);
                    MessageBox.Show($"Файл англійського словника не знайдено за шляхом: {englishDictionaryPath}. Створено новий файл із початковими словами.",
                                   "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    englishWords = new HashSet<string>(defaultWords.Select(w => w.ToLower()));
                    return;
                }

                var lines = await File.ReadAllLinesAsync(englishDictionaryPath);
                englishWords = new HashSet<string>(
                    lines.Where(word => word.Length >= 3)
                         .Select(word => word.Trim().ToLower())
                );

                var commonVariations = englishWords
         .Where(w => w.Length >= 3)
         .SelectMany(w => new[]
         {
            w + "123",      // apple123
            w + "!",        // apple!
            w + "123!",      // apple123!
            w + "love",     // applelove
            w + "password", // applepassword
            char.ToUpper(w[0]) + w.Substring(1), // Apple
            w + w,          // appleapple
            w + "1",         // apple1
            w.Replace('e', '3') // appl3
         })
         .ToList();

                foreach (var variation in commonVariations)
                {
                    englishWords.Add(variation);
                }

                // Діагностика: виведення вмісту englishWords
                txtBruteLog.AppendText($"Завантажено englishWords: {string.Join(", ", englishWords)}\n");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Несподівана помилка під час завантаження англійського словника: {ex.Message}",
                               "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                englishWords = new HashSet<string>();
            }
        }

        private List<string> ExtractEnglishWords(string password)
        {
            var words = Regex.Matches(password, @"([A-Z][a-z]+)|([a-z]+)")
               .Cast<Match>()
               .Select(m => m.Value.ToLower())
               .ToList();

            // Додатковий пошук слів із великої літери
            var capitalized = words.Select(w => char.ToUpper(w[0]) + w.Substring(1)).ToList();
            words.AddRange(capitalized);

            return words.Where(w => englishWords.Contains(w))
                       .Distinct()
                       .ToList();
        }

        private async void btnStartDictionary_Click_1(object sender, EventArgs e)
        {
            if (isDictionaryAttackRunning) return;
            txtBruteLog.Clear();

            if (string.IsNullOrWhiteSpace(txtBruteUser.Text))
            {
                MessageBox.Show("Будь ласка, введіть ім'я користувача", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(dictionaryPath))
            {
                MessageBox.Show("Файл словника не знайдено!", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (englishWords.Count == 0)
            {
                await LoadEnglishDictionaryAsync();
                txtBruteLog.AppendText($"Завантажено англійський словник із {englishWords.Count} словами\n");
            }

            currentUsername = txtBruteUser.Text;
            isDictionaryAttackRunning = true;
            foundPassword = null;
            bruteForceStartTime = DateTime.Now;
            bruteForceAttempts = 0;

            this.btnStartDictionary.Enabled = false;
            this.btnStopDictionary.Enabled = true;
            lblBruteStatus.Text = "Статус: Словникова атака в процесі...";

            await Task.Run(() =>
            {
                try
                {
                    dictionary = File.ReadAllLines(dictionaryPath).ToList();
                    var filteredDictionary = FilterDictionary(dictionary);

                    txtBruteLog.Invoke((MethodInvoker)delegate
                    {
                        txtBruteLog.AppendText($"Завантажено словник із {dictionary.Count} записів\n");
                        txtBruteLog.AppendText($"Після фільтрації: {filteredDictionary.Count} кандидатів\n");
                    });

                    List<string> similarPasswords = new List<string>();
                    bool exactMatchFound = false;

                    foreach (var password in filteredDictionary)
                    {
                        if (!isDictionaryAttackRunning) break;

                        bruteForceAttempts++;

                        if (bruteForceAttempts % 1000 == 0)
                        {
                            UpdateBruteLog($"Спроба: {password}");
                        }

                        if (CheckPassword(currentUsername, password))
                        {
                            foundPassword = password;
                            exactMatchFound = true;
                            UpdateBruteLog($"Знайдено пароль: {password}");
                            break;
                        }
                        else
                        {
                            var similarityScore = CalculateSimilarity("apple pie love password", password);
                            txtBruteLog.Invoke((MethodInvoker)delegate
                            {
                                txtBruteLog.AppendText($"Пароль: {password}, Схожість: {similarityScore:F2}\n");
                            });
                            if (similarityScore > 0)
                            {
                                similarPasswords.Add($"{password} (схожість: {similarityScore:F2})");
                            }
                        }
                    }

                    this.Invoke((MethodInvoker)delegate
                    {
                        isDictionaryAttackRunning = false;
                        btnStartDictionary.Enabled = true;
                        btnStopDictionary.Enabled = false;

                        if (exactMatchFound)
                        {
                            lblBruteStatus.Text = $"Статус: Знайдено! Пароль: {foundPassword}";
                            MessageBox.Show($"Пароль знайдено: {foundPassword}\nЧас: {(DateTime.Now - bruteForceStartTime).TotalSeconds:F2} сек\nСпроби: {bruteForceAttempts}",
                                "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            lblBruteStatus.Text = "Статус: Не знайдено";
                            txtBruteLog.AppendText($"Знайдено {similarPasswords.Count} подібних паролів\n");

                            if (similarPasswords.Any())
                            {
                                var topSimilar = similarPasswords.Take(10).ToList();
                                string similarList = string.Join("\n", topSimilar);
                                MessageBox.Show($"Точний пароль не знайдено, але ось схожі паролі:\n\n{similarList}",
                                    "Подібні паролі", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Пароль не знайдено в словнику, і подібних паролів також немає",
                                    "Результат", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show($"Помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }
        private double CalculateSimilarity(string targetPassword, string testPassword)
        {
            var targetWords = ExtractEnglishWords(targetPassword);
            var testWords = ExtractEnglishWords(testPassword);

            if (targetWords.Count == 0 || testWords.Count == 0)
                return 0;

            // Додаємо пошук часткових збігів (наприклад, "appl" vs "apple")
            double score = 0;
            foreach (var tw in targetWords)
            {
                foreach (var pw in testWords)
                {
                    if (tw.Contains(pw) || pw.Contains(tw))
                    {
                        score += (double)Math.Min(tw.Length, pw.Length) / Math.Max(tw.Length, pw.Length);
                    }
                }
            }

            return score / Math.Max(targetWords.Count, testWords.Count);
        }

        private List<string> FindSimilarPasswords(string targetPassword, List<string> passwordDictionary, int maxResults = 20)
        {
            var targetWords = ExtractEnglishWords(targetPassword);
            if (targetWords.Count == 0) return new List<string>();

            var similarPasswords = new Dictionary<string, int>();
            var targetWordsSet = new HashSet<string>(targetWords);

            foreach (var password in passwordDictionary)
            {
                if (similarPasswords.Count >= maxResults * 5) break;

                var commonWords = ExtractEnglishWords(password)
                    .Count(w => targetWordsSet.Contains(w));

                if (commonWords > 0)
                {
                    similarPasswords[password] = commonWords;
                }
            }

            return similarPasswords
                .OrderByDescending(p => p.Value)
                .ThenBy(p => p.Key.Length)
                .Take(maxResults)
                .Select(p => p.Key)
                .ToList();
        }

        private List<string> FilterDictionary(List<string> dictionary)
        {
            if (chkNoInfo.Checked) return dictionary;

            IEnumerable<string> filtered = dictionary;

            if (chkExactLength.Checked)
            {
                int length = (int)numLength.Value;
                filtered = filtered.Where(p => p.Length == length);
            }
            else
            {
                int approxLength = (int)numLength.Value;
                filtered = filtered.Where(p => p.Length >= approxLength - 1 && p.Length <= approxLength + 1);
            }

            var charSets = new List<string>();
            if (chkLowerLatin.Checked) charSets.Add("abcdefghijklmnopqrstuvwxyz");
            if (chkUpperLatin.Checked) charSets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            if (chkDigits.Checked) charSets.Add("0123456789");
            if (chkSpecial.Checked) charSets.Add("!@#$%^&*()_+-=[]{}|;:,.<>?");
            if (chkCyrillic.Checked) charSets.Add("абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ");

            if (charSets.Any())
            {
                filtered = filtered.Where(p => p.All(c => charSets.Any(set => set.Contains(c))));
            }

            return filtered.ToList();
        }

        private void btnStopDictionary_Click(object sender, EventArgs e)
        {
            isDictionaryAttackRunning = false;
            this.btnStopDictionary.Enabled = false;
            this.lblBruteStatus.Text = "Status: Dictionary attack stopped";
        }

        #endregion
    }
}