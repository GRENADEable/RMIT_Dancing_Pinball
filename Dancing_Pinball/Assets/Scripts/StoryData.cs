using UnityEngine;

[CreateAssetMenu(fileName = "Story_Data")]
public class StoryData : ScriptableObject
{
    #region Public Variables
    [TextArea(5, 5)] public string storyText = default;
    public Sprite storyImage = default;
    #endregion
}