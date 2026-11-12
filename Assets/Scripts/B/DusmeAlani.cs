using UnityEngine;

// Parkurun altındaki görünmez tetikleyici alan: karakter düşüp buraya girerse son kontrol noktasına döner.
// Tetikleyici olduğu için karakteri itmez, sadece "girdi" bilgisini verir.
[RequireComponent(typeof(Collider2D))]
public class DusmeAlani : MonoBehaviour
{
    // Alana giren Gezgin'i geri doğurur ve düşmeyi sayar.
    void OnTriggerEnter2D(Collider2D diger)
    {
        Gezgin gezgin = diger.GetComponent<Gezgin>();
        if (gezgin == null) return;
        gezgin.YenidenDog();
        if (ParkurSayaci.Ornek != null) ParkurSayaci.Ornek.Dustu();
    }
}
