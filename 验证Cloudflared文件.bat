@echo off
chcp 65001 >nul
echo ========================================
echo   验证 Cloudflared 文件完整性
echo ========================================
echo.

REM 检查文件是否存在
if not exist "cloudflared.exe" (
    echo [错误] cloudflared.exe 不存在
    echo.
    echo 请先下载 cloudflared.exe 到项目目录
    echo.
    pause
    exit /b 1
)

echo [OK] 找到 cloudflared.exe
echo.
echo 正在计算 SHA256 哈希值...
echo.

REM 使用 certutil 计算哈希
certutil -hashfile cloudflared.exe SHA256 > temp_hash.txt

REM 读取哈希值（第二行）
for /f "skip=1 tokens=1" %%a in (temp_hash.txt) do (
    set "file_hash=%%a"
    goto :found
)

:found
del temp_hash.txt

echo 计算出的哈希值:
echo %file_hash%
echo.

REM 官方哈希值（从用户提供）
set "official_hash=413f9b24dc6e61a455564651524f167b8ce29ac4ccd40703dea7af93cd37ed39"

echo 官方哈希值:
echo %official_hash%
echo.

REM 转换为大写进行对比
setlocal enabledelayedexpansion
set "file_hash_upper=!file_hash!"
set "official_hash_upper=%official_hash%"
for %%a in (A B C D E F G H I J K L M N O P Q R S T U V W X Y Z) do (
    set "file_hash_upper=!file_hash_upper:%%a=%%a!"
    set "official_hash_upper=!official_hash_upper:%%a=%%a!"
)

REM 简单对比（不区分大小写）
if /i "%file_hash%"=="%official_hash%" (
    echo ========================================
    echo   [OK] 文件完整性验证通过！
    echo ========================================
    echo.
    echo 文件完整且未被篡改，可以安全使用。
    echo.
) else (
    echo ========================================
    echo   [警告] 哈希值不匹配
    echo ========================================
    echo.
    echo 文件可能已损坏或被篡改。
    echo 建议重新下载 cloudflared.exe
    echo.
)

pause


