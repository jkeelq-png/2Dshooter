using UnityEngine;
using UnityEngine.InputSystem;

namespace Tanks2D
{
    public class PlayerController2D : MonoBehaviour
    {
        public enum ModelForwardAxis { X_Axis, Y_Axis, Z_Axis }

        [Header("References (Ссылки)")]
        [SerializeField] 
        [Tooltip("Точка на конце оружия, откуда вылетает пуля")]
        private Transform _firePoint;
        
        [SerializeField] 
        [Tooltip("Префаб снаряда (пули)")]
        private GameObject _bulletPrefab;

        [Header("Weapon Settings (Настройки оружия)")]
        [SerializeField] private float _bulletSpeed = 20f;
        [SerializeField] private float _fireRate = 0.2f; 

        [Header("3D Model Setup (Настройка 3D Модели)")]
        [SerializeField, Tooltip("Какая ось вашей 3D-модели смотрит вперед (вдоль ствола оружия)? Обычно для 3D моделей это Z_Axis, для 2D спрайтов — X_Axis")]
        private ModelForwardAxis _forwardAxis = ModelForwardAxis.Z_Axis;

        [Header("Firing Direction (Направление стрельбы)")]
        [SerializeField, Tooltip("Включите эту галочку, если ваша стена находится СПРАВА, а враги бегут СЛЕВА (Стрельба справа налево)")]
        private bool _shootRightToLeft = true;

        private float _nextFireTime = 0f;
        private Camera _mainCamera;

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            RotatePlayerTowardsMouse();
            
            // Стрельба по зажатию левой кнопки мыши
            if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            {
                if (Time.time >= _nextFireTime)
                {
                    Shoot();
                    _nextFireTime = Time.time + _fireRate;
                }
            }
        }

        private void RotatePlayerTowardsMouse()
        {
            if (_mainCamera == null) return;

            // Получаем координаты мыши в игровом мире
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, _mainCamera.nearClipPlane));
            mouseWorldPos.z = 0f;

            // Вычисляем вектор направления от персонажа к мыши
            Vector3 direction = (mouseWorldPos - transform.position).normalized;

            // Считаем базовый угол в плоскости XY (возвращает от -180 до 180)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // ИСПРАВЛЕНО: Динамическое переключение зоны обзора 180 градусов
            if (_shootRightToLeft)
            {
                // Ограничиваем углы для ЛЕВОЙ половины экрана (от 90 до 180 и от -180 до -90)
                if (angle > 0f)
                {
                    angle = Mathf.Clamp(angle, 90f, 180f);
                }
                else
                {
                    angle = Mathf.Clamp(angle, -180f, -90f);
                }
            }
            else
            {
                // Ограничиваем углы для ПРАВОЙ половины экрана (от -90 до 90)
                angle = Mathf.Clamp(angle, -90f, 90f);
            }

            // Поворачиваем персонажа в зависимости от осей 3D-модели
            switch (_forwardAxis)
            {
                case ModelForwardAxis.X_Axis:
                    transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    break;

                case ModelForwardAxis.Y_Axis:
                    transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
                    break;

                case ModelForwardAxis.Z_Axis:
                    // Для 3D моделей разворот по Y инвертирует направление взгляда
                    transform.rotation = Quaternion.Euler(0f, -angle, 0f);
                    break;
            }
        }

        private void Shoot()
{
    if (_bulletPrefab == null || _firePoint == null) return;

    // Спавним пулю
    GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, _firePoint.rotation);
    
    // Защита от улетания по Z
    Vector3 safePos = bullet.transform.position;
    safePos.z = 0f;
    bullet.transform.position = safePos;

    // НОВОЕ: Находим или добавляем наш чистый скрипт движения и запускаем пулю!
    BulletMovement2D bulletScript = bullet.GetComponent<BulletMovement2D>();
    if (bulletScript == null)
    {
        bulletScript = bullet.AddComponent<BulletMovement2D>();
    }
    
    bulletScript.Initialize(_bulletSpeed);
}

    }
}