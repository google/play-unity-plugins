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
/// Constant data for cars.
/// </summary>
public static class CarList
{
    public class Car
    {
        public Car(CarName name, int speed, float price, bool isRealMoneyPurchase, string productId)
        {
            Name = name;
            Speed = speed;
            Price = price;
            IsRealMoneyPurchase = isRealMoneyPurchase;
            ProductId = productId;
        }

        public CarName Name { get; }

        public int Speed { get; }

        public float Price { get; set; }

        public bool IsRealMoneyPurchase { get; }

        public string ProductId { get; }

        public GameObject StoreItemCarGameObj { get; set; }

        public GameObject GarageItemGameObj { get; set; }

        public GameObject PlayCarGameObj { get; set; }
    }

    public static Car CarSedan = new Car(CarName.Sedan, 500, 0, true, "car_sedan");
    public static Car CarTruck = new Car(CarName.Truck, 600, 20, false, "car_truck");
    public static Car CarJeep = new Car(CarName.Jeep, 650, 2.99f, true, "car_jeep");
    public static Car CarKart = new Car(CarName.Kart, 1000, 4.99f, true, "car_kart");
    public static List<Car> List = new List<Car>() {CarSedan, CarTruck, CarJeep, CarKart};

    // Return car object by name.
    public static Car GetCarByName(CarName carName)
    {
        return List[(int) carName];
    }

    public static Car GetCarByCarIndex(int carIndex)
    {
        return List[carIndex];
    }
}