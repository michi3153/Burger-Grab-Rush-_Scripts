using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GrabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Outline buttonOutline;
    public Vector3 highlightedScale = new Vector3(1.1f, 1.1f, 1.1f);
    public Vector3 normalScale = Vector3.one;

    private void Start()
    {
        // アウトラインは常に表示
        if (buttonOutline != null)
        {
            buttonOutline.enabled = true;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = highlightedScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = normalScale;
    }
}
