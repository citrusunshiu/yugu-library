using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Controllers;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;
using YuguLibrary.Utilities;

namespace YuguLibrary
{
    public class OverworldObjectCoordinator : MonoBehaviour
    {
        #region Variables
        #region Transform Movement Constants
        readonly float xmove = 1.225f;
        readonly float ymove = 0.6125f;
        readonly float zmove = 2.28f / 2;
        readonly float zforward = 3.5f;
        #endregion
        /// <summary>
        /// The <see cref="GameObject"/>'s sprite renderer.
        /// </summary>
        public SpriteRenderer spriteRenderer;

        /// <summary>
        /// The animation script associated with the unit coordinator.
        /// </summary>
        public AnimationScript animationScript;

        /// <summary>
        /// List of animations to be run in sequence.
        /// </summary>
        public Queue<IEnumerator> animationQueue = new Queue<IEnumerator>();

        /// <summary>
        /// Whether or not an animation coroutine is currently running.
        /// </summary>
        public bool currentlyAnimating;

        /// <summary>
        /// The <see cref="Models.OverworldObject"/> object associated with the unit coordinator.
        /// </summary>
        public OverworldObject overworldObject;
        #endregion

        #region Functions
        // Start is called before the first frame update
        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {
            RunAnimationQueue();
        }

        public void AttachAnimationScript(AnimationScript animationScript)
        {
            this.animationScript = animationScript;
            this.animationScript.AttachOverworldObjectCoordinator(this);
        }

        public void FrameMove(Directions direction)
        {
            int totalMoveFrames;
            int animationIndex = 1;

            switch (direction)
            {
                case Directions.Up:
                    totalMoveFrames = overworldObject.ascentFrames;
                    break;
                case Directions.Down:
                    totalMoveFrames = overworldObject.descentFrames;
                    break;
                default:
                    totalMoveFrames = overworldObject.GetTotalMovementFrames();
                    break;

            }

            if (this != null)
            {
                SetAnimation(animationIndex, totalMoveFrames, false);
                StartCoroutine(MoveSegment(direction, totalMoveFrames));
            }
        }

        /// <summary>
        /// Changes the overworld object's current animation to the specified index.
        /// </summary>
        /// <param name="animationIndex">The index number of the animation to change to.</param>
        /// <param name="framesGiven">The amount of frames given for the animation to play fully. (set to -1 for default play speed)</param>
        /// <param name="isLooping">Whether or not the animation should loop after completion.</param>
        public void SetAnimation(int animationIndex, int framesGiven, bool isLooping)
        {
            StopAllCoroutines();
            currentlyAnimating = false;
            animationScript.SetAnimationPatternIndex(animationIndex);
            animationScript.SetFramesGiven(framesGiven);
            animationScript.SetIsLooping(isLooping);
            animationQueue.Clear();
            animationScript.PlayAnimation();
        }

        /// <summary>
        /// Runs a new animation coroutine from <see cref="animationQueue"/> if there are none currently running.
        /// </summary>
        private void RunAnimationQueue()
        {
            if (animationQueue.Count != 0)
            {
                if (!currentlyAnimating)
                {
                    StartCoroutine(animationQueue.Dequeue());
                }
            }
            else if (animationScript.IsLooping())
            {
                animationScript.PlayAnimation();
            }
        }

        private IEnumerator MoveSegment(Directions direction, int totalMoveFrames, int moveProgress = 0)
        {
            yield return new WaitForSeconds(0.016667F);

            float dx = xmove / totalMoveFrames;
            float dy = ymove / totalMoveFrames;
            float dz = 0;

            switch (direction)
            {
                case Directions.NW:
                    dx *= -1;
                    break;
                case Directions.NE:
                    break;
                case Directions.SE:
                    dy *= -1;
                    break;
                case Directions.SW:
                    dx *= -1;
                    dy *= -1;
                    break;
                case Directions.Up:
                    dx = 0;
                    dy = zmove / totalMoveFrames;
                    dz = zforward / totalMoveFrames;
                    break;
                case Directions.Down:
                    dx = 0;
                    dy = (zmove / totalMoveFrames) * -1;
                    dz = (zforward / totalMoveFrames) * -1;
                    break;
            }

            transform.position = new Vector3(transform.position.x + dx, transform.position.y + dy, transform.position.z + dz);
            Player player = UtilityFunctions.GetActivePlayer();
            if (overworldObject.Equals(player.GetCurrentOverworldObject()))
            {
                SetCameraPosition();
            }

            moveProgress++;
            if (moveProgress < totalMoveFrames)
            {
                RepeatMove(direction, totalMoveFrames, moveProgress);
            }
            else
            {
                SetAnimation(0, -1, true);
                UtilityFunctions.SetSpriteDefaultPosition(this);
            }
        }

        private void SetCameraPosition()
        {
            Player player = UtilityFunctions.GetActivePlayer();

            Vector3 camerapos = new Vector3(transform.position.x, transform.position.y, player.mainCamera.transform.position.z);
            player.mainCamera.transform.position = camerapos;
        }

        private void RepeatMove(Directions direction, int totalMoveFrames, int moveProgress)
        {
            StartCoroutine(MoveSegment(direction, totalMoveFrames, moveProgress));
        }
        #endregion
    }
}

