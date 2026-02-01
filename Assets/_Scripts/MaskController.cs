using System;
using System.Collections.Generic;
using DG.Tweening;
using GGJ.Managers;
using GGJ.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GGJ.Controllers
{
    public class MaskController : MonoBehaviour
    {
        private const float DURATION = 0.7f;
        
        [SerializeField] public GameObject Components;
        [SerializeField] public SpriteRenderer Accessory;
        [SerializeField] public SpriteRenderer Hat;
        [SerializeField] public SpriteRenderer Eyes;
        [SerializeField] public SpriteRenderer Nose;
        [SerializeField] public SpriteRenderer Mouth;
        [SerializeField] public SpriteRenderer FaceType;
        [SerializeField] public SpriteRenderer Ears;
        [SerializeField] public Canvas Canvas;
        [SerializeField] public TextMeshProUGUI Dialogue;
        [SerializeField] public Image SpeachBubble;
        [SerializeField] public GameObject PotentialMaskPosition;

        public Tween Tween => _tween;
        [HideInInspector] public int Id;
        [HideInInspector] public float Amplitude = 0.025f;
        [HideInInspector] public string DialogueKnowledgeType;
        [HideInInspector] public List<SpriteRenderer> MaskComponents;
        [HideInInspector] public List<TextMeshProUGUI> WhatTheySaidAboutYou;
        
        public string PersonalityType
        {
            get => _personalityType;
            set => _personalityType = value;
        }

        public bool IsKiller
        {
            get => _isKiller;
            set => _isKiller = value;
        }

        public string Role
        {
            get => _role;
            set => _role = value;
        }

        public bool IsTruthful
        {
            get => _isTruthful;
            set => _isTruthful = value;
        }
        
        public string Voice
        {
            get => _voice;
            set => _voice = value;
        }
        
        public string PlayerVerdict
        {
            get => _playerVerdict;
            set => _playerVerdict = value;
        }
        
        public string AtWhatTime
        {
            get => _atWhatTime;
            set => _atWhatTime = value;
        }
        
        public string WhereWereThey
        {
            get => _whereWereThey;
            set => _whereWereThey = value;
        }

        public bool Onscreen => _onScreen;
        public bool WasOnScreen => _wasOnScreen;
        public bool Clickable;
        public Action<MaskController> OnMaskClicked;

        private string _personalityType;
        private bool _isKiller;
        private string _role;
        private bool _isTruthful;
        private string _voice;
        private string _playerVerdict = "Innocent";
        private Tween _tween;
        private bool _onScreen;
        private bool _wasOnScreen;
        private Camera _camera;
        private string _atWhatTime;
        private string _whereWereThey;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame && Clickable && !ScreenFader.Instance.IsTweening)
            {
                var worldPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        OnMaskClicked?.Invoke(this);
                    }
                }
            }
        }

        public void OnScreen()
        {
            _tween = Components.transform.DOMoveY(transform.position.y + Amplitude, DURATION);
            switch (PersonalityType)
            {
                case "Nervous":
                    _tween.SetEase(Ease.Flash);
                    break;
                case "Calm":
                    _tween.SetEase(Ease.InOutSine);
                    break;
                case "Confident":
                    _tween.SetEase(Ease.InBounce);
                    break;
                case "Aggressive":
                    _tween.SetEase(Ease.InOutFlash);
                    break;
                case "Shady":
                    _tween.SetEase(Ease.InOutCirc);
                    break;
            }
            _tween.SetLoops(-1, LoopType.Yoyo);
            _onScreen = true;
            _wasOnScreen = true;
        }

        public void OffScreen()
        {
            _tween.Kill();
            _onScreen = false;
        }
    }
}