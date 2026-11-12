using UnityEngine;

// Yıkılabilir cam blok. Top yeterince hızlı çarparsa kırılır; yavaş çarparsa kırılmaz, top sadece seker.
// Böylece çarpışmanın sonucu topun hızına bağlıdır.
[RequireComponent(typeof(Collider2D))]
public class CamBlok : MonoBehaviour
{
    [SerializeField] protected float kirilmaHizi = 3f; // Kırılması için gereken en düşük çarpma hızı (önce 5: camlar zor kırılıyordu)

    bool kirildi; // İki kez kırılmasın diye

    // Top çarptığında çarpma hızına bakar; eşiği geçtiyse bloğu kırar.
    void OnCollisionEnter2D(Collision2D carpisma)
    {
        if (kirildi) return;
        MasaTopu top = carpisma.rigidbody != null ? carpisma.rigidbody.GetComponent<MasaTopu>() : null;
        if (top == null) return;
        if (carpisma.relativeVelocity.magnitude < kirilmaHizi) return;

        kirildi = true;
        Kirilinca(top);
        if (MasaOyun.Ornek != null) MasaOyun.Ornek.CamKirildi();
        Destroy(gameObject);
    }

    // Kırılma anında ek bir şey olacaksa alt sınıflar bunu değiştirir (normal cam için boş).
    protected virtual void Kirilinca(MasaTopu top) { }
}
