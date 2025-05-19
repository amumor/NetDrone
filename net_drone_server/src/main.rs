use drone_server::server::DroneServer;
use tokio::net::UdpSocket;

#[tokio::main]
async fn main() -> anyhow::Result<()> {
    let socket = UdpSocket::bind("0.0.0.0:8080").await?;
    println!("🚀 Server listening on UDP port 8080");

    let mut server = DroneServer::new();
    let mut buf = [0u8; 1024];

    loop {
        let (len, addr) = socket.recv_from(&mut buf).await?;
        server.handle_message(&socket, &buf[..len], addr).await?;
    }
}