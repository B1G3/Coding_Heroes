using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BlockHolder : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference grabAction;          // ← 여기

    [Header("Hand Settings")]
    [SerializeField] private Transform _holdPoint;

    [Header("UI")]
    [SerializeField] private TMP_Text _text;

    [Header("LayerMasks")]
    [Tooltip("땅에만 설치 가능")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("겹치면 설치 불가")]
    [SerializeField] private LayerMask blockLayer;

    [Header("Placement")]
    [SerializeField] private float _placementCheckRadius = 0.1f;
    [SerializeField] private float _groundCheckDistance  = 0.2f;

    private GameObject currentBlock;
    private bool _isHolding;

    public event Action <Vector3, GameObject> OnPlaceBlock;
    
    private void OnEnable()
    {
        grabAction.action.Enable();
    }
    private void OnDisable()
    {
        grabAction.action.Disable();
    }

    void Update()
    {
        if (currentBlock == null) return;

        // Grab 버튼 눌렀을 때
        if (!_isHolding && grabAction.action.WasPressedThisFrame())
        {
            PickUpBlock();
        }
        // Grab 버튼 뗐을 때
        else if (_isHolding && grabAction.action.WasReleasedThisFrame())
        {
            TryPlaceBlock();
        }

        if (_isHolding)
            currentBlock.transform.position = _holdPoint.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isHolding) return;
        var clicker = other.GetComponent<BlockClicker>();
        if (clicker != null)
        {
            currentBlock = clicker.GetBlock();
            _text.text = currentBlock.name;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isHolding) return;
        if (other.GetComponent<BlockClicker>() != null)
        {
            currentBlock = null;
            _text.text = "";
        }
    }

    private void PickUpBlock()
    {
        _isHolding = true;
        // 픽업 했을 때 로직
    }

    private void TryPlaceBlock()
    {
        _isHolding = false;

        // 1) 땅 위에 있는지
        bool onGround = Physics.Raycast(
            _holdPoint.position,
            Vector3.down,
            out var hit,
            _groundCheckDistance,
            groundLayer
        );

        if (onGround)
            PlaceBlock(hit.point);
        else
            ResetHold();
    }

    private void PlaceBlock(Vector3 pos)
    {
        OnPlaceBlock?.Invoke(pos, currentBlock);
        currentBlock = null;
    }

    private void ResetHold()
    {
        currentBlock = null;
        _text.text = "Reset";
    }
}
