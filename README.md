# TBD_Yaroshenko

## Description
This project is an authentication and password security analysis system built in C# using Windows Forms and SQL Server. It provides user login, registration, password management, and tools to test password strength through brute force and dictionary attacks.

### Key Features:
- **User Authentication** with three access control types:
  - 🔒 Mandatory
  - 🔑 Discretionary
  - 👥 Role-Based
- **Password Complexity Check** (min. 8 characters, uppercase/lowercase, digits or special characters)
- **Password Security**:
  - Encryption with salt (PBKDF2)
  - Password history (prevents reuse of the last 3 passwords)
  - Password expiration with warnings
- **Testing Attacks**:
  - ⚡ Brute Force: Parallel password guessing with customizable character sets and lengths
  - 📖 Dictionary Attack: Password analysis using a dictionary with similarity scoring

---

## Features
- **Seamless SQL Server Integration** via a configurable connection string
- **User Interface**:
  - Simple login/password input with paste protection (Ctrl+V disabled)
  - CAPTCHA enforcement after failed login attempts
  - Real-time attack progress display
- **Security**:
  - Limits login attempts (max 3)
  - Throttles rapid input (50ms delay)
- **Attack Flexibility**:
  - Customizable character sets (Latin, Cyrillic, digits, special characters)
  - Uses `rockyou.txt` dictionary and an English word list

---

## Usage
1. **Setup**:
   - Install SQL Server and create a database with `LOGIN_TBL` and `PASSWORD_HISTORY` tables (see `db_scripts.sql`).
   - Update the connection string in `App.config`.
2. **Running**:
   - Compile the project in Visual Studio.
   - Enter a username and password to log in or register.
3. **Password Testing**:
   - Enable Brute Force: Select character sets and length, then click "Start".
   - Enable Dictionary Attack: Specify a username and click "Start".
4. **Exit**: Click "Stop" to halt attacks or close the form.

---

## Requirements
- .NET Framework
- Microsoft SQL Server
- Visual Studio (for development)
- Dictionary files (`rockyou.txt`, `english_words.txt`) at specified paths

---

## Author
Developed by: Yaroshenko Iryna, Group B-125-21-3-B
