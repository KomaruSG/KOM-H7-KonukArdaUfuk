using UnityEngine;

// Karakter içinden geçince doğma noktası burası olur; düşerse buradan devam eder.
[RequireComponent(typeof(Collider2D))]
public class KontrolNoktasi : MonoBehaviour
{
    [SerializeField] Color aktifRenk = new Color(0.4f, 1f, 0.5f); // Alındıktan sonraki renk

    bool alindi; // Bu nokta daha önce alındı mı

    // Gezgin geçince doğma noktasını buraya taşır ve rengi değiştirir.
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (alindi) return;
        Gezgin gezgin = diger.GetComponent<Gezgin>();
        if (gezgin == null) return;

        gezgin.DogmaNoktasi = transform.position;
        alindi = true;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = aktifRenk;
    }
}
