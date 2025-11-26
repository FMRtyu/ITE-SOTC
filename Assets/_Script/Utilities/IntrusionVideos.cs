using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Data/IntrusionVideo")]
public class IntrusionVideo : ScriptableObject
{
    public string scenarioName;

    [Header("Intrusion Icon")]
    [Tooltip("The icon representing this intrusion video scenario.")]
    public Sprite icon;


    [Header("Storyboard Sequence")]
    [Tooltip("The opening video clip for this scenario. Usually played before gameplay or before the storyboard starts.")]
    public VideoClip opening;

    public VideoClip approach;
    public VideoClip solution;
    
    [Tooltip("Correct Staff Indexes, 1 for Arun Rajenoran, 2 for chen hao, 3 for Ananya Rajesh")]
    public int[] correctStaffIndex;  

}
