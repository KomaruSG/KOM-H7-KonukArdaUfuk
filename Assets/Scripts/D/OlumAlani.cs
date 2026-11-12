using UnityEngine;

// Tetikleyici ölüm alanı: dikenler ve çukurun dibi. Oyuncu girince bir can kaybeder.
[RequireComponent(typeof(Collider2D))]
public class OlumAlani : MonoBehaviour
{
    [SerializeField] string sebep = "Düştün!"; // Ekranda gösterilecek ölüm sebebi

    // Oyuncu girince yöneticiye ölümü bildirir.
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.GetComponent<Gezgin>() != null && PrizmaOyun.Ornek != null)
            PrizmaOyun.Ornek.Ol(sebep);
    }
}
