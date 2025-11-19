using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(menuName = "Data/IntrusionVideo")]
public class IntrusionVideo : ScriptableObject
{
    public string scenarioName;


    [Header("Opening Video")]
    [Tooltip("The opening video clip for this scenario. Usually played before gameplay or before the storyboard starts.")]
    public VideoClip opening;

    [Header("Storyboard Sequence")]
    [Tooltip("A sequence of storyboard video clips. These will be played in order as part of the narrative.")]
    public VideoClip[] storyBoard;
    [Tooltip("minimal staff required to trigger this intrusion video scenario")]
    public int minimalStaffRequired = 1;


}
