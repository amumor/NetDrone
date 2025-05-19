use crate::message::{ClientMessage, Command, ServerMessage, Vec3};
use std::collections::HashMap;
use std::net::SocketAddr;
use tokio::net::UdpSocket;
use tokio::time::{sleep, Duration};

pub struct DroneServer {
    // x, y, z
    drone_states: HashMap<String, (f32, f32, f32)>,
}

impl DroneServer {
    pub fn new() -> Self {
        Self {
            drone_states: HashMap::new(),
        }
    }

    pub async fn handle_message(
        &mut self,
        socket: &UdpSocket,
        data: &[u8],
        addr: SocketAddr,
    ) -> anyhow::Result<()> {
        if let Ok(msg) = serde_json::from_slice::<ClientMessage>(data) {
            println!("📥 Received from {}: {:?}", addr, msg);

            match msg {
                ClientMessage::Command { drone_id, command } => {
                    let entry = self
                        .drone_states
                        .entry(drone_id.clone())
                        .or_insert((0.0, 0.0, 0.0));

                    match command {
                        Command::Move(Vec3 { x, y, z }) => {
                            entry.0 += x as f32;
                            entry.1 += y as f32;
                            entry.2 += z as f32;
                        }
                    }

                    // Simulate latency
                    sleep(Duration::from_millis(150)).await;

                    let response = ServerMessage::State {
                        drone_id,
                        x: entry.0,
                        y: entry.1,
                        z: entry.2,
                    };

                    let json = serde_json::to_vec(&response)?;
                    socket.send_to(&json, addr).await?;
                }
            }
        } else {
            eprintln!("⚠️ Could not parse incoming message.");
        }

        Ok(())
    }
}