# How to run NetDroneClient:
### 1. Build the project:
```bash
dotnet build
```
### 2. Run the project:
The NetDrone client can be run in two modes:
- Operator mode: This mode is used to control the drone.
- Drone mode: This mode is used to simulate the drone.

The run command also needs to specify the drone ID.

**Running in Operator mode:**
```bash
dotnet run --operator --id=1
```

**Running in Drone mode:**
```bash
dotnet run --drone --id=1
```
