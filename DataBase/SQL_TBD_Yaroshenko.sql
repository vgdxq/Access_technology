-- Використання бази даних
USE WINFORM_DB;
GO

-- Вставка даних у таблицю LOGIN_TBL
INSERT INTO LOGIN_TBL (USERNAME, PASS, PASSWORD_COMPLEXITY, ROLE)
VALUES 
    ('adm', 'Password1!', 'High', 'Admin'),
    ('dev2', 'Password2@', 'High', 'Developer'),
    ('dev3', 'Password3#','High', 'Developer'),
    ('Us4', 'Pass4','Low', 'User'),
    ('Us5', 'Pass5','Low', 'User');
GO

DROP TABLE IF EXISTS FILE_ACCESS;
CREATE TABLE FILE_ACCESS (
    FILE_NAME VARCHAR(100) PRIMARY KEY,
    CONFIDENTIALITY_LEVEL VARCHAR(20) NOT NULL
);

-- Оновлення даних
SELECT * FROM LOGIN_TBL;
SELECT * FROM FILE_ACCESS;
SELECT * FROM PASSWORD_HISTORY;
GO

-- Очищення таблиці
--DELETE FROM LOGIN_TBL;
--DELETE FROM FILE_ACCESS;
--DELETE FROM PASSWORD_HISTORY;

--GO