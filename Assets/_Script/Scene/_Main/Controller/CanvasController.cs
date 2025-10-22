using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    [SerializeField] private GameObject LandingPage;
    [SerializeField] private GameObject MainDashboard;
    public void HomeScene()
    {
        MainDashboard.SetActive(true);

        LeanTween.alphaCanvas(LandingPage.GetComponent<CanvasGroup>(), 1f, 0.5f).setOnComplete(() =>
                {
                    LandingPage.SetActive(true);
                    MainDashboard.SetActive(false);
                });
    }
}
