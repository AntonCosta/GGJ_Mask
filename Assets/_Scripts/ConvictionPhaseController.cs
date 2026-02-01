using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGJ.Managers;
using GGJ.Utils;
using GJJ.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

namespace GGJ.Controllers
{
    public class ConvictionPhaseController : MonoBehaviour
    {
        [SerializeField] private CircularLayout _circularLayout;
        [SerializeField] private GameObject _maskFile;
        [SerializeField] private GameObject _background;
        [SerializeField] private GameObject _yes;
        [SerializeField] private GameObject _no;
        [SerializeField] private TextMeshPro _location;
        [SerializeField] private TextMeshPro _time;
        [SerializeField] private TextMeshPro _voice;

        private MaskFileController _maskFileController;
        private Camera _camera;
        private MaskController _currentMask;
        private List<MaskController> _maskCopies;

        private void Start()
        {
            _maskFileController = _maskFile.GetComponent<MaskFileController>();
            _camera = Camera.main;
        }

        public void MoveMasksToConviction()
        {
            _maskCopies = MaskManager.Instance.Masks.ToList();
            _maskCopies.ForEach(mask =>
            {
                mask.transform.SetParent(transform, false);
                mask.transform.localPosition = Vector3.zero;
                mask.Canvas.gameObject.SetActive(false);
                mask.PotentialMaskPosition.gameObject.SetActive(false);
                mask.Tween.Kill();
                mask.Clickable = true;
                mask.OnMaskClicked -= PopulateMaskFile;
                mask.OnMaskClicked += PopulateMaskFile;
            });
            _circularLayout.Arrange();

            _maskCopies.ForEach(mask =>
            {
                mask.Amplitude = 0.01f;
                mask.OnScreen();
            });
        }

        private void Update()
        {
            if (Keyboard.current.escapeKey.wasPressedThisFrame && _maskFile.activeSelf && !ScreenFader.Instance.IsTweening)
            {
                ReturnToConviction();
            }
            
            if (Mouse.current.leftButton.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                var worldPoint = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                var hit = Physics2D.Raycast(worldPoint, Vector2.zero, 0f);
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == _yes.gameObject)
                    {
                        PressedYes();
                    }
                    else if(hit.collider.gameObject == _no.gameObject)
                    {
                        PressedNo();
                    }
                }
            }
        }

        private void ReturnToConviction()
        {
            ScreenFader.Instance.FadeHide(() =>
            {
                _maskFile.SetActive(false);
                _background.SetActive(true);
                _maskCopies.ForEach(mask =>
                {
                    mask.gameObject.SetActive(true);
                    mask.transform.localScale = new Vector3(1f, 1f, 1f);
                });

                _circularLayout.Arrange();
                _maskCopies.ForEach(mask =>
                {
                    mask.Amplitude = 0.01f;
                    mask.OnScreen();
                });
            });
        }

        private void PressedYes()
        {
            if (_currentMask.IsKiller)
            {
                ScreenFader.Instance.FadeHide(GameManager.Instance.YouWin);
                _maskFile.SetActive(false);
                _background.SetActive(true);
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
            }
            else
            {
                _maskCopies.RemoveAll(mask => mask.Id == _currentMask.Id);
                _currentMask.gameObject.SetActive(false);
                if (_maskCopies.Count == 1)
                {
                    ScreenFader.Instance.FadeHide(GameManager.Instance.YouLose);
                    _maskFile.SetActive(false);
                    _background.SetActive(true);
                    for (int i = transform.childCount - 1; i >= 0; i--)
                    {
                        Destroy(transform.GetChild(i).gameObject);
                    }
                    return;
                }
                var randomMask = _maskCopies.RandomElement();
                while (randomMask.IsKiller)
                {
                    randomMask = _maskCopies.RandomElement();
                }
                _maskCopies.RemoveAll(mask => mask.Id == randomMask.Id);
                randomMask.gameObject.SetActive(false);
                ReturnToConviction();
            }
        }

        private void PressedNo()
        {
            ReturnToConviction();
        }

        public void PopulateMaskFile(MaskController mask)
        {
            ScreenFader.Instance.FadeHide(() =>
            {
                _currentMask = mask;
                MaskManager.Instance.Masks.ForEach(otherMask =>
                {
                    otherMask.gameObject.SetActive(false);
                    otherMask.Tween.Kill();
                });
                mask.gameObject.SetActive(true);
                _maskFile.SetActive(true);
                _background.SetActive(false);
                _maskFileController.ConfessionText.text = mask.Dialogue.text;
                _maskFileController.OthersText.text = string.Join("\n", mask.WhatTheySaidAboutYou.Where(t => t != null).Select(t => t.text));
                mask.transform.localPosition = _maskFileController.MaskPosition.transform.localPosition;
                mask.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                _location.text = "Location: " + mask.WhereWereThey;
                _time.text = "Time: " + mask.AtWhatTime;
                _voice.text = "Voice: " + mask.Voice;
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