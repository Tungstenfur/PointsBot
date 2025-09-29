#!/bin/bash

# Build script for PointsBot
# Usage: ./build.sh [platform]
# Platforms: linux-x64, win-x64, all

echo "PointsBot Build Script"
echo "====================="

PLATFORM=${1:-"all"}

build_for_platform() {
    local platform=$1
    echo "Building for $platform..."
    
    dotnet publish --configuration Release --runtime $platform --self-contained --output "publish/$platform"
    
    if [ $? -eq 0 ]; then
        echo "✅ Build successful for $platform"
        echo "📁 Output directory: publish/$platform"
        
        if [ "$platform" = "linux-x64" ]; then
            echo "🐧 Executable: publish/$platform/silly-kronos"
        elif [ "$platform" = "win-x64" ]; then
            echo "🪟 Executable: publish/$platform/silly-kronos.exe"
        fi
    else
        echo "❌ Build failed for $platform"
        return 1
    fi
    echo ""
}

# Check if dotnet is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed or not in PATH"
    echo "Please install .NET 9.0 SDK from https://dotnet.microsoft.com/download"
    exit 1
fi

# Restore dependencies first
echo "Restoring dependencies..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ Failed to restore dependencies"
    exit 1
fi
echo ""

# Build based on platform argument
case $PLATFORM in
    "linux-x64")
        build_for_platform "linux-x64"
        ;;
    "win-x64")
        build_for_platform "win-x64"
        ;;
    "all")
        build_for_platform "linux-x64"
        build_for_platform "win-x64"
        ;;
    *)
        echo "❌ Unknown platform: $PLATFORM"
        echo "Available platforms: linux-x64, win-x64, all"
        exit 1
        ;;
esac

echo "🎉 Build process completed!"
echo ""
echo "📋 Next steps:"
echo "   1. Copy appsettings.template.json to appsettings.json"
echo "   2. Configure your Discord bot token in appsettings.json"
echo "   3. Run the executable for your platform"