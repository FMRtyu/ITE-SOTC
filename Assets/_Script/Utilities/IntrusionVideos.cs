using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Data/IntrusionVideo")]
public class IntrusionVideo : ScriptableObject
{
    public string scenarioName;

    [Header("Video Module")]
    [Tooltip("Folder name inside persistentDataPath/videos (ex: module1, module2)")]
    public string moduleFolder;

    [Header("Intrusion Icon")]
    [Tooltip("The icon representing this intrusion video scenario.")]
    public Sprite icon;


    [Header("Storyboard Video (file name only)")]
    [Tooltip("File name only, without extension")]
    public string openingURL; // runtime path
    public string approachURL;
    public string solutionURL;
    
    [Tooltip("Correct Staff Indexes, 1 for Arun Rajenoran, 2 for chen hao, 3 for Ananya Rajesh")]
    public int[] correctStaffIndex;  

}
