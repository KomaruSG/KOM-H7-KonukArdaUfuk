using UnityEngine;

// Bölümün sonu: oyuncu girince oyun kazanılır.
[RequireComponent(typeof(Collider2D))]
public class PrizmaVaris : MonoBehaviour
{
    // Oyuncu girince yöneticiye bitişi bildirir.
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.GetComponent<Gezgin>() != null && PrizmaOyun.Ornek != null)
            PrizmaOyun.Ornek.Bitir();
    }
}
