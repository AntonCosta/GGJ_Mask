using TMPro;
using UnityEngine;

namespace GGJ.Controllers
{
    public class MaskFileController : MonoBehaviour
    {
        [SerializeField] public GameObject MaskPosition;
        [SerializeField] public TextMeshPro ConfessionText;
        [SerializeField] public TextMeshPro OthersText;
        [SerializeField] public GameObject GuiltyMark;
        [SerializeField] public GameObject SuspiciousMark;
        [SerializeField] public GameObject InnocentMark;
    }
}