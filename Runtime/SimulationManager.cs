using Koala.Simulation.Chronos;
using Koala.Simulation.Input;
using Koala.Simulation.Placement;
using UnityEngine;
using UnityEngine.InputSystem;
using Koala.Simulation.Inventory;

namespace Koala.Simulation
{
    [AddComponentMenu("Simulation/Simulation Manager")]
    [Icon("Packages/com.koala.simulation/Editor/Icons/koala_important.png")]
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-100)]
    public class SimulationManager : MonoBehaviour
    {
        public static SimulationManager Singleton { get; private set; }

        [SerializeField, Tooltip("Reference to the prefab database that holds all registered prefabs.")]
        private PrefabRegistry _prefabRegistry;

        [SerializeField, Tooltip("If true, containers will be loaded from save data.")]
        private bool _autoLoadContainers = true;

        [SerializeField, Tooltip("Main camera used by the placement system. If left empty, Camera.main will be used.")]
        private Camera _mainCamera;

        [SerializeField, Tooltip("Maximum range allowed for object placement.")]
        private float _placementRange = 10f;

        [SerializeField, Tooltip("Step angle (in degrees) used when rotating objects during placement.")]
        private float _placementRotationStep = 15f;

        [SerializeField, Tooltip("Smoothing speed applied to object position/rotation during placement.")]
        private float _placementSmoothSpeed = 5f;

        [SerializeField, Tooltip("Length of one in-game day in real-world minutes.")]
        private int _cycleLengthInMinutes = 60;

        [SerializeField, Tooltip("Hour of the day when daytime begins (e.g., 6 = 06:00 AM).")]
        private int _dayStartHour = 6;

        [SerializeField, Tooltip("Hour of the day when nighttime begins (e.g., 18 = 06:00 PM).")]
        private int _nightStartHour = 18;

        [SerializeField, Tooltip("If true, only the date (Year/Month/Day) will be loaded from save data. Time will reset to default morning values.")]
        private bool _dateOnlyLoad = true;

        [SerializeField, Tooltip("Reference to the input action asset that holds all registered input actions.")]
        private InputActionAsset _inputActionAsset;

        [SerializeField, Tooltip("Default action map name in the input action asset.")]
        private string _defaultActionMap = "Gameplay";

        [SerializeField, Tooltip("Reference to the input sprite asset that holds all registered input sprites.")]
        private InputSpriteAsset _inputSpriteAsset;

        private PlacementManager _placementManager;
        private SimulationTime _simulationTime;
        private InputManager _inputManager;
        private InventoryFactory _inventoryFactory;

        #region Initialization
        private void Awake()
        {
            if (Singleton != null && Singleton != this)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            DontDestroyOnLoad(gameObject);

            InitPlacement();
            InitSimulationTime();
            InitInputManager();
            InitInventory();
        }

        private void InitInventory()
        {
            _inventoryFactory = new InventoryFactory(_prefabRegistry);
            if (_autoLoadContainers)
                InventoryBootstrapper.Initialize();
        }

        private void InitPlacement()
        {
            if (_mainCamera == null)
                _mainCamera = Camera.main;

            _placementManager = new PlacementManager(_mainCamera, _placementRange, _placementRotationStep, _placementSmoothSpeed);
        }

        private void InitSimulationTime()
        {
            _simulationTime = new SimulationTime(_cycleLengthInMinutes, _dayStartHour, _nightStartHour);
            _simulationTime.Load(_dateOnlyLoad);
        }

        private void InitInputManager()
        {
            _inputManager = new InputManager(_inputActionAsset, _defaultActionMap, _inputSpriteAsset);
        }
        #endregion

        #region Unity Callbacks
        private void Update()
        {
            _placementManager?.Update();
            _simulationTime?.Update(Time.deltaTime);
        }

        private void OnDestroy()
        {
            _placementManager?.Dispose();
            _simulationTime?.Dispose();
            _inputManager?.Dispose();
            _inventoryFactory?.Dispose();
        }

        private void OnApplicationQuit()
        {
            InventorySaver.SaveAll();
            _simulationTime?.Save();
        }
        #endregion
    }
}