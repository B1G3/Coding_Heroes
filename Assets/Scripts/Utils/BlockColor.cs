using UnityEngine;

public class BlockColor : MonoBehaviour
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
        if (TryGetComponent(out StartNode _))
            return Color.forestGreen;
        if (TryGetComponent(out EndNode _))
            return Color.greenYellow;
        if (TryGetComponent(out Block _))
            return Color.darkBlue;
        if (TryGetComponent(out PathTile _))
            return Color.saddleBrown;
        if(TryGetComponent(out MoveNode _))
            return Color.cyan;
        if(TryGetComponent(out DataNode _))
            return Color.orangeRed;
        

        return Color.gray; // 기본 색
    }
}