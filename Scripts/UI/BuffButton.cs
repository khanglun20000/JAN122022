using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuffButton : MonoBehaviour, IPointerEnterHandler
{
    public Button button;
    public Image frameImage;
    public Image buffImage;

    public int buffExpCost;
    public Buff buff;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        buff.ApplyBuff();
        PlayerBuffController.instance.OnCick_BuffButton(this);
    }

    public void SetButtonImageAndFrameColor(Sprite _sprite)
    {
        buffImage.sprite = _sprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.instance.SetBuffNameAndDescription(buff.buffData.buffName, buff.buffData.description);
    }
}
