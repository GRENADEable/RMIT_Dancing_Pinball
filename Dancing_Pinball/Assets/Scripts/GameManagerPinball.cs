using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
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
    #endregion

    #region Private Variables
    //[SerializeField] private float _ballPower = default;
    private bool _isCharging = default;
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
        SetPowerBall();
    }
    #endregion

    #region My Functions
    /// <summary>
    /// Sets the power of the ball when Spacebar is Held;
    /// </summary>
    void SetPowerBall()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            _isCharging = true;

        if (Input.GetKeyUp(KeyCode.Space))
            _isCharging = false;

        if (_isCharging)
        {
            powerSlider.value++;

            if (powerSlider.value >= powerSlider.maxValue)
                _isCharging = false;
        }
        else
        {
            powerSlider.value--;

            if (powerSlider.value <= powerSlider.minValue)
                _isCharging = true;
        }

    }
    #endregion

    #region Coroutines

    #endregion

    #region Events

    #endregion
}