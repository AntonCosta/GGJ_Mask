using System.Collections.Generic;
using System.Linq;
using GGJ.Controllers;
using GGJ.Models;
using GGJ.Utils;
using GJJ.Managers;
using TMPro;
using UnityEngine;

namespace GGJ.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        private Dialogues _dialogues;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void AddDialogues(Dialogues dialogues)
        {
            _dialogues = dialogues;
        }

        public void AddText(List<MaskController> masks)
        {
            for (var i = 0; i < masks.Count; i++)
            {
                var mask = masks[i];
                SetUpMaskDialogue(mask, mask.PersonalityType);
                ReplaceDialogueVariables(mask);
            }
        }

        private void SetUpMaskDialogue(MaskController mask, string personalityType)
        {
            var dialogueType = _dialogues.DialogueData.First(line => line.Personality == personalityType);
            var randomTypeText = dialogueType.Text.RandomElement();
            mask.DialogueKnowledgeType = randomTypeText.Id;
            mask.Dialogue.text = randomTypeText.Dialogues.RandomElement().Text;
        }

        private void ReplaceDialogueVariables(MaskController mask)
        {
            var gameManager = GameManager.Instance;
            var killerMask = MaskManager.Instance.Masks.First(m => m.IsKiller);
            if (mask.IsTruthful)
            {
                var maskCopy = killerMask.gameObject;
                var randomMaskComponent = maskCopy.GetComponent<MaskController>().MaskComponents.RandomElement();
                while (randomMaskComponent.sprite == null)
                {
                    randomMaskComponent = maskCopy.GetComponent<MaskController>().MaskComponents.RandomElement();
                }

                int id = 0;
                for (var i = 0; i < mask.Dialogue.spriteAsset.spriteCharacterTable.Count; i++)
                {
                    var characterTable = mask.Dialogue.spriteAsset.spriteCharacterTable[i];
                    var spriteName = characterTable.name;
                    if (spriteName == randomMaskComponent.sprite.name)
                    {
                        id = i;
                    }
                }

                mask.Dialogue.text = mask.Dialogue.text
                    .Replace("${PLACE}", gameManager.CrimeLocation)
                    .Replace("${TIME}", gameManager.CrimeTime)
                    .Replace("${VOICE}", killerMask.Voice)
                    .Replace("${MASK_DETAIL}", $"<space=0.2><size=300%><voffset=0.06><sprite={id}></voffset></size>");

                if (mask.Dialogue.text.Contains("MASK_NAME"))
                {
                    mask.SpeachBubble.gameObject.SetActive(true);
                    var newMask = Instantiate(maskCopy, mask.PotentialMaskPosition.transform, false);
                    newMask.transform.localPosition = new Vector3(0f, 0f, -1f);
                    newMask.transform.localRotation = Quaternion.identity;
                    newMask.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    newMask.GetComponent<MaskController>().Canvas.gameObject.SetActive(false);
                }

                mask.Dialogue.text = mask.Dialogue.text.Replace("${MASK_NAME}", "     ");
                killerMask.WhatTheySaidAboutYou.Add(mask.Dialogue);
            }
            else
            {
                var maskCopyController = MaskManager.Instance.Masks.RandomElement();
                while (maskCopyController.Id == mask.Id)
                {
                    maskCopyController = MaskManager.Instance.Masks.RandomElement();
                }
                var maskCopy = maskCopyController.gameObject;
                var randomMaskComponent = maskCopy.GetComponent<MaskController>().MaskComponents.RandomElement();
                while (randomMaskComponent.sprite == null)
                {
                    randomMaskComponent = maskCopy.GetComponent<MaskController>().MaskComponents.RandomElement();
                }

                int id = 0;
                for (var i = 0; i < mask.Dialogue.spriteAsset.spriteCharacterTable.Count; i++)
                {
                    var characterTable = mask.Dialogue.spriteAsset.spriteCharacterTable[i];
                    var spriteName = characterTable.name;
                    if (spriteName == randomMaskComponent.sprite.name)
                    {
                        id = i;
                    }
                }

                mask.Dialogue.text = mask.Dialogue.text
                    .Replace("${PLACE}", gameManager.GetRandomLocation())
                    .Replace("${TIME}", gameManager.GetRandomTime())
                    .Replace("${VOICE}", Constants.MASK_VOICE.RandomElement())
                    .Replace("${MASK_DETAIL}", $"<space=0.2><size=300%><voffset=0.06><sprite={id}></voffset></size>");

                if (mask.Dialogue.text.Contains("MASK_NAME"))
                {
                    mask.SpeachBubble.gameObject.SetActive(true);
                    var newMask = Instantiate(maskCopy, mask.PotentialMaskPosition.transform, false);
                    newMask.transform.localPosition = new Vector3(0f, 0f, -1f);
                    newMask.transform.localRotation = Quaternion.identity;
                    newMask.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
                    newMask.GetComponent<MaskController>().Canvas.gameObject.SetActive(false);
                }

                mask.Dialogue.text = mask.Dialogue.text.Replace("${MASK_NAME}", "     ");
                maskCopyController.WhatTheySaidAboutYou.Add(mask.Dialogue);
            }
        }
    }
}