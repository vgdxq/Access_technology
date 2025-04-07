using System;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TBD_Yaroshenko
{
    public partial class radioNoInfo : Form
    {
        // Рядок підключення до бази даних з конфігурації
        private string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;

        // Паттерн для перевірки складності пароля:
        // - Мінімум 8 символів
        // - Містить цифри або спецсимволи
        // - Містить великі та малі літери
        private string complexPattern = "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$";

        // Час останнього натискання клавіші (для запобігання швидкому вводу)
        private DateTime lastKeyPress = DateTime.MinValue;

        // Максимальна кількість спроб входу
        private const int MAX_ATTEMPTS = 3;

        // Мінімальний інтервал між натисканнями клавіш (в мілісекундах)
        private const int MIN_INPUT_DELAY_MS = 50;

        public radioNoInfo()
        {
            InitializeComponent();
            // Додаємо варіанти контролю доступу у комбобокс
            comboBoxAccessControl.Items.AddRange(new string[] { "Mandatory", "Discretionary", "Role-Based" });
            comboBoxAccessControl.SelectedIndex = 0;

            // Підключаємо обробники подій для поля пароля
            textBox2.KeyPress += textBox2_KeyPress;
            textBox2.KeyDown += textBox2_KeyDown;

            // Вимкнення контекстного меню (щоб заборонити вставку)
            textBox2.ContextMenuStrip = new ContextMenuStrip();
        }

        // Обробник пункту меню "Довідка"
        private void довідкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Completed by: Yaroshenko Iryna, group B-125-21-3-B", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Визначення складності пароля
        private string GetPasswordComplexity(string password)
        {
            return Regex.IsMatch(password, complexPattern) ? "High" : "Low";
        }

        // Збереження історії паролів
        private void SavePasswordHistory(string username, string password)
        {
            try
            {
                // Отримання поточної солі для користувача
                string currentSalt;
                using (var con = new SqlConnection(cs))
                using (var saltCmd = new SqlCommand("SELECT Salt FROM LOGIN_TBL WHERE USERNAME = @username", con))
                {
                    saltCmd.Parameters.AddWithValue("@username", username);
                    con.Open();
                    currentSalt = saltCmd.ExecuteScalar()?.ToString();
                }

                if (string.IsNullOrEmpty(currentSalt)) return;

                // Шифрування пароля з поточною сіллю перед збереженням
                string encryptedPassword = EncryptPassword(password, currentSalt);

                // Збереження в історію
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "INSERT INTO PASSWORD_HISTORY (USERNAME, OLD_PASSWORD, CHANGE_DATE) VALUES (@username, @password, GETDATE())", con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", encryptedPassword);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Перевірка, чи пароль використовувався раніше
        private bool IsPasswordInHistory(string username, string password)
        {
            try
            {
                // Отримання поточної солі користувача
                string currentSalt;
                using (var con = new SqlConnection(cs))
                using (var saltCmd = new SqlCommand("SELECT Salt FROM LOGIN_TBL WHERE USERNAME = @username", con))
                {
                    saltCmd.Parameters.AddWithValue("@username", username);
                    con.Open();
                    currentSalt = saltCmd.ExecuteScalar()?.ToString();
                }

                if (string.IsNullOrEmpty(currentSalt)) return false;

                // Шифрування нового пароля для порівняння
                string encryptedNewPassword = EncryptPassword(password, currentSalt);

                // Перевірка проти останніх 3 паролів в історії
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    @"SELECT COUNT(*) FROM (
                        SELECT TOP 3 OLD_PASSWORD 
                        FROM PASSWORD_HISTORY 
                        WHERE USERNAME = @username 
                        ORDER BY CHANGE_DATE DESC
                      ) AS LastPasswords 
                      WHERE OLD_PASSWORD = @password", con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", encryptedNewPassword);
                    con.Open();
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // Обробник зміни стану чекбоксу "Показати пароль"
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        // Обробник зміни стану чекбоксу "Перевірити складність"
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("Current password does not meet complexity requirements", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Запобігання швидкому вводу
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TimeSpan timeSinceLastPress = DateTime.Now - lastKeyPress;
            if (timeSinceLastPress.TotalMilliseconds < MIN_INPUT_DELAY_MS)
            {
                e.Handled = true;
                MessageBox.Show("Too fast input. Please enter the password manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            lastKeyPress = DateTime.Now;

            // Блокування спеціальних символів (крім Backspace)
            if (char.IsControl(e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

        // Запобігання вставці тексту
        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Control && e.KeyCode == Keys.V) || (e.Shift && e.KeyCode == Keys.Insert))
            {
                e.SuppressKeyPress = true;
                e.Handled = true;
                MessageBox.Show("Password insertion is prohibited. Enter the password manually.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            string username = textBox1.Text;
            string accessControlType = comboBoxAccessControl.SelectedItem?.ToString() ?? "Undefined";

            using (var con = new SqlConnection(cs))
            {
                con.Open();

                // Отримання даних користувача
                var userCheckCmd = new SqlCommand(
                    "SELECT Salt, IsBlocked, FailedAttempts, PASSWORD_CREATION_DATE, PASSWORD_EXPIRY_DAYS " +
                    "FROM LOGIN_TBL WHERE USERNAME = @user", con);
                userCheckCmd.Parameters.AddWithValue("@user", username);

                string salt = null;
                bool isBlocked = false;
                int failedAttempts = 0;
                DateTime? creationDate = null;
                int expiryDays = 0;

                using (var reader = userCheckCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        salt = reader["Salt"]?.ToString();
                        isBlocked = reader.GetBoolean(1);
                        failedAttempts = reader.GetInt32(2);
                        creationDate = reader.IsDBNull(3) ? (DateTime?)null : reader.GetDateTime(3);
                        expiryDays = reader.GetInt32(4);
                    }
                }

                if (string.IsNullOrEmpty(salt))
                {
                    MessageBox.Show("Invalid username or password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (isBlocked)
                {
                    MessageBox.Show("This account has been blocked due to too many failed attempts", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // CAPTCHA після 1 невдалої спроби
                if (failedAttempts >= 1)
                {
                    using (CaptchaForm captchaForm = new CaptchaForm())
                    {
                        if (captchaForm.ShowDialog() != DialogResult.OK)
                        {
                            MessageBox.Show("You failed the CAPTCHA", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                // Перевірка терміну дії пароля
                if (creationDate.HasValue)
                {
                    DateTime expiryDate = creationDate.Value.AddDays(expiryDays);
                    int daysRemaining = (int)(expiryDate - DateTime.Now).TotalDays;

                    if (DateTime.Now > expiryDate)
                    {
                        MessageBox.Show("Your password has expired. Please change your password.",
                        "Password Expiry", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else if (daysRemaining <= 7)
                    {
                        MessageBox.Show($"Warning! Your password will expire in {daysRemaining} days.",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                // Шифрування введеного пароля
                string encryptedPassword = EncryptPassword(textBox2.Text, salt);

                // Перевірка облікових даних
                string query = "SELECT SECURITY_LEVEL, ROLE FROM LOGIN_TBL WHERE USERNAME = @user AND PASS = @pass";
                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@user", username);
                cmd.Parameters.AddWithValue("@pass", encryptedPassword);

                string securityLevel = null;
                string userRole = null;
                bool loginSuccessful = false;

                using (var dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        dr.Read();
                        securityLevel = dr["SECURITY_LEVEL"]?.ToString() ?? "Unclassified";
                        userRole = dr["ROLE"]?.ToString() ?? "User";
                        loginSuccessful = true;
                    }
                }

                if (loginSuccessful)
                {
                    // Скидання лічильника невдалих спроб
                    string resetAttemptsQuery = "UPDATE LOGIN_TBL SET FailedAttempts = 0, IsBlocked = 0 WHERE USERNAME = @user";
                    var resetCmd = new SqlCommand(resetAttemptsQuery, con);
                    resetCmd.Parameters.AddWithValue("@user", username);
                    resetCmd.ExecuteNonQuery();

                    // Оновлення типу контролю доступу
                    string updateQuery = "UPDATE LOGIN_TBL SET ACCESS_CONTROL_TYPE = @accessControlType WHERE USERNAME = @user";
                    var updateCmd = new SqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@accessControlType", accessControlType);
                    updateCmd.Parameters.AddWithValue("@user", username);
                    updateCmd.ExecuteNonQuery();

                    MessageBox.Show($"Login successful! Security Level: {securityLevel}, Access Control Type: {accessControlType}",
 "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Відкриття відповідної головної форми
                    if (accessControlType == "Discretionary")
                    {
                        MainWind_Discretionary mainWindDiscretionary = new MainWind_Discretionary(username, accessControlType);
                        mainWindDiscretionary.Show();
                    }
                    else if (accessControlType == "Role-Based")
                    {
                        MainWind_RoleBased mainWindRoleBased = new MainWind_RoleBased(username, userRole);
                        mainWindRoleBased.Show();
                    }
                    else
                    {
                        MainWind mainWind = new MainWind(username);
                        mainWind.Show();
                    }
                    this.Hide();
                }
                else
                {
                    // Оновлення лічильника невдалих спроб
                    string updateAttemptsQuery = @"
                        UPDATE LOGIN_TBL 
                        SET FailedAttempts = FailedAttempts + 1,
                            IsBlocked = CASE WHEN FailedAttempts + 1 >= @maxAttempts THEN 1 ELSE 0 END
                        WHERE USERNAME = @user";
                    var updateCmd = new SqlCommand(updateAttemptsQuery, con);
                    updateCmd.Parameters.AddWithValue("@user", username);
                    updateCmd.Parameters.AddWithValue("@maxAttempts", MAX_ATTEMPTS);
                    updateCmd.ExecuteNonQuery();

                    // Отримання оновлених даних
                    var checkCmd = new SqlCommand("SELECT FailedAttempts, IsBlocked FROM LOGIN_TBL WHERE USERNAME = @user", con);
                    checkCmd.Parameters.AddWithValue("@user", username);
                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            failedAttempts = reader.GetInt32(0);
                            isBlocked = reader.GetBoolean(1);

                            if (isBlocked)
                            {
                                MessageBox.Show("This account has been blocked due to too many failed attempts",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                MessageBox.Show($"Login failed. Invalid username or password. Remaining attempts: {MAX_ATTEMPTS - failedAttempts}",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
            }
        }

        // Генерація солі для пароля
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Шифрування пароля з використанням солі
        private string EncryptPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                byte[] hash = pbkdf2.GetBytes(32); // 256-бітний вихід
                return Convert.ToBase64String(hash);
            }
        }

        // Обробник кнопки реєстрації
        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("Password does not meet complexity requirements", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string salt = GenerateSalt();
                string encryptedPassword = EncryptPassword(textBox2.Text, salt);
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "INSERT INTO LOGIN_TBL (USERNAME, PASS, PASSWORD_COMPLEXITY, SECURITY_LEVEL, ACCESS_CONTROL_TYPE, PASSWORD_CREATION_DATE, PASSWORD_EXPIRY_DAYS, IsBlocked, FailedAttempts, Salt) " +
                    "VALUES (@user, @pass, @complexity, 'Не класифіковано', @accessControlType, GETDATE(), @expiryDays, 0, 0, @salt)", con))
                {
                    cmd.Parameters.AddWithValue("@user", textBox1.Text);
                    cmd.Parameters.AddWithValue("@pass", encryptedPassword);
                    cmd.Parameters.AddWithValue("@complexity", GetPasswordComplexity(textBox2.Text));
                    cmd.Parameters.AddWithValue("@accessControlType", comboBoxAccessControl.SelectedItem?.ToString() ?? "Не визначено");
                    cmd.Parameters.AddWithValue("@expiryDays", numExpiryDays.Value);
                    cmd.Parameters.AddWithValue("@salt", salt);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("User successfully registered", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник кнопки зміни пароля
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Please fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string username = textBox1.Text;
            string newPassword = textBox2.Text;

            try
            {
                using (SqlConnection con = new SqlConnection(cs))
                {
                    con.Open();

                    // 1. Отримання поточної інформації про користувача
                    string currentPassword, currentSalt;
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT PASS, Salt FROM LOGIN_TBL WHERE USERNAME = @username", con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                            currentPassword = reader["PASS"].ToString();
                            currentSalt = reader["Salt"].ToString();
                        }
                    }

                    // 2. Перевірка, чи новий пароль відрізняється від поточного
                    string encryptedNewPassword = EncryptPassword(newPassword, currentSalt);
                    if (encryptedNewPassword == currentPassword)
                    {
                        MessageBox.Show("New password cannot be the same as current password",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 3. Перевірка проти останніх 3 паролів в історії
                    using (SqlCommand cmd = new SqlCommand(
                        @"SELECT TOP 3 OLD_PASSWORD, Salt FROM PASSWORD_HISTORY 
                          WHERE USERNAME = @username 
                          ORDER BY CHANGE_DATE DESC", con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string oldPassword = reader["OLD_PASSWORD"].ToString();
                                string oldSalt = reader["Salt"].ToString();

                                // Повторне шифрування нового пароля для порівняння
                                string testPassword = EncryptPassword(newPassword, oldSalt);
                                if (testPassword == oldPassword)
                                {
                                    MessageBox.Show("You cannot use one of the last 3 passwords",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    return;
                                }
                            }
                        }
                    }

                    // 4. Генерація нової солі для нового пароля
                    string newSalt = GenerateSalt();
                    string newEncryptedPassword = EncryptPassword(newPassword, newSalt);

                    // 5. Збереження поточного пароля в історію перед зміною
                    using (SqlCommand cmd = new SqlCommand(
                        "INSERT INTO PASSWORD_HISTORY (USERNAME, OLD_PASSWORD, Salt) " +
                        "VALUES (@username, @oldPassword, @salt)", con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@oldPassword", currentPassword);
                        cmd.Parameters.AddWithValue("@salt", currentSalt);
                        cmd.ExecuteNonQuery();
                    }

                    // 6. Оновлення пароля в основній таблиці
                    using (SqlCommand cmd = new SqlCommand(
                        "UPDATE LOGIN_TBL SET PASS = @newPass, Salt = @newSalt, " +
                        "PASSWORD_COMPLEXITY = @complexity, PASSWORD_CREATION_DATE = GETDATE() " +
                        "WHERE USERNAME = @username", con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@newPass", newEncryptedPassword);
                        cmd.Parameters.AddWithValue("@newSalt", newSalt);
                        cmd.Parameters.AddWithValue("@complexity", GetPasswordComplexity(newPassword));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Password updated successfully",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Password update failed",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Обробник завантаження форми
        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    

        #region Brute Force Implementation

private volatile bool isBruteForceRunning = false;
        private DateTime bruteForceStartTime;
        private string currentUsername;
        private string foundPassword;
        private long bruteForceAttempts;
        private long totalIterations;
        private long currentIteration;
        private CancellationTokenSource cts;
        private readonly object progressLock = new object();
        private System.Threading.Timer progressTimer;

        private void btnStartBrute_Click_1(object sender, EventArgs e)
        {
            if (isBruteForceRunning) return;

            try
            {
                InitializeBruteForceOperation();
                StartBruteForceAttack();
            }
            catch (Exception ex)
            {
                HandleInitializationError(ex);
            }
        }

        private void InitializeBruteForceOperation()
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtBruteUser?.Text))
            {
                throw new ArgumentException("Username cannot be empty");
            }

            // Clear previous state
            txtBruteLog.Clear();
            foundPassword = null;
            bruteForceAttempts = 0;
            currentIteration = 0;

            // Setup character sets
            currentUsername = txtBruteUser.Text;
            var charSets = BuildCharacterSets();
            if (charSets.Count == 0)
            {
                throw new ArgumentException("At least one character set must be selected");
            }

            // Calculate parameters
            int targetLength = chkNoInfo.Checked ? 0 : (int)numLength.Value;
            bool exactLength = chkExactLength.Checked && !chkNoInfo.Checked;
            string chars = string.Join("", charSets);
            totalIterations = exactLength ? (long)Math.Pow(chars.Length, targetLength) : 0;

            // Initialize cancellation
            cts?.Dispose();
            cts = new CancellationTokenSource();
            bruteForceStartTime = DateTime.Now;

            // Setup UI
            SetupBruteForceUI(exactLength, targetLength);
        }

        private List<string> BuildCharacterSets()
        {
            var charSets = new List<string>();

            if (chkNoInfo.Checked || (!chkLowerLatin.Checked && !chkUpperLatin.Checked &&
                !chkDigits.Checked && !chkSpecial.Checked && !chkCyrillic.Checked))
            {
                // Default character sets
                charSets.Add("abcdefghijklmnopqrstuvwxyz");
                charSets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                charSets.Add("0123456789");
                charSets.Add("!@#$%^&*()_+-=[]{}|;:,.<>?");
                charSets.Add("абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ".Replace(" ", ""));
            }
            else
            {
                // Selected character sets
                if (chkLowerLatin.Checked) charSets.Add("abcdefghijklmnopqrstuvwxyz");
                if (chkUpperLatin.Checked) charSets.Add("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
                if (chkDigits.Checked) charSets.Add("0123456789");
                if (chkSpecial.Checked) charSets.Add("!@#$%^&*()_+-=[]{}|;:,.<>?");
                if (chkCyrillic.Checked)
                    charSets.Add("абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ".Replace(" ", ""));
            }

            return charSets;
        }

        private void SetupBruteForceUI(bool exactLength, int targetLength)
        {
            progressBarBrute.Style = exactLength && targetLength <= 8 ?
                ProgressBarStyle.Continuous : ProgressBarStyle.Marquee;
            progressBarBrute.Value = 0;
            lblProgress.Text = "";
            lblBruteStatus.Text = "Status: In progress...";
            btnStartBrute.Enabled = false;
            btnStopBrute.Enabled = true;

            progressTimer = new System.Threading.Timer(UpdateProgressUICallback, null, 1000, 1000);
            isBruteForceRunning = true;
        }

        private void UpdateProgressUICallback(object state) => UpdateProgressUI();

        private void StartBruteForceAttack()
        {
            int targetLength = chkNoInfo.Checked ? 0 : (int)numLength.Value;
            bool exactLength = chkExactLength.Checked && !chkNoInfo.Checked;
            var charSets = BuildCharacterSets();
            string chars = string.Join("", charSets);

            Task.Run(() =>
            {
                try
                {
                    if (exactLength && !chkNoInfo.Checked)
                    {
                        ExecuteParallelBruteForce(currentUsername, chars, targetLength, cts.Token);
                    }
                    else
                    {
                        ExecuteVariableLengthBruteForce(currentUsername, chars, targetLength, cts.Token);
                    }
                }
                catch (OperationCanceledException)
                {
                    UpdateUI(() => lblBruteStatus.Text = "Status: Canceled by user");
                }
                catch (Exception ex)
                {
                    LogError($"Brute force error: {ex.Message}");
                }
                finally
                {
                    CompleteBruteForceOperation();
                }
            }, cts.Token);
        }

        private void ExecuteParallelBruteForce(string username, string chars, int length, CancellationToken ct)
        {
            long totalCombinations = (long)Math.Pow(chars.Length, length);
            var partitions = PartitionRange(totalCombinations, Environment.ProcessorCount);

            Parallel.ForEach(partitions, new ParallelOptions { CancellationToken = ct }, range =>
            {
                using (var connection = new SqlConnection(cs))
                {
                    connection.Open();
                    BruteForceRange(username, chars, length, range, connection, ct);
                }
            });
        }

        private void BruteForceRange(string username, string chars, int length, Range range,
                                   SqlConnection connection, CancellationToken ct)
        {
            var indices = new int[length];
            var password = new char[length];
            long localAttempts = 0;

            for (long i = range.Start; i < range.End && !ct.IsCancellationRequested && foundPassword == null; i++)
            {
                // Convert number to character indices
                long n = i;
                for (int j = length - 1; j >= 0; j--)
                {
                    indices[j] = (int)(n % chars.Length);
                    n /= chars.Length;
                }

                // Build password from indices
                for (int k = 0; k < length; k++)
                {
                    password[k] = chars[indices[k]];
                }

                // Update attempt counter
                localAttempts++;
                if (localAttempts % 1000000 == 0)
                {
                    Interlocked.Add(ref bruteForceAttempts, localAttempts);
                    localAttempts = 0;
                }

                // Check password
                if (CheckPassword(username, new string(password), connection))
                {
                    foundPassword = new string(password);
                    cts.Cancel();
                    break;
                }
            }

            // Final counter update
            Interlocked.Add(ref bruteForceAttempts, localAttempts);
        }

        private void ExecuteVariableLengthBruteForce(string username, string chars, int approximateLength, CancellationToken ct)
        {
            int[] lengths = approximateLength == 0 ?
                Enumerable.Range(1, 5).ToArray() : // Default lengths if unknown
                new[] { approximateLength - 1, approximateLength, approximateLength + 1 };

            foreach (int len in lengths)
            {
                if (ct.IsCancellationRequested || foundPassword != null) break;
                ExecuteParallelBruteForce(username, chars, len, ct);
            }
        }

        private bool CheckPassword(string username, string password, SqlConnection connection)
        {
            try
            {
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM LOGIN_TBL WHERE USERNAME = @user AND PASS = @pass", connection))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@pass", password);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private IEnumerable<Range> PartitionRange(long total, int partitions)
        {
            long chunkSize = total / partitions;
            for (int i = 0; i < partitions; i++)
            {
                long start = i * chunkSize;
                long end = (i == partitions - 1) ? total : start + chunkSize;
                yield return new Range(start, end);
            }
        }

        private void UpdateProgressUI()
        {
            if (!isBruteForceRunning) return;

            Invoke((MethodInvoker)delegate
            {
                if (foundPassword != null)
                {
                    txtBruteLog.AppendText($"{DateTime.Now:HH:mm:ss}: Password found: {foundPassword}\n");
                    txtBruteLog.ScrollToCaret();
                }
                else if (bruteForceAttempts > 0)
                {
                    double seconds = (DateTime.Now - bruteForceStartTime).TotalSeconds;
                    double speed = bruteForceAttempts / seconds;
                    lblProgress.Text = $"Attempts: {bruteForceAttempts:N0} | Speed: {speed:N0}/sec";

                    if (totalIterations > 0)
                    {
                        int progress = (int)((currentIteration * 100) / totalIterations);
                        progressBarBrute.Value = Math.Min(progress, 100);
                    }
                }
            });
        }

        private void CompleteBruteForceOperation()
        {
            try
            {
                progressTimer?.Dispose();

                UpdateUI(() =>
                {
                    isBruteForceRunning = false;
                    btnStartBrute.Enabled = true;
                    btnStopBrute.Enabled = false;
                    progressBarBrute.Value = progressBarBrute.Maximum;

                    if (foundPassword != null)
                    {
                        ShowSuccessMessage();
                    }
                    else if (cts?.IsCancellationRequested == true)
                    {
                        lblBruteStatus.Text = "Status: Canceled by user";
                        lblProgress.Text = "Canceled";
                    }
                    else
                    {
                        ShowFailureMessage();
                    }
                });
            }
            finally
            {
                cts?.Dispose();
                cts = null;
            }
        }

        private void ShowSuccessMessage()
        {
            lblBruteStatus.Text = $"Status: Found! Password: {foundPassword}";
            lblProgress.Text = "100%";
            MessageBox.Show($"Password found: {foundPassword}\nTime: {(DateTime.Now - bruteForceStartTime).TotalSeconds:F2} sec\nAttempts: {bruteForceAttempts:N0}",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowFailureMessage()
        {
            lblBruteStatus.Text = "Status: Not Found";
            lblProgress.Text = "Completed";
            MessageBox.Show("Password not found with the specified parameters",
                "Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnStopBrute_Click_1(object sender, EventArgs e)
        {
            if (isBruteForceRunning && cts != null)
            {
                cts.Cancel();
                UpdateUI(() =>
                {
                    lblBruteStatus.Text = "Status: Stopping...";
                    lblProgress.Text = "Please wait...";
                });
            }
        }

        private void chkNoInfo_CheckedChanged_1(object sender, EventArgs e)
        {
            bool noInfoMode = chkNoInfo.Checked;
            numLength.Enabled = !noInfoMode;
            chkExactLength.Enabled = !noInfoMode;
            chkLowerLatin.Enabled = !noInfoMode;
            chkUpperLatin.Enabled = !noInfoMode;
            chkDigits.Enabled = !noInfoMode;
            chkSpecial.Enabled = !noInfoMode;
            chkCyrillic.Enabled = !noInfoMode;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isBruteForceRunning)
            {
                var result = MessageBox.Show("Brute force attack is still running. Cancel and exit?",
                                           "Confirmation",
                                           MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                cts?.Cancel();
                Task.Delay(300).Wait();
            }

            CleanupResources();
            base.OnFormClosing(e);
        }

        private void CleanupResources()
        {
            progressTimer?.Dispose();
            cts?.Dispose();
        }

        private void HandleInitializationError(Exception ex)
        {
            MessageBox.Show($"Initialization error: {ex.Message}", "Error",
                           MessageBoxButtons.OK, MessageBoxIcon.Error);
            CompleteBruteForceOperation();
        }

        private void LogError(string message)
        {
            UpdateUI(() =>
            {
                txtBruteLog.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
                txtBruteLog.ScrollToCaret();
            });
        }

        private void UpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate { action(); });
            }
            else
            {
                action();
            }
        }

        private struct Range
        {
            public long Start { get; }
            public long End { get; }

            public Range(long start, long end)
            {
                Start = start;
                End = end;
            }
        }

        #endregion



        #region Dictionary Attack

        private bool isDictionaryAttackRunning = false;
        private string dictionaryPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\rockyou.txt";
        private string englishDictionaryPath = @"C:\Users\Ярина\Desktop\NAU2025\TBD_Yaroshenko\TBD_Yaroshenko\english_words.txt";
        private List<string> dictionary;
        private HashSet<string> englishWords = new HashSet<string>();

        // Клас для зберігання інформації про схожі паролі
        public class SimilarPassword
        {
            public string Password { get; set; }
            public string SimilarityType { get; set; }
        }

        // Фільтрує словник за заданими критеріями
        private List<string> FilterDictionary(List<string> dictionary)
        {
            // Фільтруємо паролі довжиною від 3 символів
            return dictionary.Where(p => p.Length >= 3).ToList();
        }

        // Асинхронно завантажує англійський словник
        private async Task LoadEnglishDictionaryAsync()
        {
            try
            {
                if (!File.Exists(englishDictionaryPath))
                {
                    // Створюємо директорію, якщо її немає
                    string directory = Path.GetDirectoryName(englishDictionaryPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    // Створюємо базовий словник за замовчуванням
                    string[] defaultWords = { "hello", "world", "password", "admin", "test", "qwerty", "123456", "welcome" };
                    await File.WriteAllLinesAsync(englishDictionaryPath, defaultWords);
                    englishWords = new HashSet<string>(defaultWords.Select(w => w.ToLower()));
                    return;
                }

                // Читаємо та обробляємо словник
                var lines = await File.ReadAllLinesAsync(englishDictionaryPath);
                var baseWords = lines.Where(word => !string.IsNullOrWhiteSpace(word) && word.Length >= 3)
                                    .Select(word => word.Trim().ToLower())
                                    .Distinct()
                                    .ToList();

                // Генеруємо варіації слів
                var variations = new List<string>();
                foreach (var word in baseWords)
                {
                    variations.AddRange(GenerateCommonVariations(word));
                }

                englishWords = new HashSet<string>(baseWords.Concat(variations));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading English dictionary: {ex.Message}");
                englishWords = new HashSet<string>();
            }
        }

        // Генерує поширені варіації заданого слова
        private List<string> GenerateCommonVariations(string word)
        {
            var variations = new List<string>();

            // Додаємо базові варіації
            variations.Add(word + "123");
            variations.Add(word + "!");
            variations.Add(word + "123!");
            variations.Add(word + "1");
            variations.Add(word + "2023");
            variations.Add(word + "@");

            // Варіація з великою літерою
            if (word.Length > 0)
            {
                variations.Add(char.ToUpper(word[0]) + word.Substring(1));
            }

            // Leet speak варіація
            variations.Add(word.Replace('a', '@').Replace('e', '3').Replace('i', '1').Replace('o', '0'));

            // Подвоєння слова
            variations.Add(word + word);

            return variations;
        }

        // Визначає тип схожості на основі оцінки
        private string CalculateSimilarityType(double score)
        {
            return score switch
            {
                1.0 => "Exact match",
                > 0.8 => "Very similar",
                > 0.6 => "Similar",
                > 0.4 => "Somewhat similar",
                _ => "Weak similarity"
            };
        }

        // Обчислює схожість між двома паролями
        private double CalculateSimilarity(string realPassword, string testPassword)
        {
            if (string.IsNullOrEmpty(realPassword) || string.IsNullOrEmpty(testPassword))
                return 0;

            // Перевіряємо точний збіг
            if (realPassword == testPassword) return 1.0;

            // Перевіряємо збіг без урахування регістру
            if (string.Equals(realPassword, testPassword, StringComparison.OrdinalIgnoreCase))
                return 0.9;

            // Обчислюємо схожість підрядків
            double substringScore = CalculateSubstringSimilarity(realPassword, testPassword);

            // Обчислюємо відстань Левенштейна
            int maxLength = Math.Max(realPassword.Length, testPassword.Length);
            int distance = LevenshteinDistance(realPassword, testPassword);
            double levenshteinSimilarity = 1.0 - (double)distance / maxLength;

            // Комбінуємо оцінки
            double totalSimilarity = (substringScore * 0.5) + (levenshteinSimilarity * 0.5);
            return Math.Round(totalSimilarity, 2);
        }

        // Обчислює схожість на основі спільних підрядків
        private double CalculateSubstringSimilarity(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return 0;

            string longer = s1.Length > s2.Length ? s1 : s2;
            string shorter = s1.Length > s2.Length ? s2 : s1;

            int maxLength = 0;
            for (int i = 0; i < shorter.Length; i++)
            {
                for (int j = i + 1; j <= shorter.Length; j++)
                {
                    string substring = shorter.Substring(i, j - i);
                    if (longer.Contains(substring) && substring.Length > maxLength)
                    {
                        maxLength = substring.Length;
                    }
                }
            }

            return (double)maxLength / longer.Length;
        }

        // Обчислює відстань Левенштейна між двома рядками
        private int LevenshteinDistance(string s, string t)
        {
            if (string.IsNullOrEmpty(s)) return string.IsNullOrEmpty(t) ? 0 : t.Length;
            if (string.IsNullOrEmpty(t)) return s.Length;

            int[,] d = new int[s.Length + 1, t.Length + 1];

            for (int i = 0; i <= s.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= t.Length; j++)
                d[0, j] = j;

            for (int j = 1; j <= t.Length; j++)
            {
                for (int i = 1; i <= s.Length; i++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }

            return d[s.Length, t.Length];
        }

        // Обробник кнопки запуску словникової атаки
        private async void btnStartDictionary_Click_1(object sender, EventArgs e)
        {
            if (isDictionaryAttackRunning) return;
            txtBruteLog.Clear();

            if (string.IsNullOrWhiteSpace(txtBruteUser.Text))
            {
                MessageBox.Show("Please enter a username", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            currentUsername = txtBruteUser.Text;
            string realPassword = GetUserPasswordFromDB(currentUsername);

            if (realPassword == null)
            {
                MessageBox.Show("User not found in database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(dictionaryPath))
            {
                MessageBox.Show("Dictionary file not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ініціалізація англійського словника, якщо ще не завантажено
            if (englishWords.Count == 0)
            {
                await LoadEnglishDictionaryAsync();
            }

            isDictionaryAttackRunning = true;
            foundPassword = null;
            bruteForceStartTime = DateTime.Now;
            bruteForceAttempts = 0;

            this.btnStartDictionary.Enabled = false;
            this.btnStopDictionary.Enabled = true;
            lblBruteStatus.Text = "Status: Dictionary attack in progress...";

            await Task.Run(() =>
            {
                try
                {
                    dictionary = File.ReadAllLines(dictionaryPath)
                                    .Where(line => !string.IsNullOrWhiteSpace(line))
                                    .ToList();

                    var filteredDictionary = FilterDictionary(dictionary);

                    UpdateBruteLog($"Loaded dictionary: {dictionary.Count} entries");
                    UpdateBruteLog($"After filtering: {filteredDictionary.Count} candidates");
                    UpdateBruteLog($"Analyzing password: {realPassword}");

                    var similarPasswords = new ConcurrentBag<SimilarPassword>();
                    bool exactMatchFound = false;

                    // Паралельна обробка словника
                    Parallel.ForEach(filteredDictionary, (password, state) =>
                    {
                        if (!isDictionaryAttackRunning || exactMatchFound)
                        {
                            state.Stop();
                            return;
                        }

                        Interlocked.Increment(ref bruteForceAttempts);

                        if (CheckPassword(currentUsername, password))
                        {
                            foundPassword = password;
                            exactMatchFound = true;
                            UpdateBruteLog($"Found exact match: {password}");
                            state.Stop();
                            return;
                        }

                        var similarityScore = CalculateSimilarity(realPassword, password);
                        if (similarityScore > 0.3)
                        {
                            similarPasswords.Add(new SimilarPassword
                            {
                                Password = password,
                                SimilarityType = CalculateSimilarityType(similarityScore)
                            });
                        }

                        if (bruteForceAttempts % 10000 == 0)
                        {
                            UpdateBruteLog($"Checked {bruteForceAttempts} passwords...");
                        }
                    });

                    this.Invoke((MethodInvoker)delegate
                    {
                        isDictionaryAttackRunning = false;
                        btnStartDictionary.Enabled = true;
                        btnStopDictionary.Enabled = false;

                        if (exactMatchFound)
                        {
                            lblBruteStatus.Text = $"Status: Found! Password: {foundPassword}";
                            MessageBox.Show($"Password found: {foundPassword}\nTime: {(DateTime.Now - bruteForceStartTime).TotalSeconds:F2} sec\nAttempts: {bruteForceAttempts}",
                                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            ShowSimilarPasswordsReport(realPassword, similarPasswords.ToList());
                        }
                    });
                }
                catch (Exception ex)
                {
                    this.Invoke((MethodInvoker)delegate
                    {
                        isDictionaryAttackRunning = false;
                        btnStartDictionary.Enabled = true;
                        btnStopDictionary.Enabled = false;
                        MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
            });
        }

        // Показує звіт про схожі паролі
        private void ShowSimilarPasswordsReport(string realPassword, List<SimilarPassword> similarPasswords)
        {
            lblBruteStatus.Text = "Status: Exact match not found";

            if (!similarPasswords.Any())
            {
                UpdateBruteLog("No similar passwords found");
                MessageBox.Show("No passwords similar to the original were found", "Result",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Групуємо та сортуємо результати
            var topSimilar = similarPasswords
                .GroupBy(p => p.Password)
                .Select(g => g.First())
                .OrderByDescending(p => p.SimilarityType)
                .Take(10)
                .ToList();

            // Формуємо звіт
            var report = new StringBuilder();
            report.AppendLine("Password Similarity Analysis");
            report.AppendLine($"User: {currentUsername}");
            report.AppendLine($"Original password: {realPassword}");
            report.AppendLine($"\nTop 10 similar passwords:");

            foreach (var item in topSimilar)
            {
                report.AppendLine($"- {item.Password} ({item.SimilarityType})");
            }

            report.AppendLine($"\nAnalysis time: {(DateTime.Now - bruteForceStartTime).TotalSeconds:F2} sec");
            report.AppendLine($"Passwords checked: {bruteForceAttempts:N0}");

            UpdateBruteLog(report.ToString());

            // Показуємо результат
            MessageBox.Show(report.ToString(), "Similarity Analysis Results",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // Оновлює лог атаки
        private void UpdateBruteLog(string message)
        {
            if (txtBruteLog.InvokeRequired)
            {
                txtBruteLog.Invoke((MethodInvoker)delegate
                {
                    txtBruteLog.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
                    txtBruteLog.ScrollToCaret();
                });
            }
            else
            {
                txtBruteLog.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
                txtBruteLog.ScrollToCaret();
            }
        }

        // Отримує пароль користувача з бази даних
        private string GetUserPasswordFromDB(string username)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT PASS FROM LOGIN_TBL WHERE USERNAME = @user", con))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        var result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving password: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        // Обробник кнопки зупинки атаки
        private void btnStopDictionary_Click(object sender, EventArgs e)
        {
            isDictionaryAttackRunning = false;
            btnStopDictionary.Enabled = false;
            lblBruteStatus.Text = "Status: Dictionary attack stopped";
        }

        // Перевіряє пароль у базі даних
        private bool CheckPassword(string username, string password)
        {
            try
            {
                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM LOGIN_TBL WHERE USERNAME = @user AND PASS = @pass", con))
                    {
                        cmd.Parameters.AddWithValue("@user", username);
                        cmd.Parameters.AddWithValue("@pass", password);
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

    }
}