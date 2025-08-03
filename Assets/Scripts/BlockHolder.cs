using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using static BlockConfig;

public class BlockHolder : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    
    [Header("Input")]
    [SerializeField] private InputActionReference grabAction;          // ← 여기
    
    [Header("Hand Settings")]
    [SerializeField] private Transform holdPoint;

    [Header("LayerMasks")]
    [Tooltip("땅에만 설치 가능")]
    [SerializeField] private LayerMask groundLayer;
    [Tooltip("길은 블럭부터 설치")]
    [SerializeField] private LayerMask blockLayer;
    
    [Header("Placement")]
    [SerializeField] private float placementCheckDistance = 0.2f;
    

    private enum Mode { None, Block, PlacingPath }
    private Mode mode = Mode.None;
    
    private int interactMask;
    private RaycastHit hit;
    private bool OnGround;
    private bool OnBlock;
    
    private GameObject currentBlock;
    private BlockType currentBlockType = BlockType.None;
    private bool isHolding;
    
    private GameObject pathStartBlock;
    private IOutput pathStartOutput;
    private Vector3Int pathEnd;

    public event Action <Vector3, GameObject> OnPlaceBlock;
    public event Action <GameObject, Vector3Int, GameObject> OnPlaceCorner;
    public event Action <Vector3, bool> OnPreviewBlock;
    public event Action <Vector3, Vector3Int, bool> OnPreviewPath;
    public event Action OnDestroyPreview;
    
    private void Awake()
    {
        interactMask = groundLayer.value | blockLayer.value;
    }
    
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

        if (mode != Mode.None)
        {
            ConfigPosition();
        }

        if (mode == Mode.Block)
        {
            // 현재 설치할라고 하고 있고 땅 위에 있을 경우에만
            if (isHolding && (OnGround || OnBlock))
            {
                OnPreviewBlock?.Invoke(hit.point, OnGround);
            }
            
            // Grab 버튼 눌렀을 때
            if (!isHolding && grabAction.action.WasPressedThisFrame())
            {
                PickUpBlock();
            }
            // Grab 버튼 뗐을 때
            else if (isHolding && grabAction.action.WasReleasedThisFrame())
            {
                TryPlaceBlock();
            }
        }
        else if (mode == Mode.PlacingPath)
        {
            if (!isHolding && (OnGround || OnBlock))
            {
                var canEnd = BlockManager.Instance.IsOutputBlock(hit.collider);
                OnPreviewBlock?.Invoke(hit.point, canEnd);
            }
            if (isHolding && (OnGround || OnBlock))
            {
                var canEnd = OnGround || (OnBlock && BlockManager.Instance.IsInputBlock(hit.collider));
                
                if (pathStartBlock == null)
                {
                    text.text = "pathStartBlock is null";
                    return;
                }
    
                if (pathStartOutput == null)
                {
                    text.text = "pathStartOutput is null";
                    return;
                }
                
                text.text = $"Path from {pathStartBlock.name} to ";
                switch (pathStartOutput.GetOutputDirection())
                {
                    case WorldDirection.East:
                        text.text += $"East";
                        break;
                    case WorldDirection.West:
                        text.text += $"West";
                        break;
                    case WorldDirection.South:
                        text.text += $"South";
                        break;
                    case WorldDirection.North:
                        text.text += $"North";
                        break;
                    default:
                        text.text += $"Error";
                        break;
                }
                
                pathEnd = GridUtils.SnapToDirection(
                    pathStartBlock.transform.position, 
                    hit.point, 
                    pathStartOutput.GetOutputDirection()
                );
                OnPreviewPath?.Invoke(pathStartBlock.transform.position, pathEnd, canEnd);
            }
            
            // Grab 버튼 눌렀을 때
            if (!isHolding && grabAction.action.WasPressedThisFrame())
            {
                TryGetStartBlock();
            }
            // Grab 버튼 뗐을 때
            else if (isHolding && grabAction.action.WasReleasedThisFrame())
            {
                TryPlacePath();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (isHolding) return;
        var clicker = other.GetComponent<BlockClicker>();
        if (clicker != null)
        {
            currentBlock = clicker.GetBlock();
            currentBlockType = clicker.GetBlockType();
            
            if (currentBlockType is BlockType.Block or BlockType.Unit or BlockType.Data)
            {
                mode = Mode.Block;
            }
            else if (currentBlockType is BlockType.Path)
            {
                mode = Mode.PlacingPath;
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
        if (isHolding || currentBlockType is BlockType.Path) return;
        if (other.GetComponent<BlockClicker>() != null)
        {
            ResetHold();
        }
    }

    private void ConfigPosition()
    {
        if (Physics.Raycast(holdPoint.position, Vector3.down,
                out hit, placementCheckDistance, interactMask))
        {
            int mask = 1 << hit.collider.gameObject.layer;
            OnGround = (mask & groundLayer.value) != 0;
            OnBlock  = (mask & blockLayer.value)  != 0;
        }
        else
        {
            OnGround = OnBlock = false;
        }
    }

    private void PickUpBlock()
    {
        isHolding = true;
        // 픽업 했을 때 로직
    }

    private void TryPlaceBlock()
    {
        isHolding = false;

        if (OnGround)
            PlaceBlock(hit.point);
        
        ResetHold();
    }

    private void PlaceBlock(Vector3 pos)
    {
        OnPlaceBlock?.Invoke(pos, currentBlock);
    }

    private void TryGetStartBlock()
    {
        isHolding = true;
        
        if (OnBlock && BlockManager.Instance.IsOutputBlock(hit.collider))
        {
            isHolding = true;
            pathStartBlock = hit.collider.gameObject;
            pathStartOutput = BlockManager.Instance.GetOutputBlock(hit.collider);
        }
        else
        {
            isHolding = false;
            ResetHold(); // 이거 초기화 안하면 무한으로 설치 가능 >> 이거를 나중에 바꿔서 무한으로 설치하도록
        }
    }

    private void TryPlacePath()
    {
        isHolding = false;
        
        var canPlacePath = OnGround || (OnBlock && BlockManager.Instance.IsInputBlock(hit.collider));
        
        if (canPlacePath && pathStartBlock != null && pathStartOutput != null)
        {
            OnPlaceCorner?.Invoke(
                pathStartBlock,
                pathEnd,
                currentBlock
            );
        }
        
        pathStartBlock = null;
        pathStartOutput = null;
        ResetHold();
    }

    private void ResetHold()
    {
        OnDestroyPreview?.Invoke();
        currentBlock = null;
        currentBlockType = BlockType.None;
        mode = Mode.None;
        OnGround = OnBlock = false;
    }
}
