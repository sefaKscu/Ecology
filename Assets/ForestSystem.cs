using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Ecology
{
    public class ForestSystem : MonoBehaviour
    {
        [Header("System Settings")]
        public UnitProfile[] UnitProfiles;
        [HideInInspector]
        public Transform[] UnitHolders;

        [Header("Session Settings")]
        public int[] startingUnits;

        [Header("Session Data")]
        public int numberOfTicksInSession = 0;
        public int[] unitCount;
        [HideInInspector]
        public int[] unitsCreated;
        public int unitTypeCount;

        // Use this for initialization
        void Start()
        {
            unitTypeCount = UnitProfiles.Length;
            if (startingUnits.Length < unitTypeCount)
            {
                Debug.LogError("Not enough starting unit data.");
                return;
            }
            else if (startingUnits.Length > unitTypeCount)
            {
                Debug.LogWarning("More starting unit data than expected.");
            }
            SetUpForest();
        }

        void SetUpForest()
        {
            UnitHolders = new Transform[unitTypeCount];
            unitCount = new int[unitTypeCount];
            unitsCreated = new int[unitTypeCount];

            for (int i = 0; i < unitTypeCount; i++)
            {
                UnitHolders[i] = new GameObject(UnitProfiles[i].name).transform;
                UnitHolders[i].SetParent(transform);
                for (int j = 0; j < startingUnits[i]; j++)
                {
                    CreateUnit(UnitProfiles[i], i);
                }
            }
        }

        void CreateUnit(UnitProfile profile, int i)
        {
            Unit newUnit = new GameObject().AddComponent<Unit>();
            newUnit.AddComponent<BoxCollider2D>();
            newUnit.AddComponent<Rigidbody2D>().isKinematic = true;
            newUnit.Initialize(profile, this, i);
        }

        public void IncrementTick()
        {

            for (int i = 0; i < unitTypeCount; i++)
            {
                CheckForMultiply(UnitProfiles[i], UnitHolders[i], i);
                if (UnitProfiles[i].Consumer)
                {
                    foreach (Transform u in UnitHolders[i])
                    {
                        u.GetComponent<Unit>().OnTick();
                    }
                }
            }
            numberOfTicksInSession++;
        }

        void RespawnIfNoParentsNeeded(UnitProfile profile, int unitHolderIndex)
        {
            if (profile.ParentCountRequired > 0)
                return;
            if (UnitHolders[unitHolderIndex].childCount > 0)
                return;

            CheckForMultiply(profile, UnitHolders[unitHolderIndex], unitHolderIndex);
        }

        void CheckForMultiply(UnitProfile profile, Transform holder, int iter)
        {
            if ((numberOfTicksInSession + 1) % profile.MultiplyFrequency != 0)
                return;

            int healthyUnits = 0;
            if (profile.Consumer)
            {
                foreach (Transform u in holder)
                {
                    if (u.GetComponent<Unit>().IsHealthy)
                        healthyUnits++;
                }
            }
            else
            {
                healthyUnits = holder.childCount;
            }
            int timesToMultiply;
            if (holder.childCount == 0 && profile.ParentCountRequired == 0)
            {
                timesToMultiply = profile.ChildCountProduced * 10;
            }
            else if (profile.ParentCountRequired > 0)
            {
                timesToMultiply = healthyUnits / profile.ParentCountRequired;
            }
            else
            {
                timesToMultiply = healthyUnits;
            }
            if (holder.childCount > 10000)
            {
                timesToMultiply = 2;
            }
            else if (holder.childCount > 2000 && timesToMultiply > 20)
            {
                timesToMultiply = 20;
            }

            int newChildCount = profile.ChildCountProduced;
            for (int i = 0; i < timesToMultiply; i++)
            {
                for (int j = 0; j < newChildCount; j++)
                {
                    CreateUnit(profile, iter);
                }
            }
        }

        public void OnUnitDeath(UnitProfile profile)
        {
            for (int i = 0; i < unitTypeCount; i++)
            {
                if (profile == UnitProfiles[i])
                {
                    unitCount[i]--;
                }
            }
        }

        public bool FindPrey(UnitProfile hunterProfile, UnitProfile preyProfile, out Unit prey)
        {
            prey = null;
            float hunterCount = 0;
            float preyCount = 0;
            for (int i = 0; i < unitTypeCount; ++i)
            {
                if (hunterProfile == UnitProfiles[i])
                {
                    hunterCount = UnitHolders[i].childCount;
                }
            }
            for (int i = 0; i < unitTypeCount; ++i)
            {
                if (preyProfile == UnitProfiles[i])
                {
                    preyCount = UnitHolders[i].childCount;
                    if (preyCount > 0)
                    {
                        prey = UnitHolders[i].GetChild(0).GetComponent<Unit>();
                    }
                }
            }
            float totalPopulation = preyCount + hunterCount;
            float preyAbundancy = (preyCount * 0.9f) / totalPopulation;
            float hunterAbundancy = hunterCount / totalPopulation;
            float chanceToFindPrey = preyAbundancy / hunterAbundancy;

            bool isSuccess = chanceToFindPrey + hunterProfile.ChanceToHuntSuccesfully - preyProfile.ChanceToEvade > 1;
            if (!isSuccess)
            {
                prey = null;
            }
            return isSuccess;
        }
        Collider2D[] hitResultOfHunting = new Collider2D[225];
        List<Unit> hunterlist = new List<Unit>();
        List<Unit> preyList = new List<Unit>();
        public bool FindPreyNearby(Unit hunter, UnitProfile preyProfile, out Unit prey)
        {
            prey = null;

            // Refactor Zone

            //Collider2D[] hitCols = Physics2D.OverlapBoxAll(hunter.transform.position, Vector2.one * hunter.Profile.HuntingRange, 0);

            //foreach (Collider2D col in hitCols)
            //{
            //    if (col.GetComponent<Unit>().Profile.Equals(preyProfile))
            //        preyList.Add(col.GetComponent<Unit>());
            //    if (col.GetComponent<Unit>().Profile.Equals(hunter.Profile))
            //        hunterlist.Add(col.GetComponent<Unit>());
            //}

            int hitCount = Physics2D.OverlapBoxNonAlloc(hunter.transform.position, Vector2.one * hunter.Profile.HuntingRange, 0, hitResultOfHunting);
            for (int i = 0; i < hitCount; i++)
            {
                if (hitResultOfHunting[i].GetComponent<Unit>().Profile.Equals(preyProfile))
                {
                    preyList.Add(hitResultOfHunting[i].GetComponent<Unit>());
                    continue;
                }
                else if (hitResultOfHunting[i].GetComponent<Unit>().Profile.Equals(hunter.Profile))
                {
                    hunterlist.Add(hitResultOfHunting[i].GetComponent<Unit>());
                }
            }
            int preyCount = preyList.Count;
            if (preyCount <= 0)
            {
                return false;
            }
            int hunterCount = hunterlist.Count;

            int totalPopulation = hunterCount + preyCount;
            float preyAbundance = preyCount / totalPopulation;
            float hunterAbundance = hunterCount / totalPopulation;
            float chanceToFindPrey = preyAbundance / hunterAbundance;

            bool isSuccess = Mathf.Clamp01(chanceToFindPrey) + hunter.Profile.ChanceToHuntSuccesfully - preyProfile.ChanceToEvade > 1;
            if (isSuccess)
            {
                prey = preyList[Random.Range(0, preyCount)];
            }
            preyList.Clear();
            hunterlist.Clear();
            return isSuccess;
        }
    }
}
