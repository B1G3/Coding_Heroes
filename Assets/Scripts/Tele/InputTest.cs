using UnityEngine;
using UnityEngine.Events;

public class InputTest : MonoBehaviour
{
    public UnityEvent onButtonDown;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            OnButtonDown();
    }
    
    private void OnButtonDown()
    {
        onButtonDown.Invoke();
    }
}
