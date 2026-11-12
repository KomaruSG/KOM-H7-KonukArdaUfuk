using UnityEngine;
using UnityEngine.InputSystem;

// Prizma'nın tek tuşu: Boşluk ile faz değiştirme.
// Her değişim enerji harcar; maliyet ilerledikçe artar (zorluk sayısı). Faz değişince:
//   - bütün renkli nesneler katı/geçirgen olur (Tek tuş → Çarpışma),
//   - karakterin hareket profili değişir (Tek tuş → Hareket).
// Öteki renkteki bir bloğun içindeyken faz değiştirilirse o blok katılaşır ve oyuncu ezilir.
public class FazKontrol : MonoBehaviour
{
    [Header("Bağlantılar")]
    [SerializeField] Gezgin gezgin;              // Hareketi yöneten karakter betiği (B'den)
    [SerializeField] HareketAyari kirmiziAyar;   // Kırmızı fazdaki hareket profili
    [SerializeField] HareketAyari maviAyar;      // Mavi fazdaki hareket profili
    [SerializeField] HareketAyari notrAyar;      // Bağlantı testi: hareket hissi kapatılınca kullanılan düz profil

    [Header("Faz")]
    [SerializeField] FazRengi baslangicFazi = FazRengi.Kirmizi; // Oyun başındaki faz
    [SerializeField, Range(0.5f, 1f)] float ezilmeKutusuOrani = 0.8f; // Ezilme kontrolünde karakter kutusunun ne kadarı sayılır (kenara sürtünme affedilir)

    [Header("Enerji")]
    [SerializeField] float azamiEnerji = 8f;          // Enerjinin üst sınırı
    [SerializeField] float baslangicEnerjisi = 5f;    // Oyun başındaki enerji
    [SerializeField] float baslangicMaliyeti = 1f;    // İlk faz değişiminin maliyeti
    [SerializeField] float maliyetArtisi = 0.005f;    // Her metre ilerleyişte maliyete eklenen miktar (zorluk sayısı)
    [SerializeField] float azamiMaliyet = 2f;         // Maliyetin üst sınırı

    public FazRengi Faz { get; private set; }  // Şu anki faz
    public float Enerji { get; private set; }  // Şu anki enerji
    public float AzamiEnerji => azamiEnerji;   // Arayüz için
    public float KilitKalan => Mathf.Max(0f, kilitBitisi - Time.time); // Faz kilidinin bitmesine kalan süre
    public string Uyari => Time.time < uyariBitisi ? uyari : "";       // Ekranda kısa süre gösterilen uyarı

    // Şu anki faz değiştirme maliyeti: ilerledikçe artar.
    public float Maliyet
    {
        get
        {
            float mesafe = PrizmaOyun.Ornek != null ? PrizmaOyun.Ornek.Mesafe : 0f;
            return Mathf.Min(azamiMaliyet, baslangicMaliyeti + maliyetArtisi * mesafe);
        }
    }

    Collider2D govdeKutusu;   // Karakterin çarpıştırıcısı (ezilme kontrolü için)
    SpriteRenderer govdeGorseli; // Karakterin rengini faz rengine boyamak için
    float kilitBitisi;        // Faz kilidinin biteceği an
    string uyari = "";        // Son uyarı metni
    float uyariBitisi;        // Uyarının ekrandan kalkacağı an

    // Bileşenleri bulur ve enerjiyi başlatır.
    void Awake()
    {
        govdeKutusu = gezgin.GetComponent<Collider2D>();
        govdeGorseli = gezgin.GetComponent<SpriteRenderer>();
        Enerji = baslangicEnerjisi;
    }

    // Bütün faz nesneleri listeye girdikten sonra başlangıç fazını uygular.
    void Start()
    {
        FazUygula(baslangicFazi);
    }

    // Girdi Update'te okunur: Boşluk'a basılınca faz değiştirmeyi dener.
    void Update()
    {
        if (PrizmaOyun.Ornek != null && PrizmaOyun.Ornek.OyunBitti) return;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) Dene();
    }

    // Oyuncunun isteğiyle faz değiştirme: kilit ve enerji kontrolünden geçerse maliyeti öder ve değiştirir.
    void Dene()
    {
        if (!PrizmaOyun.TekTusAcik) return;
        if (KilitKalan > 0f) { UyariVer("Faz kilitli!"); return; }
        if (Enerji < Maliyet) { UyariVer("Enerji yetmiyor!"); return; }

        Enerji -= Maliyet;
        Degistir(FazRenkleri.Ters(Faz));
    }

    // Prizma kristali çağırır: fazı bedavaya ama zorla değiştirir ve tuşu bir süre kilitler.
    public void ZorlaDegistir(float kilitSuresi)
    {
        Degistir(FazRenkleri.Ters(Faz));
        kilitBitisi = Time.time + kilitSuresi;
    }

    // Fazı değiştirir; karakter yeni fazda katılaşacak bir nesnenin içindeyse ezilir.
    void Degistir(FazRengi yeni)
    {
        bool ezildi = YeniFazdaIcindeMi(yeni);
        FazUygula(yeni);
        if (ezildi && PrizmaOyun.Ornek != null) PrizmaOyun.Ornek.Ol("Ezildin! Bloğun içindeyken faz değiştirdin.");
    }

    // Karakter, verilen fazda katı olacak bir faz nesnesiyle iç içe mi?
    bool YeniFazdaIcindeMi(FazRengi yeni)
    {
        Bounds b = govdeKutusu.bounds;
        Collider2D[] degenler = Physics2D.OverlapBoxAll(b.center, b.size * ezilmeKutusuOrani, 0f);
        foreach (Collider2D c in degenler)
        {
            FazNesnesi n = c.GetComponent<FazNesnesi>();
            if (n != null && n.Renk == yeni) return true;
        }
        return false;
    }

    // Fazı uygular: bütün renkli nesneleri ayarlar, hareket profilini ve karakter rengini günceller.
    public void FazUygula(FazRengi faz)
    {
        Faz = faz;
        foreach (FazNesnesi n in FazNesnesi.Hepsi) n.FazaGoreAyarla(faz);
        HareketiGuncelle();
    }

    // Faza (ya da bağlantı testi açıksa düz profile) göre hareket profilini seçer.
    public void HareketiGuncelle()
    {
        HareketAyari ayar = !PrizmaOyun.HareketAcik ? notrAyar : (Faz == FazRengi.Kirmizi ? kirmiziAyar : maviAyar);
        gezgin.AyarUygula(ayar);
        govdeGorseli.color = FazRenkleri.Renk(Faz); // Karakter her zaman fazının renginde görünür
    }

    // Kristal kırılınca enerji ekler (Çarpışma → Tek tuş).
    public void EnerjiEkle(float miktar) => Enerji = Mathf.Min(azamiEnerji, Enerji + miktar);

    // Kontrol noktasından doğarken enerjiyi ayarlar.
    public void EnerjiAyarla(float deger) => Enerji = Mathf.Clamp(deger, 0f, azamiEnerji);

    // Ekranda kısa bir uyarı gösterir.
    void UyariVer(string metin)
    {
        uyari = metin;
        uyariBitisi = Time.time + 1.2f;
    }
}
