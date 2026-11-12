using UnityEngine;
using UnityEngine.InputSystem;

// Q tuşuyla Gezgin'in hareket profilini sıradaki profile geçirir (Kızak ↔ Kertenkele).
public class ProfilDegistirici : MonoBehaviour
{
    [SerializeField] Gezgin gezgin;             // Profili değişecek karakter
    [SerializeField] HareketAyari[] profiller;  // Sırayla geçilecek profiller

    int sira; // Şu an seçili profilin listedeki sırası

    // Oyun başında ilk profili uygular.
    void Start()
    {
        if (profiller.Length > 0) gezgin.AyarUygula(profiller[0]);
    }

    // Q'ya basılınca sıradaki profile geçer.
    void Update()
    {
        if (Keyboard.current == null || profiller.Length == 0) return;
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            sira = (sira + 1) % profiller.Length;
            gezgin.AyarUygula(profiller[sira]);
        }
    }
}
