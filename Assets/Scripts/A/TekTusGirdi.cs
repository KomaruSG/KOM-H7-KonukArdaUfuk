using UnityEngine.InputSystem;

// Oyun A'nın tek girdisi: Boşluk tuşu ya da fare sol tıklaması aynı tuş sayılır.
// Girdi her zaman Update içinde okunur; FixedUpdate'te okunursa kısa basışlar kaybolabilir.
public static class TekTusGirdi
{
    // Bu karede tuşa yeni basıldıysa true döndürür.
    public static bool BasildiMi()
    {
        bool bosluk = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        bool tiklama = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        return bosluk || tiklama;
    }
}
