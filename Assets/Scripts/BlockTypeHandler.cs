using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlockTypeHandler : MonoBehaviour
{
    [Serializable]
    class UIConfig
    {
        [SerializeField]
        public GameObject typeObject;

        [SerializeField]
        public string buttonText;
    }
    
    [SerializeField] TMP_Text text;
    [SerializeField] List<UIConfig> typeList;
    
    private int currentTypeIndex = 0;
    
    public void SetType(int index)
    {
        typeList[currentTypeIndex].typeObject.SetActive(false);
        text.text = typeList[index].buttonText;
        typeList[index].typeObject.SetActive(true);
        currentTypeIndex = index;
    }
    
}
