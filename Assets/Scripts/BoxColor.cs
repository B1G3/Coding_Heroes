using UnityEngine;

public class BoxColor : MonoBehaviour
{
    [SerializeField] private Color color = Color.clear; // 인스펙터에서 지정 없을 시 자동 결정

    private void Awake()
    {
        if (color == Color.clear)
        {
            color = GetColorByType();
        }

        ApplyColor();
    }

    private void ApplyColor()
    {
        var renderer = GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(renderer.material); // 인스턴스화
            renderer.material.color = color;
        }
    }

    private Color GetColorByType()
    {
        if (TryGetComponent(out TreeResourceBox _))
            return new Color(0.3f, 0.8f, 0.3f); // 녹색
        if (TryGetComponent(out HarvesterBox _))
            return new Color(0.2f, 0.6f, 1.0f); // 파랑
        if (TryGetComponent(out StorageBox _))
            return new Color(1.0f, 0.8f, 0.2f); // 노랑

        return Color.gray; // 기본 색
    }
}