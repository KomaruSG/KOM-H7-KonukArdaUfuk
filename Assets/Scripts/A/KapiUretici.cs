using UnityEngine;

// Ekranın sağında belli aralıklarla rastgele renkte kapı üretir.
// Kapılar arası mesafe zamanla kısalır: ikinci zorluk sayısı.
public class KapiUretici : MonoBehaviour
{
    [SerializeField] RenkKapisi kapiPrefabi;        // Üretilecek kapı (prefab)
    [SerializeField] float dogmaX = 11f;            // Kapının doğduğu x konumu (ekranın sağ dışı)
    [SerializeField] float ilkBekleme = 1.5f;       // Oyun başında ilk kapıdan önceki bekleme (sn)
    [SerializeField] float baslangicAraligi = 7f;   // Oyun başında iki kapı arası mesafe (birim)
    [SerializeField] float asgariAralik = 3.5f;     // Mesafenin inebileceği en kısa değer
    [SerializeField] float aralikAzalmasi = 0.05f;  // Mesafenin her saniye kısalma miktarı
    [SerializeField, Range(0f, 1f)] float renkDegismeOlasiligi = 0.55f; // Yeni kapının bir öncekinden farklı renkte olma olasılığı

    float sonrakiDogmaZamani;  // Bir sonraki kapının doğacağı an
    KapiRengi oncekiRenk;      // Bir önceki kapının rengi

    // İlk kapı zamanını ayarlar.
    void Start()
    {
        sonrakiDogmaZamani = Time.time + ilkBekleme;
        oncekiRenk = KapiRengi.Kirmizi;
    }

    // Zamanı gelince kapı üretir ve bir sonrakinin zamanını mesafe / hız ile hesaplar.
    void Update()
    {
        RenkKapisiOyun oyun = RenkKapisiOyun.Ornek;
        if (oyun == null || oyun.OyunBitti) return;
        if (Time.time < sonrakiDogmaZamani) return;

        Uret();
        float aralik = Mathf.Max(asgariAralik, baslangicAraligi - aralikAzalmasi * oyun.GecenSure);
        sonrakiDogmaZamani = Time.time + aralik / oyun.KapiHizi;
    }

    // Prefabdan bir kapı oluşturur ve rengini seçer.
    void Uret()
    {
        KapiRengi renk = Random.value < renkDegismeOlasiligi ? KapiRenkleri.Ters(oncekiRenk) : oncekiRenk;
        RenkKapisi kapi = Instantiate(kapiPrefabi, new Vector3(dogmaX, 0f, 0f), Quaternion.identity);
        kapi.Kur(renk);
        oncekiRenk = renk;
    }
}
