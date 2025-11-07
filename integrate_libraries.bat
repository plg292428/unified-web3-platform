@echo off
chcp 65001 >nul
echo ========================================
echo   Integrate Libraries
echo ========================================
echo.

cd /d "%~dp0src\Backend"

REM Add HFastKit projects to solution
echo [Step 1] Adding HFastKit projects to solution...
dotnet sln add ..\..\Libraries\HFastKit\HFastKit\HFastKit.csproj
dotnet sln add ..\..\Libraries\HFastKit\HFastKit.AspNetCore\HFastKit.AspNetCore.csproj
dotnet sln add ..\..\Libraries\HFastKit\HFastKit.AspNetCore.Shared\HFastKit.AspNetCore.Shared.csproj
echo [OK] HFastKit projects added
echo.

REM Add Nblockchain projects to solution
echo [Step 2] Adding Nblockchain projects to solution...
dotnet sln add ..\..\Libraries\Nblockchain\Nblockchain.Signer\Nblockchain.Signer.csproj
dotnet sln add ..\..\Libraries\Nblockchain\Nblockchain.Tron\Nblockchain.Tron.csproj
dotnet sln add ..\..\Libraries\Nblockchain\Nblockchain.Tron.Protocol\Nblockchain.Tron.Protocol.csproj
echo [OK] Nblockchain projects added
echo.

REM Add references to UnifiedPlatform.Shared
echo [Step 3] Adding HFastKit references to Shared project...
cd UnifiedPlatform.Shared
dotnet add reference ..\..\Libraries\HFastKit\HFastKit\HFastKit.csproj
dotnet add reference ..\..\Libraries\HFastKit\HFastKit.AspNetCore.Shared\HFastKit.AspNetCore.Shared.csproj
echo [OK] Shared project references configured
echo.

REM Add references to UnifiedPlatform.DbService
echo [Step 4] Adding references to DbService project...
cd ..\UnifiedPlatform.DbService
dotnet add reference ..\..\Libraries\HFastKit\HFastKit.AspNetCore.Shared\HFastKit.AspNetCore.Shared.csproj
echo [OK] DbService project references configured
echo.

REM Add references to UnifiedPlatform.WebApi
echo [Step 5] Adding references to WebApi project...
cd ..\UnifiedPlatform.WebApi
dotnet add reference ..\..\Libraries\HFastKit\HFastKit.AspNetCore\HFastKit.AspNetCore.csproj
dotnet add reference ..\..\Libraries\HFastKit\HFastKit.AspNetCore.Shared\HFastKit.AspNetCore.Shared.csproj
dotnet add reference ..\..\Libraries\Nblockchain\Nblockchain.Tron\Nblockchain.Tron.csproj
echo [OK] WebApi project references configured
echo.

REM Restore packages
echo [Step 6] Restoring NuGet packages...
cd ..
dotnet restore
echo [OK] Packages restored
echo.

echo ========================================
echo   Library Integration Complete!
echo ========================================
echo.
pause

