using GJJ.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Contollers
{
    public class VerdictController : MonoBehaviour
    {
        public static VerdictController Instance { get; private set; }

        [SerializeField] private Button Guilty;
        [SerializeField] private Button Suspicious;
        [SerializeField] private Button Innocent;
        [SerializeField] private Image ExclamationMark;
        [SerializeField] private Image QuestionMark;
        [SerializeField] private Image InnocentHalo;
        
        private Vector4 _defaultColor = new Vector4(1f, 1f, 1f, 50f/255f);
        private Vector4 _hoverColor = new Vector4(1f, 1f, 1f, 125f/255f);
        private Vector4 _selectedColor = new Vector4(1f, 1f, 1f, 1f);
        
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }
        
        private void Start()
        {
            ExclamationMark.color = _defaultColor;
            QuestionMark.color = _defaultColor;
            InnocentHalo.color = _defaultColor;

            Guilty.onClick.AddListener(() => SelectVerdict(0));
            Suspicious.onClick.AddListener(() => SelectVerdict(1));
            Innocent.onClick.AddListener(() => SelectVerdict(2));
        }

        public void CheckCurrentVerdict()
        {
            switch (MaskManager.Instance.CurrentMask.PlayerVerdict)
            {
                case "Guilty":
                    SelectVerdict(0);
                    break;
                case "Suspicious":
                    SelectVerdict(1);
                    break;
                case "Innocent":
                    SelectVerdict(2);
                    break;
                default:
                    SelectVerdict(-1);
                    break;
            }
        }

        private void SelectVerdict(int verdict)
        {
            switch (verdict)
            {
                case 0:
                    ExclamationMark.color = _selectedColor;
                    QuestionMark.color = _defaultColor;
                    InnocentHalo.color = _defaultColor;
                    MaskManager.Instance.CurrentMask.PlayerVerdict = "Guilty";
                    break;
                case 1:
                    QuestionMark.color = _selectedColor;
                    ExclamationMark.color = _defaultColor;
                    InnocentHalo.color = _defaultColor;
                    MaskManager.Instance.CurrentMask.PlayerVerdict = "Suspicious";
                    break;
                case 2:
                    InnocentHalo.color = _selectedColor;
                    QuestionMark.color = _defaultColor;
                    ExclamationMark.color = _defaultColor;
                    MaskManager.Instance.CurrentMask.PlayerVerdict = "Innocent";
                    break;
                default:
                    InnocentHalo.color = _defaultColor;
                    QuestionMark.color = _defaultColor;
                    ExclamationMark.color = _defaultColor;
                    break;
            }
        }
    }
}