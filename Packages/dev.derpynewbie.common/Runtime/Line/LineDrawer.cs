using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common
{
    /// <summary>
    /// Draws a line with provided values.
    /// </summary>
    /// <seealso cref="LineManager"/>
    /// <seealso cref="LineManager.Draw(Vector3[], float, float, float, Gradient)"/>
    [AddComponentMenu("Newbie Commons/Utils/Line Drawer")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LineDrawer : UdonSharpBehaviour
    {
        [SerializeField]
        private LineRenderer lineRenderer;

        /// <summary>
        /// Sets up LineRenderer with provided values.
        /// </summary>
        /// <param name="positions">world space points to render this line.</param>
        /// <param name="startWidth">starting width of this line.</param>
        /// <param name="endWidth">ending width of this line.</param>
        /// <param name="gradient">color gradient of this line.</param>
        [PublicAPI]
        public void Draw(Vector3[] positions, float startWidth, float endWidth, Gradient gradient)
        {
            lineRenderer.SetPositions(positions);
            lineRenderer.colorGradient = gradient;
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = endWidth;
        }

        public void StartClearTimer(float seconds)
        {
            SendCustomEventDelayedSeconds(nameof(Clear), seconds);
        }

        public void StartClearTimer(int frames)
        {
            SendCustomEventDelayedFrames(nameof(Clear), frames);
        }

        /// <summary>
        /// Destroys this gameObject.
        /// </summary>
        /// <remarks>
        /// This method needs to be called explicitly when providing <c>float.NaN</c> or <c>-1</c> for <see cref="LineManager"/> to draw.
        /// </remarks>
        [PublicAPI]
        public void Clear()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Gets world space positions of line.
        /// </summary>
        /// <returns>world space positions of this line.</returns>
        [PublicAPI]
        public Vector3[] GetPositions()
        {
            var positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            return positions;
        }

        /// <summary>
        /// Sets world space positions of line.
        /// </summary>
        /// <param name="positions">world space positions of this line.</param>
        [PublicAPI]
        public void SetPositions(Vector3[] positions)
        {
            lineRenderer.SetPositions(positions);
        }
    }
}