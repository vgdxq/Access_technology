USE WINFORM_DB;
GO

-- �������� �� ��������� ������� LOGIN_TBL
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

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LOGIN_TBL') AND name = 'ACCESS_CONTROL_TYPE')
BEGIN
    ALTER TABLE LOGIN_TBL
    ADD ACCESS_CONTROL_TYPE VARCHAR(20) NOT NULL DEFAULT 'Undefined';
END
-- �������� �� ��������� ������� FILE_ACCESS
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

-- �������� �� ��������� ������� PASSWORD_HISTORY
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

-- �������� �� ��������� ������� USER_FILE_ACCESS
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

-- ������� ����� � LOGIN_TBL
INSERT INTO LOGIN_TBL (USERNAME, PASS, PASSWORD_COMPLEXITY, SECURITY_LEVEL, ACCESS_CONTROL_TYPE)
VALUES 
    ('Admin', 'Admin12345!', 'High', 'Administrative', 'Mandatory'),
    ('Yar1', 'Password1!', 'High', 'Top Secret', 'Discretionary'),
    ('Yar2', 'Password2@', 'High', 'Secret', 'Role-Based'),
    ('Yar3', 'Password3#', 'High', 'Confidential', 'Mandatory'),
    ('Yar4', 'Pass4', 'Low', 'FOUO', 'Discretionary'),
    ('Yar5', 'Pass5', 'Low', 'Unclassified', 'Role-Based');
GO

-- ������� ����� � FILE_ACCESS
MERGE INTO FILE_ACCESS AS target
USING (VALUES
    ('app.lnk', 'Top Secret'),
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

-- ������� ����� ��� ����� �������
INSERT INTO USER_FILE_ACCESS (USERNAME, FILE_NAME, CAN_READ, CAN_WRITE, OWN, DURATION_SECONDS)
VALUES 
    ('Yar1', 'config.txt', 1, 1, 1, 0),
    ('Yar1', 'logo.png', 1, 0, 0, 10),
    ('Yar2', 'config.txt', 1, 0, 0, 20),
    ('Yar2', 'data.txt', 1, 1, 1, 0),
    ('Yar3', 'app.lnk', 1, 0, 1, 20),
    ('Yar3', 'data.txt', 1, 0, 0, 15),
    ('Yar3', 'readme.txt', 1, 1, 1, 0),
    ('Yar4', 'readme.txt', 1, 0, 0, 10),
    ('Yar4', 'logo.png', 1, 1, 1, 0),
    ('Yar5', 'app.lnk', 1, 0, 1, 0),
    ('Admin', 'app.lnk', 1, 1, 1, 0),
    ('Admin', 'config.txt', 1, 1, 0, 0),
    ('Admin', 'data.txt', 1, 1, 0, 0),
    ('Admin', 'readme.txt', 1, 1, 0, 0),
    ('Admin', 'logo.png', 1, 1, 0, 0);
GO

-- ��������� ��� Yar2 � config.txt
UPDATE USER_FILE_ACCESS
SET CAN_WRITE = 1
WHERE USERNAME = 'Yar2' AND FILE_NAME = 'config.txt';

-- ��������� ��� Yar3 � app.exe
UPDATE USER_FILE_ACCESS
SET CAN_EXECUTE = 1, OWN = 0, CAN_READ = 0
WHERE USERNAME = 'Yar3' AND FILE_NAME = 'app.lnk';

-- ��������� ��� Yar4 � readme.txt
UPDATE USER_FILE_ACCESS
SET CAN_WRITE = 1
WHERE USERNAME = 'Yar4' AND FILE_NAME = 'readme.txt';

-- ��������� ��� Yar5 � app.exe
UPDATE USER_FILE_ACCESS
SET CAN_EXECUTE = 1
WHERE USERNAME = 'Yar5' AND FILE_NAME = 'app.lnk';

-- ��������� ��� Admin � app.exe
UPDATE USER_FILE_ACCESS
SET CAN_READ = 0, CAN_WRITE = 0, OWN = 0, CAN_EXECUTE = 1
WHERE USERNAME = 'Admin' AND FILE_NAME = 'app.lnk';

UPDATE USER_FILE_ACCESS
SET CAN_READ = 0, CAN_WRITE = 1, OWN = 1, CAN_EXECUTE = 1
WHERE USERNAME = 'Yar5' AND FILE_NAME = 'app.lnk';

-- ��������� ������� Role �� LOGIN_TBL
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('LOGIN_TBL') AND name = 'ROLE')
BEGIN
    ALTER TABLE LOGIN_TBL
    ADD ROLE VARCHAR(20) NOT NULL DEFAULT 'User' CHECK (ROLE IN ('Admin', 'Developer', 'User'));
    PRINT 'Column ROLE added to LOGIN_TBL.';
END
ELSE
BEGIN
    PRINT 'Column ROLE already exists in LOGIN_TBL.';
END
GO

-- ��������� ������� Role ��� �������� ������������
UPDATE LOGIN_TBL SET ROLE = 'Admin' WHERE USERNAME = 'Admin';
UPDATE LOGIN_TBL SET ROLE = 'Developer' WHERE USERNAME IN ('Yar1', 'Yar2', 'Yar3');
UPDATE LOGIN_TBL SET ROLE = 'User' WHERE USERNAME IN ('Yar4', 'Yar5');
GO

-- ��������� ������� CONFIDENTIALITY_LEVEL_RB �� FILE_ACCESS
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('FILE_ACCESS') AND name = 'CONFIDENTIALITY_LEVEL_RB')
BEGIN
    ALTER TABLE FILE_ACCESS
    ADD CONFIDENTIALITY_LEVEL_RB VARCHAR(20) NOT NULL DEFAULT 'Public' 
    CHECK (CONFIDENTIALITY_LEVEL_RB IN ('Secret', 'Confidential', 'Public'));
    PRINT 'Column CONFIDENTIALITY_LEVEL_RB added to FILE_ACCESS.';
END
ELSE
BEGIN
    PRINT 'Column CONFIDENTIALITY_LEVEL_RB already exists in FILE_ACCESS.';
END
GO

-- ��������� ������� CONFIDENTIALITY_LEVEL_RB ��� �������� �����
UPDATE FILE_ACCESS SET CONFIDENTIALITY_LEVEL_RB = 'Secret' WHERE FILE_NAME = 'config.txt';
UPDATE FILE_ACCESS SET CONFIDENTIALITY_LEVEL_RB = 'Confidential' WHERE FILE_NAME IN ('app.lnk', 'data.txt');
UPDATE FILE_ACCESS SET CONFIDENTIALITY_LEVEL_RB = 'Public' WHERE FILE_NAME IN ('readme.txt', 'logo.png');
GO

-- ��������� �����
SELECT * FROM LOGIN_TBL;
SELECT * FROM USER_FILE_ACCESS;
SELECT * FROM FILE_ACCESS;
SELECT * FROM PASSWORD_HISTORY;
GO


--DELETE  FROM LOGIN_TBL;
--DELETE  FROM USER_FILE_ACCESS;
--DELETE  FROM FILE_ACCESS;
--DELETE  FROM PASSWORD_HISTORY;