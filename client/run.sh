#!/bin/bash

# Use system-installed Godot
GODOT_BINARY="godot-mono"

echo "🔧 Building C# project..."
dotnet build

if [ $? -ne 0 ]; then
  echo "❌ Build failed. Exiting."
  exit 1
fi

echo "🚀 Launching Godot project..."
$GODOT_BINARY --path . -v