using System;
using DG.Tweening;
using GGJ.Managers;
using GGJ.Utils;
using GJJ.Managers;
using UnityEngine;

namespace GGJ.Controllers
{
    public class ConvictionPhaseController : MonoBehaviour
    {
        [SerializeField] private CircularLayout _circularLayout;

        public void MoveMasksToConviction()
        {
            MaskManager.Instance.Masks.ForEach(mask =>
            {
                mask.transform.SetParent(transform, false);
                mask.transform.localPosition = Vector3.zero;
                mask.Canvas.gameObject.SetActive(false);
                mask.Tween.Kill();
                mask.Amplitude = 0.01f;
                mask.OnScreen();
            });
            _circularLayout.Arrange();
        }
    }
}