using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TBD_Yaroshenko
{
    public partial class MainWind_RoleBased : Form
    {
        private string _username;

        public MainWind_RoleBased(string username)
        {
            InitializeComponent();
            _username = username; // Зберігаємо ім'я користувача
            LoadUserData(); // Завантажуємо дані користувача
        }

        private void LoadUserData()
        {

        }
    }
}
