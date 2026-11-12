using UnityEngine;

// Yuvarlak tampon: TASARLANMIŞ çarpışma tepkisi.
// Fizik motorunun hesapladığı sekmeyi kullanmaz; topu her zaman tamponun merkezinden dışarı doğru,
// çarpma noktasına göre belirlenen açıyla ve daha hızlı geri fırlatır.
[RequireComponent(typeof(Collider2D))]
public class Tampon : MonoBehaviour
{
    [SerializeField] float hizCarpani = 1.3f;   // Gelen hız bu sayıyla çarpılarak geri verilir (hız artışı)
    [SerializeField] float enAzCikisHizi = 7f;  // Yavaş gelen top bile en az bu hızla çıkar
    [SerializeField] float azamiCikisHizi = 20f; // Çıkış hızının üst sınırı (top kontrolden çıkmasın)
    [SerializeField] float parlamaSuresi = 0.12f; // Çarpınca tamponun büyüyüp parladığı süre (geri bildirim)

    Vector3 ilkOlcek;     // Tamponun normal boyutu
    float parlamaBitisi;  // Parlamanın biteceği an

    // Normal boyutu kaydeder.
    void Awake()
    {
        ilkOlcek = transform.localScale;
    }

    // Top çarptığında: yön = tampon merkezinden topa doğru, hız = gelen hız × çarpan (sınırlar içinde).
    void OnCollisionEnter2D(Collision2D carpisma)
    {
        Rigidbody2D top = carpisma.rigidbody;
        if (top == null) return;

        Vector2 yon = (top.position - (Vector2)transform.position).normalized;
        float hiz = Mathf.Clamp(carpisma.relativeVelocity.magnitude * hizCarpani, enAzCikisHizi, azamiCikisHizi);
        top.linearVelocity = yon * hiz;

        transform.localScale = ilkOlcek * 1.2f;
        parlamaBitisi = Time.time + parlamaSuresi;
    }

    // Parlama süresi bitince tamponu normal boyutuna döndürür.
    void Update()
    {
        if (Time.time >= parlamaBitisi) transform.localScale = ilkOlcek;
    }
}
