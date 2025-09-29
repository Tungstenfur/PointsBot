@echo off
REM Build script for PointsBot
REM Usage: build.bat [platform]
REM Platforms: linux-x64, win-x64, all

echo PointsBot Build Script
echo =====================

set PLATFORM=%1
if "%PLATFORM%"=="" set PLATFORM=all

REM Check if dotnet is installed
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ❌ .NET SDK is not installed or not in PATH
    echo Please install .NET 9.0 SDK from https://dotnet.microsoft.com/download
    exit /b 1
)

REM Restore dependencies first
echo Restoring dependencies...
dotnet restore
if errorlevel 1 (
    echo ❌ Failed to restore dependencies
    exit /b 1
)
echo.

REM Build based on platform argument
if "%PLATFORM%"=="linux-x64" (
    call :build_for_platform linux-x64
) else if "%PLATFORM%"=="win-x64" (
    call :build_for_platform win-x64
) else if "%PLATFORM%"=="all" (
    call :build_for_platform linux-x64
    call :build_for_platform win-x64
) else (
    echo ❌ Unknown platform: %PLATFORM%
    echo Available platforms: linux-x64, win-x64, all
    exit /b 1
)

echo 🎉 Build process completed!
echo.
echo 📋 Next steps:
echo    1. Copy appsettings.template.json to appsettings.json
echo    2. Configure your Discord bot token in appsettings.json
echo    3. Run the executable for your platform
goto :eof

:build_for_platform
echo Building for %1...
dotnet publish --configuration Release --runtime %1 --self-contained --output "publish/%1"
if errorlevel 1 (
    echo ❌ Build failed for %1
    exit /b 1
) else (
    echo ✅ Build successful for %1
    echo 📁 Output directory: publish/%1
    if "%1"=="linux-x64" (
        echo 🐧 Executable: publish/%1/silly-kronos
    ) else if "%1"=="win-x64" (
        echo 🪟 Executable: publish/%1/silly-kronos.exe
    )
)
echo.
goto :eof