using UnityEngine;

namespace UI
{
    public class DynamicCanvasSwapper : MonoBehaviour
    {
        [SerializeField]
        private bool instantiateOriginal;
        [SerializeField]
        private GameObject orignalCanvas;
        [SerializeField]
        private GameObject letterBoxCanvas;

        private void Awake()
        {
            if (!this.IsLetterbox())
            {
                if (instantiateOriginal)
                {
                    Instantiate(orignalCanvas);
                }
                
                return;
            }

            if (instantiateOriginal)
            {
                Instantiate(letterBoxCanvas);
            }
            else
            {
                var parent = orignalCanvas.transform.parent;
                Destroy(orignalCanvas);
                if (parent)
                {
                    Instantiate(letterBoxCanvas, parent);
                }
                else
                {
                    Instantiate(letterBoxCanvas);
                }
            }
        }

        private bool IsLetterbox()
        {
            if (Screen.orientation == ScreenOrientation.LandscapeLeft)
            {
                return GetAspectRation() < 1.45f;
            }

            return GetAspectRation() > 0.65f;
        }

        private float GetAspectRation()
        {
            return Screen.width / (float) Screen.height;
        }
    }
}
