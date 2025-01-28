#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Enemies;
using LevelEvents;

namespace Management
{
    /// <summary>
    /// A custom editor window for creating and editing levels
    /// </summary>
    public class LevelEditorWindow : EditorWindow
    {
        private GameDifficultyCalulator gameDifficultyCalulator;

        private Level level;
        private Vector2 scrollPosition;

        [MenuItem("Window/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow(typeof(LevelEditorWindow), false, "Level Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Settings", EditorStyles.boldLabel);

            if (level != null)
                GUILayout.Label($"Editing: {level.name}");

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Create Level"))
            {
                CreateLevel();
            }

            if (GUILayout.Button("Load Level"))
            {
                level = LoadLevel();
            }

            if (level != null && GUILayout.Button("Save Level"))
            {
                SaveLevel();
            }

            if (level != null && GUILayout.Button("Delete Level"))
            {
                DeleteLevel();
            }

            if (level != null && GUILayout.Button("Duplicate Level"))
            {
                DuplicateLevel();
            }

            GUILayout.EndHorizontal();

            if (level == null)
            {
                EditorGUILayout.HelpBox("Create or load a level to begin.", MessageType.Info);
                return;
            }

            GUILayout.Space(20);

            level.LevelIndex = EditorGUILayout.IntField("Level Index", level.LevelIndex);
            level.StartingLives = EditorGUILayout.IntField("Starting Lives", level.StartingLives);
            level.StartingGold = EditorGUILayout.IntField("Starting Gold", level.StartingGold);

            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Wave", GUILayout.Width(100)))
            {
                AddWave();
            }
            GUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (level.waves != null)
                for (int i = 0; i < level.waves.Count; i++)
                {
                    DisplayWaveGUI(level.waves[i], i);
                }

            EditorGUILayout.EndScrollView();
        }

        private void OnEnable()
        {
            gameDifficultyCalulator = FindObjectOfType<GameDifficultyCalulator>();
        }

        private void OnDisable()
        {
            if (level != null)
            {
                EditorUtility.SetDirty(level);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void CreateLevel()
        {
            string path = EditorUtility.SaveFilePanel("Create Level", "Assets/Levels", "NewLevel", "asset");

            if (path.Length != 0)
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                level = ScriptableObject.CreateInstance<Level>();
                AssetDatabase.CreateAsset(level, relativePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void SaveLevel()
        {
            EditorUtility.SetDirty(level);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private Level LoadLevel()
        {
            string path = EditorUtility.OpenFilePanel("Load Level", "Assets/Levels", "asset");

            if (path.Length != 0)
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                return AssetDatabase.LoadAssetAtPath<Level>(relativePath);
            }

            return ScriptableObject.CreateInstance<Level>();
        }

        private void DeleteLevel()
        {
            bool delete = EditorUtility.DisplayDialog("Delete Level?",
                                  "Are you sure you want to delete this level? This action cannot be undone.",
                                 "Yes", "No");

            if (delete)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(level));
                AssetDatabase.Refresh();
                level = null;
            }
        }

        private void DuplicateLevel()
        {
            string path = EditorUtility.SaveFilePanel("Duplicate Level", "Assets/Levels", level.name + "Copy", "asset");

            if (path.Length != 0)
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                Level newLevel = Instantiate(level);
                newLevel.name = Path.GetFileNameWithoutExtension(path);
                AssetDatabase.CreateAsset(newLevel, relativePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void AddWave()
        {
            level.waves ??= new List<Wave>();
            level.waves.Add(new Wave());
        }

        private void DisplayWaveGUI(Wave wave, int index)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);

            GUILayout.Label($"Wave {index + 1}", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Event", GUILayout.Width(100)))
            {
                wave.waveEvents.Add(new WaveEvent());
            }
            GUILayout.EndHorizontal();

            for (int j = 0; j < wave.waveEvents.Count; j++)
            {
                DisplayEventGUI(wave, wave.waveEvents[j], j);
            }

            GUILayout.Space(7.5f);

            GUILayout.BeginVertical();

            int totalEnemies = 0;
            foreach (WaveEvent waveEvent in wave.waveEvents)
            {
                if (waveEvent.eventType == WaveEventType.SpawnEnemy)
                {
                    totalEnemies += waveEvent.spawnQuantity;
                }
            }

            GUILayout.Label($"Total Enemies: {totalEnemies}");

            float totalTime = 0;
            float delayTime = 0;

            foreach (WaveEvent waveEvent in wave.waveEvents)
            {
                if (waveEvent.eventType == WaveEventType.SpawnEnemy)
                {
                    totalTime += waveEvent.spawnInterval * waveEvent.spawnQuantity;
                }
            }

            foreach (WaveEvent waveEvent in wave.waveEvents)
            {
                if (waveEvent.eventType == WaveEventType.WaitDuration)
                {
                    totalTime += waveEvent.waitDuration;
                    delayTime += waveEvent.waitDuration;
                }
            }

            GUILayout.Label($"Total Time: {totalTime}");

            float difficultyRating = 0;

            foreach (WaveEvent waveEvent in wave.waveEvents)
            {
                if (waveEvent.eventType == WaveEventType.SpawnEnemy)
                {
                    float spawnInterval = waveEvent.spawnInterval;
                    if (spawnInterval == 0)
                    {
                        spawnInterval = 1;
                    }

                    if (gameDifficultyCalulator.EnemyDifficultyRatings.ContainsKey(waveEvent.enemyType) == false)
                    {
                        continue;
                    }

                    if (gameDifficultyCalulator.EnemyDifficultyRatings[waveEvent.enemyType] == 0)
                    {
                        continue;
                    }

                    float waveIntensity = Mathf.Round((waveEvent.spawnQuantity * gameDifficultyCalulator.EnemyDifficultyRatings[waveEvent.enemyType]) / spawnInterval);


                    difficultyRating += (waveIntensity);
                }
            }

            GUILayout.Label($"Wave Difficulty: {difficultyRating}");

            GUILayout.EndVertical();

            GUILayout.Space(7.5f);

            GUILayout.BeginHorizontal();

            GUILayout.Label("Next Wave Delay", GUILayout.Width(150));
            wave.nextWaveDelay = EditorGUILayout.IntField(wave.nextWaveDelay, GUILayout.Width(50));

            GUILayout.EndHorizontal();

            GUILayout.Space(15);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove Wave", GUILayout.Width(100)))
            {
                level.waves.RemoveAt(index);
                return;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        private void DisplayEventGUI(Wave wave, WaveEvent waveEvent, int index)
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                wave.waveEvents.RemoveAt(index);
                return;
            }

            waveEvent.eventType = (WaveEventType)EditorGUILayout.EnumPopup("Event Type", waveEvent.eventType);

            switch (waveEvent.eventType)
            {
                case WaveEventType.SpawnEnemy:

                    GUILayout.Label("Start Loc", GUILayout.Width(55));
                    waveEvent.spawnLocation = EditorGUILayout.IntField(waveEvent.spawnLocation, GUILayout.Width(15));

                    GUILayout.Label("End Loc", GUILayout.Width(50));
                    waveEvent.endLocation = EditorGUILayout.IntField(waveEvent.endLocation, GUILayout.Width(15));

                    waveEvent.enemyType = (EnemyType)EditorGUILayout.EnumPopup("Enemy Type", waveEvent.enemyType);
                    waveEvent.spawnQuantity = EditorGUILayout.IntField("Spawn Quantity", waveEvent.spawnQuantity);
                    waveEvent.spawnInterval = EditorGUILayout.FloatField("Spawn Interval", waveEvent.spawnInterval);
                    break;

                case WaveEventType.WaitDuration:
                    waveEvent.waitDuration = EditorGUILayout.FloatField("Wait Duration", waveEvent.waitDuration);
                    break;
            }

            if (GUILayout.Button("▲", GUILayout.Width(20)))
            {
                if (index > 0)
                {
                    wave.waveEvents.RemoveAt(index);
                    wave.waveEvents.Insert(index - 1, waveEvent);
                }
            }

            if (GUILayout.Button("▼", GUILayout.Width(20)))
            {
                if (index < wave.waveEvents.Count - 1)
                {
                    wave.waveEvents.RemoveAt(index);
                    wave.waveEvents.Insert(index + 1, waveEvent);
                }
            }

            GUILayout.EndHorizontal();
        }
    }

}
#endif
