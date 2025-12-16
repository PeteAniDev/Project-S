using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WorldGenerator : MonoBehaviour {

	public int size = 4;
	public Dictionary<Vector2Int, RoomData> rooms = new Dictionary<Vector2Int, RoomData>();
	public float wallChance = 0.5f;

	[Header("Gizmo Settings")]
	public float roomSize = 2f;
	public float wallThickness = 0.1f;
	public Color roomColor = Color.cyan;
	public Color wallColor = Color.red;
	public Color startRoomColor = Color.green;
	public Color endRoomColor = Color.yellow;

	private void Start() {
		TryGenerate();
	}

	private void Update() {
		if (Input.GetKeyUp(KeyCode.S)) {
			TryGenerate();
		}
	}

	public bool TryGenerate() {
		rooms.Clear();
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				Vector2Int pos = new Vector2Int(x, y);
				rooms[pos] = new RoomData();
			}
		}

		foreach (Vector2Int pos in rooms.Keys) {
			RoomData room = rooms[pos];
			Vector2Int nePos = new Vector2Int(pos.x, pos.y + 1);
			Vector2Int sePos = new Vector2Int(pos.x + 1, pos.y);
			if (rooms.ContainsKey(nePos)) {
				room.Connect(rooms[nePos], RoomDirection.NorthEast);
			}
			if (rooms.ContainsKey(sePos)) {
				room.Connect(rooms[sePos], RoomDirection.SouthEast);
			}
		}

		GenerateWalls();

		return true;
	}

	private void GenerateWalls() {
		// Process each 2x2 grid in the dungeon
		for (int x = 0; x < size - 1; x++) {
			for (int y = 0; y < size - 1; y++) {
				if (!(x == 0 && y == 0) && !(x == size - 2 && y == size - 2)) {
					ProcessGrid2x2(x, y);
				}
			}
		}
	}

	private void ProcessGrid2x2(int startX, int startY) {
		// Define the four rooms in the 2x2 grid
		Vector2Int bottomLeft = new Vector2Int(startX, startY);
		Vector2Int bottomRight = new Vector2Int(startX + 1, startY);
		Vector2Int topLeft = new Vector2Int(startX, startY + 1);
		Vector2Int topRight = new Vector2Int(startX + 1, startY + 1);

		// Check if all rooms exist
		if (!rooms.ContainsKey(bottomLeft) || !rooms.ContainsKey(bottomRight) ||
			!rooms.ContainsKey(topLeft) || !rooms.ContainsKey(topRight)) {
			return;
		}

		// Check if there are any walls in the plus pattern
		if (HasWallsInPlusPattern(bottomLeft, bottomRight, topLeft, topRight)) {
			return; // Skip this grid if walls already exist
		}

		// Randomly create a wall between two adjacent rooms
		CreateRandomWallInGrid(bottomLeft, bottomRight, topLeft, topRight);
	}

	private bool HasWallsInPlusPattern(Vector2Int bottomLeft, Vector2Int bottomRight, Vector2Int topLeft, Vector2Int topRight) {
		RoomData blRoom = rooms[bottomLeft];
		RoomData brRoom = rooms[bottomRight];
		RoomData tlRoom = rooms[topLeft];
		RoomData trRoom = rooms[topRight];

		// Check horizontal walls (bottom-left to bottom-right, top-left to top-right)
		bool horizontalWall1 = blRoom.GetRoom(RoomDirection.SouthEast) == null;
		bool horizontalWall2 = tlRoom.GetRoom(RoomDirection.SouthEast) == null;

		// Check vertical walls (bottom-left to top-left, bottom-right to top-right)
		bool verticalWall1 = blRoom.GetRoom(RoomDirection.NorthEast) == null;
		bool verticalWall2 = brRoom.GetRoom(RoomDirection.NorthEast) == null;

		// Return true if any wall exists in the plus pattern
		return horizontalWall1 || horizontalWall2 || verticalWall1 || verticalWall2;
	}

	private void CreateRandomWallInGrid(Vector2Int bottomLeft, Vector2Int bottomRight, Vector2Int topLeft, Vector2Int topRight) {
		// Define possible wall positions in the plus pattern
		List<WallPosition> possibleWalls = new List<WallPosition> {
			new WallPosition(bottomLeft, bottomRight, RoomDirection.SouthEast),
			new WallPosition(topLeft, topRight, RoomDirection.SouthEast),
			new WallPosition(bottomLeft, topLeft, RoomDirection.NorthEast),
			new WallPosition(bottomRight, topRight, RoomDirection.NorthEast)
		};

		// Filter out walls that already exist or would be invalid
		List<WallPosition> validWalls = new List<WallPosition>();
		foreach (WallPosition wall in possibleWalls) {
			if (CanCreateWall(wall)) {
				validWalls.Add(wall);
			}
		}

		// Randomly select and create a wall if possible
		if (validWalls.Count > 0 && Random.value < wallChance) {
			WallPosition selectedWall = validWalls[Random.Range(0, validWalls.Count)];
			CreateWall(selectedWall);
		}
	}

	private bool CanCreateWall(WallPosition wall) {
		RoomData room1 = rooms[wall.room1];
		RoomData room2 = rooms[wall.room2];

		// Check if rooms are currently connected
		return room1.GetRoom(wall.direction) == room2;
	}

	private void CreateWall(WallPosition wall) {
		RoomData room1 = rooms[wall.room1];
		RoomData room2 = rooms[wall.room2];

		// Disconnect the rooms to create a wall
		room1.Disconnect(wall.direction);
		room2.Disconnect(GetOppositeDirection(wall.direction));
	}

	private RoomDirection GetOppositeDirection(RoomDirection direction) {
		switch (direction) {
			case RoomDirection.NorthEast:
				return RoomDirection.SouthWest;
			case RoomDirection.SouthEast:
				return RoomDirection.NorthWest;
			case RoomDirection.SouthWest:
				return RoomDirection.NorthEast;
			case RoomDirection.NorthWest:
				return RoomDirection.SouthEast;
			default:
				return RoomDirection.NorthEast;
		}
	}

	// Helper struct to represent a wall position
	private struct WallPosition {
		public Vector2Int room1;
		public Vector2Int room2;
		public RoomDirection direction;

		public WallPosition(Vector2Int r1, Vector2Int r2, RoomDirection dir) {
			room1 = r1;
			room2 = r2;
			direction = dir;
		}
	}

	private void OnDrawGizmos() {
		if (rooms == null || rooms.Count == 0)
			return;

		DrawRooms();
		DrawWalls();
	}

	private void DrawRooms() {
		foreach (Vector2Int pos in rooms.Keys) {
			Vector3 roomCenter = new Vector3(pos.x * roomSize, pos.y * roomSize, 0);

			// Set room color based on special rooms
			if (pos == Vector2Int.zero) {
				Gizmos.color = startRoomColor; // Bottom-left start room
			} else if (pos == new Vector2Int(size - 1, size - 1)) {
				Gizmos.color = endRoomColor; // Top-right end room
			} else {
				Gizmos.color = roomColor;
			}

			// Draw room as a wireframe cube
			Gizmos.DrawWireCube(roomCenter, Vector3.one * roomSize * 0.9f);
		}
	}

	private void DrawWalls() {
		Gizmos.color = wallColor;

		foreach (Vector2Int pos in rooms.Keys) {
			RoomData room = rooms[pos];
			Vector3 roomCenter = new Vector3(pos.x * roomSize, pos.y * roomSize, 0);

			// Check each direction and draw walls where there's no connection
			DrawWallIfNeeded(room, pos, RoomDirection.NorthEast, roomCenter);
			DrawWallIfNeeded(room, pos, RoomDirection.SouthEast, roomCenter);
			DrawWallIfNeeded(room, pos, RoomDirection.SouthWest, roomCenter);
			DrawWallIfNeeded(room, pos, RoomDirection.NorthWest, roomCenter);
		}
	}

	private void DrawWallIfNeeded(RoomData room, Vector2Int pos, RoomDirection direction, Vector3 roomCenter) {
		// Check if there's a connection in this direction
		bool hasConnection = room.GetRoom(direction) != null;

		// Also check if the adjacent room exists
		Vector2Int adjacentPos = GetAdjacentPosition(pos, direction);
		bool adjacentRoomExists = rooms.ContainsKey(adjacentPos);

		// Draw wall if no connection exists or if we're at the edge of the dungeon
		if (!hasConnection || !adjacentRoomExists) {
			DrawWallInDirection(roomCenter, direction);
		}
	}

	private Vector2Int GetAdjacentPosition(Vector2Int pos, RoomDirection direction) {
		switch (direction) {
			case RoomDirection.NorthEast:
				return new Vector2Int(pos.x, pos.y + 1);
			case RoomDirection.SouthEast:
				return new Vector2Int(pos.x + 1, pos.y);
			case RoomDirection.SouthWest:
				return new Vector2Int(pos.x, pos.y - 1);
			case RoomDirection.NorthWest:
				return new Vector2Int(pos.x - 1, pos.y);
			default:
				return pos;
		}
	}

	private void DrawWallInDirection(Vector3 roomCenter, RoomDirection direction) {
		float halfRoom = roomSize * 0.5f;
		Vector3 wallCenter = Vector3.zero;
		Vector3 wallSize = Vector3.zero;

		switch (direction) {
			case RoomDirection.NorthEast: // Top wall
				wallCenter = roomCenter + new Vector3(0, halfRoom, 0);
				wallSize = new Vector3(roomSize, wallThickness, wallThickness);
				break;
			case RoomDirection.SouthEast: // Right wall
				wallCenter = roomCenter + new Vector3(halfRoom, 0, 0);
				wallSize = new Vector3(wallThickness, roomSize, wallThickness);
				break;
			case RoomDirection.SouthWest: // Bottom wall
				wallCenter = roomCenter + new Vector3(0, -halfRoom, 0);
				wallSize = new Vector3(roomSize, wallThickness, wallThickness);
				break;
			case RoomDirection.NorthWest: // Left wall
				wallCenter = roomCenter + new Vector3(-halfRoom, 0, 0);
				wallSize = new Vector3(wallThickness, roomSize, wallThickness);
				break;
		}

		Gizmos.DrawCube(wallCenter, wallSize);
	}

}
