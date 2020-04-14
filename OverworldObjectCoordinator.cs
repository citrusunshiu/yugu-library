using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YuguLibrary.Enumerations;
using YuguLibrary.Models;

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

        /// <summary>
        /// Runs a new animation coroutine from <see cref="animationQueue"/> if there are none currently running.
        /// </summary>
        void RunAnimationQueue()
        {
            if (animationQueue.Count != 0)
            {
                if (!currentlyAnimating)
                {
                    StartCoroutine(animationQueue.Dequeue());
                }
            }
            else
            {
                animationScript.PlayAnimation();
            }
        }
        /*
        public void FrameMove(Directions direction)
        {
            int totalMoveFrames;

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
                StartCoroutine(MoveSegment(direction, totalMoveFrames));
            }
        }

        IEnumerator MoveSegment(Directions direction, int totalMoveFrames, int moveProgress = 0)
        {
            yield return new WaitForSeconds(1 / 60);

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
            PlayerController playerController = (PlayerController)UtilityFunctions.GetController(typeof(PlayerController));
            if (overworldObject.Equals(playerController.currentOverworldObject))
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
                UtilityFunctions.SetSpriteDefaultPosition(this);
            }
        }

        public void SetCameraPosition()
        {
            PlayerController playerController = (PlayerController)UtilityFunctions.GetController(typeof(PlayerController));

            Vector3 camerapos = new Vector3(transform.position.x, transform.position.y, playerController.mainCamera.transform.position.z);
            playerController.mainCamera.transform.position = camerapos;
        }

        void RepeatMove(Directions direction, int totalMoveFrames, int moveProgress)
        {
            StartCoroutine(MoveSegment(direction, totalMoveFrames, moveProgress));
        } */
        #endregion
    }
}

