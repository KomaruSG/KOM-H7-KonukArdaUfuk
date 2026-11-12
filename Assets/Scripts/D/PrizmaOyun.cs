using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Oyun D (Prizma) yöneticisi: can, ölüm ve kontrol noktasından doğma, ilerleme (mesafe), süre, kazanma/kaybetme.
// Bağlantı testi için üç mekanik oyun içinde tek tek kapatılabilir:
//   F1: tek tuş (faz değiştirme)   F2: hareket hissi (fazların profilleri)   F3: çarpışma tasarımı (kristallerin kırılması)
public class PrizmaOyun : MonoBehaviour
{
    public static PrizmaOyun Ornek { get; private set; } // Diğer betiklerin yöneticiye ulaştığı tek örnek

    // Bağlantı testi anahtarları. Statik oldukları için R ile yeniden başlatınca da korunurlar.
    public static bool TekTusAcik = true;   // Faz değiştirme açık mı
    public static bool HareketAcik = true;  // Fazlara özel hareket profilleri açık mı (kapalıysa düz profil)
    public static bool CarpismaAcik = true; // Kristaller kırılıp enerji ve sıçrama veriyor mu

    [SerializeField] Gezgin gezgin;          // Oyuncu karakteri
    [SerializeField] FazKontrol faz;         // Oyuncunun faz/enerji betiği
    [SerializeField] int canSayisi = 5;      // Toplam can
    [SerializeField] float toplamMesafe = 210f; // Başlangıçtan varışa yaklaşık mesafe (arayüzdeki ilerleme için)

    public float Mesafe => enIleriX - baslangicX; // İlerleme ölçüsü: ulaşılan en ileri nokta (metre)
    public bool OyunBitti => kazandi || kaybetti; // Oyun sona erdi mi

    float baslangicX;       // Oyuncunun başladığı x
    float enIleriX;         // Ulaşılan en ileri x
    float sure;             // Geçen süre (sn)
    int olumSayisi;         // Toplam ölüm
    bool kazandi;           // Varışa ulaşıldı mı
    bool kaybetti;          // Canlar bitti mi
    float kayitliEnerji;    // Son kontrol noktasındaki enerji
    FazRengi kayitliFaz;    // Son kontrol noktasındaki faz
    string mesaj = "";      // Ekranda kısa süre görünen mesaj
    float mesajBitisi;      // Mesajın kalkacağı an

    // Tek örneği kaydeder.
    void Awake()
    {
        Ornek = this;
    }

    // Başlangıç değerlerini kaydeder.
    void Start()
    {
        baslangicX = gezgin.transform.position.x;
        enIleriX = baslangicX;
        kayitliEnerji = faz.Enerji;
        kayitliFaz = faz.Faz;
    }

    // Girdi: R baştan, F1–F3 bağlantı testi anahtarları. Süre ve ilerlemeyi günceller.
    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k != null)
        {
            if (k.rKey.wasPressedThisFrame) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (k.f1Key.wasPressedThisFrame) TekTusAcik = !TekTusAcik;
            if (k.f2Key.wasPressedThisFrame) { HareketAcik = !HareketAcik; faz.HareketiGuncelle(); }
            if (k.f3Key.wasPressedThisFrame) CarpismaAcik = !CarpismaAcik;
        }

        if (OyunBitti) return;
        sure += Time.deltaTime;
        enIleriX = Mathf.Max(enIleriX, gezgin.transform.position.x);
    }

    // Oyuncu öldüğünde: can düşer; can kaldıysa son kontrol noktasından, kaydedilen faz ve enerjiyle doğar.
    // Enerji en az iki faz değişimine yetecek kadar verilir (oyuncu hiç değiştiremeyeceği bir yerde sıkışmasın).
    public void Ol(string sebep)
    {
        if (OyunBitti) return;
        olumSayisi++;
        canSayisi--;
        MesajVer(sebep);

        if (canSayisi <= 0)
        {
            kaybetti = true;
            gezgin.enabled = false;
            return;
        }

        gezgin.YenidenDog();
        faz.EnerjiAyarla(Mathf.Max(kayitliEnerji, faz.Maliyet * 2f));
        faz.FazUygula(kayitliFaz);
    }

    // Kontrol noktası alınınca doğma yeri, enerji ve faz kaydedilir.
    public void KontrolNoktasiAlindi(Vector2 konum)
    {
        gezgin.DogmaNoktasi = konum;
        kayitliEnerji = faz.Enerji;
        kayitliFaz = faz.Faz;
        MesajVer("Kontrol noktası");
    }

    // Varışa ulaşılınca oyun kazanılır.
    public void Bitir()
    {
        if (OyunBitti) return;
        kazandi = true;
        gezgin.enabled = false;
        Rigidbody2D rb = gezgin.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
    }

    // Ekranda kısa süreli mesaj gösterir.
    void MesajVer(string metin)
    {
        mesaj = metin;
        mesajBitisi = Time.time + 1.8f;
    }

    // Arayüz: can, faz, enerji çubuğu, maliyet, kilit, ilerleme, süre, test anahtarları, mesajlar.
    void OnGUI()
    {
        GUIStyle stil = new GUIStyle(GUI.skin.label) { fontSize = 22 };
        GUIStyle kucuk = new GUIStyle(GUI.skin.label) { fontSize = 16 };
        GUIStyle orta = new GUIStyle(GUI.skin.label) { fontSize = 28, alignment = TextAnchor.MiddleCenter };

        string kilit = faz.KilitKalan > 0f ? $"   KİLİT {faz.KilitKalan:0.0} sn" : "";
        GUI.Label(new Rect(20, 12, 1200, 30),
            $"Can: {canSayisi}    Faz: {FazRenkleri.Ad(faz.Faz)}    Maliyet: {faz.Maliyet:0.00}{kilit}    Mesafe: {Mesafe:0} / {toplamMesafe:0} m    Süre: {sure:0.0}", stil);

        // Enerji çubuğu.
        Rect cerceve = new Rect(20, 48, 240, 18);
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        GUI.DrawTexture(cerceve, Texture2D.whiteTexture);
        GUI.color = FazRenkleri.Renk(faz.Faz);
        GUI.DrawTexture(new Rect(cerceve.x, cerceve.y, cerceve.width * (faz.Enerji / faz.AzamiEnerji), cerceve.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(270, 44, 300, 26), $"Enerji {faz.Enerji:0.0} / {faz.AzamiEnerji:0}", kucuk);

        GUI.Label(new Rect(20, 72, 1200, 24), "A/D: yürü    W / ↑: zıpla    Boşluk: faz değiştir    Kristale üstten düşerek kır    R: baştan", kucuk);
        GUI.Label(new Rect(20, 94, 1200, 24),
            $"Test → F1 Tek tuş: {Durum(TekTusAcik)}   F2 Hareket hissi: {Durum(HareketAcik)}   F3 Çarpışma: {Durum(CarpismaAcik)}", kucuk);

        string uyari = faz.Uyari;
        if (uyari != "") GUI.Label(new Rect(0, Screen.height * 0.3f, Screen.width, 40), uyari, orta);
        if (Time.time < mesajBitisi) GUI.Label(new Rect(0, Screen.height * 0.3f + 40, Screen.width, 40), mesaj, orta);

        if (OyunBitti)
        {
            GUIStyle buyuk = new GUIStyle(GUI.skin.label) { fontSize = 38, alignment = TextAnchor.MiddleCenter };
            string son = kazandi
                ? $"Prizma geçildi!  {sure:0.0} sn, {olumSayisi} ölüm"
                : $"Canın bitti.  {Mesafe:0} m ilerledin";
            GUI.Label(new Rect(0, Screen.height / 2f - 30, Screen.width, 50), son + "   (R: tekrar)", buyuk);
        }
    }

    // Anahtar durumunu yazıya çevirir.
    static string Durum(bool acik) => acik ? "AÇIK" : "KAPALI";
}
