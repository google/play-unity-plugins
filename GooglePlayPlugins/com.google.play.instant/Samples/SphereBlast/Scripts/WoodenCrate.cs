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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodenCrate : MonoBehaviour
{
    public GameObject ExplodingVersion;

    private void SwapExplode()
    {
        Debug.Log("SwapExplode triggered");
        Instantiate(ExplodingVersion, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision with crate detected");
        var baseGame = GameObject.Find("BaseGame").GetComponent<BaseGame>();
        baseGame.IncrementScore();
        baseGame.ResetTimer();
        baseGame.UpdateScoreDisplay();
        SwapExplode();
    }
}