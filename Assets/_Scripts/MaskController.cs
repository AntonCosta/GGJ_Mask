using System;
using DG.Tweening;
using GGJ.Managers;
using TMPro;
using UnityEngine;
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

        public Tween Tween => _tween;
        public float Amplitude = 0.025f;
        
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
        
        public string PlayerVerdict
        {
            get => _playerVerdict;
            set => _playerVerdict = value;
        }

        public bool Onscreen => _onScreen;
        public bool WasOnScreen => _wasOnScreen;

        private string _personalityType;
        private bool _isKiller;
        private string _role;
        private string _playerVerdict;
        private Tween _tween;
        private bool _onScreen;
        private bool _wasOnScreen;

        public void OnScreen()
        {
            _tween = Components.transform.DOMoveY(transform.position.y + Amplitude, DURATION);
            switch (PersonalityType)
            {
                case "Calm":
                    _tween.SetEase(Ease.InOutSine);
                    break;
                case "Deceptive":
                    _tween.SetEase(Ease.InOutBounce);
                    break;
                case "Nervous":
                    _tween.SetEase(Ease.Flash);
                    break;
                case "Aggressive":
                    _tween.SetEase(Ease.InBounce);
                    break;
                case "Shady":
                    _tween.SetEase(Ease.InOutCirc);
                    break;
                case "Vague":
                    _tween.SetEase(Ease.Linear);
                    break;
            }
            _tween.SetLoops(-1, LoopType.Yoyo);
            Debug.Log(PersonalityType + " PersonalityType " + IsKiller + " IsKiller " + Role + " Role");
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