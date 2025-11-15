-- 数据库索引优化建议
-- 用于提升订单支付验证和查询性能

-- ========================================
-- 订单表索引
-- ========================================

-- 1. 支付状态和链ID索引（用于查询待验证的订单）
CREATE NONCLUSTERED INDEX IX_Orders_PaymentStatus_ChainId_PaymentMode
ON Orders(PaymentStatus, ChainId, PaymentMode)
WHERE PaymentMode = 1 
  AND PaymentStatus IN (1, 2)  -- AwaitingOnChainConfirmation, Confirmed
  AND Status = 0;  -- PendingPayment

-- 2. 支付过期时间索引（用于查询过期订单）
CREATE NONCLUSTERED INDEX IX_Orders_PaymentExpiresAt_Status
ON Orders(PaymentExpiresAt, Status, PaymentStatus)
WHERE PaymentExpiresAt IS NOT NULL 
  AND Status = 0  -- PendingPayment
  AND PaymentStatus NOT IN (2, 4);  -- 排除 Confirmed 和 Cancelled

-- 3. 交易哈希索引（用于快速查找订单）
CREATE NONCLUSTERED INDEX IX_Orders_PaymentTransactionHash
ON Orders(PaymentTransactionHash)
WHERE PaymentTransactionHash IS NOT NULL;

-- 4. 用户ID和订单状态索引（用于查询用户订单）
CREATE NONCLUSTERED INDEX IX_Orders_Uid_Status
ON Orders(Uid, Status, CreateTime DESC);

-- 5. 订单编号索引（唯一索引，用于快速查找）
-- 注意：如果 OrderNumber 已经有唯一约束，则不需要此索引
-- CREATE UNIQUE NONCLUSTERED INDEX IX_Orders_OrderNumber
-- ON Orders(OrderNumber);

-- ========================================
-- 订单商品表索引
-- ========================================

-- 6. 订单ID索引（用于查询订单商品）
CREATE NONCLUSTERED INDEX IX_OrderItems_OrderId
ON OrderItems(OrderId);

-- ========================================
-- 支付日志表索引
-- ========================================

-- 7. 订单ID和创建时间索引（用于查询支付日志）
CREATE NONCLUSTERED INDEX IX_OrderPaymentLogs_OrderId_CreateTime
ON OrderPaymentLogs(OrderId, CreateTime DESC);

-- ========================================
-- 购物车表索引
-- ========================================

-- 8. 用户ID和商品ID索引（用于查询购物车）
CREATE NONCLUSTERED INDEX IX_ShoppingCartItems_Uid_ProductId
ON ShoppingCartItems(Uid, ProductId);

-- ========================================
-- 商品表索引
-- ========================================

-- 9. 分类ID和上架状态索引（用于商品列表查询）
CREATE NONCLUSTERED INDEX IX_Products_CategoryId_IsPublished
ON Products(CategoryId, IsPublished)
WHERE IsPublished = 1;

-- ========================================
-- 库存表索引
-- ========================================

-- 10. 商品ID索引（用于查询库存）
-- 注意：如果 ProductId 已经有唯一约束，则不需要此索引
-- CREATE UNIQUE NONCLUSTERED INDEX IX_ProductInventory_ProductId
-- ON ProductInventory(ProductId);

-- ========================================
-- 执行说明
-- ========================================

-- 1. 在生产环境执行前，建议先在测试环境验证
-- 2. 索引创建可能需要一些时间，建议在低峰期执行
-- 3. 可以使用以下命令查看索引使用情况：
--    SELECT * FROM sys.dm_db_index_usage_stats 
--    WHERE object_id = OBJECT_ID('Orders')

-- ========================================
-- 索引维护
-- ========================================

-- 定期重建索引（建议每月执行一次）
-- ALTER INDEX ALL ON Orders REBUILD;
-- ALTER INDEX ALL ON OrderItems REBUILD;
-- ALTER INDEX ALL ON OrderPaymentLogs REBUILD;

-- 更新统计信息（建议每周执行一次）
-- UPDATE STATISTICS Orders;
-- UPDATE STATISTICS OrderItems;
-- UPDATE STATISTICS OrderPaymentLogs;

