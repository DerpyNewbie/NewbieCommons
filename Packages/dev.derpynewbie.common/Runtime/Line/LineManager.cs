using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;

namespace DerpyNewbie.Common.Line
{
    /// <summary>
    /// Useful line drawing manager for debug visualization purposes.
    /// </summary>
    [AddComponentMenu("Newbie Commons/Utils/Line Manager")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LineManager : UdonSharpBehaviour
    {
        [SerializeField]
        private GameObject lineDrawerPrefab;
        [SerializeField]
        private Transform lineParent;
        [SerializeField]
        private Gradient defaultLineGradient;

        /// <summary>
        /// Draws a line between <paramref name="a"/> and <paramref name="b"/> for <paramref name="time"/> seconds.
        /// </summary>
        /// <param name="a">first world space point of a line.</param>
        /// <param name="b">second world space point of a line.</param>
        /// <param name="time">seconds until line disappear. <c>float.NaN</c> will make line never disappear.</param>
        /// <param name="startWidth">starting width of a line.</param>
        /// <param name="endWidth">ending width of a line.</param>
        /// <param name="gradient">gradient color of a line.</param>
        /// <remarks>
        /// To clear lines with <c>float.NaN</c> <paramref name="time"/>, Call returned <see cref="LineDrawer"/>'s <see cref="LineDrawer.Clear"/>.
        /// </remarks>
        [PublicAPI]
        public LineDrawer Draw(Vector3 a, Vector3 b, float time = 5F, float startWidth = 0.01F, float endWidth = 0.01F,
            Gradient gradient = null)
        {
            return Draw(new[] { a, b }, time, startWidth, endWidth, gradient);
        }

        /// <summary>
        /// Draws a line of <paramref name="vectors"/> for <paramref name="seconds"/> seconds.
        /// </summary>
        /// <param name="vectors">line vectors in world space.</param>
        /// <param name="seconds">seconds until line disappear. <c>float.NaN</c> will make line never disappear.</param>
        /// <param name="startWidth">starting width of a line.</param>
        /// <param name="endWidth">ending width of a line.</param>
        /// <param name="gradient">gradient color of a line.</param>
        /// <remarks>
        /// To clear lines with <c>float.NaN</c> <paramref name="seconds"/>, Call returned <see cref="LineDrawer"/>'s <see cref="LineDrawer.Clear"/>.
        /// </remarks>
        [PublicAPI]
        public LineDrawer Draw(Vector3[] vectors, float seconds = 5F, float startWidth = 0.01F, float endWidth = 0.01F,
            Gradient gradient = null)
        {
            if (gradient == null)
                gradient = defaultLineGradient;
            var line = CreateLineDrawer();
            line.Draw(vectors, startWidth, endWidth, gradient);
            if (!float.IsNaN(seconds))
                line.StartClearTimer(seconds);
            return line;
        }

        /// <summary>
        /// Draws a line between <paramref name="a"/> and <paramref name="b"/> for <paramref name="frames"/> frame.
        /// </summary>
        /// <param name="a">first world space point of a line.</param>
        /// <param name="b">second world space point of a line.</param>
        /// <param name="frames">frames until line disappear. <c>-1</c> to make line never disappear.</param>
        /// <param name="startWidth">starting width of a line.</param>
        /// <param name="endWidth">ending width of a line.</param>
        /// <param name="gradient">gradient color of a line.</param>
        /// <remarks>
        /// To clear lines with <c>-1</c> <paramref name="frames"/>, Call returned <see cref="LineDrawer"/>'s <see cref="LineDrawer.Clear"/>.
        /// </remarks>
        [PublicAPI]
        public LineDrawer Draw(Vector3 a, Vector3 b, int frames = 1, float startWidth = 0.01F, float endWidth = 0.01F,
            Gradient gradient = null)
        {
            return Draw(new[] { a, b }, frames, startWidth, endWidth, gradient);
        }

        /// <summary>
        /// Draws a line of <paramref name="vectors"/> for <paramref name="frames"/> frame.
        /// </summary>
        /// <param name="vectors">line vectors in world space.</param>
        /// <param name="frames">frames until line disappear. <c>-1</c> to make line never disappear.</param>
        /// <param name="startWidth">starting width of a line.</param>
        /// <param name="endWidth">ending width of a line.</param>
        /// <param name="gradient">gradient color of a line.</param>
        /// <remarks>
        /// To clear lines with <c>-1</c> <paramref name="frames"/>, Call returned <see cref="LineDrawer"/>'s <see cref="LineDrawer.Clear"/>.
        /// </remarks>
        [PublicAPI]
        public LineDrawer Draw(Vector3[] vectors, int frames = 1, float startWidth = 0.01F, float endWidth = 0.01F,
            Gradient gradient = null)
        {
            if (gradient == null)
                gradient = defaultLineGradient;

            var line = CreateLineDrawer();
            line.Draw(vectors, startWidth, endWidth, gradient);
            if (frames != -1)
                line.StartClearTimer(frames);
            return line;
        }

        /// <summary>
        /// Gets all <see cref="LineDrawer"/> instances created by this <see cref="LineManager"/>.
        /// </summary>
        /// <returns>All currently active instances of <see cref="LineDrawer"/>.</returns>
        /// <remarks>
        /// This method is expensive, though cannot be stored too long as it destroys themselves after specified time.
        /// It is recommended to store LineDrawers returned at Draw methods for manipulation.
        /// </remarks>
        [PublicAPI] [ItemCanBeNull]
        public LineDrawer[] GetAllLines()
        {
            var count = lineParent.childCount;
            var result = new LineDrawer[count];
            for (var i = 0; i < count; i++)
                result[i] = lineParent.GetChild(i).GetComponent<LineDrawer>();
            return result;
        }

        /// <summary>
        /// Clears all <see cref="LineDrawer"/> instances created by this <see cref="LineManager"/>.
        /// </summary>
        /// <returns>Numbers of instances cleared by this operation.</returns>
        [PublicAPI]
        public int Clear()
        {
            var lineDrawers = GetAllLines();
            var count = lineDrawers.Length;
            foreach (var drawer in lineDrawers)
                if (drawer != null)
                    drawer.Clear();
            return count;
        }

        private LineDrawer CreateLineDrawer()
        {
            var lineDrawerObj = Instantiate(lineDrawerPrefab, lineParent);
            return lineDrawerObj.GetComponent<LineDrawer>();
        }
    }
}