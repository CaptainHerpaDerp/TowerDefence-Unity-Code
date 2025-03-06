using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using Core;
using Sirenix.OdinInspector;
using Core.Debugging;

namespace AudioManagement
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        [Header("Volume")]
        [Range(0, 1)]
        [BoxGroup("Volume Control"), SerializeField] public float masterVolume = 1, uiVolume = 1, musicVolume = 1;

        [BoxGroup("Debug"), SerializeField] private bool enableWebGLAudio;

        public float MasterVolume => masterVolume;
        public float UIVolume => uiVolume;
        public float MusicVolume => musicVolume;

        private Bus masterBus;
        private List<EventInstance> activeEvents = new List<EventInstance>();
        private bool isInitialized = false;

        private void Start()
        {
            if (IsWebGLBuild())
            {
                PrintGLLog("WebGL Detected - Waiting for user input...");
            }
            else
            {
                // Setup master bus for non-webgl builds
                masterBus = RuntimeManager.GetBus("bus:/");
            }
        }

        private void Update()
        {
            if (IsWebGLBuild() && !isInitialized && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
            {
                InitializeFMOD();
            }

            ApplyBusVolume();
        }

        #region FMOD Initialization (WebGL Only)

        private void InitializeFMOD()
        {
            isInitialized = true;
            PrintGLLog("User interacted - Initializing FMOD!");

            LoadFMODBanks();
            SetupFMODBus();
            ResumeFMODAudio();
            TestFMODSound();
        }

        private void LoadFMODBanks()
        {
            RuntimeManager.LoadBank("Master", true);
            RuntimeManager.LoadBank("Master.strings", true);
            RuntimeManager.LoadBank("Spells", true);
            RuntimeManager.LoadBank("Towers", true);
            RuntimeManager.LoadBank("UI", true);
            RuntimeManager.LoadBank("Units", true);
            RuntimeManager.WaitForAllSampleLoading();

            PrintGLLog("FMOD Banks Loaded Successfully.");
        }

        private void SetupFMODBus()
        {
            masterBus = RuntimeManager.GetBus("bus:/");
            if (!masterBus.isValid())
            {
                PrintGLLog("FMOD ERROR: Master bus not found!");
                return;
            }

            PrintGLLog("FMOD Master Bus Found.");
            masterBus.setVolume(1.0f);
            masterBus.setPaused(false);
        }

        private void ResumeFMODAudio()
        {
            RuntimeManager.CoreSystem.mixerSuspend();
            RuntimeManager.CoreSystem.mixerResume();
            PrintGLLog("FMOD Audio Resumed.");
        }

        private void TestFMODSound()
        {
            PrintGLLog("Playing FMOD Test Sound...");
            RuntimeManager.PlayOneShot(FMODEvents.Instance.gameStartSound, Vector2.zero);
        }

        #endregion

        #region Sound Playback

        public void PlayOneShot(EventReference sound, Vector2 position)
        {
            RuntimeManager.PlayOneShot(sound, position);
        }

        public EventInstance CreateInstance(EventReference eventReference, Vector2 position)
        {
            EventInstance newEventInstance = RuntimeManager.CreateInstance(eventReference);

            if (!newEventInstance.isValid())
            {
                PrintGLLog("FMOD ERROR: Failed to create event instance!");
                return new EventInstance(); // Return an empty event instance to prevent crashes
            }

            newEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

            // Ensure activeEvents is not null before adding
            if (activeEvents == null)
            {
                activeEvents = new List<EventInstance>();
            }

            activeEvents.Add(newEventInstance);
            return newEventInstance;
        }


        public void PlayTowerConstructionSound(EventReference sound, int towerLevel, Vector2 position)
        {
            float clampedLevel = Mathf.Clamp(towerLevel - 1, 0, 7);
            EventInstance towerConstructionInstance = CreateInstance(sound, position);

            if (!towerConstructionInstance.isValid())
            {
                PrintGLLog("FMOD ERROR: Invalid event instance, cannot play!");
                return;
            }

            towerConstructionInstance.setParameterByName("Tower Level", clampedLevel);
            towerConstructionInstance.start();
            towerConstructionInstance.release();
        }


        #endregion

        #region Volume Control

        private void ApplyBusVolume()   
        {
            if (masterBus.isValid())
            {
                masterBus.setVolume(masterVolume);
            }
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = volume;
        }

        #endregion

        #region Cleanup

        private void CleanUpEventInstances()
        {
            foreach (EventInstance eventInstance in activeEvents)
            {
                if (eventInstance.isValid())
                {
                    eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                    eventInstance.release();
                }
            }

            activeEvents.Clear();


            PrintGLLog("FMOD: All events cleared from memory.");
        }


        private void OnDestroy()
        {
            CleanUpEventInstances();
        }

        #endregion

        #region Helper Functions

        private bool IsWebGLBuild()
        {
            return Application.platform == RuntimePlatform.WebGLPlayer || enableWebGLAudio;
        }

        private void PrintGLLog(string message)
        {
            if (IsWebGLBuild())
            {
                WebGLScreenLog.Instance.Log(message);
            }
        }

        #endregion
    }
}
