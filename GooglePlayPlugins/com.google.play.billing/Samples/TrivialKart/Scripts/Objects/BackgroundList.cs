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

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constant data for backgrounds.
/// </summary>
public class BackgroundList
{
    public class Background
    {
        public Background(BackgroundName name)
        {
            Name = name;
        }

        public BackgroundName Name { get; }

        public GameObject GarageItemGameObj { get; set; }

        public Sprite ImageSprite { get; set; }
    }

    public static readonly Background BlueGrassBackground =
        new Background(BackgroundName.BlueGrass);

    public static readonly Background MushroomBackground =
        new Background(BackgroundName.Mushroom);

    public static readonly List<Background> List = new List<Background>()
        {BlueGrassBackground, MushroomBackground};

    public static Background GetBackgroundByName(BackgroundName backgroundName)
    {
        return List[(int) backgroundName];
    }

    public static Background GetBackgroundByIndex(int backgroundIndex)
    {
        return List[backgroundIndex];
    }
}