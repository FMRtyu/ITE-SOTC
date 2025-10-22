using System.Collections.Generic;
using UnityEngine;

public class MainDashboard : MonoBehaviour
{
    private GameObject[] preHomeUIElements;
    [SerializeField]private GameObject[] dashboardUIElements;
    [SerializeField] private GameObject[] staticDashboardUI;
    //Drags = the different menus we have
    public _MenuState[] allMenus;

    //State-object dictionary to make it easier to activate a menu 
    private Dictionary<PageNavType, _MenuState> menuDictionary = new Dictionary<PageNavType, _MenuState>();

    //The current active menu
    private _MenuState activeState;

    //To easier jump back one step, we can use a stack
    //This was also suggested in the Game Programming Patterns book
    //If so we don't have to hard-code in each state what happens when we jump back one step
    private Stack<PageNavType> stateHistory = new Stack<PageNavType>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void initDashboard()
    {
        foreach (GameObject child in dashboardUIElements)
        {
            child.SetActive(false);
        }

        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    
        //Jump back one step = what happens when we press escape or one of the back buttons
    public void JumpBack()
    {
        //If we have just one item in the stack then, it means we are at the state we set at start, so we have to jump forward
        if (stateHistory.Count <= 1)
        {
            SetActiveState(PageNavType.home);
        }
        else
        {
            //Remove one from the stack
            stateHistory.Pop();

            //Activate the menu that's on the top of the stack
            SetActiveState(stateHistory.Peek(), isJumpingBack: true);
        }
    }

    //Activate a menu
    public void SetActiveState(PageNavType newState, bool isJumpingBack = false)
    {
        //First check if this menu exists
        if (!menuDictionary.ContainsKey(newState))
        {
            Debug.LogWarning($"The key <b>{newState}</b> doesn't exist so you can't activate the menu!");

            return;
        }

        //Deactivate the old state
        if (activeState != null)
        {
            activeState.gameObject.SetActive(false);
        }

        //Activate the new state
        activeState = menuDictionary[newState];

        activeState.gameObject.SetActive(true);

        //If we are jumping back we shouldn't add to history because then we will get doubles
        if (!isJumpingBack)
        {
            stateHistory.Push(newState);
        }
    }
}
