# NetDrone

Last CI run: [NetDrone Github Actions](https://github.com/amumor/NetDrone/actions)

Installation instructions: [Installation](#installation-and-usage)

## Intro

This project is the final assignment for the **IDATT2104 Network Programming** course at NTNU. The goal was to develop a netcode library supporting either a client-server or peer-to-peer architecture. Each student was expected to contribute approximately 30–40 hours of work. This solution was developed collaboratively by Victor Undæs and Amund Mørk, both of whom had no prior experience with netcode development.

In light of ongoing global conflicts, we chose to create a system aimed at improving the safety of drone operators in high-risk areas. Our solution enables a drone pilot to operate remotely from the safety of a bunker via a deployed server, such as a low-cost Raspberry Pi. The server interfaces with the drone hardware (e.g., radio transmitters) and is capable of managing multiple drones simultaneously.

Given that the project’s primary focus is on netcode, we intentionally kept visualization and user interface components minimal. The main emphasis is on the core networking libraries:

- **NetDroneServerLib**: Implements a UDP server that handles communication between the operator and the drone.
- **NetDroneClientLib**: Provides client-side functionality and the bulk of the netcode logic.

## Functionality
Our solution is unidirectional in nature, meaning only the operator can issue commands to the drone. However, the system is designed to account for autonomous movements or disturbances on the drone side, such as those caused by wind or other external factors. This ensures that the operator receives accurate feedback about the drone’s actual position, even when unexpected movements occur, providing a robust and precise control experience.

### Prediction
TODO

### Reconsilitation
TODO

### Interpolation
TODO

## Future Work and Current Limitations 
TODO

## External Dependencies
The only external dependency in this project is the Godot open source game engine with C#, used for the visualization. The libraries can easily be used with other forms of UI.

[godotengine.org](https://godotengine.org/)

## Installation and Usage

### Demo
To start the demo implementing NetDroneServerLib and NetDroneClientLib, follow these steps:

1. **Clone the repository:**
   ```sh
   git clone https://github.com/amumor/NetDrone.git
   cd NetDrone
   ```

2. **Start the server:**
   ```sh
   cd NetDroneServerImpl
   dotnet run
   ```

3. **In another terminal, start the visualization:**
   ```sh
   cd NetDrone/GodotDroneVisualization
   ./runNetDrone.sh both
   ```


### Using NetDroneClientLib
TODO

### Using NetDroneServerLib
Server example usage:

```cs
int dronePort = 4001;
int clientPort = 4002;

var server = new NetDroneServer();
var cts = new CancellationTokenSource();
await server.StartListenersAsync(dronePort, clientPort, cts.Token);
```

## Running Tests

To run the tests for the server and client libraries, use the following commands:

```sh
cd NetDroneServerLib.Test
dotnet test
```

```sh
cd NetDroneClientLib.Test
dotnet test
```

To see the GitHub actions runs, visit the link below. Note that an C# version error in dotnet.yaml caused all runs: #1 -> #43, this and more was fixed in commit: **8ff8965**.

[https://github.com/amumor/NetDrone/actions/workflows/dotnet.yml](https://github.com/amumor/NetDrone/actions/workflows/dotnet.yml)