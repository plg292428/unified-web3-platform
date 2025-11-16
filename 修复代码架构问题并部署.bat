@echo off
chcp 65001 >nul
title 修复代码架构问题并部署

echo.
echo ========================================
echo   修复代码架构问题并部署
echo ========================================
echo.
echo 已修复的问题:
echo   ✅ 路由配置改为直接重定向
echo   ✅ 路由守卫使用 window.location 强制跳转
echo   ✅ App.vue 使用 window.location 强制跳转
echo   ✅ NoWalletDetected 组件完全不渲染内容
echo.

cd /d "%~dp0"

echo [步骤 1/4] 检查代码状态...
git status --short
if %errorlevel% neq 0 (
    echo   [ERROR] 不在 Git 仓库中
    pause
    exit /b 1
)

echo.
echo [步骤 2/4] 提交代码修改...
echo   添加修改的文件...
git add "src/Frontend/web-app/src/router/index.ts"
git add "src/Frontend/web-app/src/App.vue"
git add "src/Frontend/web-app/src/views/errors/NoWalletDetected.vue"

echo   检查是否有未提交的修改...
git diff --cached --quiet
if %errorlevel% neq 0 (
    echo   正在提交...
    git commit -m "fix: 完全移除 NoWalletDetected 错误页面路由和组件渲染，确保买家可以正常访问"
    if %errorlevel% equ 0 (
        echo   [OK] 代码已提交
    ) else (
        echo   [WARNING] 提交可能失败
    )
) else (
    echo   [INFO] 没有需要提交的修改
)

echo.
echo [步骤 3/4] 检查未推送的提交...
git fetch origin 2>nul
git log origin/main..HEAD --oneline >nul 2>&1
if %errorlevel% equ 0 (
    echo   [INFO] 有未推送的提交
    echo   [步骤 4/4] 推送到 GitHub...
    git push
    if %errorlevel% equ 0 (
        echo   [OK] 代码已推送到 GitHub
    ) else (
        echo   [ERROR] 推送失败
        echo   [提示] 请检查网络连接和 GitHub 配置
        pause
        exit /b 1
    )
) else (
    echo   [OK] 所有提交已推送
)

echo.
echo ========================================
echo   代码已推送到 GitHub
echo ========================================
echo.
echo ⚠️  现在必须执行以下操作:
echo.
echo 1. 等待 Cloudflare Pages 部署完成 (2-5分钟)
echo    - 访问: https://dash.cloudflare.com/
echo    - 进入: Workers ^& Pages ^> Pages ^> 您的项目
echo    - 查看部署状态，等待变为 "Success"
echo.
echo 2. 清除 Cloudflare CDN 缓存 (必须！)
echo    - 在 Cloudflare Dashboard 中
echo    - 选择您的域名
echo    - 点击 Caching ^> Configuration
echo    - 点击 Purge Everything
echo    - 等待 2-3 分钟
echo.
echo 3. 清除浏览器缓存
echo    - 按 Ctrl+Shift+Delete
echo    - 选择 "缓存的图片和文件"
echo    - 时间范围: "全部时间"
echo    - 点击 "清除数据"
echo.
echo 4. 使用无痕模式测试
echo    - 按 Ctrl+Shift+N 打开无痕窗口
echo    - 访问: https://www.a292428dsj.dpdns.org
echo    - 应该直接看到首页，不显示错误页面
echo.
echo ========================================
echo.
echo 关键修复:
echo   - 路由完全移除，改为直接重定向
echo   - 组件完全不渲染内容
echo   - 使用 window.location 强制跳转
echo   - 确保买家可以正常访问网站
echo.
pause

