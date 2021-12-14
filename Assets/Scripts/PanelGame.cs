using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PanelGame : MonoBehaviour {
    [SerializeField] private RectTransform layoutGroup;
    [SerializeField] private LogContainer pLogContainer;

    [SerializeField] private Sprite[] audioSprites;
    [SerializeField] private Image audioImg_Btn;
    private enum AudioState { Mute = 0, Low = 1, High = 2 }
    [SerializeField] private AudioState audioState = AudioState.Low;
    public const string AudioKey = "Audio";

    private void Start() {
        var registros = DatabaseManager.LoadRegistros();
        layoutGroup.sizeDelta = new Vector2(layoutGroup.sizeDelta.x, registros.Count * 100);
        registros.ForEach(r => Instantiate(pLogContainer, layoutGroup).Register = r);
        audioState = (AudioState)PlayerPrefs.GetInt(AudioKey);
        audioImg_Btn.sprite = audioSprites[(int)audioState];
    }
    private void LoadScene(GameManager.Difficulty difficulty) {
        GameManager.difficulty = difficulty;
        SceneManager.LoadScene(0);
    }
    public void GameEasy() => LoadScene(GameManager.Difficulty.Easy);
    public void GameNormal() => LoadScene(GameManager.Difficulty.Normal);
    public void GameHard() => LoadScene(GameManager.Difficulty.Hard);
    public void AudioControl() {
        audioState = (AudioState)((int)(audioState + 1) % audioSprites.Length);
        audioImg_Btn.sprite = audioSprites[(int)audioState];
        PlayerPrefs.SetInt(AudioKey, (int)audioState);
    }
}