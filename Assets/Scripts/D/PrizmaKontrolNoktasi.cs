using UnityEngine;

// Kontrol noktası: oyuncu geçince doğma yeri, o anki enerji ve faz kaydedilir.
[RequireComponent(typeof(Collider2D))]
public class PrizmaKontrolNoktasi : MonoBehaviour
{
    [SerializeField] Color alinmisRenk = new Color(0.4f, 1f, 0.5f); // Alındıktan sonraki renk

    bool alindi; // Daha önce alındı mı

    // Oyuncu geçince yöneticiye kaydettirir ve rengini değiştirir.
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (alindi || diger.GetComponent<Gezgin>() == null || PrizmaOyun.Ornek == null) return;
        alindi = true;
        PrizmaOyun.Ornek.KontrolNoktasiAlindi(transform.position);
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = alinmisRenk;
    }
}
