using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour {
    public static AdsManager Instance => FindObjectOfType<AdsManager>();
    public const string IdAds = "Ads";
    private string androidGameId = "4366733";
    private string iOsGameId = "4366732";
    [SerializeField] private bool testMode = false;
    private string gameId;

    private void Awake() {
        gameId = Application.platform == RuntimePlatform.Android ? androidGameId : iOsGameId;
        Advertisement.Initialize(gameId, testMode);
    }
    public void ShowInterstitialAd() {
        if (PlayerPrefs.GetInt(IdAds) >= 5) {

            if (Advertisement.IsReady()) {
                Advertisement.Show();
                PlayerPrefs.SetInt(IdAds, 0);
            }
        }
        PlayerPrefs.SetInt(IdAds, PlayerPrefs.GetInt(IdAds) + 1);
    }
}