using UnityEngine;

namespace GGJ.Utils
{
    [ExecuteAlways]
    public class CircularLayout : MonoBehaviour
    {
        private float radius = 0.8f;
        private float startAngleDegrees = 90f; // 90 = top
        private bool faceOutwards = false;

        public void Arrange()
        {
            var n = transform.childCount;
            if (n == 0) return;

            var step = 360f / n;

            for (var i = 0; i < n; i++)
            {
                var angle = (startAngleDegrees + step * i) * Mathf.Deg2Rad;
                var pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;

                var child = transform.GetChild(i);
                child.localPosition = pos;

                if (faceOutwards)
                {
                    var z = startAngleDegrees + step * i - 90f;
                    child.localRotation = Quaternion.Euler(0f, 0f, z);
                }
            }
        }

        private void OnEnable() => Arrange();
        private void OnValidate() => Arrange();
    }
}