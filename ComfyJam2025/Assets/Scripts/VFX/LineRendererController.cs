using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    public List<LineRenderer> lineRenderers;

    public void SetPosition(Vector3 startPos, Vector3 endPos) {
        if (lineRenderers.Count <= 0) return;

        for (int i = 0; i < lineRenderers.Count; i++) {
            if (lineRenderers[i].positionCount >= 2) {
                lineRenderers[i].SetPosition(0, startPos);
                lineRenderers[i].SetPosition(1, endPos);
            }
        }

    }
}
