using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.Controllers;
using GGJ.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ
{
    public class LevelGenerator : MonoBehaviour
    {
        private const string DIALOGUE_RESOURCE_PATH = "Data/mask_dialogue";
        private const string MASK_PREFAB = "Prefabs/Mask";

        public static LevelGenerator Instance { get; private set; }

        [SerializeField] private List<GameObject> _maskPositions;
        [SerializeField] private List<Sprite> _accessory;
        [SerializeField] private List<Sprite> _hats;
        [SerializeField] private List<Sprite> _eyes;
        [SerializeField] private List<Sprite> _nose;
        [SerializeField] private List<Sprite> _mouth;
        [SerializeField] private List<Sprite> _faceType;
        [SerializeField] private List<Sprite> _ears;

        private Dialogues _dialogueModel;
        private int _nrOfMasks = 0;
        private int _currentMaskIndex = 0;
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void GenerateLevel()
        {
            _nrOfMasks = Random.Range(5, 7);
            ReadDialogue();
            CreateMasks();
        }

        private void ReadDialogue()
        {
            _dialogueModel = JsonUtility.FromJson<Dialogues>(Resources.Load<TextAsset>(DIALOGUE_RESOURCE_PATH).text);
            Debug.Log(_dialogueModel.Dialogue_Lines.First().Text);
        }

        private void CreateMasks()
        {
            for (var i = 0; i < _nrOfMasks; i++)
            {
                var newMaskPrefab = Resources.Load<GameObject>(MASK_PREFAB);
                var newMask = Instantiate(newMaskPrefab, transform.position, Quaternion.identity);
                newMask.transform.parent = _maskPositions[i].transform;
                newMask.transform.localPosition = new Vector3(0, 0, 0);

                var maskController = newMask.GetComponent<MaskController>();
                maskController.Accessory.sprite = _accessory.RandomElement();
                maskController.Hat.sprite = _hats.RandomElement();
                maskController.Eyes.sprite = _eyes.RandomEvenElement();
                maskController.Nose.sprite = _nose.RandomElement();
                maskController.Mouth.sprite = _mouth.RandomElement();
                maskController.FaceType.sprite = _faceType.RandomElement();
                maskController.Ears.sprite = _ears.RandomEvenElement();
            }
        }

        public void NextMask()
        {
            _currentMaskIndex++;
            _maskPositions.ForEach(mask =>
            {
                var pos = mask.transform.position;
                pos.x -= 300f;
                mask.transform.position = pos;
            });
        }

        public void PreviousMask()
        {
            _currentMaskIndex++;
            _maskPositions.ForEach(mask =>
            {
                var pos = mask.transform.position;
                pos.x += 300f;
                mask.transform.position = pos;
            });
        }
    }
}

