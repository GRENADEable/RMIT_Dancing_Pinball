using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    #region Public Variables
    public delegate void SendEvents();
    /// <summary>
    /// Event sent from PlayerBall to GameManagerPinball Scripts;
    /// Restarts the Game;
    /// </summary>
    public static event SendEvents OnGameFail;
    #endregion

    #region Unity Callbacks
    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Finish"))
            OnGameFail?.Invoke();
    }
    #endregion
}