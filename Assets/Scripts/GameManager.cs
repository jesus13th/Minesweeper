using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    private static GameManager instance;
    public static GameManager Instance => instance;
    public enum AudioClipEnum { Uncover, Flagged, Explosion, Win, Fail }
    public enum Difficulty { Easy = 10, Normal = 20, Hard = 40 }
    public static Difficulty difficulty = Difficulty.Normal;
    public Cell[,] gridMine;
    [SerializeField] private RectTransform cellContent;
    [SerializeField] private Cell pCell;
    [SerializeField] private RectTransform _canvas;
    [SerializeField] private Text counterFlagText;
    [SerializeField] private Animator buttonAnimator;
    private int counterSuccessFlag;
    public int CounterSuccessFlag {
        set { if ((counterSuccessFlag = value) == 0) StartCoroutine(Win()); }
        get { return counterSuccessFlag; }
    }
    private int counterFlag;
    public int CounterFlag { set { counterFlagText.text = (counterFlag = value).ToString(); } get { return counterFlag; } }
    [SerializeField] private Text timerText;
    private int timer = 0;
    [HideInInspector] public bool gameOver = false;
    [SerializeField] private Button _ResetBtn;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private AudioSource _as;
    [SerializeField] private AudioClip[] clips;

    [SerializeField] private Image pPanelGame;
    private bool panelGameInstance = false;

    void Start() {
        instance = this;
        ResponsiveContent();
        SpawnCells();
        SpawnMines();
        CounterFlag = CounterSuccessFlag = (int)difficulty;
        InvokeRepeating("TimerEvent", 1, 1);
        DatabaseManager.CreateFile();
        _as.volume = PlayerPrefs.GetInt(PanelGame.AudioKey) * 0.2f;
    }
    private void ResponsiveContent() {
        float cellSize = Mathf.FloorToInt((cellContent.rect.width * _canvas.localScale.x) / 10);
        int contentWidth = Mathf.FloorToInt((cellContent.rect.width * _canvas.localScale.x) / cellSize);
        int contentHeight = Mathf.FloorToInt((cellContent.rect.height * _canvas.localScale.y) / cellSize);

        cellContent.GetComponent<GridLayoutGroup>().cellSize = Vector2.one * cellSize;
        gridMine = new Cell[contentWidth, contentHeight];
    }
    private void TimerEvent() => timerText.text = (++timer).ToString();
    private void SpawnCells() {
        for (int y = 0; y < gridMine.GetLength(1); y++) {
            for (int x = 0; x < gridMine.GetLength(0); x++) {
                var gCell = Instantiate(pCell, cellContent);
                gCell.Position = new Cell.PositionStruct { x = x, y = y };
                gridMine[x, y] = gCell;
            }
        }
    }
    private void SpawnMines() {
        int counter = 0;
        do {
            Cell cell = gridMine[Random.Range(0, gridMine.GetLength(0)), Random.Range(0, gridMine.GetLength(1))];
            if (!cell.IsMine) {
                cell.IsMine = true;
                counter++;
            }
        } while (counter < (int)difficulty);
    }
    public void ResetScene() {
        if (!panelGameInstance) {
            AdsManager.Instance.ShowInterstitialAd();
            Instantiate(pPanelGame, _canvas, false);
            panelGameInstance = true;
            buttonAnimator.SetBool("Reset", false);
        }
    }
    public void PlayAudio(AudioClipEnum clipEnum) => _as.PlayOneShot(clips[(int)clipEnum]);
    private IEnumerator Win() {
        _ResetBtn.GetComponent<Image>().sprite = sprites[0];
        yield return StartCoroutine(Uncover(Cell.CellStateEnum.Uncover, AudioClipEnum.Uncover));
        yield return new WaitForSeconds(1.0f);
        PlayAudio(AudioClipEnum.Win);
        buttonAnimator.SetBool("Reset", true);
        DatabaseManager.InsertRegister(difficulty.ToString(), timer);
    }
    public IEnumerator Loose() {
        _ResetBtn.GetComponent<Image>().sprite = sprites[1];
        yield return StartCoroutine(Uncover(Cell.CellStateEnum.Mined, AudioClipEnum.Explosion));
        yield return new WaitForSeconds(1.0f);
        PlayAudio(AudioClipEnum.Fail);
        buttonAnimator.SetBool("Reset", true);
    }
    private IEnumerator Uncover(Cell.CellStateEnum cellState, AudioClipEnum clip) {
        gameOver = true;
        CancelInvoke("TimerEvent");
        for (int y = 0; y < gridMine.GetLength(1); y++) {
            for (int x = 0; x < gridMine.GetLength(0); x++) {
                var gCell = gridMine[x, y];
                if (gCell.IsMine && cellState == Cell.CellStateEnum.Mined || !gCell.IsMine && gCell.CellState == Cell.CellStateEnum.Cover && cellState == Cell.CellStateEnum.Uncover) {
                    gCell.CellState = cellState;
                    PlayAudio(clip);
                    yield return new WaitForSeconds(0.1f);
                }
            }
        }
    }
}