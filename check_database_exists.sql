-- 检查 UnifiedWeb3Platform 数据库是否存在
SELECT 
    name AS DatabaseName,
    database_id AS DatabaseID,
    create_date AS CreateDate,
    state_desc AS State
FROM sys.databases
WHERE name = 'UnifiedWeb3Platform';

-- 如果返回结果，说明数据库存在
-- 如果没有返回结果，说明数据库不存在


