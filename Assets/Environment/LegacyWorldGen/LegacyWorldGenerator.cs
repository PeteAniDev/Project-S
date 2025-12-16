using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public class LegacyWorldGenerator : MonoBehaviour {

	private static int WORLD_SIZE = 512;

	public static int MAP_VOID = 0;
	public static int MAP_LAND = 1;
	public static int MAP_PATH = 2;
	public static int MAP_MAIN_LAND = 3;
	public static int MAP_FIXED_VOID = 4;
	public static int MAP_PATH_VOID = 5;

	public int[][] map;

	public Dictionary<int, LegacyRoomData> rooms = new Dictionary<int, LegacyRoomData>();
	public List<LegacyRoomConnection> connections = new List<LegacyRoomConnection>();

	private LegacyRoomData firstRoom;
	private LegacyRoomData lastRoom;

	private int currentRoomId = 0;

	public Texture2D representation;
	public GameObject groundBase;
	public Transform ground;

	public Sprite groundSprite;
	public Sprite edgeN;
	public Sprite edgeS;
	public Sprite cornerN;
	public Sprite cornerS;
	public Sprite cornerW;
	public Sprite cornerNW;
	public Sprite cornerSW;
	public Sprite path;

	private Dictionary<int, Color> representationColors = new Dictionary<int, Color>();

	private bool mainLand = true;

	private List<LegacyRoomData> limiterData = new List<LegacyRoomData>();

	void Start() {
		representationColors.Add(MAP_VOID, Color.white);
		representationColors.Add(MAP_LAND, Color.blue);
		representationColors.Add(MAP_PATH, Color.cyan);
		representationColors.Add(MAP_FIXED_VOID, Color.gray);
		representationColors.Add(MAP_PATH_VOID, Color.gray);
		representationColors.Add(MAP_MAIN_LAND, Color.red);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			while (!TryGenerate(10))
				;
			Represent();
		}
	}

	private void Represent() {
		foreach (Transform room in ground) {
			Destroy(room.gameObject);
		}

		foreach (LegacyRoomData room in rooms.Values) {
			CreateGround(room.a.x + 4, room.a.y + 4, groundSprite, 0).gridOffset = new Vector2(-0.5f, -0.5f);
		}

		for (int x = 1; x < WORLD_SIZE - 1; x++) {
			for (int y = 1; y < WORLD_SIZE - 1; y++) {
				TryFill(x, y);
			}
		}

		for (int x = 0; x < WORLD_SIZE; x++) {
			for (int y = 0; y < WORLD_SIZE; y++) {
				representation.SetPixel(x, y, representationColors[map[x][y]]);
			}
		}

		representation.Apply();
	}

	private void TryFill(int x, int y) {
		if (Path(x, y)) {
			CreatePath(x, y, path, 3);
		}
		if (Land(x, y)) {
			bool nw = Void(x - 1, y);
			bool se = Void(x + 1, y);
			bool sw = Void(x, y - 1);
			bool ne = Void(x, y + 1);

			if (nw && se && sw && ne) {
				return;
			}
			if (nw && se && sw) {
				CreateGround(x, y, cornerSW, 4);
				return;
			}
			if (nw && se && ne) {
				CreateGround(x, y, cornerNW, 4, true);
				return;
			}
			if (nw && sw && ne) {
				CreateGround(x, y, cornerNW, 4);
				return;
			}
			if (se && sw && ne) {
				CreateGround(x, y, cornerSW, 4, true);
				return;
			}
			if (nw && se) {
				CreateGround(x, y, edgeN, 4);
				CreateGround(x, y, edgeS, 4, true);
				return;
			}
			if (sw && ne) {
				CreateGround(x, y, edgeN, 4, true);
				CreateGround(x, y, edgeS, 4);
				return;
			}
			if (nw && ne) {
				CreateGround(x, y, cornerN, 4);
				return;
			}
			if (ne && se) {
				CreateGround(x, y, cornerW, 4, true);
				return;
			}
			if (se && sw) {
				CreateGround(x, y, cornerS, 4);
				return;
			}
			if (sw && nw) {
				CreateGround(x, y, cornerW, 4);
				return;
			}
			if (nw) {
				CreateGround(x, y, edgeN, 4);
				return;
			}
			if (ne) {
				CreateGround(x, y, edgeN, 4, true);
				return;
			}
			if (sw) {
				CreateGround(x, y, edgeS, 4);
				return;
			}
			if (se) {
				CreateGround(x, y, edgeS, 4, true);
				return;
			}
		}
	}

	private bool Land(int x, int y) {
		return map[x][y] == MAP_LAND || map[x][y] == MAP_MAIN_LAND;
	}

	private bool Path(int x, int y) {
		return map[x][y] == MAP_PATH;
	}

	private bool Void(int x, int y) {
		return map[x][y] == MAP_VOID || map[x][y] == MAP_FIXED_VOID;
	}

	private GridLock CreateGround(int x, int y, Sprite sprite, int order) {
		return CreateGround(x, y, sprite, order, false);
	}

	private GridLock CreatePath(int x, int y, Sprite sprite, int order) {
		GameObject obj = Instantiate(groundBase, (Vector3)GridLock.ToWorldPos(new Vector2(x, y)), Quaternion.identity, ground);
		SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
		sr.sprite = sprite;
		sr.sortingOrder = order;
		return obj.GetComponent<GridLock>();
	}

	private GridLock CreateGround(int x, int y, Sprite sprite, int order, bool flip) {
		GameObject obj = Instantiate(groundBase, (Vector3)GridLock.ToWorldPos(new Vector2(x, y)), Quaternion.identity, ground);
		SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
		sr.sprite = sprite;
		sr.sortingOrder = order;
		sr.flipX = flip;
		return obj.GetComponent<GridLock>();
	}

	private bool TryGenerate(int mainRoomCount) {
		map = new int[WORLD_SIZE][];
		for (int x = 0; x < WORLD_SIZE; x++) {
			map[x] = new int[WORLD_SIZE];
			for (int y = 0; y < WORLD_SIZE; y++) {
				map[x][y] = MAP_VOID;
			}
		}

		rooms.Clear();
		connections.Clear();
		limiterData.Clear();
		mainLand = true;
		LegacyRoomData r1 = CreateRoom(WORLD_SIZE / 2 - 3, WORLD_SIZE / 2 - 3, WORLD_SIZE / 2 + 3, WORLD_SIZE / 2 + 3);
		firstRoom = r1;
		LegacyRoomData r0 = null;
		List<LegacyRoomData> mainRooms = new List<LegacyRoomData> { r1 };
		limiterData.Add(r1);
		FinalizeRoom(r1);

		for (int i = 1; i < mainRoomCount; i++) {
			LegacyRoomData r2;

			if (r0 != null) {
				r2 = TryCreateRoom(r1, 10, 20, r1.Center() - r0.Center(), 62);
			} else {
				r2 = TryCreateRoom(r1, 10, 20);
			}
			r0 = r1;

			if (r2 == null) {
				Debug.LogWarning("Failed to create room");
				return false;
			}
			if (CreateConnection(r1, r2) == null) {
				Debug.LogWarning("Failed to create connection");
				return false;
			}

			r1 = r2;
			mainRooms.Add(r1);
			limiterData.Add(r1);
			if (limiterData.Count > 4) {
				limiterData.RemoveAt(0);
			}
			FinalizeRoom(r1);
		}
		lastRoom = r1;

		mainLand = false;
		if (!TryGenerateSubRooms(mainRooms, 90) || !TryGenerateSubRooms(mainRooms, -90)) {
			Debug.LogWarning("Failed to generate sub rooms");
			return false;
		}

		foreach (LegacyRoomConnection connection in connections) {
			if (!TryGeneratePath(connection)) {
				Debug.LogWarning("Failed to generate path");
			}
		}

		return true;
	}

	private bool TryGeneratePath(LegacyRoomConnection connection) {
		LegacyRoomData[] rooms = new LegacyRoomData[] { connection.room1, connection.room2 };
		Vector2Int r1 = new Vector2Int((int)rooms[0].Center().x, (int)rooms[0].Center().y);
		Vector2Int r2 = new Vector2Int((int)rooms[1].Center().x, (int)rooms[1].Center().y);
		Vector2Int[] rs = new Vector2Int[] { r1, r2 };
		Vector2Int[] gates = new Vector2Int[2];
		float angle1 = TMath.Angle(Vector2.up, r2 - r1) + 45;
		if (angle1 < 0) {
			angle1 += 360;
		}
		float angle2 = TMath.Angle(Vector2.up, r1 - r2) + 45;
		if (angle2 < 0) {
			angle2 += 360;
		}
		float[] angles = new float[] { angle1, angle2 };

		for (int j = 0; j < 2; j++) {
			LegacyRoomData room = rooms[j];
			Vector2Int r = rs[j];
			float angle = angles[j];

			List<Vector2Int> possibleGates = new List<Vector2Int>();
			if (angle >= 0 && angle < 90) {
				for (int x = room.a.x - 1; x < room.b.x + 1; x++) {
					possibleGates.Add(new Vector2Int(x, room.b.y + 1));
				}
			} else if (angle >= 90 && angle <= 180) {
				for (int y = room.b.y + 1; y > room.a.y - 1; y--) {
					possibleGates.Add(new Vector2Int(room.b.x + 1, y));
				}
			} else if (angle >= 180 && angle < 270) {
				for (int x = room.b.x + 1; x > room.a.x - 1; x--) {
					possibleGates.Add(new Vector2Int(x, room.a.y - 1));
				}
			} else {
				for (int y = room.a.y - 1; y < room.b.y + 1; y++) {
					possibleGates.Add(new Vector2Int(room.a.x - 1, y));
				}
			}

			List<Vector2Int> actualPossibleGates = new List<Vector2Int>();
			List<LegacyRoomConnection> validConnections = new List<LegacyRoomConnection>();
			for (int i = 0; i < room.connections.Count; i++) {
				Vector2Int cr = new Vector2Int((int)room.connections[i].GetOther(room).Center().x, (int)room.connections[i].GetOther(room).Center().y);
				float a = TMath.Angle(Vector2.up, cr - r) + 45;
				if (a < 0) {
					a += 360;
				}
				if (Div(angle) == Div(a)) {
					validConnections.Add(room.connections[i]);
				}
			}
			if (validConnections.Count <= 0) {
				Debug.Log("No Valid Connections");
				return false;
			}
			for (int i = 1 + possibleGates.Count * validConnections.IndexOf(connection) / validConnections.Count; i < possibleGates.Count * (validConnections.IndexOf(connection) + 1) / validConnections.Count; i++) {
				actualPossibleGates.Add(possibleGates[i]);
			}
			if (actualPossibleGates.Count <= 0) {
				Debug.Log("Can't Find Possible Gates");
				return false;
			}
			gates[j] = TMath.PickRandom(actualPossibleGates);
		}

		int[][] map = new int[this.map.Length][];
		for (int x = 0; x < map.Length; x++) {
			map[x] = new int[this.map[x].Length];
			for (int y = 0; y < map[x].Length; y++) {
				map[x][y] = Void(x, y) ? 4 : 0;
			}
		}
		foreach (LegacyRoomData room in this.rooms.Values) {
			for (int j = 1; j <= 5; j++) {
				for (int i = room.a.x - j; i <= room.b.x + j; i++) {
					if (map[i][room.a.y + j] > 0) {
						map[i][room.a.y + j] = j == 1 ? 32 : Mathf.Max(map[i][room.a.y + j], 9 - j);
					}
					if (map[i][room.b.y + j] > 0) {
						map[i][room.b.y + j] = j == 1 ? 32 : Mathf.Max(map[i][room.b.y + j], 9 - j);
					}
				}
				for (int i = room.a.y - j; i <= room.b.y + j; i++) {
					if (map[room.a.x + j][i] > 0) {
						map[room.a.x + j][i] = j == 1 ? 32 : Mathf.Max(map[room.a.x + j][i], 9 - j);
					}
					if (map[room.b.x + j][i] > 0) {
						map[room.b.x + j][i] = j == 1 ? 32 : Mathf.Max(map[room.b.x + j][i], 9 - j);
					}
				}
			}
		}

		AStar.pathFindAttempt = (Mathf.Abs(gates[0].x - gates[1].x) + Mathf.Abs(gates[0].y - gates[1].y)) * 8 + 64;
		if (AStar.PathFind(map, gates[0].x, gates[0].y, gates[1].x, gates[1].y)) {
			foreach (Vector2Int path in AStar.PathList) {
				for (int x = -1; x < 2; x++) {
					for (int y = -1; y < 2; y++) {
						try {
							if (Void(path.x + x, path.y + y)) {
								this.map[path.x + x][path.y + y] = MAP_PATH_VOID;
							}
						} catch (IndexOutOfRangeException) { }
					}
				}
				this.map[path.x][path.y] = MAP_PATH;
			}
			return true;
		}
		return false;
	}

	private int Div(float angle) {
		if (angle >= 0 && angle < 90) {
			return 0;
		} else if (angle >= 90 && angle <= 180) {
			return 1;
		} else if (angle >= 180 && angle < 270) {
			return 2;
		} else {
			return 3;
		}
	}

	private bool TryGenerateSubRooms(List<LegacyRoomData> rooms, float rotation) {
		if (rooms.Count <= 2) {
			return true;
		}
		List<LegacyRoomData> nextRooms = new List<LegacyRoomData>();

		bool first = true;
		LegacyRoomData room;
		for (int i = 0; i < rooms.Count - 2; i++) {
			Vector2 direction = TMath.Rotate(rooms[i + 1].Center() - rooms[i].Center(), rotation);
			room = TryCreateRoom(rooms[i + 1], 5, 25, direction, 47);
			if (room != null) {
				nextRooms.Add(room);
				if (!first) {
					if (CreateConnection(nextRooms[nextRooms.Count - 2], room) != null) {
						FinalizeRoom(room);
					} else {
						nextRooms.Remove(room);
					}
				}
			}
			if (first && room != null) {
				first = false;
				if (CreateConnection(rooms[1], room) != null) {
					FinalizeRoom(room);
				} else {
					first = true;
					nextRooms.Remove(room);
				}
			}

		}

		if (nextRooms.Count > 1) {
			if (CreateConnection(nextRooms[nextRooms.Count - 1], rooms[rooms.Count - 2]) != null) {
				FinalizeRoom(nextRooms[nextRooms.Count - 1]);
			} else {
				return false;
			}
		}

		return TryGenerateSubRooms(nextRooms, rotation);
	}

	private LegacyRoomData CreateRoom(int ax, int ay, int bx, int by) {
		LegacyRoomData room = new LegacyRoomData(ax, ay, bx, by);
		return room;
	}

	public void FinalizeRoom(LegacyRoomData room) {
		for (int x = room.a.x - 5; x <= room.b.x + 5; x++) {
			for (int y = room.a.y - 5; y <= room.b.y + 5; y++) {
				map[x][y] = MAP_FIXED_VOID;
			}
		}
		for (int x = room.a.x; x <= room.b.x; x++) {
			for (int y = room.a.y; y <= room.b.y; y++) {
				map[x][y] = mainLand ? MAP_MAIN_LAND : MAP_LAND;
			}
		}
		rooms.Add(currentRoomId, room);
		currentRoomId++;
	}

	private LegacyRoomData TryCreateRoom(LegacyRoomData room, int querry, int maxQuerry) {
		return TryCreateRoom(room, Vector2.up, 200, querry, maxQuerry, false, false, false, false);
	}

	private LegacyRoomData TryCreateRoom(LegacyRoomData room, int querry, int maxQuerry, Vector2 direction, float maxAngle) {
		float angleNorth = Mathf.Abs(Vector2.SignedAngle(Vector2.up, direction));
		float angleEast = Mathf.Abs(Vector2.SignedAngle(Vector2.right, direction));
		float angleSouth = Mathf.Abs(Vector2.SignedAngle(Vector2.down, direction));
		float angleWest = Mathf.Abs(Vector2.SignedAngle(Vector2.left, direction));

		return TryCreateRoom(room, direction, maxAngle, querry, maxQuerry, angleNorth > maxAngle, angleEast > maxAngle, angleSouth > maxAngle, angleWest > maxAngle);
	}

	private LegacyRoomData TryCreateRoom(LegacyRoomData room, Vector2 direction, float maxAngle, int querry, int maxQuerry, bool blockNorth, bool blockEast, bool blockSouth, bool blockWest) {
		if (maxQuerry < querry) {
			return null;
		}
		if (blockNorth && blockEast && blockSouth && blockWest) {
			return null;
		}

		bool verticalQuerry = TMath.RandBool();
		bool negativeQuerry = TMath.RandBool();
		if (blockNorth && blockSouth) {
			verticalQuerry = false;
		}
		if (blockEast && blockWest) {
			verticalQuerry = true;
		}
		if (verticalQuerry) {
			if (blockNorth) {
				negativeQuerry = true;
			}
			if (blockSouth) {
				negativeQuerry = false;
			}
		} else {
			if (blockEast) {
				negativeQuerry = true;
			}
			if (blockWest) {
				negativeQuerry = false;
			}
		}

		Vector2Int pos = new Vector2Int(negativeQuerry ? TMath.RandInt(room.a.x - 4 - querry, room.a.x - 4) : TMath.RandInt(room.b.x + 4, room.b.x + 4 + querry), TMath.RandInt(room.a.y - querry, room.b.y + querry));
		if (verticalQuerry) {
			pos = new Vector2Int(TMath.RandInt(room.a.x - querry, room.b.x + querry), negativeQuerry ? TMath.RandInt(room.a.y - 4 - querry, room.a.y - 4) : TMath.RandInt(room.b.y + 4, room.b.y + 4 + querry));
		}

		int sideSize = TMath.RandInt(6, 8);
		int sideExtend = TMath.RandInt(-sideSize + 1, sideSize - 1);
		int frontExtend = TMath.RandInt(6, 8);
		Vector2Int sideExtends = new Vector2Int(sideExtend, sideExtend + (TMath.RandBool() ? sideSize : -sideSize));

		int ax = 0;
		int ay = 0;
		int bx = 0;
		int by = 0;
		if (verticalQuerry && negativeQuerry) {
			ax = pos.x + Mathf.Min(sideExtends.x, sideExtends.y);
			ay = pos.y - frontExtend;
			bx = pos.x + Mathf.Max(sideExtends.x, sideExtends.y);
			by = pos.y;
		} else if (verticalQuerry && !negativeQuerry) {
			ax = pos.x + Mathf.Min(sideExtends.x, sideExtends.y);
			ay = pos.y;
			bx = pos.x + Mathf.Max(sideExtends.x, sideExtends.y);
			by = pos.y + frontExtend;
		} else if (!verticalQuerry && negativeQuerry) {
			ax = pos.x - frontExtend;
			ay = pos.y + Mathf.Min(sideExtends.x, sideExtends.y);
			bx = pos.x;
			by = pos.y + Mathf.Max(sideExtends.x, sideExtends.y);
		} else if (!verticalQuerry && !negativeQuerry) {
			ax = pos.x;
			ay = pos.y + Mathf.Min(sideExtends.x, sideExtends.y);
			bx = pos.x + frontExtend;
			by = pos.y + Mathf.Max(sideExtends.x, sideExtends.y);
		}
		bx--;
		by--;

		if (Mathf.Abs(Vector2.SignedAngle(direction, new Vector2(ax + bx, ay + by) / 2 - room.Center())) < maxAngle) {
			if (!CheckRoomObstruction(ax, ay, bx, by)) {
				if (mainLand) {
					for (int i = 0; i < limiterData.Count - 1; i++) {
						if (Mathf.Abs(Vector2.SignedAngle(new Vector2(ax + bx, ay + by) / 2 - limiterData[i].Center(), limiterData[i + 1].Center() - limiterData[i].Center())) >= 50) {
							return TryCreateRoom(room, direction, maxAngle, querry + 1, maxQuerry, blockNorth, blockEast, blockSouth, blockWest);
						}
					}
					return CreateRoom(ax, ay, bx, by);
				} else {
					return CreateRoom(ax, ay, bx, by);
				}
			}
		}
		return TryCreateRoom(room, direction, maxAngle, querry + 1, maxQuerry, blockNorth, blockEast, blockSouth, blockWest);
	}

	private bool CheckRoomObstruction(int ax, int ay, int bx, int by) {
		for (int x = ax; x <= bx; x++) {
			for (int y = ay; y <= by; y++) {
				if (map[x][y] != MAP_VOID) {
					return true;
				}
			}
		}
		return false;
	}

	private LegacyRoomConnection CreateConnection(LegacyRoomData room1, LegacyRoomData room2) {
		foreach (LegacyRoomConnection c in connections) {
			if (TMath.CheckLSiLS(c.room1.Center(), c.room2.Center(), room1.Center(), room2.Center())) {
				return null;
			}
		}
		LegacyRoomConnection connection = new LegacyRoomConnection(room1, room2);
		connections.Add(connection);

		LegacyRoomData[] room = new LegacyRoomData[] { room1, room2 };
		for (int j = 0; j < 2; j++) {
			if (room[j].connections.Count <= 0) {
				room[j].connections.Add(connection);
			} else {
				for (int i = 0; i < room[j].connections.Count; i++) {
					float a1 = TMath.Angle(Vector2.up, room[(j + 1) % 2].Center() - room[j].Center()) + 45;
					if (a1 < 0) {
						a1 += 360;
					}
					float a2 = TMath.Angle(Vector2.up, room[j].connections[i].GetOther(room[j]).Center() - room[j].Center()) + 45;
					if (a2 < 0) {
						a2 += 360;
					}
					if (a1 < a2) {
						room[j].connections.Insert(i, connection);
						break;
					}
				}
				if (!room[j].connections.Contains(connection)) {
					room[j].connections.Add(connection);
				}
			}
		}
		return connection;
	}

}
