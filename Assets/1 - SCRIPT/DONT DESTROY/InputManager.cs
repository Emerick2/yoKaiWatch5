using UnityEngine;
using UnityEngine.InputSystem;

namespace YKW.Input
{
    public sealed class InputManager : MonoBehaviour
    {
        private static InputManager _instance;
        public static InputManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject inputObject = new GameObject("InputManager");
                    _instance = inputObject.AddComponent<InputManager>();
                    DontDestroyOnLoad(inputObject);
                    print("InputManager has been created");
                }

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null;

        private Input _input;

        public Input Input => _input;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _input = new Input();

            // Toujours actif.
            _input.Common.Enable();
        }

        /// <summary>
        /// Désactive automatiquement tous les Action Maps,
        /// sauf Common.
        /// </summary>
        public void DisableAll()
        {
            foreach (InputActionMap map in _input.asset.actionMaps)
            {
                if (map == _input.Common.Get())
                    continue;

                map.Disable();
            }
        }

        /// <summary>
        /// Désactive les autres contextes puis active celui demandé.
        /// Common reste actif.
        /// </summary>
        public void Enable(InputActionMap map)
        {
            DisableAll();
            map.Enable();
            print("Activation de:" + map);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _input?.Dispose();
                _instance = null;
            }
        }
    }
}
