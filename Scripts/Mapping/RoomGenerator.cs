using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomGenerator : MonoBehaviour
{
    public RoomType roomType;

    public int RoomShapeType;
    [SerializeField] RoomInstance chosenRoomShape;
    [SerializeField] bool left, right, up, down;

    [SerializeField] RoomInstance[] roomIntances;
    [SerializeField] ColorToGameObject[] colorMappings;
    [SerializeField] Texture2D[] doorShapes;
    [SerializeField] Transform doorSpawn;
    [SerializeField] Transform doorWallH, doorWallV;
    [SerializeField] Transform doorH, doorV;
    [SerializeField] Transform detectH, detectV;

    [SerializeField] RoomController RC;
    [SerializeField] Transform myTransform;

    public List<Transform> detects = new List<Transform>();
    public List<Transform> doors = new List<Transform>();

    [SerializeField] Sprite All, U, D, L, R, UD, UL, UR, DL, DR, LR, UDL, UDR, ULR, DLR;
    [SerializeField] Transform mapRepresentationPf;

    SpriteRenderer mapRepRend;
    Transform mapRep;
    Color mapRepColor;
    Color mapRepTmpColor;

    Vector2 roomSizeInTiles = new Vector2(17, 49);
    Vector2 spawnPos;
    // Start is called before the first frame update
    void Start()
    {
        GenerateRoom(RoomShapeSetUp());
        MakeDoors();
        CreateMapRepresentation();

        mapRepColor = mapRepRend.color;
        mapRepTmpColor = mapRepColor;
    }

    public void SetUp(bool _left, bool _right, bool _up, bool _down, RoomType _roomType)
    {
        left = _left;
        right = _right;
        up = _up;
        down = _down;
        roomType = _roomType;
    }

    Texture2D RoomShapeSetUp()
    {
        Texture2D roomShape;

        switch (roomType)
        {
            case RoomType.RootRoom:
                RoomShapeType = 0;
                chosenRoomShape = roomIntances[RoomShapeType];
                roomShape = chosenRoomShape.roomShapes[0];
                break;
            case RoomType.MysticRoom:
                RoomShapeType = UnityEngine.Random.Range(1, 5);
                chosenRoomShape = roomIntances[RoomShapeType];
                roomShape = chosenRoomShape.roomShapes[0];
                break;
            case RoomType.BossRoom:
                RoomShapeType = 1;
                chosenRoomShape = roomIntances[RoomShapeType];
                roomShape = chosenRoomShape.roomShapes[0];
                break;
            case RoomType.RewardRoom:
                RoomShapeType = 0;
                chosenRoomShape = roomIntances[RoomShapeType];
                roomShape = chosenRoomShape.roomShapes[0];
                break;
            case RoomType.SupplyRoom:
                RoomShapeType = 0;
                chosenRoomShape = roomIntances[RoomShapeType];
                roomShape = chosenRoomShape.roomShapes[0];
                break;
            default:
                RoomShapeType = UnityEngine.Random.Range(1, 5);
                chosenRoomShape = roomIntances[RoomShapeType];
                roomShape = chosenRoomShape.roomShapes[UnityEngine.Random.Range(0, chosenRoomShape.roomShapes.Length)];
                break;
        }
        RC = RoomConTrollerFactory.GetRoomType(roomType, gameObject);
        return roomShape;
    }

    Transform CreateContainer(string name)
    {
        GameObject containerGO = new GameObject(name);
        Transform containerTf = containerGO.transform;
        containerTf.SetParent(myTransform);
        containerTf.localPosition = Vector2.zero;
        return containerTf;
    }

    void GenerateRoom(Texture2D roomShape)
    {
        Transform _wallcAndBlocksContainer = CreateContainer("WallsAndBlocksContainer");
        Transform _tilesContainer = CreateContainer("TilesContainer");
        RC.TilesContainer = _tilesContainer;
        for (int x = 0; x < roomShape.width; x++)
        {
            for (int y = 0; y < roomShape.height; y++)
            {
                GenerateTile(x, y, roomShape, _wallcAndBlocksContainer, _tilesContainer);
            }
        }
    }

    void GenerateTile(int x, int y, Texture2D shape, Transform roomTf, Transform tileContainer)
    {
        Color pixelColor = shape.GetPixel(x, y);
        if (pixelColor.a == 0)
        {
            return;
        }

        foreach (ColorToGameObject colorMapping in colorMappings)
        {
            if (colorMapping.color.Equals(pixelColor))
            {
                spawnPos = PositionFromTileGrid(x, y);
                if (colorMapping == colorMappings[0]) // get all ground tile
                {
                    Instantiate(colorMapping.prefab[UnityEngine.Random.Range(0, colorMapping.prefab.Length)], spawnPos, Quaternion.identity, tileContainer);
                }
                else
                {
                    Instantiate(colorMapping.prefab[UnityEngine.Random.Range(0, colorMapping.prefab.Length)], spawnPos, Quaternion.identity, roomTf);
                }
            }
        }
    }

    Vector2 PositionFromTileGrid(int x, int y)
    {
        Vector2 ret;
        int tileSize = 1;
        //find difference between the corner of the texture and the center of this object
        Vector2 offset = new Vector3((-roomSizeInTiles.x + 1) * tileSize, (roomSizeInTiles.y - 1) / 4 * tileSize);
        //find scaled up position at the offset
        ret = new Vector2(tileSize * (float)x, -tileSize * (float)y) + offset + (Vector2)transform.position;
        return ret;
    }

    void MakeDoors()
    {
        //left door
        spawnPos = myTransform.position + Vector3.left * roomSizeInTiles.x - Vector3.left;
        PlaceDoorL(left, spawnPos);
        //right door
        spawnPos = myTransform.position + Vector3.right * roomSizeInTiles.x - Vector3.right;
        PlaceDoorR(right, spawnPos);
        //up door
        spawnPos = myTransform.position + Vector3.up * ((roomSizeInTiles.y - 1) / 4);
        PlaceDoorU(up, spawnPos);
        //down door 
        spawnPos = myTransform.position + Vector3.down * ((roomSizeInTiles.y - 1) / 4);
        PlaceDoorD(down, spawnPos);
    }

    void PlaceDoorL(bool isDoor, Vector2 spawnPos)
    {
        Vector2 offset_1 = Vector2.zero; //offset of door and room
        Vector2 offset_2 = Vector2.zero; //offset of doorWall/detect and room
        int doorType = 0; //choose door index in doorShapes[]
        switch (RoomShapeType)
        {
            case 1:
            case 3:
                doorType = 0;
                offset_1 = new Vector2(-13, -3);
                break;
            case 0:
            case 2:
            case 4:
                doorType = 1;
                offset_1 = new Vector2(-5, -3);
                break;
        }

        switch (RoomShapeType)
        {
            case 1:
            case 3:
                offset_2 = new Vector2(0, 0);
                break;
            case 0:
            case 2:
            case 4:
                offset_2 = new Vector2(8, 0);
                break;
        }

        if (isDoor)
        {
            DoorGenerator dg = doorSpawn.GetComponent<DoorGenerator>();
            dg.tex = doorShapes[doorType];

            Instantiate(doorSpawn, spawnPos + offset_1, Quaternion.identity, myTransform);

            // spawn door
            doors.Add(Instantiate(doorV, spawnPos + offset_2, Quaternion.identity, myTransform));

            //spawn dectects
            detects.Add(Instantiate(detectV, spawnPos + offset_2 + new Vector2(2, 0), Quaternion.identity, myTransform));
        }
        else
        {
            //spawn doorWall
            Instantiate(doorWallV, spawnPos + offset_2, Quaternion.identity, myTransform);
        }
    }

    void PlaceDoorR(bool isDoor, Vector2 spawnPos)
    {
        Vector2 offset_1 = Vector2.zero;
        Vector2 offset_2 = Vector2.zero;
        int doorType = 0;
        switch (RoomShapeType)
        {
            case 1:
            case 3:
                doorType = 0;
                offset_1 = new Vector2(-4, -3);
                break;
            case 0:
            case 2:
            case 4:
                doorType = 1;
                offset_1 = new Vector2(-4, -3);
                break;
        }

        switch (RoomShapeType)
        {
            case 1:
            case 3:
                offset_2 = new Vector2(0, 0);
                break;
            case 0:
            case 2:
            case 4:
                offset_2 = new Vector2(-8, 0);
                break;
        }

        if (isDoor)
        {
            DoorGenerator dg = doorSpawn.GetComponent<DoorGenerator>();
            dg.tex = doorShapes[doorType];

            Instantiate(doorSpawn, spawnPos + offset_1, Quaternion.identity, myTransform);

            // spawn door
            doors.Add(Instantiate(doorV, spawnPos + offset_2, Quaternion.identity, myTransform));

            //spawn dectects
            detects.Add(Instantiate(detectV, spawnPos + offset_2 + new Vector2(-2, 0), Quaternion.identity, myTransform));
        }
        else
        {
            //spawn doorWall
            Instantiate(doorWallV, spawnPos + offset_2, Quaternion.identity, transform);
        }
    }

    void PlaceDoorU(bool isDoor, Vector2 spawnPos)
    {
        Vector2 offset_1 = Vector2.zero;
        Vector2 offset_2 = Vector2.zero;
        int doorType = 0;

        switch (RoomShapeType)
        {
            case 1:
            case 2:
                doorType = 2;
                break;
            case 0:
            case 3:
            case 4:
                doorType = 3;
                break;
        }
        switch (RoomShapeType)
        {
            case 1:
            case 2:
                break;
            case 0:
            case 3:
            case 4:
                offset_2 = new Vector2(0, -4);
                break;
        }

        if (isDoor)
        {
            DoorGenerator dg = doorSpawn.GetComponent<DoorGenerator>();
            dg.tex = doorShapes[doorType];

            Instantiate(doorSpawn, spawnPos + offset_1, Quaternion.identity, transform);

            // spawn door
            doors.Add(Instantiate(doorH, spawnPos + offset_2, Quaternion.identity, transform));

            //spawn dectects
            detects.Add(Instantiate(detectH, spawnPos + offset_2 + new Vector2(0, -3), Quaternion.identity, transform));
        }
        else
        {
            //spawn doorWall
            Instantiate(doorWallH, spawnPos + offset_2, Quaternion.identity ,transform);
        }
    }

    void PlaceDoorD(bool isDoor, Vector2 spawnPos)
    {
        Vector2 offset_1 = Vector2.zero;
        Vector2 offset_2 = Vector2.zero;
        int doorType = 0;

        switch (RoomShapeType)
        {
            case 1:
            case 2:
                doorType = 2;
                offset_1 = new Vector2(0, -7);
                break;
            case 0:
            case 3:
            case 4:
                doorType = 3;
                offset_1 = new Vector2(0, -3);
                break;
        }
        switch (RoomShapeType)
        {
            case 1:
            case 2:
                break;
            case 0:
            case 3:
            case 4:
                offset_2 = new Vector2(0, 4);
                break;
        }

        if (isDoor)
        {
            DoorGenerator dg = doorSpawn.GetComponent<DoorGenerator>();
            dg.tex = doorShapes[doorType];

            Instantiate(doorSpawn, spawnPos + offset_1, Quaternion.identity, transform);

            // spawn door
            doors.Add(Instantiate(doorH, spawnPos + offset_2, Quaternion.identity, transform));

            //spawn dectects
            detects.Add(Instantiate(detectH, spawnPos + offset_2 + new Vector2(0, 3), Quaternion.identity, transform));
        }
        else
        {
            //spawn doorWall
            Instantiate(doorWallH, spawnPos + offset_2, Quaternion.identity, transform);
        }
    }

    Sprite PickSprite()
    {
        if (up)
        {
            if (down)
            {
                if (left)
                {
                    if (right)
                    {
                        return All;
                    }
                    else
                    {
                        return UDL;
                    }
                }
                else
                {
                    if (right)
                    {
                        return UDR;
                    }
                    else
                    {
                        return UD;
                    }
                }
            }
            else
            {
                if (left)
                {
                    if (right)
                    {
                        return ULR;
                    }
                    else
                    {
                        return UL;
                    }
                }
                else
                {
                    if (right)
                    {
                        return UR;
                    }
                    else
                    {
                        return U;
                    }
                }
            }
        }
        else
        {
            if (down)
            {
                if (left)
                {
                    if (right)
                    {
                        return DLR;
                    }
                    else
                    {
                        return DL;
                    }
                }
                else
                {
                    if (right)
                    {
                        return DR;
                    }
                    else
                    {
                        return D;
                    }
                }
            }
            else
            {
                if (left)
                {
                    if (right)
                    {
                        return LR;
                    }
                    else
                    {
                        return L;
                    }
                }
                else
                {
                    if (right)
                    {
                        return R;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }

    void CreateMapRepresentation()
    {
        mapRep = Instantiate(mapRepresentationPf, myTransform.position, Quaternion.identity, myTransform);
        mapRepRend = mapRep.GetComponent<SpriteRenderer>();
        mapRepRend.sprite = PickSprite();
    }

    public void ActivateMapRep()
    {
        mapRepTmpColor.a *= 2;
        mapRepRend.color = mapRepTmpColor;
        MinimapController.instance.CurrentActiveRoomGen = this;
    }

    public void DeactivateMapRep()
    {
        mapRepTmpColor.a /= 2;
        mapRepRend.color = mapRepTmpColor;
    }
}

[Serializable]
public class RoomInstance
{
    public Texture2D[] roomShapes;
}
