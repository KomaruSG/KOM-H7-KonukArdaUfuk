using UnityEngine;
using UnityEngine.SceneManagement;

// Oyun A'nın yöneticisi: skoru tutar, zorluğu zamanla artırır,
// oyunun bitişini ve yeniden başlatmayı yönetir, ekrana bilgi yazar.
public class RenkKapisiOyun : MonoBehaviour
{
    public static RenkKapisiOyun Ornek { get; private set; } // Diğer betiklerin yöneticiye ulaştığı tek örnek

    [Header("Zorluk")]
    [SerializeField] float baslangicHizi = 4f;  // Kapıların oyun başındaki hızı (birim/sn)
    [SerializeField] float hizArtisi = 0.12f;   // Her saniye hıza eklenen miktar: zamanla artan zorluk sayısı
    [SerializeField] float azamiHiz = 11f;      // Hızın çıkabileceği en yüksek değer

    [Header("Bağlantılar")]
    [SerializeField] RenkKarakter karakter; // Arayüzde kalkan durumunu göstermek için

    [Header("Yeniden başlatma")]
    [SerializeField] float yenidenBaslatmaBeklemesi = 0.7f; // Kaybedince tuşa yanlışlıkla basıp hemen yeniden başlamayı önleyen bekleme

    public float KapiHizi { get; private set; } // Kapıların şu anki hızı; kapılar bunu okur
    public bool OyunBitti { get; private set; } // Oyuncu kaybettiyse true
    public float GecenSure { get; private set; } // Oyun başından beri geçen süre (sn)

    int skor;           // Geçilen kapı sayısı
    float bitisZamani;  // Oyunun bittiği an (Time.time)

    // Tek örneği kaydeder ve değerleri başlangıca çeker.
    void Awake()
    {
        Ornek = this;
        KapiHizi = baslangicHizi;
    }

    // Her kare: oyun sürüyorsa süreyi ve hızı artırır; bittiyse yeniden başlatma tuşunu bekler.
    void Update()
    {
        if (!OyunBitti)
        {
            GecenSure += Time.deltaTime;
            KapiHizi = Mathf.Min(azamiHiz, baslangicHizi + hizArtisi * GecenSure);
        }
        else if (Time.time - bitisZamani > yenidenBaslatmaBeklemesi && TekTusGirdi.BasildiMi())
        {
            // Aynı tek tuş oyunu yeniden başlatır; sahne baştan yüklenir.
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Karakter bir kapıyı doğru renkte (ya da kalkanla) geçtiğinde çağrılır.
    public void KapiGecildi()
    {
        if (!OyunBitti) skor++;
    }

    // Karakter yanlış renkte bir kapıya kalkansız çarptığında çağrılır.
    public void Kaybet()
    {
        if (OyunBitti) return;
        OyunBitti = true;
        bitisZamani = Time.time;
    }

    // Ekrana skor, hız ve kontrolleri yazar (hızlı ve basit arayüz).
    void OnGUI()
    {
        GUIStyle stil = new GUIStyle(GUI.skin.label) { fontSize = 22 };
        string kalkan = karakter == null ? "" :
            (karakter.KalkanaKalanSure <= 0f ? "Kalkan: hazır" : $"Kalkan: {karakter.KalkanaKalanSure:0.0} sn");
        GUI.Label(new Rect(20, 15, 900, 30), $"Kapı: {skor}    Hız: {KapiHizi:0.0}    {kalkan}", stil);

        GUIStyle kucuk = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        GUI.Label(new Rect(20, 48, 900, 26), "Boşluk / tık: renk değiştir    Hızlı çift bas: kalkan", kucuk);

        if (OyunBitti)
        {
            GUIStyle buyuk = new GUIStyle(GUI.skin.label) { fontSize = 36, alignment = TextAnchor.MiddleCenter };
            GUIStyle orta = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, Screen.height / 2f - 60, Screen.width, 50), $"Kaybettin!  {skor} kapı", buyuk);
            GUI.Label(new Rect(0, Screen.height / 2f, Screen.width, 40), "Yeniden başlamak için Boşluk", orta);
        }
    }
}
