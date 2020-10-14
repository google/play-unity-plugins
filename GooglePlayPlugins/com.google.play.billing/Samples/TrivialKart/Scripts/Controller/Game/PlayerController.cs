// Copyright 2020 Google LLC
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
using UnityEngine;

/// <summary>
/// Controller for car movement.
/// It updates the car movement data (e.g., distance and speed);
/// It updates the car game object in the play page when car in use changes;
/// It updates the main game camera position along with the car object.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public GameObject cam;

    private const int EndOfRoadPositionX = 25;
    private static readonly int Speed = Animator.StringToHash("speed");
    private GameObject _carInUseGameObj;
    private Animator _carInUseAnimator;
    private Gas _gas;
    private Vector3 _carStartPos;
    private Rigidbody2D _rigidbody2D;
    private int _circleCount;
    private Vector3 _camOffset;

    
    private void Start()
    {
        UpdateCarInUse();
        InitValues();
    }

    private void FixedUpdate()
    {
        // Warp back to the start point when the car reaches the end.
        if (_carInUseGameObj.transform.position.x >= EndOfRoadPositionX)
        {
            _circleCount++;
            _carInUseGameObj.transform.position = _carStartPos;
        }
        
        _carInUseAnimator.SetFloat(Speed, _rigidbody2D.velocity.magnitude);
        
        // Update gas level.
        var distancePerCircle = EndOfRoadPositionX - _carStartPos.x;
        var distanceTraveledInCurCircle =  (float) Math.Round(_carInUseGameObj.transform.position.x - _carStartPos.x, 1);
        var totalDistanceTraveled = distancePerCircle * _circleCount + distanceTraveledInCurCircle;
        _gas.SetGasLevel(totalDistanceTraveled);

        // Update cam position.
        var carPosition = _carInUseGameObj.transform.position;
        cam.transform.position = new Vector3(carPosition.x, carPosition.y, carPosition.z) + _camOffset;
    }
    
    private void InitValues()
    {
        _circleCount = 0;
        _gas = GetComponent<Gas>();
        _carStartPos = _carInUseGameObj.transform.position;
        _camOffset = cam.transform.position - _carStartPos;
    }
    
    // Update the active car for the play mode when player switches the car.
    public void UpdateCarInUse()
    {
        // Set all cars to be inactive.
        foreach (var car in CarList.List)
        {
            car.PlayCarGameObj.SetActive(false);
        }
        
        // Set the car in use game object to be active.
        var carInUseGameObj = GameDataController.GetGameData().CarInUseObj.PlayCarGameObj;
        SetCarInUseState(carInUseGameObj);
    }

    private void SetCarInUseState(GameObject carInUseGameObj)
    {
        carInUseGameObj.SetActive(true);
        if (!(_carInUseGameObj is null))
        {
            // Sync next use car position with current position.
            carInUseGameObj.transform.position = _carInUseGameObj.transform.position;
        }

        _carInUseGameObj = carInUseGameObj;
        _carInUseAnimator = _carInUseGameObj.GetComponent<Animator>();
        _rigidbody2D = _carInUseGameObj.GetComponent<Rigidbody2D>();
    }
}
