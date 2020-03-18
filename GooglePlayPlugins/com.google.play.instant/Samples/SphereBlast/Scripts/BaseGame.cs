// Copyright 2018 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     https://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using Google.Play.Instant;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BaseGame : MonoBehaviour
{
    public GameObject Crate;
    public Camera ThirdPersonCamera;
    public Camera OverheadCamera;
    private const int MAX_SCORE_PER_LEVEL = 5;
    private int _maxTime = 10;
    private int _timer;
    private int _level;
    private int _score;
    private float _deltaTimeSum;
    private GameObject _gameOver;
    private GameObject _sphere;
    private GameObject _installButtonPersistent;
    private GameObject _installButton;
    private Text _timeLeftText;
    private Text _levelText;
    private Text _scoreText;

    private readonly int[] _levelTimeLimitMapping = new int[]
    {
        10,
        9,
        8,
        7,
        6,
        5,
        4
    };

    private static readonly Vector3[] _positions = new Vector3[]
    {
        new Vector3(37, 3.87f, 11),
        new Vector3(37, 3.87f, -4),
        new Vector3(37, 3.87f, -16),
        new Vector3(0, 3.87f, -16),
        new Vector3(0, 3.87f, -4),
        new Vector3(0, 3.87f, 11),
        new Vector3(-23, 3.87f, 11),
        new Vector3(-23, 3.87f, -4),
        new Vector3(-23, 3.87f, -16)
    };

    // Use this for initialization
    public void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        
        _timer = _maxTime;
        _gameOver = GameObject.Find("GameOverPanel").gameObject;
        _timeLeftText = GameObject.Find("TimeLeftText").GetComponent<Text>();
        _levelText = GameObject.Find("LevelText").GetComponent<Text>();
        _scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
        _sphere = GameObject.Find("Sphere");
        _installButton = GameObject.Find("InstallButton");
        _installButtonPersistent = GameObject.Find("InstallButtonPersistent");
        _installButton.SetActive(false);
        _installButtonPersistent.SetActive(false);
        SetUpGameState();
        UpdateTimer();
        ReadGameStateFromCookie();
        SetLevel(_level);
        SetScore(_score);
        HideGameOver();
        CreateCrate();
    }

    private void SetUpGameState()
    {
        _level = 1;
        _deltaTimeSum = 0;
        _score = 0;
#if PLAY_INSTANT
        _installButton.SetActive(true);
        _installButtonPersistent.SetActive(true);
#endif
    }

    private void FixedUpdate()
    {
        if (_timer == -1)
        {
            return;
        }

        _deltaTimeSum += Time.deltaTime;
        if ((int) _deltaTimeSum == 1)
        {
            _timer--;
            _deltaTimeSum = 0;
        }

        UpdateTimer();

        if (_timer > 0) return;
        Destroy(_sphere);
        ShowGameOver();
    }

    /* TIMER METHODS */

    public void ResetTimer()
    {
        if (_timer > 0)
        {
            _timer = _maxTime;
        }
    }

    private void UpdateTimer()
    {
        _timeLeftText.text = Math.Max(_timer, 0).ToString();
    }

    /* SCENE SET UP METHODS */

    public void CreateCrate()
    {
        var randomPosition = Random.Range(0, 9);
        Debug.Log("Crate: " + Crate);
        Instantiate(Crate, _positions[randomPosition], Quaternion.identity);
    }

    private static void WriteGameStateToCookie(int level, int score)
    {
#if !UNITY_EDITOR
        var gameStateCsv = level + "," + score;
        CookieApi.SetInstantAppCookie(gameStateCsv);
#endif
    }

    private void ReadGameStateFromCookie()
    {
#if !UNITY_EDITOR
        var results = CookieApi.GetInstantAppCookie();


        Debug.Log("readGameStateFromCookie: " + results);
        if (string.IsNullOrEmpty(results)) return;
        var attributes = results.Split(',');
        if (attributes.Length < 2)
        {
            return;
        }

        var cookieLevel = attributes[0];
        var cookieScore = attributes[1];
        _level = int.Parse(cookieLevel);
        _score = int.Parse(cookieScore);
        Debug.Log("CookieLevel: " + cookieLevel);
        Debug.Log("CookieScore: " + cookieScore);
#endif
    }

    /* LEVEL METHODS */

    private void IncrementLevel()
    {
        SetLevel(_level + 1);
    }

    private void SetLevel(int newLevel)
    {
        _level = newLevel;
        _maxTime = _levelTimeLimitMapping[_level - 1];
        _timer = _maxTime;
        _levelText.text = _level.ToString();
    }

    public int GetLevel()
    {
        return _level;
    }

    /* UI METHODS */

    public void ShowGameOver()
    {
        _gameOver.SetActive(true);
    }

    private void HideGameOver()
    {
        _gameOver.SetActive(false);
    }

    public void UpdateScoreDisplay()
    {
        var displayScore = _score % MAX_SCORE_PER_LEVEL;
        _scoreText.text = displayScore.ToString();
    }

    private void SetScore(int newScore)
    {
        _score = newScore;
#if !UNITY_EDITOR
       Debug.Log("Is instant app: " + GooglePlayInstantUtils.IsInstantApp());
#endif
        Debug.Log("before setting level " + (_score % MAX_SCORE_PER_LEVEL > 0));
        if (_score != 0 &&
            _score % MAX_SCORE_PER_LEVEL == 0)
        {
            IncrementLevel();
        }

        UpdateScoreDisplay();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("SphereScene");
    }

    public void IncrementScore()
    {
        Debug.Log("Increment score");
        SetScore(_score + 1);
    }

    public void Install()
    {
        Debug.Log("Installing game");
        WriteGameStateToCookie(_level, _score);
        Debug.Log("After writing GameState");
        ReadGameStateFromCookie();
        Debug.Log("After reading GameState");
        InstallLauncher.ShowInstallPrompt();
    }

    public void ShowOverheadView()
    {
        ThirdPersonCamera.enabled = false;
        OverheadCamera.enabled = true;
    }

    public void ShowFirstPersonView()
    {
        ThirdPersonCamera.enabled = true;
        OverheadCamera.enabled = false;
    }
}