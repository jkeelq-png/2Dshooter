using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tanks2D
{
    public class EnemySpawner2D : MonoBehaviour
    {
        [System.Serializable]
        public class EnemySpawnConfig
        {
            public string enemyName;
            public GameObject enemyPrefab;
            [Range(0, 100)] public int spawnChance = 50;
            [Min(0)] public int goldReward = 10;
        }

        [Header("References (Ссылки)")]
        [SerializeField] private EnemySpawnConfig[] _enemiesConfigs;

        [Header("Spawn Settings (Настройки появления)")]
        [SerializeField] private float _spawnRate = 2f;

        [Header("Spawn Zone Boundaries (Границы зоны спавна)")]
        [SerializeField] private float _minY = -4f;
        [SerializeField] private float _maxY = 4f;

        [Header("Boss Settings (Настройки Босса)")]
        [Tooltip("Префаб вашего Босса")]
        [SerializeField] private GameObject bossPrefab;
        [Tooltip("Сколько обычных врагов нужно убить, чтобы пришел Босс")]
        [SerializeField] private int killsNeededForBoss = 10;
        [Tooltip("Сколько золота дадут за убийство Босса")]
        [SerializeField] private int bossGoldReward = 100;

        [Header("Transition Settings (Переход после победы)")]
        [Tooltip("Имя сцены лагеря, куда перенесет после победы над боссом")]
        [SerializeField] private string campSceneName = "Camp";
        [Tooltip("Задержка перед переходом в лагерь после смерти босса (в секундах)")]
        [SerializeField] private float delayBeforeCamp = 3f;

        [Header("Music Settings (Настройки музыки)")]
        [SerializeField] private AudioSource musicAudioSource;
        [SerializeField] private AudioClip normalWaveMusic;
        [SerializeField] private AudioClip bossMusic;

        private float _nextSpawnTime = 0f;
        private Wall _cachedWallOnScene;

        private int _currentKillsCount = 0;
        private bool _isBossSpawned = false;
        private bool _isBossDefeated = false;

        public static EnemySpawner2D Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _cachedWallOnScene = Object.FindFirstObjectByType<Wall>();
            if (_cachedWallOnScene == null)
            {
                Debug.LogError("[EnemySpawner2D] Ошибка: Спавнер не смог найти объект со скриптом Wall на сцене!");
            }

            if (musicAudioSource != null && normalWaveMusic != null)
            {
                musicAudioSource.clip = normalWaveMusic;
                musicAudioSource.loop = true;
                musicAudioSource.Play();
            }
        }

        private void Update()
        {
            if (ShopController.IsPaused) return;
            if (_isBossSpawned) return;
            if (_enemiesConfigs == null || _enemiesConfigs.Length == 0) return;

            if (Time.time >= _nextSpawnTime)
            {
                SpawnEnemy();
                _nextSpawnTime = Time.time + _spawnRate;
            }
        }

        private void SpawnEnemy()
        {
            EnemySpawnConfig selectedConfig = GetRandomEnemyConfig();
            if (selectedConfig == null || selectedConfig.enemyPrefab == null) return;

            float randomY = Random.Range(_minY, _maxY);
            Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0f);

            GameObject newEnemy = Instantiate(selectedConfig.enemyPrefab, spawnPosition, Quaternion.identity);

            Vector3 safePos = newEnemy.transform.position;
            safePos.z = 0f;
            newEnemy.transform.position = safePos;

            PigEnemy enemyScript = newEnemy.GetComponent<PigEnemy>();
            if (enemyScript != null)
            {
                enemyScript.Initialize(selectedConfig.goldReward, false); // Передаем, что это НЕ босс
                enemyScript.SetTargetWall(_cachedWallOnScene);
            }
        }

        // ИСПРАВЛЕНО: Добавлен параметр isBoss. Теперь метод точно знает, кто погиб
        public void RegisterEnemyDeath(bool isBoss)
        {
            if (isBoss)
            {
                if (!_isBossDefeated)
                {
                    _isBossDefeated = true;
                    Debug.Log("[Spawner] НАСТОЯЩИЙ БОСС ПОВЕРЖЕН! Переход в лагерь...");
                    Invoke(nameof(LoadCampScene), delayBeforeCamp);
                }
            }
            else
            {
                // Если умер обычный моб, мы просто двигаем счетчик (даже если босс уже пришел, этот моб не активирует победу)
                if (!_isBossSpawned)
                {
                    _currentKillsCount++;
                    Debug.Log($"[Spawner] Обычный враг повержен! Всего убито: {_currentKillsCount} / {killsNeededForBoss}");

                    if (_currentKillsCount >= killsNeededForBoss)
                    {
                        SpawnBoss();
                    }
                }
            }
        }

        private void SpawnBoss()
        {
            if (bossPrefab == null)
            {
                Debug.LogError("[EnemySpawner2D] Префаб Босса не назначен в инспекторе спавнера!");
                return;
            }

            _isBossSpawned = true;
            Debug.Log("[Spawner] ВНИМАНИЕ! ПОЯВИЛСЯ БОСС!");

            if (musicAudioSource != null && bossMusic != null)
            {
                musicAudioSource.Stop();
                musicAudioSource.clip = bossMusic;
                musicAudioSource.loop = true;
                musicAudioSource.Play();
            }

            Vector3 spawnPosition = new Vector3(transform.position.x, 0f, 0f);
            GameObject bossGo = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);

            PigEnemy bossScript = bossGo.GetComponent<PigEnemy>();
            if (bossScript != null)
            {
                bossScript.Initialize(bossGoldReward, true); // ИСПРАВЛЕНО: Передаем true, помечая, что это БОСС
                bossScript.SetTargetWall(_cachedWallOnScene);
            }
        }

        private void LoadCampScene()
        {
            Debug.Log($"[Spawner] Загрузка сцены: {campSceneName}");
            SceneManager.LoadScene(campSceneName);
        }

        private EnemySpawnConfig GetRandomEnemyConfig()
        {
            int totalWeight = 0;
            foreach (var config in _enemiesConfigs)
            {
                if (config.enemyPrefab != null && config.spawnChance > 0)
                {
                    totalWeight += config.spawnChance;
                }
            }

            if (totalWeight == 0) return null;

            int randomWeight = Random.Range(0, totalWeight);
            int currentWeightSum = 0;

            foreach (var config in _enemiesConfigs)
            {
                if (config.enemyPrefab == null || config.spawnChance <= 0) continue;

                currentWeightSum += config.spawnChance;
                if (randomWeight < currentWeightSum)
                {
                    return config;
                }
            }

            return null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 top = new Vector3(transform.position.x, _maxY, 0f);
            Vector3 bottom = new Vector3(transform.position.x, _minY, 0f);
            Gizmos.DrawLine(top, bottom);
        }
    }
}
