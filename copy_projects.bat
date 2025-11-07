@echo off
chcp 65001 >nul
echo ========================================
echo   UnifiedWeb3Platform项目文件复制
echo ========================================
echo.

cd /d "%~dp0\.."

REM 复制HFastKit
echo [步骤1] 复制HFastKit项目...
if exist "Projects\HFastKit\HFastKit-master" (
    if not exist "UnifiedWeb3Platform\src\Libraries\HFastKit" (
        xcopy /E /I /Y "Projects\HFastKit\HFastKit-master" "UnifiedWeb3Platform\src\Libraries\HFastKit"
        echo   [OK] 已复制HFastKit项目
    ) else (
        echo   [INFO] HFastKit目标目录已存在
    )
) else (
    echo   [WARN] HFastKit源目录不存在
)
echo.

REM 复制Nblockchain
echo [步骤2] 复制Nblockchain项目...
if exist "Project20251105\Nblockchain-master" (
    if not exist "UnifiedWeb3Platform\src\Libraries\Nblockchain" (
        xcopy /E /I /Y "Project20251105\Nblockchain-master" "UnifiedWeb3Platform\src\Libraries\Nblockchain"
        echo   [OK] 已复制Nblockchain项目
    ) else (
        echo   [INFO] Nblockchain目标目录已存在
    )
) else (
    echo   [WARN] Nblockchain源目录不存在
)
echo.

REM SmallTarget前端已经在前面复制了
echo [步骤3] SmallTarget前端项目...
if exist "UnifiedWeb3Platform\src\Frontend\web-app\package.json" (
    echo   [OK] SmallTarget前端项目已存在
) else (
    echo   [INFO] 正在复制SmallTarget前端...
    if exist "Projects\SmallTarget\SmallTarget-master\SmallTarget.DappFrontEnd" (
        xcopy /E /I /Y "Projects\SmallTarget\SmallTarget-master\SmallTarget.DappFrontEnd\*" "UnifiedWeb3Platform\src\Frontend\web-app\"
        echo   [OK] 已复制SmallTarget前端项目
    ) else (
        echo   [WARN] SmallTarget前端源目录不存在
    )
)
echo.

echo ========================================
echo   文件复制完成！
echo ========================================
echo.
pause

