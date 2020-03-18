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

public class WoodenCrateExplosion : MonoBehaviour
{
    private const float DESTROY_DELAY = 3f;
    private float _timer = 0;
    public GameObject Sphere;
    public AudioSource ExplosionClip;

    public void Start()
    {
        ExplosionClip = GetComponent<AudioSource>();
        Sphere = GameObject.Find("Sphere");
        Collider[] currentColliders = GetComponentsInChildren<BoxCollider>();

        foreach (var t in currentColliders)
        {
            Physics.IgnoreCollision(t,
                Sphere.GetComponent<SphereCollider>());
        }

        ExplosionClip.Play();
    }

    public void Update()
    {
        _timer += Time.deltaTime;
        if (!(_timer > DESTROY_DELAY)) return;
        Destroy(gameObject);
        Debug.Log("Timer is done");
        var baseGame = GameObject.Find("BaseGame").GetComponent<BaseGame>();
        baseGame.CreateCrate();
    }
}