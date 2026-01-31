using GGJ.Utils;
using GJJ.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GGJ.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        
        private void Update()
        {
            if (Keyboard.current.dKey.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                ScreenFader.Instance.FadeHide(MaskManager.Instance.NextMask);
            }

            if (Keyboard.current.aKey.wasPressedThisFrame && !ScreenFader.Instance.IsTweening)
            {
                ScreenFader.Instance.FadeHide(MaskManager.Instance.PreviousMask);
            }
        }
    }
}