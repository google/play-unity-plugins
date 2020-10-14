﻿// Copyright 2020 Google LLC
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPriceChangeController : MonoBehaviour
{
    public Text[] TextItems;

    public void UpdatePriceTextItem(string productId, string localizedPriceString)
    {
        string searchString = string.Format("${0}", productId);
        foreach(Text textItem in TextItems)
        {
            if (textItem.text.Contains(searchString))
            {
                string newPriceString = textItem.text.Replace(searchString, 
                    localizedPriceString);
                textItem.text = newPriceString;
            }
        }
    }
}
