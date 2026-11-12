using UnityEngine;
using UnityEngine.InputSystem;

// Platform karakteri: ivme ve frenle yatay hareket, değişken yükseklikte zıplama, coyote time ve zıplama tamponu.
// Bütün sayılar HareketAyari'ndan gelir; profil değişince his de değişir.
// Kural: girdi Update'te okunur, Rigidbody2D'ye dokunan her şey FixedUpdate'te yapılır.
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class Gezgin : MonoBehaviour
{
    [SerializeField] HareketAyari ayar;                        // Şu an kullanılan hareket profili
    [SerializeField] bool boslukZiplatir = true;               // Boşluk tuşu zıplama sayılsın mı (D'de boşluk başka işe ayrılabilir)
    [SerializeField, Range(0f, 1f)] float yerNormaliEsigi = 0.7f; // Temas yüzeyinin "zemin" sayılması için normalin en az ne kadar yukarı bakması gerektiği

    public HareketAyari Ayar => ayar;              // Diğer betiklerin profili okuması için
    public bool Yerde { get; private set; }        // Bu fizik adımında yere basıyor mu
    public Vector2 DogmaNoktasi { get; set; }      // Düşünce geri döneceği nokta (kontrol noktaları değiştirir)
    public Vector2 Hiz => rb.linearVelocity;       // Anlık hız (arayüz için)

    Rigidbody2D rb;                                          // Fizik gövdesi
    SpriteRenderer sr;                                       // Profil rengini göstermek için
    readonly ContactPoint2D[] temaslar = new ContactPoint2D[8]; // Zemin kontrolü için temas listesi (her adımda yeni dizi açmamak için)
    ContactFilter2D temasFiltresi;                           // Tetikleyicileri zemin saymamak için filtre

    float girdiX;                 // Yatay girdi: -1 sol, 0 yok, 1 sağ
    bool ziplamaBasili;           // Zıplama tuşu şu an basılı mı
    bool ziplamaKesildi;          // Tuş havadayken bırakıldı mı (kısa zıplama için)
    bool ziplaniyor;              // Zıplama başladı ve henüz yere inilmedi
    float sonYerdeZamani = -99f;  // En son yere basılan an (coyote için)
    float tamponZamani = -99f;    // En son zıplama tuşuna basılan an (tampon için)

    // Bileşenleri bulur, başlangıç noktasını kaydeder, profili uygular.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        DogmaNoktasi = transform.position;
        temasFiltresi = new ContactFilter2D { useTriggers = false };
        AyarUygula(ayar);
    }

    // Profili değiştirir ve karakterin rengini yeni profile göre boyar.
    public void AyarUygula(HareketAyari yeni)
    {
        ayar = yeni;
        if (ayar != null && sr != null) sr.color = ayar.renk;
    }

    // Karakteri doğma noktasına ışınlar ve hızını sıfırlar (düşünce çağrılır).
    public void YenidenDog()
    {
        rb.position = DogmaNoktasi;
        rb.linearVelocity = Vector2.zero;
        tamponZamani = -99f;
        ziplaniyor = false;
    }

    // Girdi okuma: yön tuşları ve zıplama tuşu. Basış anı tampona yazılır, fizik adımı bunu kullanır.
    void Update()
    {
        Keyboard k = Keyboard.current;
        if (k == null) return;

        float x = 0f;
        if (k.aKey.isPressed || k.leftArrowKey.isPressed) x -= 1f;
        if (k.dKey.isPressed || k.rightArrowKey.isPressed) x += 1f;
        girdiX = x;

        bool basildi = k.wKey.wasPressedThisFrame || k.upArrowKey.wasPressedThisFrame
                       || (boslukZiplatir && k.spaceKey.wasPressedThisFrame);
        bool basili = k.wKey.isPressed || k.upArrowKey.isPressed || (boslukZiplatir && k.spaceKey.isPressed);

        if (basildi) tamponZamani = Time.time;              // Tampon: basışı hatırla
        if (ziplamaBasili && !basili) ziplamaKesildi = true; // Tuş bırakıldı: zıplamayı kısalt
        ziplamaBasili = basili;
    }

    // Fizik adımı: zemin kontrolü, yatay ivme/fren, zıplama, değişken yükseklik, yerçekimi.
    void FixedUpdate()
    {
        if (ayar == null) return;

        Yerde = YerdeMi();
        if (Yerde && rb.linearVelocityY <= 0.01f)
        {
            sonYerdeZamani = Time.time;
            ziplaniyor = false;
        }

        Vector2 v = rb.linearVelocity;

        // Yatay: hedef hıza ivmeyle yaklaş; tuş yoksa ya da ters yöne basılıyorsa frenle yavaşla.
        float hedefHiz = girdiX * ayar.azamiHiz;
        bool tersYon = girdiX != 0f && Mathf.Abs(v.x) > 0.01f && Mathf.Sign(girdiX) != Mathf.Sign(v.x);
        float oran = (girdiX == 0f || tersYon) ? ayar.fren : ayar.ivme;
        if (!Yerde) oran *= ayar.havaKontrolu;
        v.x = Mathf.MoveTowards(v.x, hedefHiz, oran * Time.fixedDeltaTime);

        // Zıplama: tampon süresi içinde basılmış VE coyote süresi içinde yerde olunmuşsa zıpla.
        bool tampondaMi = Time.time - tamponZamani <= ayar.tamponSuresi;
        bool coyotedeMi = Time.time - sonYerdeZamani <= ayar.coyoteSuresi;
        if (tampondaMi && coyotedeMi && !ziplaniyor)
        {
            v.y = ZiplamaHizi();
            tamponZamani = -99f;
            sonYerdeZamani = -99f;      // Aynı coyote penceresinde ikinci zıplama olmasın
            ziplaniyor = true;
            ziplamaKesildi = !ziplamaBasili; // Tampondaki kısa dokunuş: hemen kısa zıplama
        }

        // Değişken yükseklik: tuş erken bırakıldıysa yukarı hız bir kez kesilir.
        if (ziplamaKesildi)
        {
            if (v.y > 0f) v.y *= ayar.kisaZiplamaCarpani;
            ziplamaKesildi = false;
        }

        // Düşerken yerçekimi artar: zıplama havada asılı kalmaz.
        rb.gravityScale = ayar.yercekimiOlcegi * (v.y < 0f ? ayar.dususCarpani : 1f);
        v.y = Mathf.Max(v.y, -ayar.azamiDususHizi);

        rb.linearVelocity = v;
    }

    // İstenen yüksekliğe çıkmak için gereken ilk hız: v = √(2·g·h).
    float ZiplamaHizi()
    {
        float g = Mathf.Abs(Physics2D.gravity.y) * ayar.yercekimiOlcegi;
        return Mathf.Sqrt(2f * g * ayar.ziplamaYuksekligi);
    }

    // Temaslardan biri yukarı bakan bir yüzeyse karakter yerdedir.
    bool YerdeMi()
    {
        int sayi = rb.GetContacts(temasFiltresi, temaslar);
        for (int i = 0; i < sayi; i++)
            if (temaslar[i].normal.y >= yerNormaliEsigi) return true;
        return false;
    }
}
