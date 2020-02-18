using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIPositionHandler : MonoBehaviour
{
    public Transform ownerTransform;
    public RectTransform canvasTransform;
    private RectTransform textTransform;

    private float cameraOrthoSize = 0.0f;
    private void Start()
    {
        cameraOrthoSize = Camera.main.orthographicSize;
    }

    public void SetData(Transform ownerTrans, RectTransform canvasTrans)
    {
        ownerTransform = ownerTrans;
        canvasTransform = canvasTrans;
        textTransform = gameObject.GetComponent<RectTransform>();
    }
    public void SetUIPosition()
    {
        //Convert world position to Viewport position
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(ownerTransform.position);

        //Get Exact screen position by accounting for canvas distortions.
        Vector2 worldObject_ScreenPosition = new Vector2(
        (viewportPosition.x * canvasTransform.sizeDelta.x) - (canvasTransform.sizeDelta.x * 0.5f),
        (viewportPosition.y * canvasTransform.sizeDelta.y) - (canvasTransform.sizeDelta.y * 0.5f));

        //Set the Position of UI
        textTransform.anchoredPosition = worldObject_ScreenPosition;
    }

    private void Update()
    {
        //If Camera ZOOMS IN/OUT Re-Set the UI Position
        if (Camera.main.orthographicSize != cameraOrthoSize)
        {
            SetUIPosition();
            cameraOrthoSize = Camera.main.orthographicSize;
        }
    }
}
