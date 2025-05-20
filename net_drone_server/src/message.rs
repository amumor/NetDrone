use serde::{Deserialize, Serialize};

/// Simple 3D vector with integer components.
#[derive(Serialize, Deserialize, Debug, Clone, Copy)]
pub struct Vec3<T> {
    pub x: T,
    pub y: T,
    pub z: T,
}

/// Enum for all valid client-side commands.
#[derive(Serialize, Deserialize, Debug)]
#[serde(tag = "cmd", content = "data")]
pub enum Command {
    #[serde(rename = "move")]
    Move(Vec3<i32>),
}

/// Message sent from client to server.
#[derive(Serialize, Deserialize, Debug)]
#[serde(tag = "type")]
pub enum ClientMessage {
    #[serde(rename = "command")]
    Command {
        drone_id: i32,
        command: Command,
    },
}

/// Message sent from server to client.
#[derive(Serialize, Deserialize, Debug)]
#[serde(tag = "type")]
pub enum ServerMessage {
    #[serde(rename = "state")]
    State {
        drone_id: i32,
        x: f32,
        y: f32,
        z: f32,
    },
}