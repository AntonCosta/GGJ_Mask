using System;
using System.Collections.Generic;
using GGJ.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Controllers
{
    public class NotepadController : MonoBehaviour
    {
        [SerializeField] private GameObject _notepad;
        [SerializeField] private SpriteRenderer _notepadSprite;
        [SerializeField] private GameObject _arrowRight;
        [SerializeField] private GameObject _arrowLeft;
        [SerializeField] private List<Sprite> _notepads;
        
        private Camera _camera;
        private int _notepadIndex = 0;

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
                }
            }
        }
    }
}