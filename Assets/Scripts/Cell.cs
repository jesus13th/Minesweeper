using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cell : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    [SerializeField] private Image _image;
    [SerializeField] private Text _text;
    [System.Serializable] public struct PositionStruct { public int x; public int y; }

    [SerializeField] private PositionStruct position;
    public PositionStruct Position { set { position = value; name = $"Cell({value.x}, {value.y})"; } get { return position; } }

    [SerializeField] Sprite[] sprites;
    [SerializeField] Color[] colors;

    [SerializeField] private bool isMine;
    public bool IsMine { get { return isMine; } set { isMine = value; } }
    [SerializeField] private bool isPressed;
    [SerializeField] private bool isPressing = false;
    [SerializeField] private float timer = 0;
    [SerializeField] private float timerFlagged = 2.0f;
    public enum CellStateEnum { Cover, Uncover, Flagged, Mined }
    [SerializeField] private CellStateEnum cellState = CellStateEnum.Cover;
    public CellStateEnum CellState {
        set {
            cellState = value;
            _image.sprite = sprites[(int)value];
            if (value == CellStateEnum.Uncover) {
                var nearMines = GetNearMines;
                _text.enabled = nearMines > 0;
                _text.text = nearMines.ToString();
                _text.color = colors[nearMines];
                if (nearMines == 0) {
                    StartCoroutine(UncoverNear());
                }
            }
        }
        get {
            return cellState;
        }
    }

    Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1) };

    private void Update() {
        if (isPressing)
            if ((timer += Time.deltaTime) > timerFlagged) {
                SetState();
                isPressed = true;
            }
    }
    public void OnPointerDown(PointerEventData eventData) {
        isPressing = !GameManager.Instance.gameOver;
    }
    public void OnPointerUp(PointerEventData eventData) {
        if (!GameManager.Instance.gameOver) {
            isPressing = false;
            if (!isPressed)
                SetState();
            isPressed = false;
        }
    }
    private void SetState() {
        if (CellState == CellStateEnum.Uncover || CellState == CellStateEnum.Mined) {
            return;
        }
        if (timer > timerFlagged) {
            if (CellState == CellStateEnum.Cover) {
                GameManager.Instance.CounterFlag--;
                CellState = CellStateEnum.Flagged;
                if (IsMine)
                    GameManager.Instance.CounterSuccessFlag--;
            } else {
                GameManager.Instance.CounterFlag++;
                CellState = CellStateEnum.Cover;
                if (IsMine)
                    GameManager.Instance.CounterSuccessFlag++;
            }
            GameManager.Instance.PlayAudio(GameManager.AudioClipEnum.Flagged);
            Handheld.Vibrate();
        } else {
            if (CellState != CellStateEnum.Flagged) {
                if (IsMine) {
                    CellState = CellStateEnum.Mined;
                    GameManager.Instance.PlayAudio(GameManager.AudioClipEnum.Explosion);
                    StartCoroutine(GameManager.Instance.Loose());
                } else {
                    CellState = CellStateEnum.Uncover;
                    GameManager.Instance.PlayAudio(GameManager.AudioClipEnum.Uncover);
                }
            }
        }
        isPressing = false;
        timer = 0;
    }
    public int GetNearMines {
        get {
            List<bool> result = new List<bool>();
            foreach (var dir in directions) {
                try {
                    result.Add(GetNearCell(dir).IsMine);
                } catch { }
            }
            return result.Count(n => n);
        }
    }
    private Cell GetNearCell(Vector2Int direction) => GameManager.Instance.gridMine[position.x + direction.x, position.y + direction.y];
    private IEnumerator UncoverNear() {
        foreach (var dir in directions) {
            try {
                var cell = GetNearCell(dir);
                if (!cell.isMine && cell.CellState == CellStateEnum.Cover) {
                    cell.CellState = CellStateEnum.Uncover;
                    GameManager.Instance.PlayAudio(GameManager.AudioClipEnum.Uncover);
                }
            } catch { }
            yield return new WaitForSecondsRealtime(0.04f);
        }
    }
}