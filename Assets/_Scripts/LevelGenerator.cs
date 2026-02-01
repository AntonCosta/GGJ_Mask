using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.Controllers;
using GGJ.Managers;
using GGJ.Models;
using GJJ.Managers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGJ
{
    public class LevelGenerator : MonoBehaviour
    {
        private const string DIALOGUE_RESOURCE_PATH = "Data/mask_dialogue";
        private const string MASK_PREFAB = "Prefabs/Mask";
        private const string INTERVIEW_ROOM_PREFAB = "Prefabs/InterviewRoom";
        private const float MASK_Y_OFFSET = -0.1f;
        private const int MIN_NR_MASKS = 5;
        private const int MAX_NR_MASKS = 7;

        public static LevelGenerator Instance { get; private set; }
        
        public int NrOfMasks => _nrOfMasks;
        public List<GameObject> MaskPositions => _maskPositions;
        public List<GameObject> InterviewRooms = new();

        [SerializeField] private List<GameObject> _maskPositions;
        [SerializeField] private List<Sprite> _accessory;
        [SerializeField] private List<Sprite> _hats;
        [SerializeField] private List<Sprite> _eyes;
        [SerializeField] private List<Sprite> _nose;
        [SerializeField] private List<Sprite> _mouth;
        [SerializeField] private List<Sprite> _faceType;
        [SerializeField] private List<Sprite> _ears;
        [SerializeField] private List<Sprite> _foregrounds;
        [SerializeField] private List<Sprite> _bodies;

        private Dialogues _dialogueModel;
        private int _nrOfMasks = 0;
        private int _maskId = 0;
        
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
            _nrOfMasks = Random.Range(MIN_NR_MASKS, MAX_NR_MASKS + 1);
            ReadDialogue();
            CreateMasks();
            CreateBackgrounds();
        }

        private void ReadDialogue()
        {
            _dialogueModel = JsonUtility.FromJson<Dialogues>(Resources.Load<TextAsset>(DIALOGUE_RESOURCE_PATH).text);
            DialogueManager.Instance.AddDialogues(_dialogueModel);
        }

        private void CreateMasks()
        {
            MaskManager.Instance.Masks.Clear();
            var index = 0;
            _maskPositions.ForEach(maskPosition =>
            {
                var newPos = new Vector3(index * 30f, 0f, 0f);
                maskPosition.transform.localPosition = newPos;
                index++;
            });
            for (var i = 0; i < _nrOfMasks; i++)
            {
                var newMask = CreateMask();

                newMask.transform.parent = _maskPositions[i].transform;
                newMask.transform.localPosition = new Vector3(0, MASK_Y_OFFSET, 0);
                
                MaskManager.Instance.Masks.Add(newMask.GetComponent<MaskController>());
            }
            MaskManager.Instance.AllMasksAdded();
        }

        public GameObject CreateMask()
        {
            var newMaskPrefab = Resources.Load<GameObject>(MASK_PREFAB);
            var newMask = Instantiate(newMaskPrefab, transform.position, Quaternion.identity);

            var maskController = newMask.GetComponent<MaskController>();
            maskController.Id = _maskId++;
            maskController.Accessory.sprite = _accessory.RandomElement();
            maskController.Hat.sprite = _hats.RandomElement();
            maskController.Eyes.sprite = _eyes.RandomElement();
            maskController.Nose.sprite = _nose.RandomElement();
            maskController.Mouth.sprite = _mouth.RandomElement();
            maskController.FaceType.sprite = _faceType.RandomElement();
            maskController.Ears.sprite = _ears.RandomElement();
            maskController.MaskComponents.Add(maskController.Accessory);
            maskController.MaskComponents.Add(maskController.Hat);
            maskController.MaskComponents.Add(maskController.Eyes);
            maskController.MaskComponents.Add(maskController.Nose);
            maskController.MaskComponents.Add(maskController.Mouth);
            maskController.MaskComponents.Add(maskController.Ears);
            
            return newMask;
        }

        private void CreateBackgrounds()
        {
            for (var i = 0; i < _nrOfMasks; i++)
            {
                var newRoomPrefab = Resources.Load<GameObject>(INTERVIEW_ROOM_PREFAB);
                var newRoom = Instantiate(newRoomPrefab, transform.position, Quaternion.identity);
                newRoom.transform.parent = _maskPositions[i].transform;
                newRoom.transform.localPosition = new Vector3(0, 0, 0);
                InterviewRooms.Add(newRoom);
                
                var roomController = newRoom.GetComponent<InterviewRoomController>();
                roomController.Foreground.sprite = _foregrounds.RandomElement();
                roomController.Body.sprite = _bodies.RandomElement();
            }
        }
    }
}

