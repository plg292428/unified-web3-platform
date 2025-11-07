@echo off
chcp 65001 >nul
echo ========================================
echo   步骤2：复制结果检查
echo ========================================
echo.

cd /d "%~dp0"

set BASE=src
set HAS_CONFLICTS=0

echo [检查] 前端项目...
if exist "%BASE%\Frontend\web-app\package.json" (
    echo   [OK] 前端项目文件已复制
) else (
    echo   [FAIL] 前端项目文件缺失
    set HAS_CONFLICTS=1
)
echo.

echo [检查] WebApi项目...
if exist "%BASE%\Backend\UnifiedPlatform.WebApi\SmallTarget.WebApi.csproj" (
    echo   [WARN] 发现 SmallTarget.WebApi.csproj
    set HAS_CONFLICTS=1
)
if exist "%BASE%\Backend\UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj" (
    echo   [OK] UnifiedPlatform.WebApi.csproj 存在
)
if exist "%BASE%\Backend\UnifiedPlatform.WebApi\Program.cs" (
    echo   [OK] Program.cs 存在
)
echo.

echo [检查] DbService项目...
if exist "%BASE%\Backend\UnifiedPlatform.DbService\SmallTarget.DbService.csproj" (
    echo   [WARN] 发现 SmallTarget.DbService.csproj
    set HAS_CONFLICTS=1
)
if exist "%BASE%\Backend\UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj" (
    echo   [OK] UnifiedPlatform.DbService.csproj 存在
)
echo.

echo [检查] Shared项目...
if exist "%BASE%\Backend\UnifiedPlatform.Shared\SmallTarget.Shared.csproj" (
    echo   [WARN] 发现 SmallTarget.Shared.csproj
    set HAS_CONFLICTS=1
)
if exist "%BASE%\Backend\UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj" (
    echo   [OK] UnifiedPlatform.Shared.csproj 存在
)
echo.

echo [检查] 库项目...
if exist "%BASE%\Libraries\HFastKit\HFastKit.sln" (
    echo   [OK] HFastKit 已存在
) else (
    echo   [FAIL] HFastKit 缺失
    set HAS_CONFLICTS=1
)

if exist "%BASE%\Libraries\Nblockchain\Nblockchain.sln" (
    echo   [OK] Nblockchain 已存在
) else (
    echo   [FAIL] Nblockchain 缺失
    set HAS_CONFLICTS=1
)
echo.

echo ========================================
if %HAS_CONFLICTS%==1 (
    echo   [WARNING] 发现冲突或缺失文件
    echo ========================================
    echo.
    echo 需要处理的问题：
    echo   1. 后端项目有新旧两个项目文件
    echo   2. 需要选择使用哪个项目文件
    echo   3. 可能需要合并配置
    echo.
    echo 建议操作：
    echo   1. 保留 UnifiedPlatform.*.csproj 作为主项目文件
    echo   2. 将 SmallTarget.*.csproj 中的配置合并到新文件
    echo   3. 或者删除旧的项目文件（如果不需要）
    echo.
) else (
    echo   [SUCCESS] 所有文件复制完成
    echo ========================================
)
pause

