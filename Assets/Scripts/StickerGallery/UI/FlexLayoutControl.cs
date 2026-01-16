using UnityEngine;

namespace StickerGallery.UI
{
    [ExecuteInEditMode]
    public class FlexLayoutControl : MonoBehaviour
    {
        public enum LayoutMode
        {
            ProportionalToScreenSize,
            FixedPixelSize,
            DerivedFromAspect
        }

        public LayoutMode layoutModeWidth;
        public LayoutMode layoutModeHeight;

        public float aspect_W_over_H;

        public float fixedW;
        public float fixedH;

        public float propW;
        public float propH;

        public LayoutMode layoutModeOffsetX;
        public LayoutMode layoutModeOffsetY;

        public float fixedOffsetX;
        public float fixedOffsetY;

        public float propOffsetX;
        public float propOffsetY;

        public GameObject ElementToAffect;

        void Start()
        {
            UpdateLayout();
        }

#if UNITY_EDITOR
        void Update()
        {
            UpdateLayout();
        }
#endif

        void UpdateLayout()
        {
            RectTransform rectTrans = ElementToAffect.GetComponent<RectTransform>();

            if (rectTrans == null)
            {
                Debug.LogError("rectTrans is null");
                return;
            }

            float scrW = Screen.width;
            float scrH = Screen.height;

            float elW = 0f;
            float elH = 0f;

            float elAnchPosX = 0f;
            float elAnchPosY = 0f;

            if (layoutModeWidth == LayoutMode.DerivedFromAspect)
            {
                // Width is derived -> calculate height first!
                switch (layoutModeHeight)
                {
                    case LayoutMode.DerivedFromAspect:
                        Debug.LogError("width and height can't be both derived");
                        return;
                    case LayoutMode.ProportionalToScreenSize:
                        elH = propH * scrH;
                        break;
                    case LayoutMode.FixedPixelSize:
                        elH = fixedH;
                        break;
                }

                elW = aspect_W_over_H * elH;
            }
            else if (layoutModeHeight == LayoutMode.DerivedFromAspect)
            {
                // Height is derived -> calculate width first

                switch (layoutModeWidth)
                {
                    case LayoutMode.DerivedFromAspect:
                        Debug.LogError("width and height can't be both derived");
                        return;
                    case LayoutMode.ProportionalToScreenSize:
                        elW = propW * scrW;
                        break;
                    case LayoutMode.FixedPixelSize:
                        elW = fixedW;
                        break;
                }

                elH = elW / aspect_W_over_H;
            }
            else
            {
                switch (layoutModeWidth)
                {
                    case LayoutMode.ProportionalToScreenSize:
                        elW = propW * scrW;
                        break;
                    case LayoutMode.FixedPixelSize:
                        elW = fixedW;
                        break;
                }

                switch (layoutModeHeight)
                {
                    case LayoutMode.ProportionalToScreenSize:
                        elH = propH * scrH;
                        break;
                    case LayoutMode.FixedPixelSize:
                        elH = fixedH;
                        break;
                }
            }

            switch (layoutModeOffsetX)
            {
                case LayoutMode.ProportionalToScreenSize:
                    elAnchPosX = scrW * propOffsetX;
                    break;
                case LayoutMode.FixedPixelSize:
                    elAnchPosX = fixedOffsetX;
                    break;
            }

            switch (layoutModeOffsetY)
            {
                case LayoutMode.ProportionalToScreenSize:
                    elAnchPosY = scrH * propOffsetY;
                    break;
                case LayoutMode.FixedPixelSize:
                    elAnchPosY = fixedOffsetY;
                    break;
            }

            rectTrans.sizeDelta = new Vector2(elW, elH);
            rectTrans.anchoredPosition = new Vector2(elAnchPosX, elAnchPosY);

            //Debug.Log("screen size  = " + Screen.width.ToString() + "/" + Screen.height.ToString());
            //Debug.Log("element size = " + rectTrans.sizeDelta.ToString());
            //Debug.Log("element offset = " + rectTrans.anchoredPosition.ToString());
        }
    }
}
