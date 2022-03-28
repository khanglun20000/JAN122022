using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour
{
    public static MinimapController instance;

    RoomGenerator currentActiveRoomGen;

    [SerializeField] GameObject minimap;

    public RoomGenerator CurrentActiveRoomGen
    {
        get {return currentActiveRoomGen; }
        set
        {
            if(currentActiveRoomGen)
                currentActiveRoomGen.DeactivateMapRep();
            currentActiveRoomGen = value;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void HideMinimap()
    {
        minimap.SetActive(false);
    }

    public void ShowMinimap()
    {
        minimap.SetActive(true);
    }
}
