using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PageNav : MonoBehaviour
{
    public GameObject[] Pages;
    public Button[] navButtons;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        OpenPage(2);
        activatedButton(navButtons[0]);
    }

    public void OpenPage(int pageIndex)
    {
        for (int i = 0; i < Pages.Length; i++)
        {
            Pages[i].SetActive(false);
        }

        for (int i = 0; i < navButtons.Length; i++)
        {
            deactivatedButton(navButtons[i]);
        }
        switch ((PageNavType)pageIndex)
        {
            case PageNavType.smartBuilding:
                Pages[0].SetActive(true);
                break;
            case PageNavType.sustainableMetrics:
                Pages[1].SetActive(true);
                break;
            case PageNavType.home:
                Pages[2].SetActive(true);
                break;
            case PageNavType.gpsTracking:
                Pages[3].SetActive(true);
                break;
            case PageNavType.alarmMonitoring:
                Pages[4].SetActive(true);
                break;
            case PageNavType.virtualPatrol:
                Pages[5].SetActive(true);
                break;
            default:
                break;
        }
    }

    public void activatedButton(Button button)
    {
        button.interactable = false;
    }

    public void deactivatedButton(Button button)
    {
        button.interactable = true;
    }
}
