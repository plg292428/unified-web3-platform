@echo off
chcp 65001 >nul
echo ========================================
echo   步骤2：复制项目文件
echo ========================================
echo.

cd /d "%~dp0\.."

set BASE=UnifiedWeb3Platform
set SMALLTARGET=Projects\SmallTarget\SmallTarget-master
set HFASTKIT=Projects\HFastKit\HFastKit-master
set NBLOCKCHAIN=Project20251105\Nblockchain-master

set ALL_OK=1

REM 1. 复制SmallTarget.DappFrontEnd到src/Frontend/web-app
echo [1/6] 复制SmallTarget.DappFrontEnd...
if exist "%SMALLTARGET%\SmallTarget.DappFrontEnd" (
    xcopy /E /I /Y /Q "%SMALLTARGET%\SmallTarget.DappFrontEnd\*" "%BASE%\src\Frontend\web-app\" >nul 2>&1
    if exist "%BASE%\src\Frontend\web-app\package.json" (
        echo   [OK] SmallTarget.DappFrontEnd 复制完成
    ) else (
        echo   [FAIL] 复制失败
        set ALL_OK=0
    )
) else (
    echo   [ERROR] 源目录不存在: %SMALLTARGET%\SmallTarget.DappFrontEnd
    echo   [提示] 请检查源文件位置
    set ALL_OK=0
)
echo.

REM 2. 复制SmallTarget.WebApi到src/Backend/UnifiedPlatform.WebApi
echo [2/6] 复制SmallTarget.WebApi...
if exist "%SMALLTARGET%\SmallTarget.WebApi" (
    xcopy /E /I /Y /Q "%SMALLTARGET%\SmallTarget.WebApi\*" "%BASE%\src\Backend\UnifiedPlatform.WebApi\" >nul 2>&1
    if exist "%BASE%\src\Backend\UnifiedPlatform.WebApi\Program.cs" (
        echo   [OK] SmallTarget.WebApi 复制完成（注意：保留原有项目结构）
    ) else (
        echo   [WARN] 复制完成，但Program.cs可能已存在
    )
) else (
    echo   [ERROR] 源目录不存在: %SMALLTARGET%\SmallTarget.WebApi
    echo   [提示] 请检查源文件位置
    set ALL_OK=0
)
echo.

REM 3. 复制SmallTarget.DbService到src/Backend/UnifiedPlatform.DbService
echo [3/6] 复制SmallTarget.DbService...
if exist "%SMALLTARGET%\SmallTarget.DbService" (
    xcopy /E /I /Y /Q "%SMALLTARGET%\SmallTarget.DbService\*" "%BASE%\src\Backend\UnifiedPlatform.DbService\" >nul 2>&1
    if exist "%BASE%\src\Backend\UnifiedPlatform.DbService\SmallTarget.DbService.csproj" (
        echo   [OK] SmallTarget.DbService 复制完成（注意：保留原有项目结构）
    ) else (
        echo   [WARN] 复制完成，但项目文件可能已存在
    )
) else (
    echo   [ERROR] 源目录不存在: %SMALLTARGET%\SmallTarget.DbService
    echo   [提示] 请检查源文件位置
    set ALL_OK=0
)
echo.

REM 4. 复制SmallTarget.Shared到src/Backend/UnifiedPlatform.Shared
echo [4/6] 复制SmallTarget.Shared...
if exist "%SMALLTARGET%\SmallTarget.Shared" (
    xcopy /E /I /Y /Q "%SMALLTARGET%\SmallTarget.Shared\*" "%BASE%\src\Backend\UnifiedPlatform.Shared\" >nul 2>&1
    if exist "%BASE%\src\Backend\UnifiedPlatform.Shared\SmallTarget.Shared.csproj" (
        echo   [OK] SmallTarget.Shared 复制完成（注意：保留原有项目结构）
    ) else (
        echo   [WARN] 复制完成，但项目文件可能已存在
    )
) else (
    echo   [ERROR] 源目录不存在: %SMALLTARGET%\SmallTarget.Shared
    echo   [提示] 请检查源文件位置
    set ALL_OK=0
)
echo.

REM 5. 复制HFastKit到src/Libraries/HFastKit
echo [5/6] 复制HFastKit...
if exist "%HFASTKIT%" (
    if exist "%BASE%\src\Libraries\HFastKit\HFastKit.sln" (
        echo   [INFO] HFastKit 已存在，跳过复制
    ) else (
        xcopy /E /I /Y /Q "%HFASTKIT%\*" "%BASE%\src\Libraries\HFastKit\" >nul 2>&1
        if exist "%BASE%\src\Libraries\HFastKit\HFastKit.sln" (
            echo   [OK] HFastKit 复制完成
        ) else (
            echo   [FAIL] 复制失败
            set ALL_OK=0
        )
    )
) else (
    echo   [ERROR] 源目录不存在: %HFASTKIT%
    echo   [提示] 请检查源文件位置
    set ALL_OK=0
)
echo.

REM 6. 复制Nblockchain到src/Libraries/Nblockchain
echo [6/6] 复制Nblockchain...
if exist "%NBLOCKCHAIN%" (
    if exist "%BASE%\src\Libraries\Nblockchain\Nblockchain.sln" (
        echo   [INFO] Nblockchain 已存在，跳过复制
    ) else (
        xcopy /E /I /Y /Q "%NBLOCKCHAIN%\*" "%BASE%\src\Libraries\Nblockchain\" >nul 2>&1
        if exist "%BASE%\src\Libraries\Nblockchain\Nblockchain.sln" (
            echo   [OK] Nblockchain 复制完成
        ) else (
            echo   [FAIL] 复制失败
            set ALL_OK=0
        )
    )
) else (
    echo   [ERROR] 源目录不存在: %NBLOCKCHAIN%
    echo   [提示] 请检查源文件位置
    set ALL_OK=0
)
echo.

echo ========================================
if %ALL_OK%==1 (
    echo   [SUCCESS] 步骤2完成！
    echo ========================================
    echo.
    echo 注意事项：
    echo   1. 后端项目文件可能与现有项目结构冲突
    echo   2. 需要手动合并或重命名项目文件
    echo   3. 建议检查项目引用关系
    echo.
) else (
    echo   [WARNING] 部分文件复制失败
    echo ========================================
    echo.
    echo 请检查：
    echo   1. 源文件路径是否正确
    echo   2. 目标目录是否有写入权限
    echo   3. 是否需要手动复制某些文件
    echo.
)
pause

