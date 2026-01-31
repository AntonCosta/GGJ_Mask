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
                mask.Amplitude = 0.001f;
                mask.Clickable = true;
                mask.OnScreen();
                mask.OnMaskClicked += PopulateMaskFile;
            });
            _circularLayout.Arrange();
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame && _maskFile.activeSelf)
            {
                ScreenFader.Instance.FadeHide(() =>
                {
                    _maskFile.SetActive(false);
                    _background.SetActive(true);
                    MaskManager.Instance.Masks.ForEach(mask =>
                    {
                        mask.OnScreen();
                    });
                    
                    _circularLayout.Arrange();
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
                mask.transform.position = _maskFileController.MaskPosition.transform.position;
                switch (mask.PlayerVerdict)
                {
                    case "Guilty":
                        _maskFileController.GuiltyMark.SetActive(true);
                        break;
                    case "Suspicious":
                        _maskFileController.SuspiciousMark.SetActive(true);
                        break;
                    case "Innocent":
                        _maskFileController.InnocentMark.SetActive(true);
                        break;
                    default:
                        break;
                }
            });
        }
    }
}