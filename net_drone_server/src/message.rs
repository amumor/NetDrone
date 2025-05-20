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

    #[serde(rename = "pos")]
    Pos(Vec<i32>),
}

/// Message sent between client/drone and server.
#[derive(Serialize, Deserialize, Debug)]
#[serde(tag = "type")]
pub enum Message {
    #[serde(rename = "command")]
    Command { drone_id: i32, command: Command },
}
