using TMPro;
using UnityEngine;
using static BlockConfig;

public class RailUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NorthText;
    [SerializeField] private TextMeshProUGUI SouthText;
    [SerializeField] private TextMeshProUGUI EastText;
    [SerializeField] private TextMeshProUGUI WestText;
    
    private RailModule target;

    public void Initialize(RailModule railModule)
    {
        if (target != null)
            target.OnDirectionChanged -= UpdateUI;

        target = railModule;
        target.OnDirectionChanged += UpdateUI;

        // 처음 한 번 표시
        UpdateUI(target.GetInputDirection(), target.GetOutputDirection());
    }

    private void UpdateUI(Direction input, Direction output)
    {
        ClearUI();
        if (input == output)
        {
            SetDirectionUI(Direction.North, ">", Color.gray2);
            SetDirectionUI(Direction.South, ">", Color.gray2);
            SetDirectionUI(Direction.East, ">", Color.gray2);
            SetDirectionUI(Direction.West, ">", Color.gray2);
            return;
        }
        SetDirectionUI(input, "<", Color.red);
        SetDirectionUI(output, ">", Color.blue);
    }

    private void SetDirectionUI(Direction dir, string symbol, Color color)
    {
        switch (dir)
        {
            case Direction.North:
                NorthText.text = symbol;
                NorthText.color = color;
                break;
            case Direction.South:
                SouthText.text = symbol;
                SouthText.color = color;
                break;
            case Direction.East:
                EastText.text = symbol;
                EastText.color = color;
                break;
            case Direction.West:
                WestText.text = symbol;
                WestText.color = color;
                break;
        }
    }

    private void ClearUI()
    {
        NorthText.text = SouthText.text = EastText.text = WestText.text = "";
    }
}
