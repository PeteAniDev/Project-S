using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public enum RoomDirection {
	NorthEast,
	SouthEast,
	SouthWest,
	NorthWest
}

public class RoomData {

	public RoomData roomNE = null;
	public RoomData roomSE = null;
	public RoomData roomSW = null;
	public RoomData roomNW = null;

	public bool Connect(RoomData room, RoomDirection dir) {
		if (GetRoom(dir) != null || room.GetOpposite(dir) != null) {
			return false;
		}
		switch (dir) {
			case RoomDirection.NorthEast:
				roomNE = room;
				room.roomSW = this;
				break;
			case RoomDirection.SouthEast:
				roomSE = room;
				room.roomNW = this;
				break;
			case RoomDirection.SouthWest:
				roomSW = room;
				room.roomNE = this;
				break;
			case RoomDirection.NorthWest:
				roomNW = room;
				room.roomSE = this;
				break;
		}
		return true;
	}

	public void Disconnect(RoomDirection dir) {
		RoomData room = GetRoom(dir);
		if (room == null) {
			return;
		}
		switch (dir) {
			case RoomDirection.NorthEast:
				roomNE = null;
				room.roomSW = null;
				break;
			case RoomDirection.SouthEast:
				roomSE = null;
				room.roomNW = null;
				break;
			case RoomDirection.SouthWest:
				roomSW = null;
				room.roomNE = null;
				break;
			case RoomDirection.NorthWest:
				roomNW = null;
				room.roomSE = null;
				break;
		}
	}

	public RoomData GetRoom(RoomDirection dir) {
		switch (dir) {
			case RoomDirection.NorthEast:
				return roomNE;
			case RoomDirection.SouthEast:
				return roomSE;
			case RoomDirection.SouthWest:
				return roomSW;
			case RoomDirection.NorthWest:
				return roomNW;
			default:
				return null;
		}
	}

	public RoomData GetOpposite(RoomDirection dir) {
		switch (dir) {
			case RoomDirection.NorthEast:
				return roomSW;
			case RoomDirection.SouthEast:
				return roomNW;
			case RoomDirection.SouthWest:
				return roomNE;
			case RoomDirection.NorthWest:
				return roomSE;
			default:
				return null;
		}
	}

}
