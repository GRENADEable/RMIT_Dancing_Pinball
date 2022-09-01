using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerPinball : MonoBehaviour
{
    #region Serialized Variables
    [SerializeField]
    [Tooltip("Pinball Power Paddle")]
    private Rigidbody ballRg = default;

    [SerializeField]
    [Tooltip("Power Slider")]
    private Slider powerSlider = default;

    [SerializeField]
    [Tooltip("How fast you want the Slider to move?")]
    private float powerSliderSpeed = default;

    [SerializeField]
    [Tooltip("After how much time to move the Ball when Stuck?")]
    private float moveAfterIdleState = default;

    [SerializeField]
    [Tooltip("Idle to move Force")]
    private float moveAfterIdlieForce = default;

    [SerializeField]
    [Tooltip("Popup Text Animator Controller")]
    private Animator popupTextAnim = default;

    [SerializeField]
    [Tooltip("Popup Text Image")]
    private Image popupTextImage = default;

    [SerializeField]
    [Tooltip("Slow Time Value")]
    private float slowTimeVal = default;
    #endregion

    #region Private Variables
    private bool _isCharging = default;
    private bool _isDirUp = default;
    private float _currentIdleDuration = default;
    private bool _isBallShot = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {
        StoryCollision.OnObstacleCollide += OnObstacleCollideEventRecieved;
    }

    void OnDisable()
    {
        StoryCollision.OnObstacleCollide -= OnObstacleCollideEventRecieved;
    }

    void OnDestroy()
    {
        StoryCollision.OnObstacleCollide -= OnObstacleCollideEventRecieved;
    }
    #endregion

    void Start()
    {

    }

    void Update()
    {
        if (!_isBallShot)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                StartCharging();

            if (Input.GetKeyUp(KeyCode.Space))
                StopCharging();
        }

        if (_isCharging)
            SetPowerBall();

        //if (_isBallShot)
        //    CheckIfBallIdle();
    }
    #endregion

    #region My Functions
    /// <summary>
    /// Sets the power of the ball when Spacebar is Held;
    /// </summary>
    void SetPowerBall()
    {
        if (_isDirUp)
        {
            powerSlider.value += Time.deltaTime * powerSliderSpeed;

            if (powerSlider.value >= powerSlider.maxValue)
            {
                _isDirUp = false;
                powerSlider.value = powerSlider.maxValue;
            }
        }
        else
        {
            powerSlider.value -= Time.deltaTime * powerSliderSpeed;

            if (powerSlider.value <= powerSlider.minValue)
            {
                _isDirUp = true;
                powerSlider.value = powerSlider.minValue;
            }
        }
    }

    /// <summary>
    /// Starts charging the bar when spacebar is pressed;
    /// Changes the slider UI to move up and down;
    /// </summary>
    void StartCharging()
    {
        _isCharging = true;
        powerSlider.value = powerSlider.minValue;
        _isDirUp = true;
    }

    /// <summary>
    /// Stops charging the bar when spacebar is lifted;
    /// Sets the Force on the ball;
    /// </summary>
    void StopCharging()
    {
        _isCharging = false;
        //_isBallShot = true;
        ballRg.AddForce(Vector3.up * Mathf.Abs(powerSlider.value), ForceMode.Impulse);
    }

    /// <summary>
    /// Timer check if the ball gets stuck on the obstacle;
    /// </summary>
    void CheckIfBallIdle()
    {
        if (ballRg.IsSleeping())
            _currentIdleDuration += Time.deltaTime;

        if (_currentIdleDuration >= moveAfterIdleState)
        {
            _currentIdleDuration = 0;
            ballRg.AddForce(Vector3.up * moveAfterIdlieForce, ForceMode.Impulse);
        }
    }
    #endregion

    #region Coroutines
    IEnumerator PauseDelay()
    {
        Time.timeScale = slowTimeVal;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
    }
    #endregion

    #region Events
    /// <summary>
    /// Subbed to event StoryCollision;
    /// Stores the string and image variables;
    /// </summary>
    /// <param name="storyTxt"> Story Text from the SO Data; </param>
    /// <param name="image"> Story Image from the SO Data; </param>
    void OnObstacleCollideEventRecieved(string storyTxt, Sprite image)
    {
        popupTextAnim.Play("Popup_Story_Anim");
        StartCoroutine(PauseDelay());
        popupTextImage.sprite = image;
    }
    #endregion
}