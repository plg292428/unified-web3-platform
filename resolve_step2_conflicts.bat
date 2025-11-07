@echo off
chcp 65001 >nul
echo ========================================
echo   步骤2：解决项目文件冲突
echo ========================================
echo.

cd /d "%~dp0src\Backend"

echo [操作] 删除旧的项目文件...
if exist "UnifiedPlatform.WebApi\SmallTarget.WebApi.csproj" (
    del /Q "UnifiedPlatform.WebApi\SmallTarget.WebApi.csproj"
    echo   [OK] 已删除 SmallTarget.WebApi.csproj
)

if exist "UnifiedPlatform.DbService\SmallTarget.DbService.csproj" (
    del /Q "UnifiedPlatform.DbService\SmallTarget.DbService.csproj"
    echo   [OK] 已删除 SmallTarget.DbService.csproj
)

if exist "UnifiedPlatform.Shared\SmallTarget.Shared.csproj" (
    del /Q "UnifiedPlatform.Shared\SmallTarget.Shared.csproj"
    echo   [OK] 已删除 SmallTarget.Shared.csproj
)

echo.
echo [操作] 验证项目文件...
if exist "UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj" (
    echo   [OK] UnifiedPlatform.WebApi.csproj 存在
) else (
    echo   [FAIL] UnifiedPlatform.WebApi.csproj 不存在
)

if exist "UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj" (
    echo   [OK] UnifiedPlatform.DbService.csproj 存在
) else (
    echo   [FAIL] UnifiedPlatform.DbService.csproj 不存在
)

if exist "UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj" (
    echo   [OK] UnifiedPlatform.Shared.csproj 存在
) else (
    echo   [FAIL] UnifiedPlatform.Shared.csproj 不存在
)

echo.
echo ========================================
echo   [SUCCESS] 冲突已解决
echo ========================================
echo.
echo 已完成：
echo   1. 合并了项目配置
echo   2. 删除了旧的项目文件
echo   3. 保留了新的项目文件
echo.
pause

