using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace DigitalRubyShared
{
    /// <summary>
    /// Word game example script, handles moving letters around
    /// </summary>
    public class Gesture : MonoBehaviour
    {
        /// <summary>
        /// The letter prefab
        /// </summary>
        //[Tooltip("The letter prefab")]
        //public GameObject LetterPrefab;

        //[Tooltip("The Canvas")]
        public BlockDragManager blockDragManager;
        public GameObject container;


        private Canvas canvas;

        private readonly List<RaycastResult> raycast = new List<RaycastResult>();
        
        private PanGestureRecognizer letterMoveGesture;
        private Transform draggingLetter;
        private Vector2 dragOffset;
        private float[] boundaries;

        public float lastDelta;

        private void LetterGestureUpdated(GestureRecognizer gesture)
        {
            //Debug.Log("Pan state " + gesture.State);

            if (gesture.State == GestureRecognizerState.Began)
            {
                PointerEventData p = new PointerEventData(EventSystem.current);
                p.position = new Vector2(gesture.FocusX, gesture.FocusY);
                raycast.Clear();
                EventSystem.current.RaycastAll(p, raycast);
                foreach (RaycastResult result in raycast)
                {
                    if (result.gameObject.name.IndexOf("BlockDraggable", System.StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        // we have a letter!
                        this.boundaries = this.blockDragManager.NotifyDraggingBlockStart(result.gameObject);
                        Vector2 dragPos = FingersUtility.ScreenToCanvasPoint(canvas, new Vector2(gesture.FocusX, gesture.FocusY));
                        draggingLetter = result.gameObject.transform;
                        dragOffset = (Vector2)draggingLetter.position - dragPos;
                        break;
                    }
                }
                if (draggingLetter == null)
                {
                    gesture.Reset();
                }
            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {
                Vector2 dragPos = FingersUtility.ScreenToCanvasPoint(canvas, new Vector2(gesture.FocusX, gesture.FocusY));
                Vector3 pos = draggingLetter.transform.position;

                // don't mess with the z
                float newPosition = dragPos.x + dragOffset.x;
                float containerX = this.container.GetComponent<RectTransform>().position.x;
                if (newPosition >= this.boundaries[0] + containerX && newPosition <= this.boundaries[1] + containerX)
                {
                    this.lastDelta = newPosition - pos.x;
                    pos.x = newPosition;
                    //pos.y = dragPos.y + dragOffset.y;
                    
                    draggingLetter.transform.position = pos;
                }


            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                this.blockDragManager.NotifyDraggingBlockEnd(this.lastDelta);

                GameObject gameObject = draggingLetter.gameObject;                

                draggingLetter = null;
            }
        }

        private void OnEnable()
        {
            canvas = GetComponent<Canvas>();
            letterMoveGesture = new PanGestureRecognizer();
            letterMoveGesture.ThresholdUnits = 0.0f; // start right away
            letterMoveGesture.StateUpdated += LetterGestureUpdated;
            FingersScript.Instance.AddGesture(letterMoveGesture);
        }

        private void OnDisable()
        {
            if (FingersScript.HasInstance)
            {
                FingersScript.Instance.RemoveGesture(letterMoveGesture);
            }
        }

        public void BackButtonClicked()
        {
            FingersExampleSceneTransitionScript.PopScene();
        }
    }
}
