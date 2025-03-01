using System;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Diagnostics;

namespace TBD_Yaroshenko
{
    public partial class Form1 : Form
    {   //рядок підключення до бази даних, який береться з конфігураційного файлу.
        string cs = ConfigurationManager.ConnectionStrings["dbtbdyaroshenko"].ConnectionString;

        //регулярний вираз для перевірки складності пароля
        string complexPattern = "(?=^.{8,}$)((?=.*\\d)|(?=.*\\W+))(?![.\\n])(?=.*[A-Z])(?=.*[a-z]).*$";

        public Form1()
        {
            InitializeComponent();
        }

        //Виводить інформацію про виконавця
        private void довідкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Performed by: Yaroshenko Iryna, group B-125-21-3-B");
        }

        //Перевіряє, чи поле для логіну не порожнє. Якщо порожнє, видає помилку
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                textBox1.Focus();
                errorProvider1.SetError(textBox1, "Enter your login");
            }
            else
            {
                errorProvider1.Clear();
            }
        }
        // Метод для визначення складності пароля
        private string GetPasswordComplexity(string password)
        {
            if (password.Length >= 8 && Regex.IsMatch(password, @"[A-Z]") && Regex.IsMatch(password, @"[a-z]") && Regex.IsMatch(password, @"\d") && Regex.IsMatch(password, @"\W"))
            {
                return "High";
            }
            else
            {
                return "Low";
            }
        }


        private void InsertPasswordHistory(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string complexity = GetPasswordComplexity(password);

                string insertQuery = "INSERT INTO PASSWORD_HISTORY (username, pass, password_complexity, CHANGE_DATE) VALUES (@username, @pass, @complexity, @date)";
                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.Parameters.AddWithValue("@complexity", complexity);
                cmd.Parameters.AddWithValue("@date", DateTime.Now);

                cmd.ExecuteNonQuery();
            }
        }

        // Вставка складності пароля при додаванні або оновленні пароля
        private void SavePasswordComplexity(string username, string password)
        {
            string complexity = GetPasswordComplexity(password);

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string updateComplexityQuery = "UPDATE login_tbl SET password_complexity = @complexity WHERE username = @username";
                SqlCommand cmd = new SqlCommand(updateComplexityQuery, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@complexity", complexity);
                cmd.ExecuteNonQuery();
            }
        }

        // Зберігання нового паролю в історії
        private void SavePasswordHistory(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string insertHistoryQuery = "INSERT INTO PASSWORD_HISTORY (username, pass) VALUES (@username, @pass)";
                SqlCommand cmd = new SqlCommand(insertHistoryQuery, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@pass", password);
                cmd.ExecuteNonQuery();
            }
        }

        // Перевірка чи пароль не співпадає з останніми трьома паролями
        private bool IsPasswordInHistory(string username, string password)
        {
            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string checkHistoryQuery = "SELECT COUNT(*) FROM (SELECT TOP 3 * FROM PASSWORD_HISTORY WHERE username = @username ORDER BY CHANGE_DATE DESC) AS LastPasswords WHERE pass = @pass";
                SqlCommand cmd = new SqlCommand(checkHistoryQuery, con);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@pass", password);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }



        //Перевіряє, чи пароль відповідає вимогам складності
        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                textBox2.Focus();
                errorProvider2.SetError(textBox2, "Create a more complex password");
            }
            else
            {
                errorProvider2.Clear();
            }
        }



        //Додає нового користувача в базу даних
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

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                // Перевірка унікальності логіна
                string checkUser = "SELECT COUNT(*) FROM login_tbl WHERE username = @user";
                SqlCommand checkCmd = new SqlCommand(checkUser, con);
                checkCmd.Parameters.AddWithValue("@user", textBox1.Text);
                int userExists = (int)checkCmd.ExecuteScalar();

                if (userExists > 0)
                {
                    MessageBox.Show("A user with this login already exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Додавання нового користувача
                string insertQuery = "INSERT INTO login_tbl (username, pass) VALUES (@user, @pass)";
                SqlCommand cmd = new SqlCommand(insertQuery, con);
                cmd.Parameters.AddWithValue("@user", textBox1.Text);
                cmd.Parameters.AddWithValue("@pass", textBox2.Text);

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    // Визначаємо складність пароля
                    string complexity = GetPasswordComplexity(textBox2.Text);
                    string updateComplexityQuery = "UPDATE login_tbl SET password_complexity = @complexity WHERE username = @user";
                    SqlCommand updateCmd = new SqlCommand(updateComplexityQuery, con);
                    updateCmd.Parameters.AddWithValue("@user", textBox1.Text);
                    updateCmd.Parameters.AddWithValue("@complexity", complexity);
                    updateCmd.ExecuteNonQuery();

                    MessageBox.Show("User added successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to add user", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //Оновлює пароль користувача
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The new password does not meet the complexity requirements", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Перевірка, чи не співпадає з останніми трьома паролями
            if (IsPasswordInHistory(textBox1.Text, textBox2.Text))
            {
                MessageBox.Show("It is not possible to use one of the last three passwords", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection con = new SqlConnection(cs))
            {
                con.Open();
                string updateQuery = "UPDATE login_tbl SET pass = @newPass WHERE username = @user";
                SqlCommand cmd = new SqlCommand(updateQuery, con);
                cmd.Parameters.AddWithValue("@user", textBox1.Text);
                cmd.Parameters.AddWithValue("@newPass", textBox2.Text);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    SavePasswordHistory(textBox1.Text, textBox2.Text); // Зберігаємо новий пароль в історії
                    SavePasswordComplexity(textBox1.Text, textBox2.Text); // Зберігаємо складність пароля
                    MessageBox.Show("Password successfully updated", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //Відображає або приховує пароль
        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        //Перевіряє, чи поточний пароль відповідає вимогам складності
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

            if (checkBox2.Checked && !Regex.IsMatch(textBox2.Text, complexPattern))
            {
                MessageBox.Show("The current password does not meet the new complexity requirements", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            // Перевірка, чи заповнені поля для логіну та паролю
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Fill in all fields for authorization", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Перевірка правильності підключення до бази даних
            using (SqlConnection con = new SqlConnection(cs))
            {
                try
                {
                    con.Open(); // Відкриваємо з'єднання з базою

                    // Створення SQL запиту для перевірки логіну та паролю
                    string query = "SELECT * FROM login_tbl WHERE username = @user AND pass = @pass";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@user", textBox1.Text); // Параметр для логіну
                    cmd.Parameters.AddWithValue("@pass", textBox2.Text); // Параметр для паролю

                    SqlDataReader dr = cmd.ExecuteReader(); // Виконуємо запит

                    // Перевірка, чи знайдено користувача з таким логіном та паролем
                    if (dr.HasRows)
                    {
                        dr.Read(); // Читаємо результат
                        string username = dr["username"].ToString(); // Отримуємо логін з результату
                        MessageBox.Show($"Login {username}, successfully authorized!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        {
                            Form2 form2 = new Form2();
                            form2.Show();
                            this.Hide(); // Ховаємо поточну форму
                        }
                    }
                    else
                    {
                        MessageBox.Show("Authorization error. Incorrect login or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Обробка помилки при підключенні або виконанні запиту
                    MessageBox.Show("Database connection error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}