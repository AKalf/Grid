using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public List<Room> AvailableRooms = new List<Room>();
    public List<GameObject> RoomsOnMap = new List<GameObject>();
    public Vector2Int MapSize = new Vector2Int(100, 100);
    public int NumberOfRoomsToBuild = 2;

    [ContextMenu("Build")]
    public void BuildMap() {
        Debug.LogError("Building");
        this.transform.position = new Vector3Int((int)transform.position.x, (int)transform.position.y, (int)transform.position.z);
        if (RoomsOnMap.Count > 0) foreach (GameObject r in RoomsOnMap) { if (Application.isPlaying) MonoBehaviour.Destroy(r); else MonoBehaviour.DestroyImmediate(r); }
        RoomsOnMap.Clear();
        for (int i = 0; i < NumberOfRoomsToBuild; i++) {
            Room roomToBuild = AvailableRooms[Random.Range(0, AvailableRooms.Count - 1)];
            Vector2Int? positionToBuild = new Vector2Int((int)transform.position.x, (int)transform.position.y);
            Debug.Log("Room count: " + RoomsOnMap.Count);
            if (RoomsOnMap.Count > 0) positionToBuild = FindPositionForNewRoom(roomToBuild);
            if (positionToBuild != null && i > 0) {
                Debug.Log("Found valid position for new room: " + positionToBuild);
                GameObject newG = Instantiate(roomToBuild.gameObject);
                newG.transform.parent = transform;
                newG.transform.position = (Vector3Int)positionToBuild;
                RoomsOnMap.Add(newG);
            }
        }
    }

    private Vector2Int? FindPositionForNewRoom(Room roomToBuild) {
        for (int x = (int)transform.position.x - MapSize.x / 2; x < (int)transform.position.x + MapSize.x / 2; x++) {
            for (int y = (int)transform.position.y - MapSize.y / 2; y < (int)transform.position.y + MapSize.y / 2; y++) {
                bool isOccupied = false;
                foreach (GameObject existingRoom in RoomsOnMap) {
                    Room existingRoomComp = existingRoom.GetComponent<Room>();
                    Vector3 existingRoomPos = existingRoom.transform.position;
                    Debug.Log("Checking: " + x + ", " + y + " for room: " + existingRoomComp.transform.position);
                    if (
                       x < existingRoomPos.x && x + roomToBuild.WorldSize.x < existingRoomPos.x
                    || (y < existingRoomComp.transform.position.y && y + roomToBuild.WorldSize.y < existingRoomPos.y)
                    || x > existingRoomComp.transform.position.x + existingRoomComp.WorldSize.x || y > existingRoomComp.transform.position.y + existingRoomComp.WorldSize.y) {
                        continue;
                    }
                    else isOccupied = transform;
                }
                if (isOccupied == false) return new Vector2Int(x, y);
            }
        }
        return null;
    }
}
