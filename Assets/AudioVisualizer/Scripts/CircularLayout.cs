using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.UI.CircularLayout
{
    public class CircularLayout : MonoBehaviour
    {
        public float Radius = 500f;
        public bool IsLookAtCenter = true;
        public float SpacingBuffer = 0.1f;

        void Start()
        {
            ArrangeInCircle();
        }

        private void OnTransformChildrenChanged()
        {
            ArrangeInCircle();
        }

        private float GetWidth(GameObject child)
        {
            // For UI elements
            var rectTransform = child.GetComponent<RectTransform>();
            if (rectTransform != null) {
                return rectTransform.rect.width;
            }

            // For 3D objects with a Renderer
            var renderer = child.GetComponent<Renderer>();
            if (renderer != null) {
                return renderer.bounds.size.x; // Or .z depending on orientation
            }

            // Default or custom width if no specific component found
            return 1f;
        }

        public void ArrangeInCircle()
        {
            if (transform.childCount < 1)
                return;

            // Step 1: Calculate total width with the buffer
            var childWidth = GetWidth(transform.GetChild(0).gameObject);
            var totalWidth = (childWidth + SpacingBuffer) * transform.childCount;

            // Step 2: Checking and adjusting the radius
            var circumference = 2f * Mathf.PI * Radius;
            if (totalWidth > circumference) {
                Radius = totalWidth / (2f * Mathf.PI);
                circumference = totalWidth; // Update after increase
                Debug.Log("Radius increased to " + Radius.ToString() + " to prevent overlapping.");
            }

            // Step 3: Ñalculate the angles for each element
            var angles = new float[transform.childCount];
            var currentAngle = 0f;
            var arcLength = childWidth + SpacingBuffer;
            for (var i = 0; i < transform.childCount; i++) {
                var angleForThis = (arcLength / circumference) * 360f;
                angles[i] = currentAngle + angleForThis / 2f; // Center of arc
                currentAngle += angleForThis;
            }

            // Step 4: Correction for rounding (move everything so that the total angle is exactly 360°)
            var totalCalculatedAngle = currentAngle;
            var correction = (360f - totalCalculatedAngle) / transform.childCount;
            for (var i = 0; i < angles.Length; i++) {
                angles[i] += correction * (i + 1); // Proportional adjustment
            }

            // Step 5: Place the elements
            for (var i = 0; i < transform.childCount; i++) {
                var childTransform = transform.GetChild(i);
                var angleRad = angles[i] * Mathf.Deg2Rad;

                // Position of center of the element on circle
                float x = Mathf.Cos(angleRad) * Radius;
                float z = Mathf.Sin(angleRad) * Radius;

                var position = new Vector3(x, 0f, z);
                childTransform.position = position;

                // Rotate towards center if needed
                if (IsLookAtCenter) {
                    childTransform.LookAt(transform.position);
                }
            }
        }
    }
}
