using UnityEngine;

// Renkli kristal: yıkılabilir nesne. Sadece oyuncu kristalin rengindeyken katıdır, yani ancak o zaman kırılabilir.
// Kırılma koşulu: oyuncu ÜSTTEN, yeterince hızlı düşerek basmalı. Mavi faz daha sert düştüğü için daha kolay kırar.
// Tasarlanmış sonuç: kristal kırılınca oyuncu yukarı sıçratılır ve enerji kazanır (Çarpışma → Tek tuş).
[RequireComponent(typeof(Collider2D))]
public class Kristal : MonoBehaviour
{
    [SerializeField] protected float kirilmaHizi = 9f;    // Kırmak için gereken en düşük dikey çarpma hızı (önce 11; yerçekimi düşünce düşüş yavaşladı)
    [SerializeField] protected float enerjiKazanci = 3f;  // Kırılınca kazanılan enerji
    [SerializeField] protected float sicramaHizi = 13f;   // Kırılınca oyuncunun yukarı fırlatıldığı hız
    [SerializeField] float ustTolerans = 0.4f;            // Oyuncunun "üstte" sayılması için kristal merkezinden en az ne kadar yukarıda olması gerektiği

    bool kirildi; // İki kez kırılmasın diye

    // Çarpışma geldiğinde: oyuncu mu, üstten mi, yeterince hızlı mı? Hepsi evetse kır.
    void OnCollisionEnter2D(Collision2D carpisma)
    {
        if (kirildi || !PrizmaOyun.CarpismaAcik) return; // Bağlantı testi: çarpışma tasarımı kapalıysa sıradan blok gibi davranır
        Rigidbody2D govde = carpisma.rigidbody;
        if (govde == null) return;
        FazKontrol faz = govde.GetComponent<FazKontrol>();
        if (faz == null) return;

        bool ustten = govde.position.y > transform.position.y + ustTolerans;
        if (!ustten || Mathf.Abs(carpisma.relativeVelocity.y) < kirilmaHizi) return;

        kirildi = true;
        GetComponent<Collider2D>().enabled = false; // Aynı karedeki diğer fizik adımlarında zemin sayılmasın
        govde.linearVelocity = new Vector2(govde.linearVelocity.x, sicramaHizi);
        faz.EnerjiEkle(enerjiKazanci);
        Kirilinca(faz);
        Destroy(gameObject);
    }

    // Kırılma anında ek bir şey olacaksa alt sınıflar bunu değiştirir (renkli kristal için boş).
    protected virtual void Kirilinca(FazKontrol faz) { }
}
