using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class LegacyRoomConnection {

	public LegacyRoomData room1;
	public LegacyRoomData room2;

	public LegacyRoomConnection(LegacyRoomData room1, LegacyRoomData room2) {
		this.room1 = room1;
		this.room2 = room2;
	}

	public LegacyRoomData GetOther(LegacyRoomData room) {
		if (room == room1) {
			return room2;
		}
		if (room == room2) {
			return room1;
		}
		return null;
	}

}
