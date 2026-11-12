using UnityEngine;

// Hedef delik: TETİKLEYİCİ (isTrigger). Topu sektirmez, top içinden geçer ve "girdi" bilgisi gelir.
// Cam bloklar ise ÇARPIŞTIRICIDIR: topu durdurur/sektirir. İkisinin farkı bu sahnede yan yana görülür.
[RequireComponent(typeof(Collider2D))]
public class HedefDelik : MonoBehaviour
{
    // Top deliğe girince puan verir ve topu masadan kaldırır.
    void OnTriggerEnter2D(Collider2D diger)
    {
        MasaTopu top = diger.GetComponent<MasaTopu>();
        if (top == null) return;
        if (MasaOyun.Ornek != null) MasaOyun.Ornek.DeligeGirdi();
        top.Kaldir();
    }
}
