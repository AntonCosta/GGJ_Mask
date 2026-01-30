using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Utils
{
    public class ScreenFader : MonoBehaviour
    {
        private const float FADE_TIME = 0.5f;

        public static ScreenFader Instance { get; private set; }
        [SerializeField] private Image _fadeImage;

        private void Awake()
        {
            _fadeImage.color = new Color(0, 0, 0, 0);

            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public Tween FadeOutToBlack()
        {
            return _fadeImage.DOFade(1f, FADE_TIME).SetUpdate(true);
        }

        public Tween FadeInFromBlack()
        {
            return _fadeImage.DOFade(0f, FADE_TIME).SetUpdate(true);
        }

        public void FadeHide(Action work)
        {
            FadeOutToBlack()
                .OnComplete(() =>
                {
                    work?.Invoke();
                    FadeInFromBlack();
                });
        }
    }
}