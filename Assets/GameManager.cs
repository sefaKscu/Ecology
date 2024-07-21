using System.Collections;
using UnityEngine;

namespace Ecology
{
    public class GameManager : MonoBehaviour
    {
        public ForestSystem ForestSystem;
        public DisasterSystem DisasterSystem;
        public int TicksPerJump = 5;
        public float TickDurationInSeconds = 1;
        

        bool isRunning = false;
        bool hasStopped = true;

        private void Start()
        {
            isRunning = true;
            StartCoroutine(RunGameLoop());
        }
        void Update()
        {
            GetInputs();
        }

        private void GetInputs()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PauseSimulation();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PlaySimulation();
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                JumpTicks();
            }
            else if (Input.GetKeyDown(KeyCode.Space))
            {
                IncrementTick();
            }
        }

        IEnumerator RunGameLoop()
        {
            hasStopped = false;
            while (isRunning)
            {
                IncrementTick();
                yield return new WaitForSeconds(TickDurationInSeconds);
            }
            hasStopped = true;
        }

        void PlaySimulation()
        {
            if (!hasStopped)
                return;
            Debug.Log("Simulation started.");
            isRunning = true;
            StartCoroutine(RunGameLoop());
        }
        void PauseSimulation()
        {
            Debug.Log("Simulation paused.");
            isRunning = false;
        }

        private void JumpTicks()
        {
            for (int i = 0; i < TicksPerJump; i++)
            {
                IncrementTick();
            }
        }

        void IncrementTick()
        {
            if (ForestSystem != null)
                ForestSystem.IncrementTick();
            if (DisasterSystem != null)
                DisasterSystem.IncrementTick();
        }

    }
}
