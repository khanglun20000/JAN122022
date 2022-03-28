using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] GameObject BuffCurtain;
    [SerializeField] GameObject BuffsHolder;
    [SerializeField] TMP_Text buffNameText;
    [SerializeField] TMP_Text buffDescriptionText;
    [SerializeField] float timeDelay;
    [SerializeField] TMP_Text expCountText;
    [SerializeField] TMP_Text rerollCostText;

    private void Awake()
    {
        if(!instance)
            instance = this;
    }

    public bool isGamePaused;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && UpdateHoldWeapon.instance.IsWeaponPickedUp)
        {
            if (isGamePaused)
            {
                CloseBuffMenu();
            }
            else
            {
                OpenBuffMenu();
            }
        }

        if(Input.GetKeyDown(KeyCode.R) && BuffCurtain.activeInHierarchy)
        {
            PlayerBuffController.instance.GenerateRandomBuff();
        }
    }

    public void WrapDelayResume()
    {
        StartCoroutine(DelayResume());
    }

    IEnumerator DelayResume()
    {
        yield return new WaitForSecondsRealtime(timeDelay);
        Resume();
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isGamePaused = true;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
    }

    public void OpenBuffMenu()
    {
        BuffCurtain.SetActive(true);
        BuffsHolder.SetActive(true);
        Pause();
    }

    public void CloseBuffMenu()
    {
        BuffCurtain.SetActive(false);
        BuffsHolder.SetActive(false);
        DisableBuffNameAndDescrition();
        WrapDelayResume();
    }

    //called in BuffButton
    public void SetBuffNameAndDescription(string _buffName, string _buffDescription)
    {
        buffNameText.gameObject.SetActive(true);
        buffDescriptionText.gameObject.SetActive(true);
        buffNameText.text = _buffName;
        buffDescriptionText.text = _buffDescription;
    }

    public void DisableBuffNameAndDescrition()
    {
        buffNameText.gameObject.SetActive(false);
        buffDescriptionText.gameObject.SetActive(false);
    }

    public void UpdateExpCount(int _amount)
    {
        expCountText.text = _amount.ToString();
    }

    public void SetRerollCostText(int _amount)
    {
        rerollCostText.text = _amount.ToString();
    }
}
