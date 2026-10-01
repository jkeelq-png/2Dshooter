using UnityEngine;
using UnityEngine.InputSystem;

namespace Tanks2D
{
    public class PlayerController2D : MonoBehaviour
    {
        public enum ModelForwardAxis { X_Axis, Y_Axis, Z_Axis }

        [Header("References (Ссылки)")]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _bulletPrefab;

        [Header("Weapon Settings (Настройки оружия)")]
        [SerializeField] private float _bulletSpeed = 20f;
        
        [Tooltip("Задержка между выстрелами в секундах (сделали чуть медленнее, чтобы не было автомата)")]
        [SerializeField] private float _fireRate = 0.4f; 

        [Header("Ammo & Reload Settings (Обойма и Перезарядка)")]
        [SerializeField] private int _maxAmmo = 15; // Размер обоймы
        [SerializeField] private float _baseReloadTime = 2.0f; // Начальное время перезарядки (сек)

        [Header("3D Model Setup")]
        [SerializeField] private ModelForwardAxis _forwardAxis = ModelForwardAxis.Z_Axis;

        [Header("Firing Direction")]
        [SerializeField] private bool _shootRightToLeft = true;

        private float _nextFireTime = 0f;
        private Camera _mainCamera;

        // ВНУТРЕННИЕ ПЕРЕМЕННЫЕ
        private int _currentAmmo;
        private float _reloadEndTime = 0f;
        private bool _isReloading = false;

        // ГЛОБАЛЬНАЯ статическая переменная: текущее время перезарядки (его будет улучшать магазин)
        public static float CurrentReloadTime { get; set; } = 2.0f;

        // Для отображения в UI (по желанию)
        public int CurrentAmmo => _currentAmmo;
        public bool IsReloading => _isReloading;

        private void Start()
        {
            _mainCamera = Camera.main;
            CurrentReloadTime = _baseReloadTime; // Присваиваем базовое время
            _currentAmmo = _maxAmmo; // Заряжаем полную обойму
        }

        private void Update()
        {
            if (ShopController.IsPaused) return;

            RotatePlayerTowardsMouse();

            // Проверка: завершилась ли перезарядка?
            if (_isReloading && Time.time >= _reloadEndTime)
            {
                _currentAmmo = _maxAmmo;
                _isReloading = false;
                Debug.Log("[Weapon] Перезарядка окончена! Обойма полная.");
            }
            
            // Стрельба по зажатию левой кнопки мыши
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                // Стрелять можно, только если не идет перезарядка и пришло время следующего выстрела
                if (!_isReloading && Time.time >= _nextFireTime)
                {
                    if (_currentAmmo > 0)
                    {
                        Shoot();
                        _currentAmmo--;
                        _nextFireTime = Time.time + _fireRate;
                        Debug.Log($"[Weapon] Выстрел! Патронов осталось: {_currentAmmo}/{_maxAmmo}");

                        // Если это был последний патрон — автоматически запускаем перезарядку
                        if (_currentAmmo <= 0)
                        {
                            StartReload();
                        }
                    }
                }
            }

            // Ручная перезарядка на клавишу R (опционально, через старый инпут или проверку GetKey)
            if (Input.GetKeyDown(KeyCode.R) && !_isReloading && _currentAmmo < _maxAmmo)
            {
                StartReload();
            }
        }

        private void StartReload()
        {
            _isReloading = true;
            // Время окончания перезарядки считается по актуальному значению из магазина!
            _reloadEndTime = Time.time + CurrentReloadTime;
            Debug.Log($"[Weapon] Перезарядка... Ждем {CurrentReloadTime} сек.");
        }

        private void RotatePlayerTowardsMouse()
        {
            if (_mainCamera == null) return;

            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, _mainCamera.nearClipPlane));
            mouseWorldPos.z = 0f;

            Vector3 direction = (mouseWorldPos - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            if (_shootRightToLeft)
            {
                if (angle > 0f) angle = Mathf.Clamp(angle, 90f, 180f);
                else angle = Mathf.Clamp(angle, -180f, -90f);
            }
            else
            {
                angle = Mathf.Clamp(angle, -90f, 90f);
            }

            switch (_forwardAxis)
            {
                case ModelForwardAxis.X_Axis:
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    break;
                case ModelForwardAxis.Y_Axis:
                    transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
                    break;
                case ModelForwardAxis.Z_Axis:
                    transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                    break;
            }
        }

        private void Shoot()
        {
            if (_bulletPrefab == null || _firePoint == null) return;

            GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
            
            Vector3 safePos = bullet.transform.position;
            safePos.z = 0f;
            bullet.transform.position = safePos;

            BulletMovement2D bulletScript = bullet.GetComponent<BulletMovement2D>();
            if (bulletScript == null)
            {
                bulletScript = bullet.AddComponent<BulletMovement2D>();
            }
            
            bulletScript.Initialize(_bulletSpeed);
        }
    }
}
