using UnityEngine;

public class ConnectionLine : MonoBehaviour
{
    public void SetEndpoints(Vector3 start, Vector3 end) {
        var line = GetComponent<LineRenderer>();
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
