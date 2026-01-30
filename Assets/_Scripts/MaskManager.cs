using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGJ;
using GGJ.Controllers;
using GGJ.Managers;
using GGJ.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GJJ.Managers
{
    public class MaskManager : MonoBehaviour
    {
        private const float TWEEN_DURATION = 0.0f;
        private const float TRAVEL_DISTANCE = 30f;
        
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

            do
            {
                var guiltyIndex = Random.Range(0, Masks.Count);
                if (Masks[guiltyIndex].IsKiller) continue;
                Masks[guiltyIndex].IsKiller = true;
                Masks[guiltyIndex].Role = Constants.MASK_ROLES.Last();
                _nrOfKillerMasks++;
            } while (_nrOfKillerMasks < _maxNrOfKillerMasks);
            
            
            Masks.ForEach(mask =>
            {
                if (!mask.IsKiller)
                {
                    var roleIndex = (Random.Range(0, Constants.MASK_ROLES.Count - 1));
                    mask.Role = Constants.MASK_ROLES[roleIndex];
                    mask.IsKiller = false;
                }
            });
            Masks[0].OnScreen();
            DialogueManager.Instance.AddText(Masks);
        }

        public void NextMask()
        {
            IsTweening = true;
            _tweenSequence = DOTween.Sequence();
            if (_currentMaskIndex >= LevelGenerator.Instance.NrOfMasks - 1)
            {
                _currentMaskIndex = 0;
                Masks[_currentMaskIndex].OnScreen();
                Masks[LevelGenerator.Instance.NrOfMasks - 1].OffScreen();;
                for (var i = 0; i < LevelGenerator.Instance.NrOfMasks; i++)
                {
                    _tweenSequence.Join(
                        LevelGenerator.Instance.MaskPositions[i].transform
                            .DOMoveX(TRAVEL_DISTANCE * i, TWEEN_DURATION).SetEase(Ease.InOutSine));
                }

                _tweenSequence.OnComplete(() => IsTweening = false);

                return;
            }

            _currentMaskIndex++;
            Masks[_currentMaskIndex].OnScreen();
            Masks[_currentMaskIndex - 1].OffScreen();
            LevelGenerator.Instance.MaskPositions.ForEach(mask =>
            {
                _tweenSequence.Join(
                    mask.transform.DOMoveX(mask.transform.position.x - TRAVEL_DISTANCE, TWEEN_DURATION).SetEase(Ease.InOutSine));
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
                Masks[_currentMaskIndex].OnScreen();
                Masks[0].OffScreen();
                for (var i = 0; i < LevelGenerator.Instance.NrOfMasks; i++)
                {
                    _tweenSequence.Join(
                        LevelGenerator.Instance.MaskPositions[i].transform
                            .DOMoveX(0 - TRAVEL_DISTANCE * (LevelGenerator.Instance.NrOfMasks - 1 - i), TWEEN_DURATION)
                            .SetEase(Ease.InOutSine));
                }

                _tweenSequence.OnComplete(() => IsTweening = false);

                return;
            }

            _currentMaskIndex--;
            Masks[_currentMaskIndex].OnScreen();
            Masks[_currentMaskIndex + 1].OffScreen();
            LevelGenerator.Instance.MaskPositions.ForEach(mask =>
            {
                _tweenSequence.Join(
                    mask.transform.DOMoveX(mask.transform.position.x + TRAVEL_DISTANCE, TWEEN_DURATION).SetEase(Ease.InOutSine));
            });
            _tweenSequence.OnComplete(() => IsTweening = false);
        }
    }
}