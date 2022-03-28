using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrambleBallEvent : MysticEvent
{
    [SerializeField] Transform scrambleBallPf;
    [SerializeField] int numberOfBalls;
    Transform[] spawners;
    [SerializeField] List<Transform> balls = new List<Transform>();

    [SerializeField] Transform timer;
    [SerializeField] TMP_Text timerDisplay;
    [SerializeField] int secondLeft = 0;
    [SerializeField] bool takingAway = false;
    [SerializeField] bool canCount = false;

    public override MysticType MysticType => MysticType.ScrambleBall;

    public override void StartMysticEvent()
    {
        for (int i = 0; i < numberOfBalls; i++)
        {
            balls.Add(Instantiate(scrambleBallPf, spawners[i].position , Quaternion.identity, transform));
        }
        SetUpTimer();
        Invoke(nameof(InvokeTimer), 3f);
    }

    private void Awake()
    {
        if(GetComponent<RoomGenerator>() != null)
        {
            timerDisplay = Resources.Load<Transform>("Prefabs/MysticRoom/TimerDisplay").GetComponent<TMP_Text>();
            scrambleBallPf = Resources.Load<Transform>("Prefabs/MysticRoom/ScrambleBall");
            switch (GetComponent<RoomGenerator>().RoomShapeType)
            {
                case 1:
                    numberOfBalls = 12;
                    break;
                case 2:
                case 3:
                    numberOfBalls = 6;
                    break;
                case 4:
                    numberOfBalls = 3;
                    break;
            }
            LocateSpawners(numberOfBalls);
        }
    }

    private void Update()
    {
        if(!takingAway && secondLeft > 0 && canCount)
        {
            StartCoroutine(TimerTake());
        }

        if(secondLeft <= 0 && canCount)
        {
            GetComponent<RoomController>().RoomDeactivated();
            foreach(Transform _Ball in balls)
            {
               _Ball.GetComponent<ScrambleBallCtrlr>().StopEvent?.Invoke();
            }
            Destroy(timer.gameObject);
            Destroy(this);
        }
    }

    void InvokeTimer()
    {
        canCount = true;
    }

    void SetUpTimer()
    {
        secondLeft = 20;
        timer = Instantiate(timerDisplay).transform;
        timer.SetParent(UIManager.instance.transform, false);
        timer.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2, Screen.height * 4 / 5);
        timerDisplay = timer.GetComponent<TMP_Text>();
        timerDisplay.text = secondLeft.ToString();
    }

    IEnumerator TimerTake()
    {
        takingAway = true;
        yield return new WaitForSeconds(1);
        secondLeft -= 1;
        timerDisplay.text = secondLeft.ToString();
        takingAway = false;
    }

    void LocateSpawners (int _maxBallNum)
    {
        spawners = new Transform[_maxBallNum];
        Transform _TilesContainer = GetComponent<RoomController>().TilesContainer;
        HashSet<int> _AlreadyChosenTile = new HashSet<int>();

        for (int _BallNum = 0; _BallNum < _maxBallNum; _BallNum++)
        {
            int _RandTileIndex;

            do
            {
                _RandTileIndex = UnityEngine.Random.Range(0, _TilesContainer.childCount);
            }
            while (_AlreadyChosenTile.Contains(_RandTileIndex));


            _AlreadyChosenTile.Add(_RandTileIndex);
            spawners[_BallNum] = _TilesContainer.GetChild(_RandTileIndex);
        }
    }
}

