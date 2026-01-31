using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using GGJ;
using GGJ.Contollers;
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
        private int _maxNumberOfLiers = 2;
        private int _maxNumberOfAggresive = 1;
        private int _maxNumberOfShady = 1;
        private int _currentNrLiers = 0;
        private int _currentNrAggresive= 0;
        private int _currentNrShady = 0;
        
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
                while (mask.PersonalityType == "Aggressive" && _currentNrAggresive >= _maxNumberOfAggresive)
                {
                    mask.PersonalityType = Constants.MASK_PERSONALITY_TYPES.RandomElement();
                }
                while (mask.PersonalityType == "Shady" && _currentNrShady >= _maxNumberOfShady)
                {
                    mask.PersonalityType = Constants.MASK_PERSONALITY_TYPES.RandomElement();
                }
                if (mask.PersonalityType == "Aggressive")
                {
                    _currentNrAggresive++;
                }
                else if (mask.PersonalityType == "Shady")
                {
                    _currentNrShady++;
                }
                mask.Voice = Constants.MASK_VOICE.RandomElement();
            });
            _maxNumberOfLiers = Masks.Count / 2 - 1;

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
                    mask.WhereWereThey = GameManager.Instance.GetRandomLocation();
                    var time = DateTime.ParseExact(GameManager.Instance.CrimeTime, "HH:mm", CultureInfo.InvariantCulture);

                    string time1 = time.AddHours(-1).ToString("HH:mm");
                    string time2 = time.AddHours( 1).ToString("HH:mm");
                    string time3 = time.AddHours( 0).ToString("HH:mm");
                    var tempTime = new List<string>() { time1, time2, time3 };
                    mask.AtWhatTime = tempTime.RandomElement();
                }
                else
                {
                    mask.WhereWereThey = GameManager.Instance.CrimeLocation;
                    mask.AtWhatTime = GameManager.Instance.CrimeTime;
                }
                
            });
            
            var masksCopy = Masks.ToList();
            var rng = new System.Random();
            for (var i = masksCopy.Count - 1; i > 0; i--)
            {
                var j = rng.Next(i + 1);
                (masksCopy[i], masksCopy[j]) = (masksCopy[j], masksCopy[i]);
            }
            masksCopy.ForEach(mask =>
            {
                if (_currentNrLiers < _maxNumberOfLiers)
                {
                    mask.IsTruthful = Random.Range(0, 1) == 1;
                    if (!mask.IsTruthful)
                    {
                        _currentNrLiers++;
                    }
                }
            });
            
            Masks[0].OnScreen();
            DialogueManager.Instance.AddText(Masks);
        }

        public void NextMask()
        {
            if (Masks.All(mask => mask.WasOnScreen))
            {
                GameManager.Instance.ShowNextPhaseButton();
            }
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

                _tweenSequence.OnComplete(() =>
                {
                    IsTweening = false;
                    VerdictController.Instance.CheckCurrentVerdict();
                });

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
            _tweenSequence.OnComplete(() =>
            {
                IsTweening = false;
                VerdictController.Instance.CheckCurrentVerdict();
            });
        }

        public void PreviousMask()
        {
            if (Masks.All(mask => mask.WasOnScreen))
            {
                GameManager.Instance.ShowNextPhaseButton();
            }
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

                _tweenSequence.OnComplete(() =>
                {
                    IsTweening = false;
                    VerdictController.Instance.CheckCurrentVerdict();
                });

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
            _tweenSequence.OnComplete(() =>
            {
                IsTweening = false;
                VerdictController.Instance.CheckCurrentVerdict();
            });
        }
        
        public MaskController CurrentMask => Masks[_currentMaskIndex];
    }
}