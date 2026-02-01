using System;
using GGJ.Utils;
using GJJ.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] public GameObject Navigation;
        [SerializeField] private GameObject _arrowRight;
        [SerializeField] private GameObject _arrowLeft;

        [Header("Audio")]
        [SerializeField] private AudioManager audioManager;

        public static PlayerController Instance { get; private set; }

        private Camera _camera;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (audioManager == null)
                audioManager = FindFirstObjectByType<AudioManager>();
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        void CycleNext()
        {
            if (audioManager != null) audioManager.PlayNpcCycle();
            ScreenFader.Instance.FadeHide(MaskManager.Instance.NextMask);
        }

        void CyclePrevious()
        {
            if (audioManager != null) audioManager.PlayNpcCycle();
            ScreenFader.Instance.FadeHide(MaskManager.Instance.PreviousMask);
        }

        private void Update()
        {
            if (Keyboard.current.dKey.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                CycleNext();
            }

            if (Keyboard.current.aKey.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                CyclePrevious();
            }

            if (Mouse.current.leftButton.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                var worldPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == _arrowRight)
                    {
                        CycleNext();
                    }
                    else if (hit.collider.gameObject == _arrowLeft)
                    {
                        CyclePrevious();
                    }
                }
            }
        }
    }
}
