
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Towers;
using Enemies;

namespace Management
{
    /// <summary>
    /// Custom editor window for editing game settings
    /// </summary>
    public class GameSettingsEditorWindow : EditorWindow
    {
        private GameSettings gameSettings;
        private SettingsApplier settingsApplier;
        private PurchaseManager purchaseManager;
        private GameDifficultyCalulator gameDifficultyCalulator;

        Dictionary<EnemyType, float> enemyDifficultyRatings => gameDifficultyCalulator.EnemyDifficultyRatings;

        public void OnEnable()
        {
            settingsApplier = SettingsApplier.Instance;
            purchaseManager = PurchaseManager.Instance;
            gameDifficultyCalulator = GameDifficultyCalulator.Instance;

            if (gameDifficultyCalulator == null)
            {
                gameDifficultyCalulator = FindObjectOfType<GameDifficultyCalulator>();
            }

            if (gameDifficultyCalulator != null && gameSettings != null)
            gameDifficultyCalulator.DoDifficultyRatingCalculations(gameSettings);
        }

        [MenuItem("Window/Game Settings Editor")]
        public static void ShowWindow()
        {
            GetWindow<GameSettingsEditorWindow>("Game Settings Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Game Settings", EditorStyles.boldLabel);

            // Display the game settings name
            if (gameSettings != null)
            {
                GUILayout.Space(7.5f);
                GUILayout.Label($"Editing: {gameSettings.name}");
            }

            GUILayout.Space(15);

            // Buttons for creating, loading, and saving game settings
            GUILayout.BeginHorizontal();

            // Button to create a new game settings
            if (GUILayout.Button("Create Game Settings"))
            {
                CreateGameSettings();
            }

            // Button to load game settings
            if (GUILayout.Button("Load Game Settings"))
            {
                LoadGameSettings();
            }

            // Button to save game settings
            if (gameSettings != null && GUILayout.Button("Save Game Settings"))
            {
                SaveGameSettings();
            }

            // Button to apply game settings
            if (gameSettings != null && GUILayout.Button("Apply Game Settings"))
            {
                SaveGameSettings();
                ApplyGameSetting();
                // Implement logic to apply game settings
            }

            GUILayout.EndHorizontal();

            if (gameSettings == null)
            {
                EditorGUILayout.HelpBox("Create or load game settings to begin.", MessageType.Info);
                return;
            }

            // Display parameters to edit
            DisplayParameters();
        }

        private void CreateGameSettings()
        {
            string path = EditorUtility.SaveFilePanel("Create Game Settings", "Assets/GameSettings", "NewGameSettings", "asset");

            if (path.Length != 0)
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                gameSettings = ScriptableObject.CreateInstance<GameSettings>();
                AssetDatabase.CreateAsset(gameSettings, relativePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void LoadGameSettings()
        {
            string path = EditorUtility.OpenFilePanel("Load Game Settings", "Assets/GameSettings", "asset");

            if (path.Length != 0)
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                gameSettings = AssetDatabase.LoadAssetAtPath<GameSettings>(relativePath);
            }
        }

        private void SaveGameSettings()
        {
            EditorUtility.SetDirty(gameSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void ApplyGameSetting()
        {
            if (settingsApplier == null)
            {
                settingsApplier = SettingsApplier.Instance;
            }

            if (purchaseManager == null)
            {
                purchaseManager = PurchaseManager.Instance;
            }

            // If the settings applier hasn't been instantiated (trying to apply settings in editor mode)
            if (settingsApplier == null)
            {
                settingsApplier = FindObjectOfType<SettingsApplier>();
            }

            if (gameDifficultyCalulator == null)
            {
                gameDifficultyCalulator = FindObjectOfType<GameDifficultyCalulator>();
            }

            settingsApplier.ApplySettings(gameSettings);
            gameDifficultyCalulator.DoDifficultyRatingCalculations(gameSettings);
        }

        private Vector2 scrollPosition;
        private bool archerTowerSettingsFoldout = true;
        private bool mageTowerSettingsFoldout = true;
        private bool militiaTowerSettingsFoldout = true;
        private bool catapultTowerSettingsFoldout = true;

        private bool orcEnemySettingsFoldout = true;
        private bool slimeEnemySettingsFoldout = true;
        private bool spikedSlimeSettingsFoldout = true;
        private bool wolfEnemySettingsFoldout = true;
        private bool mountedOrcEnemySettingsFoldout = true;
        private bool beeEnemySettingsFoldout = true;
        private bool queenBeeEnemySettingsFoldout = true;
        private bool beeHiveEnemySettingsFoldout = true;
        private bool squidEnemySettingsFoldout = true;
        private bool anglerEnemySettingsFoldout = true;
        private bool turtleEnemySettingsFoldout = true;
        private bool gullEnemySettingsFoldout = true;
        private bool kingAnglerEnemySettingsFoldout = true;
        private bool giantSquidEnemySettingsFoldout = true;
        private bool elderTurtleEnemySettingsFoldout = true;
        private bool larvaEnemySettingsFoldout = true;
        private bool witchEnemySettingsFoldout = true;
        private bool lizardEnemySettingsFoldout = true;
        private bool bombBatEnemySettingsFoldout = true;
        private bool giantLizardEnemySettingsFoldout = true;
        private bool queenLarvaEnemySettingsFoldout = true;
        private bool treemanEnemySettingsFoldout = true;

        private void DisplayParameters()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Space(15);

            // Create a header to display tower settings
            EditorGUILayout.LabelField("Tower Settings", EditorStyles.boldLabel);

            GUILayout.Space(15);

            #region Archer Tower Settings

            archerTowerSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(archerTowerSettingsFoldout, "Archer Tower Settings");

            if (archerTowerSettingsFoldout)
            {
                GUILayout.Space(15);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Total Upgrade Cost", GUILayout.Width(130));
                gameSettings.archerTowerTotalUpgradeCost = EditorGUILayout.IntField(gameSettings.archerTowerTotalUpgradeCost, GUILayout.Width(50));

                GUILayout.Label("Purchase Cost", GUILayout.Width(100));
                gameSettings.archerTowerPurchaseCost = EditorGUILayout.IntField(gameSettings.archerTowerPurchaseCost, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                // Add label and field for archer tower starting damage, set text box width to 50
                GUILayout.Label("Starting Damage", GUILayout.Width(100));
                gameSettings.archerTowerStartingDamage = EditorGUILayout.FloatField(gameSettings.archerTowerStartingDamage, GUILayout.Width(50));

                // Add label and field for archer tower damage increase per upgrade, set text box width to 50
                GUILayout.Label("Damage Increase Per Upgrade", GUILayout.Width(180));
                gameSettings.archerTowerDamageIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.archerTowerDamageIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                // Add label and field for archer tower starting attack speed, set text box width to 50
                GUILayout.Label("Starting Attack Interval", GUILayout.Width(150));
                gameSettings.archerTowerStartingAttackSpeed = EditorGUILayout.FloatField(gameSettings.archerTowerStartingAttackSpeed, GUILayout.Width(50));

                // Add label and field for archer tower attack speed increase per upgrade, set text box width to 50
                GUILayout.Label("Attack Interval Decrease Per Upgrade", GUILayout.Width(230));
                gameSettings.archerTowerAttackSpeedIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.archerTowerAttackSpeedIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                // Add label and field for archer tower starting range, set text box width to 50
                GUILayout.Label("Starting Range", GUILayout.Width(100));
                gameSettings.archerTowerStartingRange = EditorGUILayout.FloatField(gameSettings.archerTowerStartingRange, GUILayout.Width(50));

                // Add label and field for archer tower range increase per upgrade, set text box width to 50
                GUILayout.Label("Range Increase Per Upgrade", GUILayout.Width(180));
                gameSettings.archerTowerRangeIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.archerTowerRangeIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                // Calculate DPS at Level 1
                float dpsAtLevel1 = gameSettings.archerTowerStartingDamage / gameSettings.archerTowerStartingAttackSpeed;
                GUILayout.Label("DPS at Level 1: " + dpsAtLevel1.ToString("F2"), GUILayout.Width(150));

                // Calculate DPS at Level 7
                float dpsAtLevel7 = (gameSettings.archerTowerStartingDamage + (gameSettings.archerTowerDamageIncreasePerUpgrade * 6)) / (gameSettings.archerTowerStartingAttackSpeed - (gameSettings.archerTowerAttackSpeedIncreasePerUpgrade * 6));
                GUILayout.Label("DPS at Level 7: " + dpsAtLevel7.ToString("F2"), GUILayout.Width(150));

                GUILayout.EndHorizontal();

                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                // Calculate Cost per Damage at Level 1
                float costPerDamageLevel1 = gameSettings.archerTowerPurchaseCost / dpsAtLevel1;
                GUILayout.Label("Cost per Damage (Lvl 1): " + costPerDamageLevel1.ToString("F2"), GUILayout.Width(200));

                // Calculate Cost per Damage at Level 7
                float costPerDamageLevel7 = (gameSettings.archerTowerPurchaseCost + gameSettings.archerTowerTotalUpgradeCost) / dpsAtLevel7;
                GUILayout.Label("Cost per Damage (Lvl 7): " + costPerDamageLevel7.ToString("F2"), GUILayout.Width(200));

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(30);

            #region Mage Tower Settings

            mageTowerSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(mageTowerSettingsFoldout, "Mage Tower Settings");

            if (mageTowerSettingsFoldout)
            {
                GUILayout.Space(15);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Total Upgrade Cost", GUILayout.Width(130));
                gameSettings.mageTowerTotalUpgradeCost = EditorGUILayout.IntField(gameSettings.mageTowerTotalUpgradeCost, GUILayout.Width(50));

                GUILayout.Label("Purchase Cost", GUILayout.Width(100));
                gameSettings.mageTowerPurchaseCost = EditorGUILayout.IntField(gameSettings.mageTowerPurchaseCost, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();


                GUILayout.Label("Starting Damage", GUILayout.Width(100));
                gameSettings.mageTowerStartingDamage = EditorGUILayout.FloatField(gameSettings.mageTowerStartingDamage, GUILayout.Width(50));


                GUILayout.Label("Damage Increase Per Upgrade", GUILayout.Width(180));
                gameSettings.mageTowerDamageIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.mageTowerDamageIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Starting Attack Interval", GUILayout.Width(150));
                gameSettings.mageTowerStartingAttackSpeed = EditorGUILayout.FloatField(gameSettings.mageTowerStartingAttackSpeed, GUILayout.Width(50));

                GUILayout.Label("Attack Interval Decrease Per Upgrade", GUILayout.Width(230));
                gameSettings.mageTowerAttackSpeedIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.mageTowerAttackSpeedIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Starting Range", GUILayout.Width(100));
                gameSettings.mageTowerStartingRange = EditorGUILayout.FloatField(gameSettings.mageTowerStartingRange, GUILayout.Width(50));

                GUILayout.Label("Range Increase Per Upgrade", GUILayout.Width(180));
                gameSettings.mageTowerRangeIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.mageTowerRangeIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                // Calculate DPS at Level 1
                float dpsAtLevel1 = gameSettings.mageTowerStartingDamage / gameSettings.mageTowerStartingAttackSpeed;
                GUILayout.Label("DPS at Level 1: " + dpsAtLevel1.ToString("F2"), GUILayout.Width(150));

                // Calculate DPS at Level 7
                float dpsAtLevel7 = (gameSettings.mageTowerStartingDamage + (gameSettings.mageTowerDamageIncreasePerUpgrade * 6)) / (gameSettings.mageTowerStartingAttackSpeed - (gameSettings.mageTowerAttackSpeedIncreasePerUpgrade * 6));
                GUILayout.Label("DPS at Level 7: " + dpsAtLevel7.ToString("F2"), GUILayout.Width(150));

                GUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                // Calculate Cost per Damage at Level 1
                float costPerDamageLevel1 = gameSettings.mageTowerPurchaseCost / dpsAtLevel1;
                GUILayout.Label("Cost per Damage (Lvl 1): " + costPerDamageLevel1.ToString("F2"), GUILayout.Width(200));

                // Calculate Cost per Damage at Level 7
                float costPerDamageLevel7 = (gameSettings.mageTowerPurchaseCost + gameSettings.mageTowerTotalUpgradeCost) / dpsAtLevel7;
                GUILayout.Label("Cost per Damage (Lvl 7): " + costPerDamageLevel7.ToString("F2"), GUILayout.Width(200));

                GUILayout.EndHorizontal();

            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(30);

            #region Militia Tower Settings

            militiaTowerSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(militiaTowerSettingsFoldout, "Militia Tower Settings");

            if (militiaTowerSettingsFoldout)
            {
                GUILayout.Space(15);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Total Upgrade Cost", GUILayout.Width(130));
                gameSettings.militiaTowerTotalUpgradeCost = EditorGUILayout.IntField(gameSettings.militiaTowerTotalUpgradeCost, GUILayout.Width(50));

                GUILayout.Label("Purchase Cost", GUILayout.Width(100));
                gameSettings.militiaTowerPurchaseCost = EditorGUILayout.IntField(gameSettings.militiaTowerPurchaseCost, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Starting Unit Damage", GUILayout.Width(150));
                gameSettings.militiaTowerUnitStartingDamage = EditorGUILayout.FloatField(gameSettings.militiaTowerUnitStartingDamage, GUILayout.Width(50));

                GUILayout.Label("Damage Unit Increase Per Upgrade", GUILayout.Width(240));
                gameSettings.militiaTowerUnitDamageIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.militiaTowerUnitDamageIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Starting Unit Health", GUILayout.Width(150));
                gameSettings.militiaTowerUnitStartingHealth = EditorGUILayout.FloatField(gameSettings.militiaTowerUnitStartingHealth, GUILayout.Width(50));

                GUILayout.Label("Unit Health Increase Per Upgrade", GUILayout.Width(270));
                gameSettings.militiaTowerUnitHealthIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.militiaTowerUnitHealthIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Unit Percent Heal Per Second", GUILayout.Width(250));
                gameSettings.militiaUnitPercentHealPerSecond = EditorGUILayout.FloatField(gameSettings.militiaUnitPercentHealPerSecond, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Unit Attack Speed", GUILayout.Width(150));
                gameSettings.militiaTowerUnitAttackSpeed = EditorGUILayout.FloatField(gameSettings.militiaTowerUnitAttackSpeed, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Unit Respawn Time", GUILayout.Width(150));
                gameSettings.militiaUnitRespawnTime = EditorGUILayout.IntField(gameSettings.militiaUnitRespawnTime, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Unit Placement Range", GUILayout.Width(150));
                gameSettings.militiaTowerPlacementRange = EditorGUILayout.FloatField(gameSettings.militiaTowerPlacementRange, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(30);

            #region Catapult Tower Settings

            catapultTowerSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(catapultTowerSettingsFoldout, "Catapult Tower Settings");

            if (catapultTowerSettingsFoldout)
            {
                GUILayout.Space(15);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Total Upgrade Cost", GUILayout.Width(130));
                gameSettings.catapultTowerTotalUpgradeCost = EditorGUILayout.IntField(gameSettings.catapultTowerTotalUpgradeCost, GUILayout.Width(50));

                GUILayout.Label("Purchase Cost", GUILayout.Width(100));
                gameSettings.catapultTowerPurchaseCost = EditorGUILayout.IntField(gameSettings.catapultTowerPurchaseCost, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Starting Damage Per Projectile", GUILayout.Width(200));
                gameSettings.catapultTowerStartingDamagePerProjectile = EditorGUILayout.FloatField(gameSettings.catapultTowerStartingDamagePerProjectile, GUILayout.Width(50));

                GUILayout.Label("Damage Increase Per Upgrade", GUILayout.Width(180));
                gameSettings.catapultTowerProjectileDamageIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.catapultTowerProjectileDamageIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Starting Attack Interval", GUILayout.Width(150));
                gameSettings.catapultTowerStartingAttackSpeed = EditorGUILayout.FloatField(gameSettings.catapultTowerStartingAttackSpeed, GUILayout.Width(50));

                GUILayout.Label("Attack Interval Decrease Per Upgrade", GUILayout.Width(230));
                gameSettings.catapultTowerAttackSpeedIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.catapultTowerAttackSpeedIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                // Add a space vertically
                GUILayout.Space(25);

                EditorGUILayout.BeginHorizontal();

                GUILayout.Label("Starting Range", GUILayout.Width(100));
                gameSettings.catapultTowerStartingRange = EditorGUILayout.FloatField(gameSettings.catapultTowerStartingRange, GUILayout.Width(50));

                GUILayout.Label("Range Increase Per Upgrade", GUILayout.Width(180));
                gameSettings.catapultTowerRangeIncreasePerUpgrade = EditorGUILayout.FloatField(gameSettings.catapultTowerRangeIncreasePerUpgrade, GUILayout.Width(50));

                EditorGUILayout.EndHorizontal();

                /*
                 *  I have spent ages trying to figure out why this section does not work due to beggining/end calls, I have no choice but to comment it out
                 */

                GUILayout.Space(25);

                EditorGUILayout.LabelField("Catapult Tower Projectile Changes", EditorStyles.boldLabel);

                GUILayout.Space(15);

                // Ensure that the list is initialized before accessing it
                if (gameSettings.catapultProjectileChanges == null)
                {
                    gameSettings.catapultProjectileChanges = new List<ProjectileChange>();
                }

                // Display a button to add a new projectile change, make it small
                if (GUILayout.Button("Add Projectile Change", GUILayout.Width(150)))
                {
                    gameSettings.catapultProjectileChanges.Add(new ProjectileChange());
                }

                GUILayout.Space(10);

                //// Inside the DisplayParameters method, where level-specific projectile changes are displayed
                if (gameSettings.catapultProjectileChanges.Count == 0)
                {
                    EditorGUILayout.HelpBox("Add projectile changes to the catapult tower.", MessageType.Info);
                }
                else
                {
                    GUILayout.Space(25);

                    for (int i = 0; i < gameSettings.catapultProjectileChanges.Count; i++)
                    {
                        var projectileChange = gameSettings.catapultProjectileChanges[i];

                        // EditorGUILayout.BeginHorizontal();

                        // Use EditorGUI instead of EditorGUILayout for IntField with specified width
                        projectileChange.level = EditorGUILayout.IntField("Level", projectileChange.level, GUILayout.Width(200));
                        projectileChange.numberOfProjectiles = EditorGUILayout.IntField("Number Of Projectiles", projectileChange.numberOfProjectiles, GUILayout.Width(250));

                        // Add a button to remove the projectile change
                        if (GUILayout.Button("Remove", GUILayout.Width(80)))
                        {
                            gameSettings.catapultProjectileChanges.RemoveAt(i);
                            i--;
                            continue; // Skip the rest of the loop for this iteration
                        }

                        GUILayout.Space(10);

                        // Calculate and display DPS at this level
                        float dpsAtThisLevel = CalculateCatapultDPS(
                            gameSettings.catapultTowerStartingDamagePerProjectile + (gameSettings.catapultTowerProjectileDamageIncreasePerUpgrade * (projectileChange.level - 1)),
                            gameSettings.catapultTowerStartingAttackSpeed - (gameSettings.catapultTowerAttackSpeedIncreasePerUpgrade * (projectileChange.level - 1)),
                            projectileChange.numberOfProjectiles
                        );

                        EditorGUILayout.LabelField($"Max DPS at Level {projectileChange.level}: {dpsAtThisLevel.ToString("F2")}");

                        // Calculate Cost per Damage at Level
                        float costPerDamageAtLevel = (gameSettings.catapultTowerPurchaseCost + gameSettings.catapultTowerTotalUpgradeCost) / dpsAtThisLevel;
                        EditorGUILayout.LabelField($"Cost per Damage (Lvl {projectileChange.level}): {costPerDamageAtLevel.ToString("F2")}");

                        GUILayout.Space(25);
                    }
                }
                //else
                //{
                //    for (int i = 0; i < gameSettings.catapultProjectileChanges.Count; i++)
                //    {
                //        var projectileChange = gameSettings.catapultProjectileChanges[i];

                //        EditorGUILayout.BeginHorizontal();

                //        // Use EditorGUI instead of EditorGUILayout for IntField with specified width
                //        projectileChange.level = EditorGUILayout.IntField("Level", projectileChange.level, GUILayout.Width(200));
                //        projectileChange.numberOfProjectiles = EditorGUILayout.IntField("Number Of Projectiles", projectileChange.numberOfProjectiles, GUILayout.Width(250));

                //        // Add a button to remove the projectile change
                //        if (GUILayout.Button("Remove", GUILayout.Width(80)))
                //        {
                //            gameSettings.catapultProjectileChanges.RemoveAt(i);
                //            i--;
                //            continue; // Skip the rest of the loop for this iteration
                //        }

                //        GUILayout.Space(10);

                //        EditorGUILayout.EndHorizontal();

                //        // Calculate and display DPS at this level
                //        float dpsAtThisLevel = CalculateCatapultDPS(
                //            gameSettings.catapultTowerStartingDamagePerProjectile + (gameSettings.catapultTowerProjectileDamageIncreasePerUpgrade * (projectileChange.level - 1)),
                //            gameSettings.catapultTowerStartingAttackSpeed - (gameSettings.catapultTowerAttackSpeedIncreasePerUpgrade * (projectileChange.level - 1)),
                //            projectileChange.numberOfProjectiles
                //        );

                //        EditorGUILayout.LabelField($"Max DPS at Level {projectileChange.level}: {dpsAtThisLevel.ToString("F2")}");

                //        EditorGUILayout.BeginHorizontal();

                //        // Calculate Cost per Damage at Level
                //        float costPerDamageAtLevel = (gameSettings.catapultTowerPurchaseCost + gameSettings.catapultTowerTotalUpgradeCost) / dpsAtThisLevel;
                //        EditorGUILayout.LabelField($"Cost per Damage (Lvl {projectileChange.level}): {costPerDamageAtLevel.ToString("F2")}");

                //        EditorGUILayout.EndHorizontal();

                //        GUILayout.Space(25);
                //    }
                //}
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            #endregion

            GUILayout.Space(30);

            // Create a header to display tower settings
            EditorGUILayout.LabelField("Enemy Settings", EditorStyles.boldLabel);

            GUILayout.Space(15);

            #region Orc Settings

            orcEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(orcEnemySettingsFoldout, "Orc Settings");

            if (orcEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.orcHealth = EditorGUILayout.IntField(gameSettings.orcHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.orcDamage = EditorGUILayout.FloatField(gameSettings.orcDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.orcSpeed = EditorGUILayout.FloatField(gameSettings.orcSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.orcAttackSpeed = EditorGUILayout.FloatField(gameSettings.orcAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.orcMoneyCarried = EditorGUILayout.IntField(gameSettings.orcMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Orc]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Wolf Settings

            wolfEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(wolfEnemySettingsFoldout, "Wolf Settings");

            if (wolfEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.wolfHealth = EditorGUILayout.IntField(gameSettings.wolfHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.wolfDamage = EditorGUILayout.FloatField(gameSettings.wolfDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.wolfSpeed = EditorGUILayout.FloatField(gameSettings.wolfSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.wolfAttackSpeed = EditorGUILayout.FloatField(gameSettings.wolfAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.wolfMoneyCarried = EditorGUILayout.IntField(gameSettings.wolfMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Wolf]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Slime Settings

            slimeEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(slimeEnemySettingsFoldout, "Slime Settings");

            if (slimeEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.slimeHealth = EditorGUILayout.IntField(gameSettings.slimeHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.slimeDamage = EditorGUILayout.FloatField(gameSettings.slimeDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Split Chance (upon landing a jump)
                GUILayout.Label("Split Chance", GUILayout.Width(80));
                gameSettings.slimeSplitChange = EditorGUILayout.IntField(gameSettings.slimeSplitChange, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.slimeMoneyCarried = EditorGUILayout.IntField(gameSettings.slimeMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal(); 
                
                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Slime]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Mounted Orc Settings

            mountedOrcEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(mountedOrcEnemySettingsFoldout, "Mounted Orc Settings");

            if (mountedOrcEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.mountedOrcHealth = EditorGUILayout.IntField(gameSettings.mountedOrcHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.mountedOrcDamage = EditorGUILayout.FloatField(gameSettings.mountedOrcDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.mountedOrcSpeed = EditorGUILayout.FloatField(gameSettings.mountedOrcSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Charge Speed
                GUILayout.Label("Charge Speed", GUILayout.Width(100));
                gameSettings.mountedOrcChargeSpeed = EditorGUILayout.FloatField(gameSettings.mountedOrcChargeSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Charge Damage
                GUILayout.Label("Charge Damage", GUILayout.Width(100));
                gameSettings.mountedOrcChargeDamage = EditorGUILayout.FloatField(gameSettings.mountedOrcChargeDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Charge Time
                GUILayout.Label("Time Before Charge", GUILayout.Width(130));
                gameSettings.mountedOrcTimeBeforeCharge = EditorGUILayout.FloatField(gameSettings.mountedOrcTimeBeforeCharge, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(90));
                gameSettings.mountedOrcAttackSpeed = EditorGUILayout.FloatField(gameSettings.mountedOrcAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.mountedOrcMoneyCarried = EditorGUILayout.IntField(gameSettings.mountedOrcMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.MountedOrc]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Spiked Slime Settings

            spikedSlimeSettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(spikedSlimeSettingsFoldout, "Spiked Slime Settings");

            if (spikedSlimeSettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.spikedSlimeHealth = EditorGUILayout.IntField(gameSettings.spikedSlimeHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.spikedSlimeDamage = EditorGUILayout.FloatField(gameSettings.spikedSlimeDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Speed
                GUILayout.Label("Attack Speed", GUILayout.Width(100));
                gameSettings.spikedSlimeAttackSpeed = EditorGUILayout.FloatField(gameSettings.spikedSlimeAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.spikedSlimeMoneyCarried = EditorGUILayout.IntField(gameSettings.spikedSlimeMoneyCarried, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Range
                GUILayout.Label("Attack Range", GUILayout.Width(100));
                gameSettings.spikedSlimeAttackRange = EditorGUILayout.FloatField(gameSettings.spikedSlimeAttackRange, GUILayout.Width(50));

                GUILayout.Space(15);

                // Spread Range
                GUILayout.Label("Spread Range", GUILayout.Width(100));
                gameSettings.spikedSlimeSpreadRange = EditorGUILayout.FloatField(gameSettings.spikedSlimeSpreadRange, GUILayout.Width(50));

                GUILayout.Space(15);

                // Mini Spiked Slime Damage
                GUILayout.Label("Mini Spiked Slime Damage", GUILayout.Width(150));
                gameSettings.miniSpikedSlimeDamage = EditorGUILayout.FloatField(gameSettings.miniSpikedSlimeDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Mini Spiked Slime Health
                GUILayout.Label("Mini Spiked Slime Health", GUILayout.Width(150));
                gameSettings.miniSpikedSlimeHealth = EditorGUILayout.FloatField(gameSettings.miniSpikedSlimeHealth, GUILayout.Width(50));

                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.SpikedSlime]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Bee Settings

            beeEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(beeEnemySettingsFoldout, "Bee Settings");

            if (beeEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.beeHealth = EditorGUILayout.IntField(gameSettings.beeHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.beeSpeed = EditorGUILayout.FloatField(gameSettings.beeSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.beeMoneyCarried = EditorGUILayout.IntField(gameSettings.beeMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal(); 
                
                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Bee]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Queen Bee Settings

            queenBeeEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(queenBeeEnemySettingsFoldout, "Queen Bee Settings");

            if (queenBeeEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.queenBeeHealth = EditorGUILayout.IntField(gameSettings.queenBeeHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.queenBeeDamage = EditorGUILayout.FloatField(gameSettings.queenBeeDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.queenBeeSpeed = EditorGUILayout.FloatField(gameSettings.queenBeeSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.queenBeeAttackSpeed = EditorGUILayout.FloatField(gameSettings.queenBeeAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.queenBeeMoneyCarried = EditorGUILayout.IntField(gameSettings.queenBeeMoneyCarried, GUILayout.Width(50));

                GUILayout.Space(15);

                // Hive Spawn Interval
                GUILayout.Label("Hive Spawn Interval", GUILayout.Width(150));
                gameSettings.hiveSpawnInterval = EditorGUILayout.FloatField(gameSettings.hiveSpawnInterval, GUILayout.Width(50));

                GUILayout.Space(15);

                // Hive Min Spawn Distance
                GUILayout.Label("Hive Min Spawn Distance", GUILayout.Width(200));
                gameSettings.hiveMinEndDistance = EditorGUILayout.FloatField(gameSettings.hiveMinEndDistance, GUILayout.Width(50));

                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.QueenBee]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Bee Hive Settings

            beeHiveEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(beeHiveEnemySettingsFoldout, "Bee Hive Settings");

            if (beeHiveEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.beeHiveHealth = EditorGUILayout.IntField(gameSettings.beeHiveHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Spawn Interval
                GUILayout.Label("Spawn Interval", GUILayout.Width(100));
                gameSettings.beeHiveSpawnTime = EditorGUILayout.FloatField(gameSettings.beeHiveSpawnTime, GUILayout.Width(50));

                GUILayout.Space(15);

                // Death Spawn Quantity
                GUILayout.Label("Death Spawn Quantity", GUILayout.Width(150));
                gameSettings.beeHiveDeathSpawnQuantity = EditorGUILayout.IntField(gameSettings.beeHiveDeathSpawnQuantity, GUILayout.Width(50));

                GUILayout.Space(15);

                // Hive Total Spawn Quantity
                GUILayout.Label("Hive Total Spawn Quantity", GUILayout.Width(200));
                gameSettings.beeHiveMaxSpawnQuantity = EditorGUILayout.IntField(gameSettings.beeHiveMaxSpawnQuantity, GUILayout.Width(50));

                GUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Squid Settings

            squidEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(squidEnemySettingsFoldout, "Squid Settings");

            if (squidEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();

                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.squidHealth = EditorGUILayout.IntField(gameSettings.squidHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.squidSpeed = EditorGUILayout.FloatField(gameSettings.squidSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.squidMoneyCarried = EditorGUILayout.IntField(gameSettings.squidMoneyCarried, GUILayout.Width(50));

                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Squid]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Angler Settings

            anglerEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(anglerEnemySettingsFoldout, "Angler Settings");

            if (anglerEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.anglerHealth = EditorGUILayout.IntField(gameSettings.anglerHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.anglerDamage = EditorGUILayout.FloatField(gameSettings.anglerDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.anglerSpeed = EditorGUILayout.FloatField(gameSettings.anglerSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.anglerAttackSpeed = EditorGUILayout.FloatField(gameSettings.anglerAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.anglerMoneyCarried = EditorGUILayout.IntField(gameSettings.anglerMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Angler]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Turtle Settings

            turtleEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(turtleEnemySettingsFoldout, "Turtle Settings");

            if (turtleEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.turtleHealth = EditorGUILayout.IntField(gameSettings.turtleHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.turtleDamage = EditorGUILayout.FloatField(gameSettings.turtleDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.turtleSpeed = EditorGUILayout.FloatField(gameSettings.turtleSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.turtleAttackSpeed = EditorGUILayout.FloatField(gameSettings.turtleAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.turtleMoneyCarried = EditorGUILayout.IntField(gameSettings.turtleMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Turtle]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Gull Settings

            gullEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(gullEnemySettingsFoldout, "Gull Settings");

            if (gullEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.gullHealth = EditorGUILayout.IntField(gameSettings.gullHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.gullSpeed = EditorGUILayout.FloatField(gameSettings.gullSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.gullMoneyCarried = EditorGUILayout.IntField(gameSettings.gullMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Gull]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Angler King Settings

            kingAnglerEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(kingAnglerEnemySettingsFoldout, "King Angler Settings");

            if (kingAnglerEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.kingAnglerHealth = EditorGUILayout.IntField(gameSettings.kingAnglerHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.kingAnglerDamage = EditorGUILayout.FloatField(gameSettings.kingAnglerDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.kingAnglerSpeed = EditorGUILayout.FloatField(gameSettings.kingAnglerSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.kingAnglerAttackSpeed = EditorGUILayout.FloatField(gameSettings.kingAnglerAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.kingAnglerCarriedMoney = EditorGUILayout.IntField(gameSettings.kingAnglerCarriedMoney, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.KingAngler]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Giant Squid Settings

            giantSquidEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(giantSquidEnemySettingsFoldout, "Giant Squid Settings");

            if (giantSquidEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();

                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.giantSquidHealth = EditorGUILayout.IntField(gameSettings.giantSquidHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.giantSquidSpeed = EditorGUILayout.FloatField(gameSettings.giantSquidSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.giantSquidMoneyCarried = EditorGUILayout.IntField(gameSettings.giantSquidMoneyCarried, GUILayout.Width(50));

                GUILayout.EndHorizontal(); 
                
                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.GiantSquid]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Elder Turtle Settings

            elderTurtleEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(elderTurtleEnemySettingsFoldout, "Elder Turtle Settings");

            if (elderTurtleEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.elderTurtleHealth = EditorGUILayout.IntField(gameSettings.elderTurtleHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.elderTurtleDamage = EditorGUILayout.FloatField(gameSettings.elderTurtleDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.elderTurtleSpeed = EditorGUILayout.FloatField(gameSettings.elderTurtleSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.elderTurtleAttackSpeed = EditorGUILayout.FloatField(gameSettings.elderTurtleAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Special State Duration
                GUILayout.Label("Shell Ability Duration", GUILayout.Width(150));
                gameSettings.specialStateDuration = EditorGUILayout.FloatField(gameSettings.specialStateDuration, GUILayout.Width(50));

                GUILayout.Space(15);

                // Shell Ability DPS Threshold
                GUILayout.Label("Shell Ability DPS Threshold", GUILayout.Width(200));
                gameSettings.shellAbilityDPSThreshold = EditorGUILayout.FloatField(gameSettings.shellAbilityDPSThreshold, GUILayout.Width(50));

                GUILayout.Space(15);

                // Shell Ability Cooldown
                GUILayout.Label("Shell Ability Cooldown", GUILayout.Width(150));
                gameSettings.shellAbilityCooldown = EditorGUILayout.FloatField(gameSettings.shellAbilityCooldown, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.elderTurtleMoneyCarried = EditorGUILayout.IntField(gameSettings.elderTurtleMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.ElderTurtle]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Larva Settings

            larvaEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(larvaEnemySettingsFoldout, "Larva Settings");

            if (larvaEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.larvaHealth = EditorGUILayout.IntField(gameSettings.larvaHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.larvaDamage = EditorGUILayout.FloatField(gameSettings.larvaDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.larvaSpeed = EditorGUILayout.FloatField(gameSettings.larvaSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.larvaAttackSpeed = EditorGUILayout.FloatField(gameSettings.larvaAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.larvaMoneyCarried = EditorGUILayout.IntField(gameSettings.larvaMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Larva]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Witch Settings

            witchEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(witchEnemySettingsFoldout, "Witch Settings");

            if (witchEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.witchHealth = EditorGUILayout.IntField(gameSettings.witchHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.witchDamage = EditorGUILayout.FloatField(gameSettings.witchDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.witchSpeed = EditorGUILayout.FloatField(gameSettings.witchSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.witchAttackSpeed = EditorGUILayout.FloatField(gameSettings.witchAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Range
                GUILayout.Label("Attack Range", GUILayout.Width(100));
                gameSettings.witchAttackRange = EditorGUILayout.FloatField(gameSettings.witchAttackRange, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.witchMoneyCarried = EditorGUILayout.IntField(gameSettings.witchMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Witch]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region

            lizardEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(lizardEnemySettingsFoldout, "Lizard Settings");

            if (lizardEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.lizardHealth = EditorGUILayout.IntField(gameSettings.lizardHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.lizardDamage = EditorGUILayout.FloatField(gameSettings.lizardDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.lizardSpeed = EditorGUILayout.FloatField(gameSettings.lizardSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Attack Interval
                GUILayout.Label("Attack Interval", GUILayout.Width(100));
                gameSettings.lizardAttackSpeed = EditorGUILayout.FloatField(gameSettings.lizardAttackSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.lizardMoneyCarried = EditorGUILayout.IntField(gameSettings.lizardMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.Lizard]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            GUILayout.Space(15);

            #region Gull Settings

            bombBatEnemySettingsFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(bombBatEnemySettingsFoldout, "Bomb Bat Settings");

            if (bombBatEnemySettingsFoldout)
            {
                GUILayout.Space(15);

                GUILayout.BeginHorizontal();
                // Health
                GUILayout.Label("Health", GUILayout.Width(50));
                gameSettings.bombBatHealth = EditorGUILayout.IntField(gameSettings.bombBatHealth, GUILayout.Width(50));

                GUILayout.Space(15);

                // Speed
                GUILayout.Label("Speed", GUILayout.Width(50));
                gameSettings.bombBatSpeed = EditorGUILayout.FloatField(gameSettings.bombBatSpeed, GUILayout.Width(50));

                GUILayout.Space(15);

                // Damage
                GUILayout.Label("Damage", GUILayout.Width(50));
                gameSettings.bombBatDamage = EditorGUILayout.FloatField(gameSettings.bombBatDamage, GUILayout.Width(50));

                GUILayout.Space(15);

                // Explosion Radius
                GUILayout.Label("Explosion Radius", GUILayout.Width(100));
                gameSettings.bombBatExplosionRadius = EditorGUILayout.FloatField(gameSettings.bombBatExplosionRadius, GUILayout.Width(50));

                GUILayout.Space(15);

                // Money Carried
                GUILayout.Label("Money Carried", GUILayout.Width(90));
                gameSettings.bombBatMoneyCarried = EditorGUILayout.IntField(gameSettings.bombBatMoneyCarried, GUILayout.Width(50));
                GUILayout.EndHorizontal();

                GUILayout.Label($"Difficulty Rating : {enemyDifficultyRatings[EnemyType.BombBat]}");
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            EditorGUILayout.EndScrollView();
        }

        private float CalculateCatapultDPS(float damagePerProjectile, float attackSpeed, int numberOfProjectiles)
        {
            return (damagePerProjectile * numberOfProjectiles) / attackSpeed;
        }

    }
}
#endif