using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Ecology
{
    [CreateAssetMenu(menuName = "Ecology/Unit Profile")]
    public class UnitProfile : ScriptableObject
    {
        [SerializeField] UnitType unitType;
        [SerializeField] GameObject unitPrefab;
        [SerializeField] int nutritionalValue;

        [Header("Reproduction")]
        [SerializeField] bool canReproduce;
        [SerializeField, FormerlySerializedAs("multiplyFrequency")] int turnsBeforeReproducing;
        [SerializeField] int parentCountRequired;
        [SerializeField, FormerlySerializedAs("maxHungerAllowed")] int maxHungerAllowedToReproduce;
        [SerializeField] int childCountMin;
        [SerializeField] int childCountMax;

        [Header("Survival")]
        [SerializeField] int lifeTimeInTurns;
        [SerializeField] int lifeTimeInTurnsMax;
        [SerializeField, Range(0, 1)] float chanceToEvade;
        [SerializeField] int maxHungerBeforeDeath;

        [Header("Consumer")]
        [SerializeField] bool consumer;
        [SerializeField] UnitProfile[] preyType;
        [SerializeField, Range(3, 15)] int huntingRange = 3;
        [SerializeField, Range(0,1)] float chanceToHuntSuccesfully;
        [SerializeField] int hungerIncreasePerTick;
        [SerializeField] int eatThreshold;

        public UnitType UnitType { get => unitType; }
        public GameObject UnitPrefab { get => unitPrefab; }
        public int NutritionalValue { get => nutritionalValue; }


        public bool CanReproduce { get => canReproduce; }
        public int MultiplyFrequency { get => turnsBeforeReproducing; }
        public int ParentCountRequired { get => parentCountRequired; }
        public int MaxHungerAllowed { get => maxHungerAllowedToReproduce; }
        public int ChildCountProduced { get => Random.Range(childCountMin, childCountMax); }


        public int LifeTimeInTurns { get => Random.Range(lifeTimeInTurns, lifeTimeInTurnsMax); }
        public float ChanceToEvade { get => chanceToEvade; }
        public int MaxHungerBeforeDeath { get => maxHungerBeforeDeath; }


        public bool Consumer { get => consumer; }
        public UnitProfile[] PreyType { get => preyType; }
        public int HuntingRange
        {
            get
            {
                if (huntingRange <= 3)
                    return 3;
                if (huntingRange > 3 &&  huntingRange <= 5)
                    return 5;
                if (huntingRange > 5 && huntingRange <= 7)
                    return 7;
                if (huntingRange > 7 && huntingRange <= 9)
                    return 9;
                if (huntingRange > 9 && huntingRange <= 11)
                    return 11;
                if (huntingRange > 11 && huntingRange <= 13)
                    return 13;
                if (huntingRange > 13)
                    return 15;
                

                return huntingRange;
            }
        }
        public float ChanceToHuntSuccesfully { get => chanceToHuntSuccesfully; }
        public int HungerIncreasePerTick { get => hungerIncreasePerTick; }
        public int EatThreshold { get => eatThreshold; }
    }

    public enum UnitType
    {
        Plant,
        Animal
    }
}
