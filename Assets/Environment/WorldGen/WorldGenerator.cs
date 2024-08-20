using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

using TMPro;

using UnityEditor.MemoryProfiler;

using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class WorldGenerator : MonoBehaviour {

	private static int WORLD_SIZE = 512;

	public static int MAP_VOID = 0;
	public static int MAP_LAND = 1;
	public static int MAP_PATH = 2;
	public static int MAP_MAIN_LAND = 3;
	public static int MAP_FIXED_VOID = 4;
	public static int MAP_PATH_VOID = 5;

	public int[][] map;

	public Dictionary<int, RoomData> rooms = new Dictionary<int, RoomData>();
	public List<RoomConnection> connections = new List<RoomConnection>();

	private RoomData firstRoom;
	private RoomData lastRoom;

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

	private Dictionary<int, Color> representationColors = new Dictionary<int, Color>();

	private bool mainLand = true;

	private List<RoomData> limiterData = new List<RoomData>();

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

		foreach (RoomData room in rooms.Values) {
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
		RoomData r1 = CreateRoom(WORLD_SIZE / 2 - 3, WORLD_SIZE / 2 - 3, WORLD_SIZE / 2 + 3, WORLD_SIZE / 2 + 3);
		firstRoom = r1;
		RoomData r0 = null;
		List<RoomData> mainRooms = new List<RoomData> { r1 };
		limiterData.Add(r1);
		FinalizeRoom(r1);

		for (int i = 1; i < mainRoomCount; i++) {
			RoomData r2;

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

		foreach (RoomConnection connection in connections) {
			TryGeneratePath(connection);
		}

		return true;
	}

	private bool exitedLand = false;

	private bool TryGeneratePath(RoomConnection connection) {
		exitedLand = false;
		bool[][] map = new bool[this.map.Length][];
		for (int x = 0; x < map.Length; x++) {
			map[x] = new bool[this.map[x].Length];
			for (int y = 0; y < map[x].Length; y++) {
				map[x][y] = false;
			}
		}
		Vector2Int r1 = new Vector2Int((int)connection.room1.Center().x, (int)connection.room1.Center().y);
		Vector2Int r2 = new Vector2Int((int)connection.room2.Center().x, (int)connection.room2.Center().y);
		return PathFind(map, r1.x, r1.y, r2.x, r2.y, 100);
	}

	private static Vector2Int[] checker = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(2, 0), new Vector2Int(2, -1), new Vector2Int(0, -2), new Vector2Int(1, -2) };
	private static Vector2Int[] movement = new Vector2Int[] { new Vector2Int(-1, 1), new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, -1), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, 1) };
	private static int[][][] move = new int[][][] {
		new int[][] { new int[] { },new int[] { 7, 6, 0, 5, 1, 4, 2, 3 }, new int[] { 3, 4, 2, 5, 1, 6, 0, 7 } },
		new int[][] { new int[] { 5, 4, 6, 3, 7, 2, 0, 1 }, new int[] { 6, 5, 7, 4, 0, 3, 1, 2 }, new int[] { 4, 5, 3, 6, 2, 7, 1, 0 } },
		new int[][] { new int[] { 1, 2, 0, 3, 7, 4, 6, 5 }, new int[] { 0, 1, 7, 2, 6, 3, 5, 4 }, new int[] { 2, 3, 1, 4, 0, 5, 7, 6 } }
	};

	private bool PathFind(bool[][] map, int x, int y, int targetX, int targetY, int attempt) {
		int moveX = 0;
		if (x > targetX) {
			moveX = 2;
		} else if (x < targetX) {
			moveX = 1;
		}
		int moveY = 0;
		if (y > targetY) {
			moveY = 2;
		} else if (y < targetY) {
			moveY = 1;
		}

		if (map[x][y]) {
			return false;
		}
		if (moveX == 0 && moveY == 0) {
			return true;
		}

		if (x == targetX && y == targetY) {
			if (Void(x, y)) {
				this.map[x][y] = MAP_PATH;
			}
			return true;
		}
		map[x][y] = true;

		for (int i = 0; i < 8; i++) {
			if (PathFind(map, x + movement[move[moveX][moveY][i]].x, y + movement[move[moveX][moveY][i]].y, targetX, targetY, attempt - 1)) {
				if (Void(x, y)) {
					this.map[x][y] = MAP_PATH;
				}
				return true;
			}
		}
		return false;
	}

	private bool TryGenerateSubRooms(List<RoomData> rooms, float rotation) {
		if (rooms.Count <= 2) {
			return true;
		}
		List<RoomData> nextRooms = new List<RoomData>();

		bool first = true;
		RoomData room;
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

	private RoomData CreateRoom(int ax, int ay, int bx, int by) {
		RoomData room = new RoomData(ax, ay, bx, by);
		return room;
	}

	public void FinalizeRoom(RoomData room) {
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

	private RoomData TryCreateRoom(RoomData room, int querry, int maxQuerry) {
		return TryCreateRoom(room, Vector2.up, 200, querry, maxQuerry, false, false, false, false);
	}

	private RoomData TryCreateRoom(RoomData room, int querry, int maxQuerry, Vector2 direction, float maxAngle) {
		float angleNorth = Mathf.Abs(Vector2.SignedAngle(Vector2.up, direction));
		float angleEast = Mathf.Abs(Vector2.SignedAngle(Vector2.right, direction));
		float angleSouth = Mathf.Abs(Vector2.SignedAngle(Vector2.down, direction));
		float angleWest = Mathf.Abs(Vector2.SignedAngle(Vector2.left, direction));

		return TryCreateRoom(room, direction, maxAngle, querry, maxQuerry, angleNorth > maxAngle, angleEast > maxAngle, angleSouth > maxAngle, angleWest > maxAngle);
	}

	private RoomData TryCreateRoom(RoomData room, Vector2 direction, float maxAngle, int querry, int maxQuerry, bool blockNorth, bool blockEast, bool blockSouth, bool blockWest) {
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

		int sideSize = TMath.RandInt(5, 8);
		int sideExtend = TMath.RandInt(-sideSize + 1, sideSize - 1);
		int frontExtend = TMath.RandInt(5, 8);
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

	private RoomConnection CreateConnection(RoomData room1, RoomData room2) {
		foreach (RoomConnection c in connections) {
			if (TMath.CheckLSiLS(c.room1.Center(), c.room2.Center(), room1.Center(), room2.Center())) {
				return null;
			}
		}
		RoomConnection connection = new RoomConnection(room1, room2);
		connections.Add(connection);
		return connection;
	}

	private void CreateLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col) {
		Vector2 t = p1;
		float frac = 1 / Mathf.Sqrt(Mathf.Pow(p2.x - p1.x, 2) + Mathf.Pow(p2.y - p1.y, 2));
		float ctr = 0;

		while ((int)t.x != (int)p2.x || (int)t.y != (int)p2.y) {
			t = Vector2.Lerp(p1, p2, ctr);
			ctr += frac;
			tex.SetPixel((int)t.x, (int)t.y, col);
		}
	}

}
