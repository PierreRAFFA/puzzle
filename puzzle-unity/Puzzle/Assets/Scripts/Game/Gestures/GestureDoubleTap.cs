using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;
using UnityEngine.EventSystems;

public class GestureDoubleTap : MonoBehaviour
{
    /// <summary>
    /// Whether to enable double tap
    /// </summary>
    [Tooltip("Whether to enable double tap")]
    public bool EnableDoubleTap = true;

    public BlockDoubleTapManager blockDoubleTapManager;

    private TapGestureRecognizer tapGesture;
    private TapGestureRecognizer doubleTapGesture;

    private readonly List<RaycastResult> raycast = new List<RaycastResult>();

    private void Start()
    {
        tapGesture = new TapGestureRecognizer { MaximumNumberOfTouchesToTrack = 10 };
        tapGesture.StateUpdated += TapGesture_StateUpdated;
        FingersScript.Instance.AddGesture(tapGesture);

        if (EnableDoubleTap)
        {
            doubleTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            doubleTapGesture.StateUpdated += DoubleTapGesture_StateUpdated;
            tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
            FingersScript.Instance.AddGesture(doubleTapGesture);
        }
    }

    private void TapGesture_StateUpdated(DigitalRubyShared.GestureRecognizer gesture)
    {
        Debug.LogFormat("Single tap state: {0}", gesture.State);
        if (gesture.State == GestureRecognizerState.Ended)
        {
            Debug.LogFormat("Single tap at {0},{1}", gesture.FocusX, gesture.FocusY);
        }
    }

    private void DoubleTapGesture_StateUpdated(DigitalRubyShared.GestureRecognizer gesture)
    {
        Debug.LogFormat("Double tap state: {0}", gesture.State);
        if (gesture.State == GestureRecognizerState.Ended)
        {
            Debug.LogFormat("Double tap at {0},{1}", gesture.FocusX, gesture.FocusY);

            PointerEventData p = new PointerEventData(EventSystem.current);
            p.position = new Vector2(gesture.FocusX, gesture.FocusY);
            raycast.Clear();
            EventSystem.current.RaycastAll(p, raycast);
            foreach (RaycastResult result in raycast)
            {
                if (result.gameObject.name == "BlockDoubleTappable")
                {
                    print(result.gameObject.GetComponent<Block>().color);
                    this.blockDoubleTapManager.NotifyBlockDoubleTapped(result.gameObject);
                    break;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            tapGesture.Enabled = !tapGesture.Enabled;
            Debug.Log("Tap gesture enabled: " + tapGesture.Enabled);
        }
    }
}

