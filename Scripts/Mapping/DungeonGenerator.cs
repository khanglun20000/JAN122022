using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator instance;
    [SerializeField] private Transform roomPlayerIn;
    public Transform RoomPlayerIn
    {
        get { return roomPlayerIn; }
        set
        {
            roomPlayerIn = value;
        }
    }
    //Variables for number of rooms in the dungeon
    [SerializeField] Vector2 rangeNumberOfRooms;
    [SerializeField] int numberOfRooms;
    private int numberOfRoomsLeft;

    //Lists that store roomnode when initialize dungeon
    Dictionary<Vector2, RoomNode> createdRNs = new Dictionary<Vector2, RoomNode>();   //store the created rooms
    Dictionary<Vector2, RoomNode> childRNs = new Dictionary<Vector2, RoomNode>();     //List of 

    [SerializeField] Transform roomRoot;
    [SerializeField] Vector2 roomDistance;
    [SerializeField] int normalRoomProb, mysticRoomProb, supplyRoomProb;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        numberOfRooms = Random.Range((int)rangeNumberOfRooms.x, (int)rangeNumberOfRooms.y + 1);
        numberOfRoomsLeft = numberOfRooms;
        CreateMap();
        CreateRooms();
    }

    //First creat a root room node and then create other room nodes, with method same as Astar
    void CreateMap()
    {
        RoomNode rootRN = new RoomNode();
        rootRN.roomType = RoomType.RootRoom;
        createdRNs.Add(new Vector2(0, 0), rootRN);
        childRNs.Add(new Vector2(0, 0), rootRN);
        numberOfRoomsLeft--;
        while (numberOfRoomsLeft > 0)
        {
            childRNs = new Dictionary<Vector2, RoomNode>(SetRoomDoors(childRNs));
        }
        foreach (KeyValuePair<Vector2, RoomNode> room in createdRNs)
        {
            //Check occupied doors
            CheckOccupiedRoom(room);
        }
        AssignBossRoom();
    }

    void CreateRooms()
    {
        foreach (KeyValuePair<Vector2, RoomNode> rn in createdRNs)
        {
            RoomGenerator newRoom = Instantiate(roomRoot, new Vector2(rn.Key.x * roomDistance.x, rn.Key.y * roomDistance.y), Quaternion.identity, transform).GetComponent<RoomGenerator>();
            newRoom.SetUp(rn.Value.left, rn.Value.right, rn.Value.up, rn.Value.down, rn.Value.roomType);
        }
    }

    void CheckOccupiedRoom(KeyValuePair<Vector2, RoomNode> room)
    {
        room.Value.left = CheckPos(createdRNs, new Vector2(room.Key.x - 1, room.Key.y));
        room.Value.right = CheckPos(createdRNs, new Vector2(room.Key.x + 1, room.Key.y));
        room.Value.up = CheckPos(createdRNs, new Vector2(room.Key.x, room.Key.y + 1));
        room.Value.down = CheckPos(createdRNs, new Vector2(room.Key.x, room.Key.y - 1));
    }

    Dictionary<Vector2, RoomNode> SetRoomDoors(Dictionary<Vector2, RoomNode> childRNs)
    {
        Dictionary<Vector2, RoomNode> childrenRN = new Dictionary<Vector2, RoomNode>();

        foreach (KeyValuePair<Vector2, RoomNode> room in childRNs)
        {
            //Check occupied doors
            CheckOccupiedRoom(room);

            //Number of doors that is not occupied
            int doorsLeft = 4 - (room.Value.left ? 1 : 0) - (room.Value.right ? 1 : 0) - (room.Value.up ? 1 : 0) - (room.Value.down ? 1 : 0);
            //Choose a random number of doors from doorsLeft to assign new door
            int doors = Random.Range(1, doorsLeft);
            //Array of occupied or not directions of doors of the room
            bool[] dir = new bool[4] { room.Value.left, room.Value.right, room.Value.up, room.Value.down };
            //Avoid infinite loop
            int count = 0;

            //Choose random doors number of doors from doorsLeft to get occupied
            while (doors > 0 && numberOfRoomsLeft > 0 && count < 10)
            {
                int ran = Random.Range(0, 4);
                if (dir[ran])
                {
                    count++;
                    continue;
                }
                else
                {
                    dir[ran] = true;
                    numberOfRoomsLeft--;
                    doors--;
                }
            }

            room.Value.left = dir[0];
            room.Value.right = dir[1];
            room.Value.up = dir[2];
            room.Value.down = dir[3];

            if (room.Value.left && !CheckPos(createdRNs, new Vector2(room.Key.x - 1, room.Key.y)))
            {//check left
                CreateNewRN(room.Key.x - 1, room.Key.y, childrenRN);
            }
            if (room.Value.right && !CheckPos(createdRNs, new Vector2(room.Key.x + 1, room.Key.y)))
            {//check right
                CreateNewRN(room.Key.x + 1, room.Key.y, childrenRN);
            }
            if (room.Value.up && !CheckPos(createdRNs, new Vector2(room.Key.x, room.Key.y + 1)))
            {//check up
                CreateNewRN(room.Key.x, room.Key.y + 1, childrenRN);
            }
            if (room.Value.down && !CheckPos(createdRNs, new Vector2(room.Key.x, room.Key.y - 1)))
            {//check down
                CreateNewRN(room.Key.x, room.Key.y - 1, childrenRN);
            }

        }
        return childrenRN;
    }

    bool CheckPos(Dictionary<Vector2, RoomNode> _rnPos, Vector2 _pos)
    {
        foreach (Vector2 rnpos in _rnPos.Keys)
        {
            if (_pos == rnpos)
            {
                return true;
            }
        }
        return false;
    }
    //Create new room node
    void CreateNewRN(float posX, float posY, Dictionary<Vector2, RoomNode> _childrenRN)
    {
        RoomNode newRN = new RoomNode();
        createdRNs.Add(new Vector2(posX, posY), newRN);
        _childrenRN.Add(new Vector2(posX, posY), newRN);

        newRN.roomType = AssignRoomType();
    }

    RoomType AssignRoomType()
    {
        int ran = Random.Range(0, 100);

        if (ran >= 0 && ran < normalRoomProb)
        {
            return RoomType.NormalRoom;
        }
        else if (ran >= normalRoomProb && ran < mysticRoomProb)
        {
            return RoomType.MysticRoom;
        }
        else if (ran >= mysticRoomProb && ran < supplyRoomProb)
        {
            return RoomType.SupplyRoom;
        }
        else
        {
            return RoomType.RewardRoom;
        }
    }

    void AssignBossRoom()
    {
        int ran = Random.Range(0, childRNs.Count);
        childRNs.ElementAt(ran).Value.roomType = RoomType.BossRoom;
    }

    //public void GetNeighborRN(KeyValuePair<Vector2, RoomNode> _currentRN)
    //{
    //    if (createdRNs.ContainsKey(new Vector2(_currentRN.Key.x - 1, _currentRN.Key.y)))
    //    {
    //        _currentRN.Value.leftRN = createdRNs[new Vector2(_currentRN.Key.x - 1, _currentRN.Key.y)];
    //    }
    //    if (createdRNs.ContainsKey(new Vector2(_currentRN.Key.x + 1, _currentRN.Key.y)))
    //    {
    //        _currentRN.Value.rightRN = createdRNs[new Vector2(_currentRN.Key.x + 1, _currentRN.Key.y)];
    //    }
    //    if (createdRNs.ContainsKey(new Vector2(_currentRN.Key.x, _currentRN.Key.y - 1)))
    //    {
    //        _currentRN.Value.downRN = createdRNs[new Vector2(_currentRN.Key.x, _currentRN.Key.y - 1)];
    //    }
    //    if (createdRNs.ContainsKey(new Vector2(_currentRN.Key.x, _currentRN.Key.y + 1)))
    //    {
    //        _currentRN.Value.upRN = createdRNs[new Vector2(_currentRN.Key.x, _currentRN.Key.y + 1)];
    //    }
    //}
}


