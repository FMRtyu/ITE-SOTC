using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{

    [SerializeField] private GameObject LandingPage;
    [SerializeField] private GameObject MainDashboard;

    public MainController mainController { get; private set; }

    void Start()
    {
        LandingPage.SetActive(true);
        MainDashboard.SetActive(false);
    }

    public void HomeScene()
    {
        LeanTween.alphaCanvas(LandingPage.GetComponent<CanvasGroup>(), 1f, 0.5f).setOnComplete(() =>
                {
                    LandingPage.SetActive(false);
                    MainDashboard.SetActive(true);
                });
    }
}
