using System;
using System.Collections.Generic;
using DG.Tweening;
using GGJ;
using GGJ.Controllers;
using GGJ.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GJJ.Managers
{
    public class MaskManager : MonoBehaviour
    {
        private const float TWEEN_DURATION = 0.3f;
        
        public static MaskManager Instance { get; private set; }
        public List<MaskController> Masks;
        public bool IsTweening;
        
        private int _currentMaskIndex = 0;
        private Sequence _tweenSequence;
        private int _nrOfKillerMasks = 0;
        private int _maxNrOfKillerMasks = 1;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void AllMasksAdded()
        {
            Masks.ForEach(mask =>
            {
                mask.PersonalityType = Constants.MASK_PERSONALITY_TYPES.RandomElement();
            });
        }

        public void NextMask()
        {
            IsTweening = true;
            _tweenSequence = DOTween.Sequence();
            if (_currentMaskIndex >= LevelGenerator.Instance.NrOfMasks - 1)
            {
                _currentMaskIndex = 0;
                for (var i = 0; i < LevelGenerator.Instance.NrOfMasks; i++)
                {
                    // var newPos = new Vector3(0 + 300f * i, 0, 0);
                    // LevelGenerator.Instance.MaskPositions[i].transform.position = newPos;

                    _tweenSequence.Join(
                        LevelGenerator.Instance.MaskPositions[i].transform
                            .DOMoveX(300f * i, TWEEN_DURATION).SetEase(Ease.InOutSine));
                }

                _tweenSequence.OnComplete(() => IsTweening = false);

                return;
            }

            _currentMaskIndex++;
            LevelGenerator.Instance.MaskPositions.ForEach(mask =>
            {
                _tweenSequence.Join(
                    mask.transform.DOMoveX(mask.transform.position.x - 300f, TWEEN_DURATION).SetEase(Ease.InOutSine));
            });
            _tweenSequence.OnComplete(() => IsTweening = false);
        }

        public void PreviousMask()
        {
            IsTweening = true;
            _tweenSequence = DOTween.Sequence();
            if (_currentMaskIndex <= 0)
            {
                _currentMaskIndex = LevelGenerator.Instance.NrOfMasks - 1;
                for (var i = 0; i < LevelGenerator.Instance.NrOfMasks; i++)
                {
                    // var newPos = new Vector3(0 - 300f * (LevelGenerator.Instance.NrOfMasks - 1 - i), 0, 0);
                    // LevelGenerator.Instance.MaskPositions[i].transform.position = newPos;
                    _tweenSequence.Join(
                        LevelGenerator.Instance.MaskPositions[i].transform
                            .DOMoveX(0 - 300f * (LevelGenerator.Instance.NrOfMasks - 1 - i), TWEEN_DURATION)
                            .SetEase(Ease.InOutSine));
                }

                _tweenSequence.OnComplete(() => IsTweening = false);

                return;
            }

            _currentMaskIndex--;
            LevelGenerator.Instance.MaskPositions.ForEach(mask =>
            {
                _tweenSequence.Join(
                    mask.transform.DOMoveX(mask.transform.position.x + 300f, TWEEN_DURATION).SetEase(Ease.InOutSine));
            });
            _tweenSequence.OnComplete(() => IsTweening = false);
        }
    }
}