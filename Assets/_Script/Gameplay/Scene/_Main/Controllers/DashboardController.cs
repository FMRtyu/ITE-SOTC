using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DashboardController : MonoBehaviour
{
    //Drags = the different menus we have
    public _MenuState[] allMenus;

    //State-object dictionary to make it easier to activate a menu 
    private Dictionary<MenuState, _MenuState> menuDictionary = new Dictionary<MenuState, _MenuState>();

    //The current active menu
    public _MenuState activeState;

    //To easier jump back one step, we can use a stack
    //This was also suggested in the Game Programming Patterns book
    //If so we don't have to hard-code in each state what happens when we jump back one step
    private Stack<MenuState> stateHistory = new Stack<MenuState>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Image backgroundIMG;
    [SerializeField] private UIManager uiManager;

    public MenuState currentMenuState = MenuState.Home;

    [SerializeField] private GameObject[] referenceObjects;

    void Start()
    {
        initDashboard();
    }

    private void initDashboard()
    {

        foreach (GameObject obj in referenceObjects)
        {
            obj.SetActive(false);
        }
        //Get reference to UIManager
        uiManager = GameManager.Instance.uIManager;
        uiManager.InitDashboardData(this);

        //Put all menus into a dictionary
        foreach (_MenuState menu in allMenus)
        {
            if (menu == null)
            {
                continue;
            }

            //Inject a reference to this script into all menus
            menu.InitState(dashboardController: this, uiManager: uiManager);

            //Check if this key already exists, because it means we have forgotten to give a menu its unique key
            if (menuDictionary.ContainsKey(menu.state))
            {
                Debug.LogWarning($"The key <b>{menu.state}</b> already exists in the menu dictionary!");

                continue;
            }

            menuDictionary.Add(menu.state, menu);
        }

        //Deactivate all menus
        foreach (MenuState state in menuDictionary.Keys)
        {
            menuDictionary[state].gameObject.SetActive(false);
        }

        //Activate the default menu
        SetActiveState(MenuState.Home);
        if (uiManager.skipLanding)
        {
            uiManager.ShowDashboard();
            ShowAllHomeChildren();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void showCampusEventPanelOnly()
    {
        Home homeMenu = menuDictionary[MenuState.Home] as Home;
        homeMenu.SetCampusEventData();
    }

    public bool CheckScenarioInProgress()
    {
        VirtualPatrol homeMenu = menuDictionary[MenuState.VirtualPatrol] as VirtualPatrol;

        if (currentMenuState == MenuState.VirtualPatrol && homeMenu.intrusionInProgress)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ShowAllHomeChildren()
    {
        Home homeMenu = menuDictionary[MenuState.Home] as Home;
        homeMenu.ShowChildren();
    }

    #region Menu State Controls

    //public void ChangeMenuS

    //Jump back one step = what happens when we press escape or one of the back buttons
    public void JumpBack()
    {
        //If we have just one item in the stack then, it means we are at the state we set at start, so we have to jump forward
        if (stateHistory.Count <= 1)
        {
            SetActiveState(MenuState.Home);
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
    public void SetActiveState(MenuState newState, bool isJumpingBack = false)
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

        currentMenuState = newState;
    }
    #endregion

}
