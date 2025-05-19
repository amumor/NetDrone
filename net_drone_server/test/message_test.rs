use drone_server::message::{ClientMessage, Command, Vec3};
use serde_json;

#[test]
fn test_serialize_move_command() {
    let msg = ClientMessage::Command {
        drone_id: "drone_1".to_string(),
        command: Command::Move(Vec3 { x: 1, y: 2, z: -1 }),
    };

    let json = serde_json::to_string_pretty(&msg).unwrap();
    println!("Serialized JSON:\n{}", json);

    assert!(json.contains("\"cmd\": \"move\""));
    assert!(json.contains("\"x\": 1"));
}

#[test]
fn test_deserialize_move_command() {
    let json = r#"
    {
        "type": "command",
        "drone_id": "drone_1",
        "command": {
            "cmd": "move",
            "data": {
                "x": 1,
                "y": 2,
                "z": -1
            }
        }
    }
    "#;

    let msg: ClientMessage = serde_json::from_str(json).unwrap();
    match msg {
        ClientMessage::Command { drone_id, command } => {
            assert_eq!(drone_id, "drone_1");
            match command {
                Command::Move(vec) => {
                    assert_eq!(vec.x, 1);
                    assert_eq!(vec.y, 2);
                    assert_eq!(vec.z, -1);
                }
                _ => panic!("Expected Move command"),
            }
        }
    }
}