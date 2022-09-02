using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerPinball : MonoBehaviour
{
    #region Serialized Variables

    #region Datas
    [Space, Header("Data")]
    [SerializeField]
    [Tooltip("GameManager Scriptable Object")]
    private GameManagerData gmData = default;

    [SerializeField]
    [Tooltip("Fade panel Animation Component")]
    private Animator fadeBG = default;
    #endregion

    #region Audios
    [Space, Header("Audio")]
    [SerializeField]
    [Tooltip("SFX Audiosource")]
    private AudioSource sfxAud = default;

    [SerializeField]
    [Tooltip("Array of Audio Clip")]
    private AudioClip[] sfxClips = default;
    #endregion

    #region Ball Variables
    [Space, Header("Ball Variables")]
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

    #endregion

    #region Paddle Variables
    [Space, Header("Paddle Variables")]
    [SerializeField]
    [Tooltip("Pinball Stationary Platform")]
    private GameObject pinballStationaryPlatform = default;

    [SerializeField]
    [Tooltip("Pinball Moving Platform")]
    private Animator pinballMovingPlatformAnim = default;
    #endregion

    #region UI Variables
    [Space, Header("Popup Variables")]
    [SerializeField]
    [Tooltip("Popup Text Animator Controller")]
    private Animator popupTextAnim = default;

    [SerializeField]
    [Tooltip("Popup Text Image")]
    private Image popupTextImage = default;

    [SerializeField]
    [Tooltip("Slow Time Value")]
    private float slowTimeVal = default;

    [SerializeField]
    [Tooltip("HUD Panel GameObject")]
    private GameObject hudPanel = default;

    [SerializeField]
    [Tooltip("Fail Panel GameObject")]
    private GameObject failPanel = default;
    #endregion

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

        PlayerBall.OnGameFail += OnGameFailEventReceived;
    }

    void OnDisable()
    {
        StoryCollision.OnObstacleCollide -= OnObstacleCollideEventRecieved;

        PlayerBall.OnGameFail -= OnGameFailEventReceived;
    }

    void OnDestroy()
    {
        StoryCollision.OnObstacleCollide -= OnObstacleCollideEventRecieved;

        PlayerBall.OnGameFail -= OnGameFailEventReceived;
    }
    #endregion

    void Start()
    {
        AudioAccess(0, 0.3f);
        fadeBG.Play("Fade_In");
        gmData.ChangeGameState("Game");
        //StartCoroutine(StartDelay());
    }

    void Update()
    {
        if (gmData.currState == GameManagerData.GameState.Game)
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

            if (_isBallShot)
                CheckIfBallIdle();

            if (Input.GetKeyDown(KeyCode.R))
                StartCoroutine(RestartDelay());
        }
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
        AudioAccess(4, 1);
        _isCharging = true;
        powerSlider.value = powerSlider.minValue;
        _isDirUp = true;
        pinballMovingPlatformAnim.Play("Paddle_Load_Anim");
    }

    /// <summary>
    /// Stops charging the bar when spacebar is lifted;
    /// Sets the Force on the ball;
    /// </summary>
    void StopCharging()
    {
        AudioAccess(3, 1);
        _isCharging = false;
        _isBallShot = true;
        ballRg.AddForce(Vector3.up * Mathf.Abs(powerSlider.value), ForceMode.Impulse);
        pinballMovingPlatformAnim.Play("Paddle_Shoot_Anim");
        pinballStationaryPlatform.tag = "Finish";
    }

    /// <summary>
    /// Plays audio once with the following parameters;
    /// </summary>
    /// <param name="audIndex"> Which audio clip to play?; </param>
    /// <param name="sfxVolume"> Audio volume; </param>
    /// <param name="isUsingPitch"> Optional: Is it using pitch?; </param>
    /// <param name="pitchVolume"> Optioal: Pitch Volume; </param>
    void AudioAccess(int audIndex, float sfxVolume, bool isUsingPitch = default, float pitchVolume = default)
    {
        sfxAud.PlayOneShot(sfxClips[audIndex], sfxVolume);

        if (isUsingPitch)
        {
            sfxAud.pitch = pitchVolume;
            sfxAud.PlayOneShot(sfxClips[audIndex], sfxVolume);
        }
        else
        {
            sfxAud.pitch = 1;
            sfxAud.PlayOneShot(sfxClips[audIndex], sfxVolume);
        }
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
    /// <summary>
    /// Slows down the gmae with a delay;
    /// </summary>
    /// <returns> Float Delay </returns>
    IEnumerator SlowDownDelay()
    {
        Time.timeScale = slowTimeVal;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
    }

    ///// <summary>
    ///// Starts game with a delay;
    ///// </summary>
    ///// <returns> Float Delay </returns>
    //IEnumerator StartDelay()
    //{
    //    gmData.ChangeGameState("Intro");
    //    fadeBG.Play("Fade_In");
    //    yield return new WaitForSeconds(0.5f);
    //    gmData.ChangeGameState("Game");
    //}

    /// <summary>
    /// Restarts game if ball doesn't leave the ramp;
    /// </summary>
    /// <returns> Float Delay </returns>
    IEnumerator FailRestartDelay()
    {
        gmData.ChangeGameState("Outro");
        yield return new WaitForSeconds(3f);
        fadeBG.Play("Fade_Out");
        yield return new WaitForSeconds(0.5f);
        gmData.ChangeLevel(1);
    }

    IEnumerator RestartDelay()
    {
        gmData.ChangeGameState("Outro");
        fadeBG.Play("Fade_Out");
        yield return new WaitForSeconds(0.5f);
        gmData.ChangeLevel(1);
    }
    #endregion

    #region Events
    /// <summary>
    /// Subbed to event StoryCollision;
    /// Stores the string and image variables;
    /// Plays the collision audio with pitch variation;
    /// </summary>
    /// <param name="storyTxt"> Story Text from the SO Data; </param>
    /// <param name="image"> Story Image from the SO Data; </param>
    void OnObstacleCollideEventRecieved(string storyTxt, Sprite image)
    {
        popupTextAnim.Play("Popup_Story_Anim");
        StartCoroutine(SlowDownDelay());
        popupTextImage.sprite = image;

        int clipIndex = Random.Range(1, 2);
        float pitchIndex = Random.Range(0.8f, 1.2f);
        AudioAccess(clipIndex, 1, true, pitchIndex);
    }

    /// <summary>
    /// Subbed to event PlayerBall;
    /// Restarts the Game with a delay;
    /// </summary>
    void OnGameFailEventReceived()
    {
        hudPanel.SetActive(false);
        failPanel.SetActive(true);

        StartCoroutine(FailRestartDelay());
    }
    #endregion
}