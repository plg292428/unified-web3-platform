@echo off
chcp 65001 >nul
title 清除 Cloudflare 缓存

echo.
echo ========================================
echo   清除 Cloudflare 缓存
echo ========================================
echo.

echo 本脚本将帮助您清除 Cloudflare 缓存
echo.
echo 方法 1: 使用 Cloudflare Dashboard (推荐)
echo   1. 访问: https://dash.cloudflare.com
echo   2. 选择域名: a292428dsj.dpdns.org
echo   3. 进入: Caching -^> Configuration
echo   4. 点击: Purge Everything
echo   5. 确认清除
echo.
echo 方法 2: 使用 API (需要 API Token)
echo   如果您有 Cloudflare API Token，可以使用以下命令:
echo   curl -X POST "https://api.cloudflare.com/client/v4/zones/ZONE_ID/purge_cache" ^
echo     -H "Authorization: Bearer YOUR_API_TOKEN" ^
echo     -H "Content-Type: application/json" ^
echo     --data "{\"purge_everything\":true}"
echo.
echo 方法 3: 清除特定 URL
echo   在 Dashboard 中:
echo   1. 进入: Caching -^> Configuration
echo   2. 点击: Custom Purge
echo   3. 输入: https://www.a292428dsj.dpdns.org
echo   4. 点击: Purge
echo.
echo ========================================
echo.
echo 正在尝试使用 wrangler 清除缓存...
echo.

cd /d "%~dp0"

if exist "%~dp0src\Frontend\web-app\node_modules\.bin\wrangler.cmd" (
    cd "%~dp0src\Frontend\web-app"
    echo 使用 wrangler 清除缓存...
    call npx wrangler pages deployment list --project-name=unified-web3-platform
    echo.
    echo 注意: wrangler 可能不支持直接清除缓存
    echo 请使用 Dashboard 方法清除缓存
) else (
    echo 未找到 wrangler，请使用 Dashboard 方法清除缓存
)

echo.
echo ========================================
echo   操作说明
echo ========================================
echo.
echo 重要提示:
echo   1. 清除缓存后，等待 1-2 分钟让更改生效
echo   2. 使用无痕模式测试，避免浏览器缓存干扰
echo   3. 清除缓存后，访问网站应该加载最新代码
echo.
echo 验证方法:
echo   1. 使用无痕模式访问: https://www.a292428dsj.dpdns.org
echo   2. 打开 Console (F12)
echo   3. 查看 Network 标签
echo   4. 找到 index-*.js 文件
echo   5. 文件名应该是新的哈希值（不是 index-D-5dwbfa.js）
echo.
pause

