using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryCollision : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Story Data ")]
    private StoryData storyDatas = default;

    public delegate void SendEventsStory(string storyTxt, Sprite image);
    /// <summary>
    /// Sends Event from StoryCollision to GameManagerPinball Scripts;
    /// Sends the data of the image and text to the Manager;
    /// </summary>
    public static event SendEventsStory OnObstacleCollide = default;
    #endregion

    #region Private Variables
    private bool _isCollided;
    #endregion

    #region Unity Callbacks
    void Start()
    {

    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player") && !_isCollided)
        {
            OnObstacleCollide?.Invoke(storyDatas.storyText, storyDatas.storyImage);
            _isCollided = true;
        }
    }
    #endregion
}