using System.Collections.Generic;

using UnityEngine;

public class AStar {

	private static Vector2Int[] movement = new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1) };

	public static List<Vector2Int> PathList = new List<Vector2Int>();
	public static int pathFindAttempt = 20;

	public struct Pair {
		public int first, second;

		public Pair(int x, int y) {
			first = x;
			second = y;
		}
	}

	public struct Cell {
		public int parent_i, parent_j;
		public int f, g, h;
	}

	public static bool PathFind(int[][] grid, int srcX, int srcY, int destX, int destY) {
		Pair src = new Pair(srcX, srcY);
		Pair dest = new Pair(destX, destY);
		int ROW = grid.Length;
		int COL = grid[0].Length;
		grid[src.first][src.second] = 1;
		grid[dest.first][dest.second] = 1;

		if (!IsValid(src.first, src.second, ROW, COL) || !IsValid(dest.first, dest.second, ROW, COL)) {
			Debug.Log("Source or destination is invalid");
			return false;
		}

		if (!IsUnBlocked(grid, src.first, src.second) || !IsUnBlocked(grid, dest.first, dest.second)) {
			Debug.Log("Source or the destination is blocked");
			return false;
		}

		if (src.first == dest.first && src.second == dest.second) {
			Debug.Log("We are already at the destination");
			return false;
		}

		bool[,] closedList = new bool[ROW, COL];

		Cell[,] cellDetails = new Cell[ROW, COL];

		for (int i = 0; i < ROW; i++) {
			for (int j = 0; j < COL; j++) {
				cellDetails[i, j].f = int.MaxValue;
				cellDetails[i, j].g = int.MaxValue;
				cellDetails[i, j].h = int.MaxValue;
				cellDetails[i, j].parent_i = -1;
				cellDetails[i, j].parent_j = -1;
			}
		}

		int x = src.first, y = src.second;
		cellDetails[x, y].f = 0;
		cellDetails[x, y].g = 0;
		cellDetails[x, y].h = 0;
		cellDetails[x, y].parent_i = x;
		cellDetails[x, y].parent_j = y;

		LinkedList<(int, Pair)> openList = new LinkedList<(int, Pair)>();
		openList.AddFirst((0, new Pair(x, y)));

		bool foundDest = false;

		while (openList.Count > 0) {
			(int f, Pair pair) p = openList.First.Value;
			openList.Remove(p);
			if (p.f > pathFindAttempt) {
				return false;
			}

			x = p.pair.first;
			y = p.pair.second;
			closedList[x, y] = true;

			int flex = 0;

			for (int i = 0; i < 4; i++) {
				int move = (i + flex) % 4;
				int mx = movement[move].x;
				int my = movement[move].y;
				int newX = x + mx;
				int newY = y + my;
				if (pathFindAttempt < 0) {
					return false;
				}

				if (IsValid(newX, newY, ROW, COL)) {
					if (IsDestination(newX, newY, dest)) {
						cellDetails[newX, newY].parent_i = x;
						cellDetails[newX, newY].parent_j = y;
						TracePath(cellDetails, dest);
						foundDest = true;
						return true;
					}

					if (!closedList[newX, newY] && IsUnBlocked(grid, newX, newY)) {
						int gNew = cellDetails[x, y].g + grid[newX][newY];
						int hNew = CalculateHValue(newX, newY, dest);
						int fNew = gNew + hNew;

						if (cellDetails[newX, newY].f == int.MaxValue || cellDetails[newX, newY].f > fNew) {
							Insert(openList, fNew, newX, newY);

							cellDetails[newX, newY].f = fNew;
							cellDetails[newX, newY].g = gNew;
							cellDetails[newX, newY].h = hNew;
							cellDetails[newX, newY].parent_i = x;
							cellDetails[newX, newY].parent_j = y;
						}
					}
				}
			}
		}

		if (!foundDest) {
			Debug.Log("Failed to find the Destination Cell");
			return false;
		} else {
			return true;
		}
	}

	public static bool IsValid(int row, int col, int ROW, int COL) {
		return (row >= 0) && (row < ROW) && (col >= 0) && (col < COL);
	}

	public static bool IsUnBlocked(int[][] grid, int row, int col) {
		return grid[row][col] != 0;
	}

	public static bool IsDestination(int row, int col, Pair dest) {
		return (row == dest.first && col == dest.second);
	}

	public static int CalculateHValue(int row, int col, Pair dest) {
		return (Mathf.Abs(row - dest.first) + Mathf.Abs(col - dest.second)) * 4;
	}

	public static void Insert(LinkedList<(int, Pair)> openList, int f, int x, int y) {
		LinkedListNode<(int, Pair)> beforeThis = null;
		for (int i = 0; i < openList.Count; i++) {
			if (i == 0) {
				beforeThis = openList.First;
			} else {
				beforeThis = beforeThis.Next;
			}
			if (beforeThis.Value.Item1 > f) {
				openList.AddBefore(beforeThis, (f, new Pair(x, y)));
				return;
			}
		}
		openList.AddLast((f, new Pair(x, y)));
	}

	public static void TracePath(Cell[,] cellDetails, Pair dest) {
		int ROW = cellDetails.GetLength(0);
		int COL = cellDetails.GetLength(1);

		int row = dest.first;
		int col = dest.second;

		Stack<Pair> Path = new Stack<Pair>();

		while (!(cellDetails[row, col].parent_i == row && cellDetails[row, col].parent_j == col)) {
			Path.Push(new Pair(row, col));
			int temp_row = cellDetails[row, col].parent_i;
			int temp_col = cellDetails[row, col].parent_j;
			row = temp_row;
			col = temp_col;
		}

		Path.Push(new Pair(row, col));
		PathList.Clear();
		while (Path.Count > 0) {
			Pair p = Path.Peek();
			Path.Pop();
			PathList.Add(new Vector2Int(p.first, p.second));
		}
	}

}
