#!/bin/bash

dotnet build

if [ "$1" = "drone" ]; then
    godot-mono main_scene.tscn --drone
elif [ "$1" = "operator" ]; then
    godot-mono main_scene.tscn --operator
elif [ "$1" = "both" ]; then
    osascript -e "tell application \"Terminal\" to do script \"cd $(pwd); ./runNetDrone.sh drone\""
    osascript -e "tell application \"Terminal\" to do script \"cd $(pwd); ./runNetDrone.sh operator\""
else
    echo "Usage: $0 [drone|operator|both]"
    exit 1
fi