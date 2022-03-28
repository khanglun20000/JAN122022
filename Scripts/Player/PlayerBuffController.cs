using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Reflection;
using System;

public class PlayerBuffController : MonoBehaviour
{
    public Abs_Attack attack;
    public static PlayerBuffController instance;

    [SerializeField] BuffButton[] buffButtons;
    [SerializeField] TMP_Text[] probabilityText;
    [SerializeField] Color[] frameColor;

    public List<BuffName> whiteBuffPool = new List<BuffName>();
    public List<BuffName> blueBuffPool = new List<BuffName>();
    public List<BuffName> yellowBuffPool = new List<BuffName>();
    public List<BuffName> redBuffPool = new List<BuffName>();

    public float whiteProb, blueProb, yellowProb, redProb;
    public int whiteExpCost, blueExpCost, yellowExpCost, redExpCost;

    BuffType currentBuffType;
    List<BuffButton> clickedBuffButtons = new List<BuffButton>();
    public int RerollExpCost;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        //apply buff probability to text
        probabilityText[0].text = whiteProb.ToString() + "%";
        probabilityText[0].color = frameColor[0];
        probabilityText[1].text = blueProb.ToString() + "%";
        probabilityText[1].color = frameColor[1];
        probabilityText[2].text = yellowProb.ToString() + "%";
        probabilityText[2].color = frameColor[2];
        probabilityText[3].text = redProb.ToString() + "%";
        probabilityText[3].color = frameColor[3];

        //generate number of instances of each buff
        GeneratePoolOfType(5, BuffFactory.whiteBuffInstances, whiteBuffPool);
        GeneratePoolOfType(5, BuffFactory.blueBuffInstances, blueBuffPool);
        GeneratePoolOfType(3, BuffFactory.yellowBuffInstances, yellowBuffPool);
        GeneratePoolOfType(1, BuffFactory.redBuffInstances, redBuffPool);
        GenerateRandomBuff();

        UIManager.instance.SetRerollCostText(RerollExpCost);
    }

    public void GenerateRandomBuff()
    {
        EnableBuffButton();
        foreach (BuffButton _buffButton in buffButtons)
        {
            currentBuffType = GetRandomBuffType(
            new RandomSelection(BuffType.White, whiteProb / 100),
            new RandomSelection(BuffType.Blue, blueProb / 100),
            new RandomSelection(BuffType.Yellow, yellowProb / 100),
            new RandomSelection(BuffType.Red, redProb / 100)
            );

            SetUpBuffColor(currentBuffType, _buffButton);
            Buff newBuff = BuffFactory.GetBuffName(GetBuffPoolFromBuffType(currentBuffType)[UnityEngine.Random.Range(0, GetBuffPoolFromBuffType(currentBuffType).Count)]);
            _buffButton.buff = newBuff;
            _buffButton.SetButtonImageAndFrameColor(newBuff.buffData.buffSprite);
        }
    }

    private void GeneratePoolOfType(int _numberObjOfType, Dictionary<BuffName, Buff> _buffDict, List<BuffName> _buffNames)
    {
        foreach(KeyValuePair<BuffName, Buff> buffType in _buffDict)
        {
            for(int i = 0; i < _numberObjOfType; i++)
            {
                _buffNames.Add(buffType.Key);
            }
        }
    }

    void SetUpBuffColor(BuffType _buffType ,BuffButton _buffButton)
    {
        switch (_buffType)
        {
            case BuffType.White:
                _buffButton.frameImage.color = frameColor[0];
                _buffButton.buffExpCost = whiteExpCost;
                break;
            case BuffType.Blue:
                _buffButton.frameImage.color = frameColor[1];
                _buffButton.buffExpCost = blueExpCost;
                break;
            case BuffType.Yellow:
                _buffButton.frameImage.color = frameColor[2];
                _buffButton.buffExpCost = yellowExpCost;
                break;
            case BuffType.Red:
                _buffButton.frameImage.color = frameColor[3];
                _buffButton.buffExpCost = redExpCost;
                break;
        }
    }

    // disable buff if clicked, called in BuffButton
    public void OnCick_BuffButton(BuffButton _buffButton)
    {
        _buffButton.button.interactable = false;
        _buffButton.buffImage.gameObject.SetActive(false);
        _buffButton.frameImage.gameObject.SetActive(false);
        clickedBuffButtons.Add(_buffButton);
        PlayerStatController.instance.GetExpSystem().SpendExp(_buffButton.buffExpCost);
    }

    void EnableBuffButton()
    {
        foreach(BuffButton _BuffButton in clickedBuffButtons)
        {
            _BuffButton.button.interactable = true;
            _BuffButton.buffImage.gameObject.SetActive(true);
            _BuffButton.frameImage.gameObject.SetActive(true);
        }
        clickedBuffButtons.Clear();
    }


    private List<BuffName> GetBuffPoolFromBuffType(BuffType _buffType)
    {
        switch (_buffType)
        {
            case BuffType.White:
                return whiteBuffPool;
            case BuffType.Blue:
                return blueBuffPool;
            case BuffType.Yellow:
                return yellowBuffPool;
            case BuffType.Red:
                return redBuffPool;
            default:
                return null;
        }
    }

    //helper structure
    struct RandomSelection
    {
        public float probability;
        public BuffType buffType;

        public RandomSelection(BuffType _buffType , float _probability)
        {
            probability = _probability;
            buffType = _buffType;
        }

        public BuffType GetBuffType() => buffType;
    }

    BuffType GetRandomBuffType(params RandomSelection[] selections)
    {
        float rand = UnityEngine.Random.value;
        float currentProb = 0;
        foreach (var selection in selections)
        {
            currentProb += selection.probability;
            if (rand <= currentProb)
                return selection.GetBuffType();
        }
        return default;
    }

    public void SpendExpOnClickReroll()
    {
        PlayerStatController.instance.GetExpSystem().SpendExp(RerollExpCost);
    }
}

public static class BuffFactory
{
    public static Dictionary<BuffName, Buff> allBuffInstances = new Dictionary<BuffName, Buff>();
    public static Dictionary<BuffName ,Buff> whiteBuffInstances = new Dictionary<BuffName, Buff>();
    public static Dictionary<BuffName, Buff> blueBuffInstances = new Dictionary<BuffName, Buff>();
    public static Dictionary<BuffName, Buff> yellowBuffInstances = new Dictionary<BuffName, Buff>();
    public static Dictionary<BuffName, Buff> redBuffInstances = new Dictionary<BuffName, Buff>();

    static Type currentType;

    static BuffFactory()
    {
        currentType = typeof(WhiteBuff);
        CreateBuffInstances(whiteBuffInstances);
        currentType = typeof(BlueBuff);
        CreateBuffInstances(blueBuffInstances);
        currentType = typeof(YellowBuff);
        CreateBuffInstances(yellowBuffInstances);
        currentType = typeof(RedBuff);
        CreateBuffInstances(redBuffInstances);
    }

    public static void CreateBuffInstances(Dictionary<BuffName, Buff> dict)
    {
        var typeTypes = Assembly.GetAssembly(currentType).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract
            && myType.IsSubclassOf(currentType));

        foreach (var type in typeTypes)
        {
            Buff tempType = Activator.CreateInstance(type, true) as Buff;
            tempType.GetBuffDataFromPath();
            dict.Add(tempType.BuffName, tempType);
            allBuffInstances.Add(tempType.BuffName, tempType);
        }
    }

    public static Buff GetBuffName(BuffName _roomType)
    {
        if (allBuffInstances.ContainsKey(_roomType))
        {
            return allBuffInstances[_roomType];
        }
        return null;
    }
}