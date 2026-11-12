using UnityEngine;

// Masada sekerek ilerleyen top. Duvarlardan sekmesi tamamen fizik motorunun işidir (simüle edilen çarpışma).
// Sürtünme (linearDamping) yüzünden yavaşlar; yeterince yavaşlayınca durmuş sayılır ve masadan kalkar.
[RequireComponent(typeof(Rigidbody2D))]
public class MasaTopu : MonoBehaviour
{
    [SerializeField] float durmaHizi = 1.5f;     // Bu hızın altına düşen top "durdu" sayılır (önce 0.4: son yavaş süzülme çok uzun sürüyordu)
    [SerializeField] float durmaBeklemesi = 0.4f; // Durmuş sayılmak için bu hızın altında kalması gereken süre (sn)

    public Rigidbody2D Govde { get; private set; } // Diğer betiklerin topun hızına erişmesi için

    float yavasSure;  // Topun ne kadar süredir yavaş olduğu
    bool gitti;       // Top masadan çıktı mı (iki kez sayılmasın diye)

    // Gövdeyi bulur ve yöneticiye yeni top olduğunu bildirir.
    void Awake()
    {
        Govde = GetComponent<Rigidbody2D>();
        if (MasaOyun.Ornek != null) MasaOyun.Ornek.TopEklendi();
    }

    // Fizik adımında topun durup durmadığını kontrol eder.
    void FixedUpdate()
    {
        if (Govde.linearVelocity.magnitude < durmaHizi) yavasSure += Time.fixedDeltaTime;
        else yavasSure = 0f;

        if (yavasSure >= durmaBeklemesi) Kaldir();
    }

    // Topu masadan kaldırır ve yöneticiye bildirir (durunca ya da deliğe girince).
    public void Kaldir()
    {
        if (gitti) return;
        gitti = true;
        if (MasaOyun.Ornek != null) MasaOyun.Ornek.TopGitti();
        Destroy(gameObject);
    }
}
