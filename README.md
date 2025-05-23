# NetDrone

Last CI run: [NetDrone Github Actions](https://github.com/amumor/NetDrone/actions)

Installation instructions: [Installation](#installation-and-usage)

## Intro

This project is the final assignment for the **IDATT2104 Network Programming** course at NTNU. The goal was to develop a netcode library supporting either a client-server or peer-to-peer architecture. Each student was expected to contribute approximately 30–40 hours of work. This solution was developed collaboratively by Victor Udnæs and Amund Mørk, both of whom had no prior experience with netcode development.

In light of ongoing global conflicts, we chose to create a system aimed at improving the safety of drone operators in high-risk areas. Our solution enables a drone pilot to operate remotely from the safety of a bunker via a deployed server, such as a low-cost Raspberry Pi. The server interfaces with the drone hardware (e.g., radio transmitters) and is capable of managing multiple drones simultaneously.

Given that the project’s primary focus is on netcode, we intentionally kept visualization and user interface components minimal. The main emphasis is on the core networking libraries:

- **NetDroneServerLib**: Implements a UDP server that handles communication between the operator and the drone.
- **NetDroneClientLib**: Provides client-side functionality and the bulk of the netcode logic.

## Functionality
Our solution is unidirectional in nature, meaning only the operator can issue commands to the drone. However, the system is designed to account for autonomous movements or disturbances on the drone side, such as those caused by wind or other external factors. This ensures that the operator receives accurate feedback about the drone’s actual position, even when unexpected movements occur, providing a robust and precise control experience.

### Prediction
Prediction is a technique used in networked applications to estimate the future state of an object or system, allowing for smoother and more responsive user experiences despite network delays. The drone can only respond to commands sent by the operator, therefore no prediction is possible. On the operator side, the simulated drone moves immediately when the user sends input to the server, providing a responsive user experience. Since there is no risk of cheating in this setup, server-side prediction is unnecessary.

### Reconciliation
Reconciliation is a technique used in networked applications to correct discrepancies between the client’s predicted state and the actual state received from the server. In our system, reconciliation ensures that the operator’s view of the drone’s position remains accurate, even if the drone experiences unexpected movements or disturbances. When the drone sends updated position data, the client can adjust its local state to match, maintaining consistency and a smooth user experience.

Our solution uses an ID-based movement system: the operator sends movement commands to the drone, each tagged with a unique ID. The drone responds with its current state, including the ID. If the returned state does not match the expected move, the drone´s state is considered authoritative and the client corrects its local state accordingly.

### Interpolation
TODO

## Future Work and Current Limitations 
TODO

## External Dependencies
The only external dependency in this project is the Godot open source game engine with C#, used for the visualization. The libraries can easily be used with other forms of UI.

[godotengine.org](https://godotengine.org/)

## Installation and Usage

### Demo
This demo is developed and tested on **macOS**, the demo should be possible to run on windows, but we are not sure how. Linux users should be able to follow the same steps as macOS, but we have not tested this. 

#### macOS (and possibly linux):
To start the demo implementing NetDroneServerLib and NetDroneClientLib, follow these steps:

1. **Install Godot 4 .NET (mono):**
- **Mac**: `brew install --cask godot-mono` [Homebrew](https://formulae.brew.sh/cask/godot-mono#default)
- **Linux**: Go to: [Godot 4 for Linux](https://godotengine.org/download/linux/), press **"Godot Enginge - .NET"**

2. **Clone the repository:**
   ```sh
   git clone https://github.com/amumor/NetDrone.git
   cd NetDrone
   ```

3. **Start the server:**
   ```sh
   cd NetDroneServerImpl
   dotnet run
   ```

4. **In another terminal, start the visualization:** (ensure you have Godot 4 mono installed on your machine and are able to use the godot-mono cli tool)
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