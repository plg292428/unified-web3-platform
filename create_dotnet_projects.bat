@echo off
chcp 65001 >nul
echo ========================================
echo   Create .NET Projects
echo ========================================
echo.

cd /d "%~dp0src\Backend"

REM Create solution
if exist "UnifiedPlatform.sln" (
    echo [INFO] Solution already exists
) else (
    dotnet new sln -n UnifiedPlatform
    echo [OK] Solution created
)
echo.

REM Create WebApi project
if exist "UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj" (
    echo [INFO] WebApi project already exists
) else (
    dotnet new webapi -n UnifiedPlatform.WebApi -f net8.0 -o UnifiedPlatform.WebApi
    dotnet sln add UnifiedPlatform.WebApi\UnifiedPlatform.WebApi.csproj
    echo [OK] WebApi project created
)
echo.

REM Create DbService project
if exist "UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj" (
    echo [INFO] DbService project already exists
) else (
    dotnet new classlib -n UnifiedPlatform.DbService -f net8.0 -o UnifiedPlatform.DbService
    dotnet sln add UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj
    echo [OK] DbService project created
)
echo.

REM Create Shared project
if exist "UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj" (
    echo [INFO] Shared project already exists
) else (
    dotnet new classlib -n UnifiedPlatform.Shared -f net8.0 -o UnifiedPlatform.Shared
    dotnet sln add UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj
    echo [OK] Shared project created
)
echo.

REM Configure project references
echo [Step] Configuring project references...
cd UnifiedPlatform.WebApi
dotnet add reference ..\UnifiedPlatform.DbService\UnifiedPlatform.DbService.csproj
dotnet add reference ..\UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj
echo [OK] WebApi references configured

cd ..\UnifiedPlatform.DbService
dotnet add reference ..\UnifiedPlatform.Shared\UnifiedPlatform.Shared.csproj
echo [OK] DbService references configured
echo.

cd ..\..
echo ========================================
echo   .NET Projects Setup Complete!
echo ========================================
echo.
pause

