using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(GridLayoutGroup))]
    public class FlexibleGridLayout : MonoBehaviour
    {
        [SerializeField]
        private Vector2 targetResolution = new Vector2(250, 250);
        [SerializeField]
        private int cellAmount = 2;
        [SerializeField]
        private int thresholdToStickAtTop = 25;
        [SerializeField]
        private GridLayoutGroup gridLayout;
        [SerializeField]
        private RectTransform rectTransform;

        private int _requiredTotalHeight;

        private void Awake()
        {
            if (rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }

            if (gridLayout == null)
            {
                gridLayout = GetComponent<GridLayoutGroup>();
            }

            _requiredTotalHeight = (int)targetResolution.y * cellAmount;
        }

        private void OnValidate()
        {
            UpdateCellSize();
        }

        private void OnEnable()
        {
            //Invoke delayed because layout system is not immediately done
            Invoke(nameof(UpdateCellSize), 0.15f);
        }

        private void UpdateCellSize()
        {
            var containerSize = rectTransform.rect;

            if (containerSize.height > -1f && containerSize.height < 1f)
            {
                return;
            }

            // Check if container has enough height
            if (containerSize.height >= _requiredTotalHeight)
            {
                var deltaHeightPerCell = (containerSize.height - (int)_requiredTotalHeight) / cellAmount;
                gridLayout.childAlignment = deltaHeightPerCell > thresholdToStickAtTop 
                    ? TextAnchor.UpperRight 
                    : TextAnchor.LowerRight;

                gridLayout.cellSize = new Vector2(targetResolution.x, targetResolution.y);
            }
            // Not enough space
            else
            {
                var deltaHeightPerCell = ((int)_requiredTotalHeight - containerSize.height) / cellAmount;
                gridLayout.cellSize = GetScaledByHeight(targetResolution.y -deltaHeightPerCell, targetResolution);
                gridLayout.childAlignment = TextAnchor.LowerRight;
            }
        }

        private Vector2 GetScaledByHeight(float height, Vector2 target)
        {
            var aspectRatio = target.x / target.y;
            var newWidth = height * aspectRatio;

            return new Vector2(newWidth, height);
        }
    }
}
