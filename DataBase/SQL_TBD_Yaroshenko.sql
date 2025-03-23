USE WINFORM_DB;
GO

-- Перевірка та створення таблиці LOGIN_TBL
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'LOGIN_TBL')
BEGIN
    CREATE TABLE LOGIN_TBL (
        USERNAME VARCHAR(50) PRIMARY KEY,
        PASS VARCHAR(100) NOT NULL,
        PASSWORD_COMPLEXITY VARCHAR(10) NOT NULL,
        SECURITY_LEVEL VARCHAR(20) NOT NULL
    );
    PRINT 'Table LOGIN_TBL created successfully.';
END
ELSE
BEGIN
    PRINT 'Table LOGIN_TBL already exists.';
END
GO

ALTER TABLE LOGIN_TBL
ADD ACCESS_CONTROL_TYPE VARCHAR(20) NOT NULL DEFAULT 'Undefined';

-- Перевірка та створення таблиці FILE_ACCESS
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'FILE_ACCESS')
BEGIN
    CREATE TABLE FILE_ACCESS (
        FILE_NAME VARCHAR(100) PRIMARY KEY,
        CONFIDENTIALITY_LEVEL VARCHAR(20) NOT NULL
    );
    PRINT 'Table FILE_ACCESS created successfully.';
END
ELSE
BEGIN
    PRINT 'Table FILE_ACCESS already exists.';
END
GO

-- Перевірка та створення таблиці PASSWORD_HISTORY
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PASSWORD_HISTORY')
BEGIN
    CREATE TABLE PASSWORD_HISTORY (
        ID INT IDENTITY(1,1) PRIMARY KEY,
        USERNAME VARCHAR(50) NOT NULL,
        OLD_PASSWORD VARCHAR(100) NOT NULL,
        CHANGE_DATE DATETIME DEFAULT GETDATE()
    );
    PRINT 'Table PASSWORD_HISTORY created successfully.';
END
ELSE
BEGIN
    PRINT 'Table PASSWORD_HISTORY already exists.';
END
GO

-- Перевірка та створення таблиці USER_FILE_ACCESS
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'USER_FILE_ACCESS')
BEGIN
    DROP TABLE USER_FILE_ACCESS;
    PRINT 'Table USER_FILE_ACCESS dropped.';
END

CREATE TABLE USER_FILE_ACCESS (
    USERNAME VARCHAR(50) NOT NULL,
    FILE_NAME VARCHAR(100) NOT NULL,
    CAN_READ BIT DEFAULT 0,
    CAN_WRITE BIT DEFAULT 0,
    CAN_EXECUTE BIT DEFAULT 0,
    OWN BIT DEFAULT 0,
    DURATION_SECONDS INT,
    PRIMARY KEY (USERNAME, FILE_NAME),
    FOREIGN KEY (USERNAME) REFERENCES LOGIN_TBL(USERNAME),
    FOREIGN KEY (FILE_NAME) REFERENCES FILE_ACCESS(FILE_NAME)
);
PRINT 'Table USER_FILE_ACCESS created successfully.';
GO

-- Вставка даних у LOGIN_TBL
INSERT INTO LOGIN_TBL (USERNAME, PASS, PASSWORD_COMPLEXITY, SECURITY_LEVEL, ACCESS_CONTROL_TYPE)
VALUES 
    ('Admin', 'Admin12345!', 'High', 'Administrative', 'Mandatory'),
    ('Yar1', 'Password1!', 'High', 'Top Secret', 'Discretionary'),
    ('Yar2', 'Password2@', 'High', 'Secret', 'Role-Based'),
    ('Yar3', 'Password3#', 'High', 'Confidential', 'Mandatory'),
    ('Yar4', 'Pass4', 'Low', 'FOUO', 'Discretionary'),
    ('Yar5', 'Pass5', 'Low', 'Unclassified', 'Role-Based');
GO

-- Вставка даних у FILE_ACCESS
MERGE INTO FILE_ACCESS AS target
USING (VALUES
    ('app.exe', 'Top Secret'),
    ('config.txt', 'Confidential'),
    ('data.txt', 'Secret'),
    ('readme.txt', 'FOUO'),
    ('logo.png', 'Unclassified')
) AS source (FILE_NAME, CONFIDENTIALITY_LEVEL)
ON target.FILE_NAME = source.FILE_NAME
WHEN MATCHED THEN
    UPDATE SET target.CONFIDENTIALITY_LEVEL = source.CONFIDENTIALITY_LEVEL
WHEN NOT MATCHED THEN
    INSERT (FILE_NAME, CONFIDENTIALITY_LEVEL)
    VALUES (source.FILE_NAME, source.CONFIDENTIALITY_LEVEL);
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'USER_FILE_ACCESS' AND COLUMN_NAME = 'CAN_EXECUTE')
BEGIN
    ALTER TABLE USER_FILE_ACCESS
    ADD CAN_EXECUTE BIT DEFAULT 0;
    PRINT 'Column CAN_EXECUTE added successfully.';
END
ELSE
BEGIN
    PRINT 'Column CAN_EXECUTE already exists.';
END
GO

-- Вставка даних про права доступу
INSERT INTO USER_FILE_ACCESS (USERNAME, FILE_NAME, CAN_READ, CAN_WRITE, OWN, DURATION_SECONDS)
VALUES 
    ('Yar1', 'config.txt', 1, 1, 1, NULL),
    ('Yar1', 'logo.png', 1, 0, 0, 30),
    ('Yar2', 'config.txt', 1, 0, 0, 90),
    ('Yar2', 'data.txt', 1, 1, 1, NULL),
    ('Yar3', 'app.lnk', 1, 0, 1, 90),
    ('Yar3', 'data.txt', 1, 0, 0, 60),
    ('Yar3', 'readme.txt', 1, 1, 1, NULL),
    ('Yar4', 'readme.txt', 1, 0, 0, 30),
    ('Yar4', 'logo.png', 1, 1, 1, NULL),
    ('Yar5', 'app.lnk', 1, 0, 1, NULL),
    ('Admin', 'app.lnk', 1, 1, 1, NULL),
    ('Admin', 'config.txt', 1, 1, 0, NULL),
    ('Admin', 'data.txt', 1, 1, 0, NULL),
    ('Admin', 'readme.txt', 1, 1, 0, NULL),
    ('Admin', 'logo.png', 1, 1, 0, NULL);
GO

-- Оновлення для Yar2 і config.txt
UPDATE USER_FILE_ACCESS
SET CAN_WRITE = 1
WHERE USERNAME = 'Yar2' AND FILE_NAME = 'config.txt';

-- Оновлення для Yar3 і app.exe
UPDATE USER_FILE_ACCESS
SET CAN_EXECUTE = 1, OWN = 0
WHERE USERNAME = 'Yar3' AND FILE_NAME = 'app.lnk';

-- Оновлення для Yar4 і readme.txt
UPDATE USER_FILE_ACCESS
SET CAN_WRITE = 1
WHERE USERNAME = 'Yar4' AND FILE_NAME = 'readme.txt';

-- Оновлення для Yar5 і app.exe
UPDATE USER_FILE_ACCESS
SET CAN_EXECUTE = 1
WHERE USERNAME = 'Yar5' AND FILE_NAME = 'app.lnk';

-- Оновлення для Admin і app.exe
UPDATE USER_FILE_ACCESS
SET CAN_READ = 1, CAN_WRITE = 1, OWN = 0, CAN_EXECUTE = 1
WHERE USERNAME = 'Admin' AND FILE_NAME = 'app.lnk';

UPDATE USER_FILE_ACCESS
SET CAN_READ = 1, CAN_WRITE = 1, OWN = 1, CAN_EXECUTE = 1
WHERE USERNAME = 'Yar5' AND FILE_NAME = 'app.lnk';

-- Виведення даних
SELECT * FROM LOGIN_TBL;
SELECT * FROM USER_FILE_ACCESS;
SELECT * FROM FILE_ACCESS;
SELECT * FROM PASSWORD_HISTORY;
GO

--DELETE  FROM LOGIN_TBL;
--DELETE  FROM FILE_ACCESS;
--DELETE  FROM USER_FILE_ACCESS;
--DELETE  FROM PASSWORD_HISTORY;