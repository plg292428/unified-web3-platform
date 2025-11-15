@echo off
chcp 65001 >nul
echo.
echo ========================================
echo   配置域名访问
echo ========================================
echo.

echo 需要管理员权限来编辑 hosts 文件
echo.

:: 检查管理员权限
net session >nul 2>&1
if %errorLevel% neq 0 (
    echo [错误] 需要管理员权限
    echo 请右键以管理员身份运行此脚本
    pause
    exit /b 1
)

set HOSTS_FILE=C:\Windows\System32\drivers\etc\hosts

echo [1/3] 备份 hosts 文件...
if exist "%HOSTS_FILE%.backup" (
    echo [INFO] 备份文件已存在，跳过备份
) else (
    copy /Y "%HOSTS_FILE%" "%HOSTS_FILE%.backup" >nul 2>&1
    if %errorLevel% equ 0 (
        echo [OK] 备份完成
    ) else (
        echo [WARN] 备份失败，继续执行...
    )
)

echo.
echo [2/3] 检查是否已配置域名...
findstr /C:"www.a292428dsj.dpdns.org" "%HOSTS_FILE%" >nul 2>&1
if %errorLevel% equ 0 (
    echo [INFO] 域名已存在，跳过添加
) else (
    echo [INFO] 添加域名映射...
    (
        echo.
        echo # Web3 Shopping Platform - 添加于 %date% %time%
        echo 127.0.0.1    www.a292428dsj.dpdns.org
        echo 127.0.0.1    a292428dsj.dpdns.org
    ) >> "%HOSTS_FILE%"
    if %errorLevel% equ 0 (
        echo [OK] 域名映射已添加
    ) else (
        echo [错误] 添加失败，请手动编辑 hosts 文件
        pause
        exit /b 1
    )
)

echo.
echo [3/3] 验证配置...
findstr /C:"www.a292428dsj.dpdns.org" "%HOSTS_FILE%" >nul 2>&1
if %errorLevel% equ 0 (
    echo [OK] 配置验证成功
) else (
    echo [错误] 配置验证失败
    pause
    exit /b 1
)

echo.
echo ========================================
echo   配置完成
echo ========================================
echo.
echo 已添加以下域名映射:
echo   127.0.0.1    www.a292428dsj.dpdns.org
echo   127.0.0.1    a292428dsj.dpdns.org
echo.
echo 访问地址:
echo   http://www.a292428dsj.dpdns.org:5173
echo   http://a292428dsj.dpdns.org:5173
echo.
echo 提示:
echo   1. 如果无法访问，请清除浏览器缓存
echo   2. 或使用无痕模式测试
echo   3. 确保前端服务已启动
echo.
pause

