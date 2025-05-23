# NetDrone

Last CI run: [NetDrone Github Actions](https://github.com/amumor/NetDrone/actions)

Installation instructions: [Installation](#installation-and-usage)

## Intro
This project is the final project in the **IDATT2104 Nettverksprogrammering** (Network Programming) course at NTNU. The task is to implement a netcode library for either client-server or peer-peer. Expected work per student is at 30-40 hours. Victor Undæs and Amund Mørk has together implemented this solution with no prior knowledge to netcode.

In these times of conflicts, we decided to try to create a solution that helps drone pilots in conflict to stay safe. Our solution is based around letting the pilot controlling a drone from the safety of a bunker, through a deployed server, for example a cheap Raspberry Pi. The drone hardware, ex: radio senders etc, is controlled by this server. The server has the capability to control multiple drones.

Since the task is focused on the netcode implementation, we have kept the visualization quite simple, keeping the main focus on the libraries. The NetDroneServerLib implements the required UDP server that relays messages between operator and drone, while the NetDroneClientLib implements the client side functionality and most of the netcode logic. 

## Functionality
Our solution is unidirectional in nature, meaning only the operator can issue commands to the drone. However, the system is designed to account for autonomous movements or disturbances on the drone side, such as those caused by wind or other external factors. This ensures that the operator receives accurate feedback about the drone’s actual position, even when unexpected movements occur, providing a more robust and realistic control experience.

### Prediction

### Reconsilitation

### Interpolation

## Future Work and Current Limitations 

## External Dependencies
The only external dependency in this project is the Godot open source game engine with C#, used for the visualization. The libraries can easily be used with other forms of UI.

[godotengine.org](https://godotengine.org/)

## Installation and Usage

## Running Tests