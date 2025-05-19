#!/bin/bash

# Name of your Godot executable (update this if needed)
GODOT_BINARY="./Godot_v4.2-stable_mono"

# Ensure the Godot binary exists
if [ ! -f "$GODOT_BINARY" ]; then
  echo "❌ Godot binary not found at: $GODOT_BINARY"
  echo "Please update the GODOT_BINARY variable in this script."
  exit 1
fi

echo "🔧 Building C# project..."
dotnet build

if [ $? -ne 0 ]; then
  echo "❌ Build failed. Exiting."
  exit 1
fi

echo "🚀 Launching Godot project..."
$GODOT_BINARY --path . -v