using System;
using DG.Tweening;
using GGJ.Managers;
using GGJ.Utils;
using GJJ.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Controllers
{
    public class ConvictionPhaseController : MonoBehaviour
    {
        [SerializeField] private CircularLayout _circularLayout;
        [SerializeField] private GameObject _maskFile;
        [SerializeField] private GameObject _background;

        private MaskFileController _maskFileController;

        private void Start()
        {
            _maskFileController = _maskFile.GetComponent<MaskFileController>();
        }

        public void MoveMasksToConviction()
        {
            MaskManager.Instance.Masks.ForEach(mask =>
            {
                mask.transform.SetParent(transform, false);
                mask.transform.localPosition = Vector3.zero;
                mask.Canvas.gameObject.SetActive(false);
                mask.Tween.Kill();
                mask.Clickable = true;
                mask.OnMaskClicked -= PopulateMaskFile;
                mask.OnMaskClicked += PopulateMaskFile;
            });
            _circularLayout.Arrange();

            MaskManager.Instance.Masks.ForEach(mask =>
            {
                mask.Amplitude = 0.01f;
                mask.OnScreen();
            });
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame && _maskFile.activeSelf &&
                !ScreenFader.Instance.IsTweening)
            {
                ScreenFader.Instance.FadeHide(() =>
                {
                    _maskFile.SetActive(false);
                    _background.SetActive(true);
                    MaskManager.Instance.Masks.ForEach(mask =>
                    {
                        mask.gameObject.SetActive(true);
                        mask.transform.localScale = new Vector3(1f, 1f, 1f);
                    });

                    _circularLayout.Arrange();
                    MaskManager.Instance.Masks.ForEach(mask =>
                    {
                        mask.Amplitude = 0.01f;
                        mask.OnScreen();
                    });
                });
            }
        }

        public void PopulateMaskFile(MaskController mask)
        {
            ScreenFader.Instance.FadeHide(() =>
            {
                MaskManager.Instance.Masks.ForEach(otherMask =>
                {
                    otherMask.gameObject.SetActive(false);
                    otherMask.Tween.Kill();
                });
                mask.gameObject.SetActive(true);
                _maskFile.SetActive(true);
                _background.SetActive(false);
                _maskFileController.ConfessionText.text = mask.Dialogue.text;
                _maskFileController.OthersText.text = mask.Dialogue.text; //TODO de uitat in dialog la toate mastile si sa vedem cine zice de X
                mask.transform.localPosition = _maskFileController.MaskPosition.transform.localPosition;
                mask.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                switch (mask.PlayerVerdict)
                {
                    case "Guilty":
                        _maskFileController.GuiltyMark.SetActive(true);
                        _maskFileController.SuspiciousMark.SetActive(false);
                        _maskFileController.InnocentMark.SetActive(false);
                        break;
                    case "Suspicious":
                        _maskFileController.SuspiciousMark.SetActive(true);
                        _maskFileController.GuiltyMark.SetActive(false);
                        _maskFileController.InnocentMark.SetActive(false);
                        break;
                    case "Innocent":
                        _maskFileController.InnocentMark.SetActive(true);
                        _maskFileController.GuiltyMark.SetActive(false);
                        _maskFileController.SuspiciousMark.SetActive(false);
                        break;
                    default:
                        _maskFileController.GuiltyMark.SetActive(false);
                        _maskFileController.SuspiciousMark.SetActive(false);
                        _maskFileController.InnocentMark.SetActive(false);
                        break;
                }
            });
        }
    }
}