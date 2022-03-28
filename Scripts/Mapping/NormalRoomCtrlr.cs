using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class NormalRoomController : RoomController
{
    public override RoomType RoomType => RoomType.NormalRoom;

    Transform appearingFrame;
    Transform[] monsterPfs;
    [SerializeField] int monsCount;
    [SerializeField] int maxMonNumPerWave;
    int ExpGain;

    int waveNumChance;
    [SerializeField] int waveNum;
    [SerializeField] Transform[] monSpawners;
    public delegate void OnSpawnMon(Transform transform);
    public OnSpawnMon MonSpawned;

    Transform newMon;

    private void Awake()
    {
        appearingFrame = Resources.Load<Transform>("Prefabs/Others/AppearingFrame");
        monsterPfs = Resources.LoadAll<Transform>("Prefabs/Enemy/Creeps");
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        MonSpawned += CreateMonster;

        waveNumChance = UnityEngine.Random.Range(1, 100);

        if(waveNumChance > 90)
        {
            waveNum = 2;
        }
        else if(waveNumChance > 75)
        {
            waveNum = 1;
        }
        else
        {
            waveNum = 0;
        }
    }


    void CreateMonster(Transform monSpawnerPos)
    {
        newMon = Instantiate(monsterPfs[UnityEngine.Random.Range(0, monsterPfs.Length)], monSpawnerPos.position, Quaternion.identity, transform);
        newMon.GetComponent<EnemyBehaviour>().GetHealthSystem().HealthOut += OnMonDead;
    }

    void OnMonDead()
    {
        monsCount -= 1;
        if(monsCount <= 0 && waveNum > 0)
        {
            waveNum -= 1;
            StartSummonMonsters();
        }
        else if(monsCount <=0 && waveNum <= 0)
        {
            RoomDeactivated();
            RoomCleared();
        }
    }

    void StartSummonMonsters()
    {
        maxMonNumPerWave = DetermineMonNum();
        LocateMonSpawners(maxMonNumPerWave);
        for(int i = 0; i < maxMonNumPerWave; i++)
        {
            Transform newAppearingFrame = Instantiate(appearingFrame, monSpawners[i].transform.position, Quaternion.identity, transform);
            newAppearingFrame.GetComponent<AppearingFrame>().RC = this;
            monsCount++;
        }
    }

    void LocateMonSpawners(int _maxMonSum)
    {
        monSpawners = new Transform[_maxMonSum];

        HashSet<int> _AlreadyChosenTile = new HashSet<int>();

        for (int _MonNum = 0; _MonNum < maxMonNumPerWave; _MonNum++)
        {
            int _RandTileIndex;
            Vector2 _TilePos;
            do
            {
                _RandTileIndex = UnityEngine.Random.Range(0, TilesContainer.childCount);
                _TilePos = TilesContainer.GetChild(_RandTileIndex).transform.localPosition;

            }
            while (_AlreadyChosenTile.Contains(_RandTileIndex) || _TilePos.x % 7 == 0 || _TilePos.x % 15 == 0 || _TilePos.y % 7 == 0 || _TilePos.y % 11 == 0);
            monSpawners[_MonNum] = TilesContainer.GetChild(_RandTileIndex);
        }
    }

    int DetermineMonNum()
    {
        int _MonNum;
        int minMonNum, maxMonNum;
        switch (RG.RoomShapeType)
        {
            default:
                minMonNum = 0;
                maxMonNum = 0;
                waveNum = 0;
                break;
            case 1:
                minMonNum = 10;
                maxMonNum = 15;
                break;
            case 2:
            case 3:
                minMonNum = 7;
                maxMonNum = 10;
                break;
            case 4:
                minMonNum = 4;
                maxMonNum = 7;
                break;
        }
        _MonNum = UnityEngine.Random.Range(minMonNum, maxMonNum);
        return _MonNum;
    }

    public override void StartRoomEvent()
    {
        StartSummonMonsters();
    }

    public override void ClearRoom()
    {
        PlayerStatController.instance.GetExpSystem().GainExp(ExpGain);
    }
}

public class RootRoomController : RoomController
{
    [SerializeField] GameObject[] StartingWeapons;

    public override RoomType RoomType => RoomType.RootRoom;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        RG.ActivateMapRep();
        GetComponent<RoomController>().RoomDeactivated += DestroyOtherStartingWeapons;
        StartingWeapons = GameObject.FindGameObjectsWithTag("StartingWeapon");
        RootRoomActivation();
        foreach (Transform _Detect in Detects)
        {
            _Detect.GetComponent<DectectPlayerEnterRoom>().SetCanTrigger(false);
        }
    }

    void RootRoomActivation()
    {
        List<Transform> doors = GetComponent<RoomController>().Doors;
        if (GetComponent<RoomGenerator>().roomType == RoomType.RootRoom)
        {
            foreach (Transform door in doors)
            {
                door.gameObject.SetActive(true);
            }
            DungeonGenerator.instance.RoomPlayerIn = transform;
        }
    }

    void DestroyOtherStartingWeapons()
    {
        foreach (GameObject weapon in StartingWeapons)
        {
            Destroy(weapon);
        }
        GetComponent<RoomController>().RoomDeactivated -= DestroyOtherStartingWeapons;
    }

    public override void StartRoomEvent()
    {
        return;
    }

    public override void ClearRoom()
    {

    }
}

public class MysticRoomCtrlr : RoomController
{
    [SerializeField] MysticType mysticType;

    public override RoomType RoomType => RoomType.MysticRoom;

    public override void StartRoomEvent()
    {
        mysticType = (MysticType)UnityEngine.Random.Range(0, Enum.GetNames(typeof(MysticType)).Length);
        MysticEventFactory.GetMysticEvent(mysticType, gameObject).StartMysticEvent();
    }

    public override void ClearRoom()
    {

    }
}

public class BossRoomCtrlr : RoomController
{
    public override RoomType RoomType => RoomType.BossRoom;
    int ExpGain;

    Transform BossPb;

    protected override void Start()
    {
        base.Start();
        BossPb = Resources.Load<Transform>("Prefabs/Enemy/Bosses/DarkPriest");
    }

    public override void StartRoomEvent()
    {
        SpawnBoss();
    }

    void SpawnBoss()
    {
        Transform newBoss = Instantiate(BossPb, myTransform.position + new Vector3(0, 2f), Quaternion.identity, myTransform);
        newBoss.GetComponent<BossBehaviour>().rootPos = myTransform.position;
    }

    public override void ClearRoom()
    {
        PlayerStatController.instance.GetExpSystem().GainExp(ExpGain);
    }
}

public class RewardRoomCtrlr : RoomController
{
    public override RoomType RoomType => RoomType.RewardRoom;

    public override void StartRoomEvent()
    {
        return;
    }

    public override void ClearRoom()
    {

    }
}

public class SupplyRoomCtrlr : RoomController
{
    public override RoomType RoomType => RoomType.SupplyRoom;

    public override void StartRoomEvent()
    {
        return;
    }

    public override void ClearRoom()
    {

    }
}
