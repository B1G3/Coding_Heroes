using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using static BlockConfig;

public class BlockHolder : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference grabAction;          // ← 여기

    [Header("Hand Settings")]
    [SerializeField] private Transform _holdPoint;

    [Header("UI")]
    // [SerializeField] private TMP_Text _text;

    [Header("LayerMasks")]
    [Tooltip("땅에만 설치 가능")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("길은 블럭부터 설치")]
    [SerializeField] private LayerMask blockLayer;

    [Header("Placement")]
    [SerializeField] private float placementCheckDistance = 0.1f;
    [SerializeField] private float groundCheckDistance  = 0.2f;

    private enum Mode { None, Block, WaitingForPathStart, PlacingPath }
    private Mode mode = Mode.None;
    
    private GameObject currentBlock;
    private BlockType currentBlockType = BlockType.None;
    private bool _isHolding;
    
    private GameObject pathStartBlock;
    private GameObject pathEndBlock;

    public event Action <Vector3, GameObject> OnPlaceBlock;
    public event Action <GameObject, GameObject, GameObject> OnPlacePath;
    
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
        if (currentBlockType == BlockType.None) return;

        if (mode == Mode.Block)
        {
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
        }
        else if (mode == Mode.WaitingForPathStart)
        {
            // Grab 버튼 눌렀을 때
            if (!_isHolding && grabAction.action.WasPressedThisFrame())
            {
                TryGetStartBlock();
            }
            // Grab 버튼 뗐을 때
            else if (_isHolding && grabAction.action.WasReleasedThisFrame())
            {
                TryPlacePath();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isHolding) return;
        var clicker = other.GetComponent<BlockClicker>();
        if (clicker != null)
        {
            currentBlock = clicker.GetBlock();
            currentBlockType = clicker.GetBlockType();
            // _text.text = currentBlock.name;
            
            if (currentBlockType == BlockType.Block || currentBlockType == BlockType.Unit || currentBlockType == BlockType.Data)
            {
                mode = Mode.Block;
            }
            else if (currentBlockType == BlockType.Path)
            {
                mode = Mode.WaitingForPathStart;
            }
            else
            {
                // other types (unit, etc.)
                mode = Mode.None;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isHolding || currentBlockType == BlockType.Path) return;
        if (other.GetComponent<BlockClicker>() != null)
        {
            ResetHold();
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
            groundCheckDistance,
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
        currentBlockType = BlockType.None;
        currentBlock = null;
    }

    private void TryGetStartBlock()
    {
        _isHolding = true;
        
        bool onBlock = Physics.Raycast(
            _holdPoint.position,
            Vector3.down,
            out var hit,
            placementCheckDistance,
            blockLayer
        );
        
        if (onBlock)
        {
            pathStartBlock = hit.collider.gameObject;
            // _text.text = $"Path Start from {pathStartBlock.name}";
            // mode = Mode.PlacingPath;
        }
        else
        {
            // _text.text = $"Path Start failed";
            _isHolding = false;
            ResetHold();
        }
    }

    private void TryPlacePath()
    {
        _isHolding = false;
        
        bool onBlock = Physics.Raycast(
            _holdPoint.position,
            Vector3.down,
            out var hit,
            placementCheckDistance,
            blockLayer
        );
        
        if (onBlock)
        {
            pathEndBlock = hit.collider.gameObject;
            // _text.text = $"Path End at {pathEndBlock.name}";
            OnPlacePath?.Invoke(pathStartBlock, pathEndBlock, currentBlock);
            pathStartBlock = null;
            pathEndBlock = null;
            // ResetHold();
            currentBlock = null;
            currentBlockType = BlockType.None;
            mode = Mode.None;
        }
        else
        {
            // _text.text = $"Path End failed";
            pathStartBlock = null;
            ResetHold();
        }
    }

    private void ResetHold()
    {
        currentBlock = null;
        currentBlockType = BlockType.None;
        mode = Mode.None;
        // _text.text = "Reset";
    }
}
