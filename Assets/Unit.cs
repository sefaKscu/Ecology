using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Ecology
{
    public class Unit : MonoBehaviour
    {

        public UnitProfile Profile;
        public ForestSystem Forest;
        public int CurrentHunger;
        public int CurrentAge;
        public int MaxAgeBeforeDeath;
        public string BaseName;

        public bool IsHealthy
        {
            get
            {
                return (!Profile.Consumer || CurrentHunger < Profile.MaxHungerAllowed);
            }
        }

        public void Initialize(UnitProfile profile, ForestSystem forest, int i)
        {
            this.Profile = profile;
            this.Forest = forest;
            BaseName = profile.name + " " + forest.unitsCreated[i];
            transform.SetParent(forest.UnitHolders[i]);
            forest.unitsCreated[i]++;
            forest.unitCount[i]++;
            if (profile.Consumer)
            {
                SetName();
            }
            else
            {
                name = BaseName;
            }
            transform.position = new Vector3(Random.Range(-50, 50), Random.Range(-30, 30), 0);
            MaxAgeBeforeDeath = profile.LifeTimeInTurns;
            Instantiate(profile.UnitPrefab, transform);
        }

        void SetName()
        {
            name = BaseName + "(" + CurrentHunger + "/" + Profile.MaxHungerAllowed + ")";
        }

        public void OnTick()
        {
            CurrentAge++;
            if (CurrentAge > MaxAgeBeforeDeath)
            {
                Destroy(gameObject);
            }
            //checking hunger level
            CurrentHunger += Profile.HungerIncreasePerTick;
            if (CurrentHunger >= Profile.EatThreshold)
            {
                EatNearby();
            }
            if (CurrentHunger > Profile.MaxHungerBeforeDeath)
            {
                MaxAgeBeforeDeath -= Profile.PreyType.Count() + 1;
            }
            SetName();

        }

        void Eat()
        {
            for (int i = 0; i < Profile.PreyType.Length; i++)
            {
                if (Forest.FindPrey(Profile, Profile.PreyType[i], out Unit food))
                {
                    CurrentHunger -= food.Profile.NutritionalValue;
                    for (int j = 0; j < Profile.PreyType.Length; j++)
                    {
                        if (food.Profile.Equals(Profile.PreyType[j]))
                        {
                            MaxAgeBeforeDeath -= j;
                        }
                    }
                    if (CurrentHunger < 0)
                    {
                        CurrentHunger = 0;
                    }
                    food.transform.parent = null;
                    Destroy(food.gameObject);
                    return;
                }
            }
        }
        void EatNearby()
        {
            for (int i = 0; i < Profile.PreyType.Length; i++)
            {
                if (Forest.FindPreyNearby(this, Profile.PreyType[i], out Unit food))
                {
                    CurrentHunger -= food.Profile.NutritionalValue;
                    for (int j = 0; j < Profile.PreyType.Length; j++)
                    {
                        if (food.Profile.Equals(Profile.PreyType[j]))
                        {
                            MaxAgeBeforeDeath -= j;
                        }
                    }
                    if (CurrentHunger < 0)
                    {
                        CurrentHunger = 0;
                    }
                    transform.position = food.transform.position;
                    food.transform.parent = null;
                    Destroy(food.gameObject);
                    return;
                }
            }
        }

        private void OnDestroy()
        {
            Forest.OnUnitDeath(Profile);
        }
    }
}
