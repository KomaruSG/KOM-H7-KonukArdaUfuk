using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Oyun B'nin yöneticisi: süreyi ve düşme sayısını tutar, varışta oyunu bitirir, R ile yeniden başlatır, ekrana bilgi yazar.
public class ParkurSayaci : MonoBehaviour
{
    public static ParkurSayaci Ornek { get; private set; } // Tetikleyicilerin yöneticiye ulaştığı tek örnek

    [SerializeField] Gezgin gezgin; // Arayüzde profil ve hız göstermek için

    float sure;       // Koşu süresi (sn)
    int dusmeSayisi;  // Kaç kez düşüldü
    bool basladi;     // Oyuncu ilk kez hareket etti mi (süre o an başlar)
    bool bitti;       // Varış noktasına ulaşıldı mı

    // Tek örneği kaydeder.
    void Awake()
    {
        Ornek = this;
    }

    // Süreyi sayar, R ile sahneyi yeniden yükler.
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (!basladi && gezgin != null && gezgin.Hiz.sqrMagnitude > 0.01f) basladi = true;
        if (basladi && !bitti) sure += Time.deltaTime;
    }

    // Düşme alanı çağırır.
    public void Dustu() => dusmeSayisi++;

    // Varış noktası çağırır.
    public void Bitir() => bitti = true;

    // Süre, profil, hız ve kontrolleri ekrana yazar.
    void OnGUI()
    {
        GUIStyle stil = new GUIStyle(GUI.skin.label) { fontSize = 22 };
        GUIStyle kucuk = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        string profil = gezgin != null && gezgin.Ayar != null ? gezgin.Ayar.ad : "-";
        float hiz = gezgin != null ? Mathf.Abs(gezgin.Hiz.x) : 0f;

        GUI.Label(new Rect(20, 15, 900, 30), $"Profil: {profil}    Süre: {sure:0.00}    Düşme: {dusmeSayisi}    Hız: {hiz:0.0}", stil);
        GUI.Label(new Rect(20, 48, 1100, 26), "A/D ya da oklar: yürü    W / ↑ / Boşluk: zıpla (basılı tut = yüksek)    Q: profil değiştir    R: baştan", kucuk);

        if (bitti)
        {
            GUIStyle buyuk = new GUIStyle(GUI.skin.label) { fontSize = 36, alignment = TextAnchor.MiddleCenter };
            GUI.Label(new Rect(0, Screen.height / 2f - 40, Screen.width, 50), $"Bitti!  {sure:0.00} sn, {dusmeSayisi} düşme   (R: tekrar)", buyuk);
        }
    }
}
