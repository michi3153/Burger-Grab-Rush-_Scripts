using UnityEngine;
using UnityEngine.Events;
using Leap;

public class FingerPressButton : MonoBehaviour
{
    public UnityEvent onPress;
    public LeapProvider leapProvider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("IndexTip"))
        {
            Frame frame = leapProvider.CurrentFrame;

            foreach (Leap.Hand hand in frame.Hands)
            {
                // l·‚µw‚¾‚¯‚ªL‚Ñ‚Ä‚¢‚éó‘Ô‚©”»’è
                bool isIndexOnly = hand.fingers[1].IsExtended &&   // index
                                   !hand.fingers[0].IsExtended &&  // thumb
                                   !hand.fingers[2].IsExtended &&  // middle
                                   !hand.fingers[3].IsExtended &&  // ring
                                   !hand.fingers[4].IsExtended;    // pinky

                if (isIndexOnly)
                {
                    Debug.Log("Index-only gesture detected.");
                    onPress?.Invoke();
                    break;
                }
            }
        }
    }
}
