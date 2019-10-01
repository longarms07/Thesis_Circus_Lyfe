using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDragListener
{
    void TouchStarted(Vector3 startPosition);

    void DragStarted(Vector3[] dragPositions);

    void DragPoisitonChanged(Vector3[] dragPositions);

    void DragEnded(Vector3[] dragPositions, List<Vector2> deltaPositions);
}
