using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;
    private readonly List<Unit> _units = new();

    private void Awake() => Instance = this;
    
    public void Register(Unit unit) => _units.Add(unit);
    public void Unregister(Unit unit) => _units.Remove(unit);
}
