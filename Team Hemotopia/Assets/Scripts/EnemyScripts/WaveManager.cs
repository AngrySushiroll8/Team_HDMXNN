using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;

    public Room[] rooms;
    public int currentRoom;

    GameObject roomContainer;

    void Awake()
    {
        instance = this;
        roomContainer = GameObject.Find("RoomContainer");

        rooms = new Room[roomContainer.transform.childCount];
        for (int roomIndex = 0; roomIndex < rooms.Length; roomIndex++)
        {
            rooms[roomIndex] = roomContainer.transform.GetChild(roomIndex).GetComponent<Room>();
        }

        if (rooms.Length > 0) currentRoom = 0;
        else currentRoom = -1;
    }

    public bool StartRoom(int roomIndex)
    {
        if (roomIndex >= rooms.Length) return false;

        rooms[roomIndex].StartWave(rooms[roomIndex].waveNumber - 1);

        return true;
    }
}
