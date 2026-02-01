using System;
using System.Collections.Generic;
using GGJ.Utils;
using GJJ.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Controllers
{
    public class NotepadController : MonoBehaviour
    {
        public static NotepadController Instance { get; private set; }

        [SerializeField] private GameObject _notepad;
        [SerializeField] private SpriteRenderer _notepadSprite;
        [SerializeField] private GameObject _arrowRight;
        [SerializeField] private GameObject _arrowLeft;
        [SerializeField] private List<Sprite> _notepads;
        [SerializeField] private AudioManager audioManager;

        
        private Camera _camera;
        private int _notepadIndex = 0;

        private void Awake()
        {
            if (audioManager == null)
                audioManager = FindObjectOfType<AudioManager>();

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
            if (Mouse.current.leftButton.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                var worldPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == _arrowRight)
                    {
                        _notepadIndex = _notepadIndex == _notepads.Count - 1 ? 0 : _notepadIndex + 1;
                        _notepadSprite.sprite = _notepads[_notepadIndex];
                    }
                    else if(hit.collider.gameObject == _arrowLeft)
                    {
                        _notepadIndex = _notepadIndex == 0 ? _notepads.Count - 1 : _notepadIndex - 1;
                        _notepadSprite.sprite = _notepads[_notepadIndex];
                    }

                    string oldVerdict = MaskManager.Instance.CurrentMask.PlayerVerdict;

                    if (_notepadIndex == 0)
                    {
                        MaskManager.Instance.CurrentMask.PlayerVerdict = "Innocent";
                    }
                    else if (_notepadIndex == 1)
                    {
                        MaskManager.Instance.CurrentMask.PlayerVerdict = "Guilty";
                    }
                    else if (_notepadIndex == 2)
                    {
                        MaskManager.Instance.CurrentMask.PlayerVerdict = "Suspicious";
                    }
                    
                    string newVerdict = MaskManager.Instance.CurrentMask.PlayerVerdict;

                    if (oldVerdict != newVerdict && audioManager != null)
                    {
                        audioManager.PlayUIWriting();
                    }

                }
            }
        }

        public void ChangeNotepad(string verdict)
        {
            switch (verdict)
            {
                case "Innocent":
                    _notepadIndex = 0;
                    break;
                case "Guilty":
                    _notepadIndex = 1;
                    break;
                case "Suspicious":
                    _notepadIndex = 2;
                    break;
            }
            _notepadSprite.sprite = _notepads[_notepadIndex];
        }
    }
}