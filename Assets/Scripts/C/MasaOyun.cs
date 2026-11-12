using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Oyun C'nin yöneticisi: atış hakkını, masadaki topları, puanı ve kalan camları tutar;
// kazanma/kaybetme kararını verir, R ile yeniden başlatır, ekrana bilgi yazar.
public class MasaOyun : MonoBehaviour
{
    public static MasaOyun Ornek { get; private set; } // Diğer betiklerin yöneticiye ulaştığı tek örnek

    [SerializeField] int atisHakki = 5;     // Oyun başındaki atış sayısı
    [SerializeField] int camPuani = 10;     // Kırılan her cam için puan
    [SerializeField] int delikPuani = 50;   // Deliğe giren her top için puan

    int puan;            // Toplam puan
    int kalanCam;        // Masada kalan kırılabilir blok sayısı
    int aktifTop;        // Masada hâlâ hareket eden top sayısı
    bool kazandi;        // Bütün camlar kırıldı mı
    bool kaybetti;       // Atış hakkı bitti ve cam kaldı mı

    public bool OyunBitti => kazandi || kaybetti; // Oyun sona erdi mi
    public int KalanAtis => atisHakki;            // Kalan atış sayısı

    // Tek örneği kaydeder.
    void Awake()
    {
        Ornek = this;
    }

    // Sahnedeki kırılabilir blokları sayar (bölünen bloklar da CamBlok'tan türediği için sayılır).
    void Start()
    {
        kalanCam = FindObjectsByType<CamBlok>(FindObjectsSortMode.None).Length;
    }

    // R ile yeniden başlatır.
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Fırlatıcı sorar: şu an atış yapılabilir mi? (Masada top yoksa ve hak varsa.)
    public bool AtisYapabilirMi() => !OyunBitti && aktifTop == 0 && atisHakki > 0;

    // Fırlatıcı atış yaptığında hakkı bir azaltır.
    public void AtisYapildi() => atisHakki--;

    // Masaya yeni bir top girdiğinde (fırlatma ya da bölünme) çağrılır.
    public void TopEklendi() => aktifTop++;

    // Bir top durduğunda ya da deliğe girdiğinde çağrılır; son top da gittiyse kaybetme kontrol edilir.
    public void TopGitti()
    {
        aktifTop = Mathf.Max(0, aktifTop - 1);
        if (aktifTop == 0 && atisHakki == 0 && kalanCam > 0) kaybetti = true;
    }

    // Bir cam kırıldığında puan ekler; hiç cam kalmadıysa kazanılır.
    public void CamKirildi()
    {
        puan += camPuani;
        kalanCam--;
        if (kalanCam <= 0) kazandi = true;
    }

    // Bir top deliğe girdiğinde puan ekler.
    public void DeligeGirdi() => puan += delikPuani;

    // Puan, kalan atış ve camları ekrana yazar.
    void OnGUI()
    {
        GUIStyle stil = new GUIStyle(GUI.skin.label) { fontSize = 22 };
        GUIStyle kucuk = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        GUI.Label(new Rect(20, 15, 900, 30), $"Puan: {puan}    Atış: {atisHakki}    Kalan cam: {kalanCam}", stil);
        GUI.Label(new Rect(20, 48, 1100, 26), "Fare: nişan al    Sol tık basılı tut: güç topla, bırak: fırlat    R: baştan", kucuk);

        if (OyunBitti)
        {
            GUIStyle buyuk = new GUIStyle(GUI.skin.label) { fontSize = 36, alignment = TextAnchor.MiddleCenter };
            string mesaj = kazandi ? $"Masa temiz!  {puan} puan" : $"Atış bitti.  {puan} puan";
            GUI.Label(new Rect(0, Screen.height / 2f - 40, Screen.width, 50), mesaj + "   (R: tekrar)", buyuk);
        }
    }
}
