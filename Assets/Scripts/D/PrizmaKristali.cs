using UnityEngine;

// Özel kristal türü: renksizdir, her iki fazda da katıdır.
// Kırılınca oyuncunun fazını BEDAVA ama ZORLA değiştirir ve tuşu bir süre kilitler (Çarpışma → Tek tuş).
// Enerji tasarrufu sağlar ama oyuncunun kontrolünü bir süreliğine elinden alır.
public class PrizmaKristali : Kristal
{
    [SerializeField] float kilitSuresi = 2f; // Kırıldıktan sonra faz tuşunun kilitli kaldığı süre (sn)

    // Kırılma anında fazı zorla değiştirir ve kilidi başlatır.
    protected override void Kirilinca(FazKontrol faz)
    {
        faz.ZorlaDegistir(kilitSuresi);
    }
}
