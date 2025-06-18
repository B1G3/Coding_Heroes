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
            return Color.forestGreen; // 녹색
        if (TryGetComponent(out HarvesterBox _))
            return Color.darkBlue;
        if (TryGetComponent(out StorageBox _))
            return Color.saddleBrown;
        if (TryGetComponent(out RailBox _))
            return Color.chocolate;

        return Color.gray; // 기본 색
    }
}