using UnityEngine;

// Sağdan sola kayan renkli kapı. Karakter kapıyla aynı renkteyse geçer, değilse kaybeder.
// Kapının çarpıştırıcısı tetikleyicidir (isTrigger): fiziksel olarak itmez, sadece "değdi" bilgisini verir.
[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(BoxCollider2D))]
public class RenkKapisi : MonoBehaviour
{
    [SerializeField] float yokOlmaX = -12f; // Kapı bu x değerinin soluna geçince silinir

    public KapiRengi Rengi { get; private set; } // Kapının rengi

    Rigidbody2D rb;       // Kapıyı fizik adımında hareket ettirmek için
    SpriteRenderer sr;    // Kapının rengini göstermek için
    BoxCollider2D kutu;   // Tetikleyici alan

    // Bileşenleri bir kez bulur.
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        kutu = GetComponent<BoxCollider2D>();
    }

    // Üretici kapıyı oluşturunca rengini verir.
    public void Kur(KapiRengi renk)
    {
        Rengi = renk;
        sr.color = KapiRenkleri.Renk(renk);
    }

    // Fizik adımı: kapıyı yöneticinin verdiği hızla sola taşır.
    // Rigidbody2D'ye dokunan hareket FixedUpdate'te yapılır.
    void FixedUpdate()
    {
        RenkKapisiOyun oyun = RenkKapisiOyun.Ornek;
        if (oyun == null || oyun.OyunBitti) return;

        rb.MovePosition(rb.position + Vector2.left * oyun.KapiHizi * Time.fixedDeltaTime);
        if (rb.position.x < yokOlmaX) Destroy(gameObject);
    }

    // Kapı geçildiğinde soluklaşır ve bir daha tetiklenmez.
    public void Gecildi()
    {
        kutu.enabled = false;
        Color c = sr.color;
        c.a = 0.25f;
        sr.color = c;
    }
}
