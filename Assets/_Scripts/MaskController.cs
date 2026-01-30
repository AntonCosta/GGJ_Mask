using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Controllers
{
    public class MaskController : MonoBehaviour
    {
        [SerializeField] public SpriteRenderer Accessory;
        [SerializeField] public SpriteRenderer Hat;
        [SerializeField] public SpriteRenderer Eyes;
        [SerializeField] public SpriteRenderer Nose;
        [SerializeField] public SpriteRenderer Mouth;
        [SerializeField] public SpriteRenderer FaceType;
        [SerializeField] public SpriteRenderer Ears;

        public string PersonalityType
        {
            get => _personalityType;
            set => _personalityType = value;
        }

        public bool IsKiller
        {
            get => _isKiller;
            set => _isKiller = value;
        }

        public string Role
        {
            get => _role;
            set => _role = value;
        }

        private string _personalityType;
        private bool _isKiller;
        private string _role;
    }
}