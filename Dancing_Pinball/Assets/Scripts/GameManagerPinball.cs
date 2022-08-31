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
    #endregion

    #region Private Variables
    private bool _isCharging = default;
    private bool _isDirUp = default;
    //private bool _canCharge = default;
    #endregion

    #region Unity Callbacks

    #region Events
    void OnEnable()
    {

    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {

    }
    #endregion

    void Start()
    {

    }

    void Update()
    {
        //if (_canCharge)
        //{
        if (Input.GetKeyDown(KeyCode.Space))
            StartCharging();

        if (Input.GetKeyUp(KeyCode.Space))
            StopCharging();
        //}

        if (_isCharging)
            SetPowerBall();
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
        ballRg.AddForce(Vector3.up * Mathf.Abs(powerSlider.value), ForceMode.Impulse);
    }
    #endregion

    #region Coroutines

    #endregion

    #region Events

    #endregion
}