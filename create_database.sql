-- 创建 UnifiedWeb3Platform 数据库
-- 在 SQL Server Management Studio 或 LocalDB 中执行

-- 检查数据库是否存在，如果存在则删除（谨慎使用）
-- USE master;
-- GO
-- IF EXISTS (SELECT name FROM sys.databases WHERE name = 'UnifiedWeb3Platform')
-- BEGIN
--     ALTER DATABASE UnifiedWeb3Platform SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
--     DROP DATABASE UnifiedWeb3Platform;
-- END
-- GO

-- 创建数据库
USE master;
GO

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'UnifiedWeb3Platform')
BEGIN
    CREATE DATABASE UnifiedWeb3Platform;
    PRINT '数据库 UnifiedWeb3Platform 创建成功！';
END
ELSE
BEGIN
    PRINT '数据库 UnifiedWeb3Platform 已存在！';
END
GO

-- 验证数据库创建
USE UnifiedWeb3Platform;
GO

SELECT 
    name AS DatabaseName,
    database_id AS DatabaseID,
    create_date AS CreateDate,
    state_desc AS State
FROM sys.databases
WHERE name = 'UnifiedWeb3Platform';
GO

PRINT '数据库检查完成！';
GO


