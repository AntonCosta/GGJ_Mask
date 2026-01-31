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
                mask.Dialogue.text = mask.Dialogue.text
                    .Replace("${PLACE}", gameManager.CrimeLocation)
                    .Replace("${TIME}", gameManager.CrimeTime)
                    .Replace("${VOICE}", killerMask.Voice);
                    //.Replace("${MASK_DETAIL}", time2)
                //  .Replace("${MASK_PIECE}", time2);
            }
            else
            {
                mask.Dialogue.text = mask.Dialogue.text
                    .Replace("${PLACE}", gameManager.GetRandomLocation())
                    .Replace("${TIME}", gameManager.GetRandomTime())
                    .Replace("${VOICE}", Constants.MASK_VOICE.RandomElement());
                //.Replace("${MASK_DETAIL}", time2)
                // .Replace("${MASK_PIECE}", time2);
            }
        }
    }
}