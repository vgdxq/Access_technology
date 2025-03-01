-- ������������ ���� �����
USE WINFORM_DB;
GO

-- ��������� ������� ������ ����� ���������� �������
DECLARE @sql NVARCHAR(MAX) = '';

-- ��������� ��� ������� ������, �� ����������� �� FILE_ACCESS
SELECT @sql = @sql + 'ALTER TABLE ' + QUOTENAME(OBJECT_SCHEMA_NAME(parent_object_id)) + '.' + QUOTENAME(OBJECT_NAME(parent_object_id)) +
                ' DROP CONSTRAINT ' + QUOTENAME(name) + ';' + CHAR(13)
FROM sys.foreign_keys
WHERE referenced_object_id = OBJECT_ID('FILE_ACCESS');

-- ��������� ��������� ������� ������
IF @sql <> ''
    EXEC sp_executesql @sql;
GO

-- ��������� �������
DROP TABLE IF EXISTS FILE_ACCESS;
DROP TABLE IF EXISTS LOGIN_TBL;
GO

-- ��������� ������� ������������
CREATE TABLE LOGIN_TBL (
    USERNAME NVARCHAR(50) PRIMARY KEY,
    PASS NVARCHAR(100) NOT NULL,
    PASSWORD_COMPLEXITY NVARCHAR(10) NULL,
    ROLE NVARCHAR(20) NOT NULL DEFAULT 'User'
);
GO

-- ��������� ������������
INSERT INTO LOGIN_TBL (USERNAME, PASS, PASSWORD_COMPLEXITY, ROLE) VALUES
    ('Yaroshenko_1', 'Pass1', 'Low', 'Admin'),
    ('Yaroshenko_2', 'Pass2', 'Low', 'Developer'),
    ('Yaroshenko_3', 'Password3!', 'High', 'Developer'),
    ('Yaroshenko_4', 'Password4@', 'High', 'User'),
    ('Yaroshenko_5', 'Password5#', 'High', 'User');
GO

-- ��������� ������� ����� � ������ ���������������
CREATE TABLE FILE_ACCESS (
    FILE_NAME NVARCHAR(100) PRIMARY KEY,
    CONFIDENTIALITY_LEVEL NVARCHAR(50) NOT NULL CHECK (CONFIDENTIALITY_LEVEL IN ('Secret', 'Confidential', 'Public'))
);
GO

-- ��������� �����
INSERT INTO FILE_ACCESS (FILE_NAME, CONFIDENTIALITY_LEVEL) VALUES
    ('config.txt', 'Secret'),
    ('data.txt', 'Confidential'),
    ('readme.txt', 'Public'),
    ('app.exe', 'Confidential'),
    ('logo.png', 'Public');
GO

-- ��������� ������� ����� ���������� (���� ���� ����)
DROP FUNCTION IF EXISTS dbo.CheckFileAccess;
GO

-- ������� ��� �������� ������� �� �����
CREATE FUNCTION dbo.CheckFileAccess(@Username NVARCHAR(50), @FileName NVARCHAR(100))
RETURNS BIT
AS
BEGIN
    DECLARE @UserRole NVARCHAR(20);
    DECLARE @ConfidentialityLevel NVARCHAR(50);

    -- ��������� ��� �����������
    SELECT @UserRole = ROLE
    FROM LOGIN_TBL
    WHERE USERNAME = @Username;

    -- ��������� ���� ��������������� �����
    SELECT @ConfidentialityLevel = CONFIDENTIALITY_LEVEL
    FROM FILE_ACCESS
    WHERE FILE_NAME = @FileName;

    -- �������� �������
    IF (@UserRole = 'Admin' AND @ConfidentialityLevel IN ('Secret', 'Confidential', 'Public'))
        OR (@UserRole = 'Developer' AND @ConfidentialityLevel IN ('Confidential', 'Public'))
        OR (@UserRole = 'User' AND @ConfidentialityLevel = 'Public')
    BEGIN
        RETURN 1; -- ������ ���������
    END

    RETURN 0; -- ������ ����������
END;
GO

-- ���������� �������
DECLARE @HasAccess BIT;

-- �������� ������� ������������ �� ��������������� �����
SET @HasAccess = dbo.CheckFileAccess('Yaroshenko_1', 'config.txt');
PRINT 'Admin -> config.txt: ' + CAST(@HasAccess AS NVARCHAR);

-- �������� ������� ���������� ����������� �� ��������������� �����
SET @HasAccess = dbo.CheckFileAccess('Yaroshenko_4', 'config.txt');
PRINT 'User -> config.txt: ' + CAST(@HasAccess AS NVARCHAR);
GO

-- �������� �������
SELECT * FROM LOGIN_TBL;
SELECT * FROM FILE_ACCESS;
GO
