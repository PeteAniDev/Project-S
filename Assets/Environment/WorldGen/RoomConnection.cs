using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class RoomConnection {

	public RoomData room1;
	public RoomData room2;

	public RoomConnection(RoomData room1, RoomData room2) {
		this.room1 = room1;
		this.room2 = room2;
	}

	public RoomData GetOther(RoomData room) {
		if (room == room1) {
			return room2;
		}
		if (room == room2) {
			return room1;
		}
		return null;
	}

}
