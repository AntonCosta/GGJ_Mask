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
        }

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Keyboard.current.dKey.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                ScreenFader.Instance.FadeHide(MaskManager.Instance.NextMask);
            }

            if (Keyboard.current.aKey.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                ScreenFader.Instance.FadeHide(MaskManager.Instance.PreviousMask);
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                var worldPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == _arrowRight)
                    {
                        ScreenFader.Instance.FadeHide(MaskManager.Instance.NextMask);
                    }
                    else if(hit.collider.gameObject == _arrowLeft)
                    {
                        ScreenFader.Instance.FadeHide(MaskManager.Instance.PreviousMask);
                    }
                }
            }
        }
    }
}