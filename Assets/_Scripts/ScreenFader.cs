using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Utils
{
    public class ScreenFader : MonoBehaviour
    {
        private const float FADE_TIME = 0.7f;

        public static ScreenFader Instance { get; private set; }
        public bool IsTweening;
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

        public Tween FadeOutToBlack(float fade_time = FADE_TIME)
        {
            IsTweening = true;
            _fadeImage.gameObject.SetActive(true);
            return _fadeImage.DOFade(1f, fade_time).SetUpdate(true).OnComplete(() => IsTweening = false);
        }

        public Tween FadeInFromBlack(float fade_time = FADE_TIME)
        {
            IsTweening = true;
            return _fadeImage.DOFade(0f, fade_time).SetUpdate(true).OnComplete(() =>
            {
                _fadeImage.gameObject.SetActive(false);
                IsTweening = false;
            });
        }

        public void FadeHide(Action work, float fade_time = FADE_TIME)
        {
            IsTweening = true;
            FadeOutToBlack(fade_time)
                .OnComplete(() =>
                {
                    work?.Invoke();
                    FadeInFromBlack(fade_time);
                });
        }
        public void FadeHide(Action beforeFade, Action work, float fade_time = FADE_TIME)
        {
            IsTweening = true;

            // Do this immediately (play SFX, etc)
            beforeFade?.Invoke();

            FadeOutToBlack(fade_time)
                .OnComplete(() =>
                {
                    work?.Invoke();
                    FadeInFromBlack(fade_time);
                });
        }

    }
}