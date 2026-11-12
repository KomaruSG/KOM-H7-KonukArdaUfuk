using UnityEngine;

// Oyun A'nın karakteri. Yerinden kıpırdamaz; tek yaptığı renk değiştirmektir.
// İki zamanlama biçimi:
//   1) Tek bas  → renk değiştir.
//   2) Çift bas → (iki değişim birbirini götürür, renk aynı kalır) kalkan açılır.
// Kalkan, yanlış renkteki bir kapıyı bir kez affeder; sonra bekleme süresine girer.
[RequireComponent(typeof(SpriteRenderer))]
public class RenkKarakter : MonoBehaviour
{
    [SerializeField] KapiRengi baslangicRengi = KapiRengi.Kirmizi; // Oyun başındaki renk
    [SerializeField] float ciftBasmaPenceresi = 0.25f; // İki basış arası bu süreden kısaysa çift basış sayılır (sn)
    [SerializeField] float kalkanSuresi = 2f;          // Kalkanın açık kalabileceği en uzun süre (sn)
    [SerializeField] float kalkanBeklemeSuresi = 5f;   // Kalkan açıldıktan sonra yeniden açılabilmesi için geçmesi gereken süre (sn)
    [SerializeField] GameObject kalkanGorseli;         // Kalkan açıkken görünen halka

    public KapiRengi Rengi { get; private set; } // Karakterin şu anki rengi

    SpriteRenderer sr;            // Rengi göstermek için
    float sonBasisZamani = -99f;  // Bir önceki basışın zamanı (çift basışı yakalamak için)
    float kalkanBitisZamani;      // Açık kalkanın kapanacağı an
    float kalkanHazirZamani;      // Kalkanın yeniden kullanılabileceği an
    bool kalkanAcik;              // Kalkan şu an açık mı

    // Bileşeni bulur, başlangıç rengini uygular, kalkanı gizler.
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        RengiAyarla(baslangicRengi);
        if (kalkanGorseli != null) kalkanGorseli.SetActive(false);
    }

    // Girdi Update'te okunur. Her basış rengi değiştirir; pencere içindeki ikinci basış kalkanı açar.
    void Update()
    {
        RenkKapisiOyun oyun = RenkKapisiOyun.Ornek;
        if (oyun != null && oyun.OyunBitti) return;

        if (TekTusGirdi.BasildiMi())
        {
            RengiAyarla(KapiRenkleri.Ters(Rengi));

            bool ciftBasis = Time.time - sonBasisZamani <= ciftBasmaPenceresi;
            if (ciftBasis && Time.time >= kalkanHazirZamani)
            {
                KalkaniAc();
                sonBasisZamani = -99f; // Üçüncü basış yeni bir çift basış başlatmasın
            }
            else
            {
                sonBasisZamani = Time.time;
            }
        }

        if (kalkanAcik && Time.time >= kalkanBitisZamani) KalkaniKapat();
    }

    // Karakter bir kapının tetikleyicisine girdiğinde karar verilir: geç, kalkanla geç ya da kaybet.
    void OnTriggerEnter2D(Collider2D diger)
    {
        RenkKapisi kapi = diger.GetComponent<RenkKapisi>();
        RenkKapisiOyun oyun = RenkKapisiOyun.Ornek;
        if (kapi == null || oyun == null || oyun.OyunBitti) return;

        if (kapi.Rengi == Rengi)
        {
            kapi.Gecildi();
            oyun.KapiGecildi();
        }
        else if (kalkanAcik)
        {
            KalkaniKapat();
            kapi.Gecildi();
            oyun.KapiGecildi();
        }
        else
        {
            sr.color = Color.white; // Kaybetme anını beyaz parlamayla göster
            oyun.Kaybet();
        }
    }

    // Rengi değiştirir ve ekrana uygular.
    void RengiAyarla(KapiRengi renk)
    {
        Rengi = renk;
        sr.color = KapiRenkleri.Renk(renk);
    }

    // Kalkanı açar ve bekleme süresini başlatır.
    void KalkaniAc()
    {
        kalkanAcik = true;
        kalkanBitisZamani = Time.time + kalkanSuresi;
        kalkanHazirZamani = Time.time + kalkanBeklemeSuresi;
        if (kalkanGorseli != null) kalkanGorseli.SetActive(true);
    }

    // Kalkanı kapatır.
    void KalkaniKapat()
    {
        kalkanAcik = false;
        if (kalkanGorseli != null) kalkanGorseli.SetActive(false);
    }

    // Arayüzde gösterilmek için kalkanın hazır olmasına kalan süre (0 ise hazır).
    public float KalkanaKalanSure => Mathf.Max(0f, kalkanHazirZamani - Time.time);
}
