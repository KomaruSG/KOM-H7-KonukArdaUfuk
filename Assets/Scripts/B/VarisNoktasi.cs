using UnityEngine;

// Parkurun sonundaki tetikleyici: karakter girince koşu biter.
[RequireComponent(typeof(Collider2D))]
public class VarisNoktasi : MonoBehaviour
{
    // Gezgin girince yöneticiye bitişi bildirir.
    void OnTriggerEnter2D(Collider2D diger)
    {
        if (diger.GetComponent<Gezgin>() != null && ParkurSayaci.Ornek != null)
            ParkurSayaci.Ornek.Bitir();
    }
}
