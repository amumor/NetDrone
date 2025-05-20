use crate::message::{Command, Message, Vec3};
use std::collections::HashMap;
use std::net::SocketAddr;
use tokio::net::UdpSocket;
use tokio::time::{sleep, Duration};

pub struct DroneServer {
    // x, y, z
    drone_states: HashMap<i32, (f32, f32, f32)>,
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
        if let Ok(msg) = serde_json::from_slice::<Message>(data) {
            println!("📥 Received from {}: {:?}", addr, msg);

            match msg {
                Message::Command { drone_id, command } => {
                    let entry = self.drone_states.entry(drone_id).or_insert((0.0, 0.0, 0.0));

                    match command {
                        Command::Move(Vec3 { x, y, z }) => {
                            entry.0 += x as f32;
                            entry.1 += y as f32;
                            entry.2 += z as f32;
                        }
                        Command::Pos(pos) => {
                            if pos.len() == 3 {
                                entry.0 = pos[0] as f32;
                                entry.1 = pos[1] as f32;
                                entry.2 = pos[2] as f32;
                            } else {
                                eprintln!("⚠️ Invalid position data: {:?}", pos);
                            }
                        }
                    }

                    // Simulate latency
                    sleep(Duration::from_millis(150)).await;

                    let response = serde_json::to_vec(&entry)?;
                    socket.send_to(&response, addr).await?;
                }
            }
        } else {
            eprintln!("⚠️ Could not parse incoming message.");
        }

        Ok(())
    }
}
