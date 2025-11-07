@echo off
chcp 65001 >nul
echo ========================================
echo   步骤1：目录结构检测
echo ========================================
echo.

cd /d "%~dp0\.."

set BASE=UnifiedWeb3Platform
set ALL_OK=1

REM Check main directory
if exist "%BASE%" (
    echo [OK] %BASE%
) else (
    echo [FAIL] %BASE%
    set ALL_OK=0
)

REM Check subdirectories
if exist "%BASE%\src\Frontend\web-app" (
    echo [OK] %BASE%\src\Frontend\web-app
) else (
    echo [FAIL] %BASE%\src\Frontend\web-app
    set ALL_OK=0
)

if exist "%BASE%\src\Backend\UnifiedPlatform.WebApi" (
    echo [OK] %BASE%\src\Backend\UnifiedPlatform.WebApi
) else (
    echo [FAIL] %BASE%\src\Backend\UnifiedPlatform.WebApi
    set ALL_OK=0
)

if exist "%BASE%\src\Backend\UnifiedPlatform.DbService" (
    echo [OK] %BASE%\src\Backend\UnifiedPlatform.DbService
) else (
    echo [FAIL] %BASE%\src\Backend\UnifiedPlatform.DbService
    set ALL_OK=0
)

if exist "%BASE%\src\Backend\UnifiedPlatform.Shared" (
    echo [OK] %BASE%\src\Backend\UnifiedPlatform.Shared
) else (
    echo [FAIL] %BASE%\src\Backend\UnifiedPlatform.Shared
    set ALL_OK=0
)

if exist "%BASE%\src\Libraries\HFastKit" (
    echo [OK] %BASE%\src\Libraries\HFastKit
) else (
    echo [FAIL] %BASE%\src\Libraries\HFastKit
    set ALL_OK=0
)

if exist "%BASE%\src\Libraries\Nblockchain" (
    echo [OK] %BASE%\src\Libraries\Nblockchain
) else (
    echo [FAIL] %BASE%\src\Libraries\Nblockchain
    set ALL_OK=0
)

echo.
echo ========================================
if %ALL_OK%==1 (
    echo   [SUCCESS] 所有目录已创建完成！
    echo ========================================
    exit /b 0
) else (
    echo   [ERROR] 部分目录缺失，请检查！
    echo ========================================
    exit /b 1
)

