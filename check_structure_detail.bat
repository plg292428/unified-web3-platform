@echo off
chcp 65001 >nul
echo ========================================
echo   步骤1：目录结构详细检测
echo ========================================
echo.

cd /d "%~dp0\.."

set BASE=UnifiedWeb3Platform
set ALL_OK=1

echo [检测] 前端项目...
if exist "%BASE%\src\Frontend\web-app\package.json" (
    echo   [OK] package.json 存在
) else (
    echo   [FAIL] package.json 不存在
    set ALL_OK=0
)

echo.
echo [检测] 后端项目...
if exist "%BASE%\src\Backend\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj" (
    echo   [OK] UnifiedPlatform.WebApi.csproj 存在
) else (
    echo   [FAIL] UnifiedPlatform.WebApi.csproj 不存在
    set ALL_OK=0
)

if exist "%BASE%\src\Backend\UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj" (
    echo   [OK] UnifiedPlatform.DbService.csproj 存在
) else (
    echo   [FAIL] UnifiedPlatform.DbService.csproj 不存在
    set ALL_OK=0
)

if exist "%BASE%\src\Backend\UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj" (
    echo   [OK] UnifiedPlatform.Shared.csproj 存在
) else (
    echo   [FAIL] UnifiedPlatform.Shared.csproj 不存在
    set ALL_OK=0
)

echo.
echo [检测] 库项目...
if exist "%BASE%\src\Libraries\HFastKit\HFastKit.sln" (
    echo   [OK] HFastKit.sln 存在
) else (
    echo   [FAIL] HFastKit.sln 不存在
    set ALL_OK=0
)

if exist "%BASE%\src\Libraries\Nblockchain\Nblockchain.sln" (
    echo   [OK] Nblockchain.sln 存在
) else (
    echo   [FAIL] Nblockchain.sln 不存在
    set ALL_OK=0
)

echo.
echo ========================================
if %ALL_OK%==1 (
    echo   [SUCCESS] 所有目录和关键文件已创建完成！
    echo ========================================
    echo.
    echo 步骤1完成情况：
    echo   [1/7] UnifiedWeb3Platform 主目录          - 完成
    echo   [2/7] src\Frontend\web-app              - 完成
    echo   [3/7] src\Backend\UnifiedPlatform.WebApi - 完成
    echo   [4/7] src\Backend\UnifiedPlatform.DbService - 完成
    echo   [5/7] src\Backend\UnifiedPlatform.Shared - 完成
    echo   [6/7] src\Libraries\HFastKit            - 完成
    echo   [7/7] src\Libraries\Nblockchain         - 完成
    echo.
    exit /b 0
) else (
    echo   [ERROR] 部分文件缺失，请检查！
    echo ========================================
    exit /b 1
)

