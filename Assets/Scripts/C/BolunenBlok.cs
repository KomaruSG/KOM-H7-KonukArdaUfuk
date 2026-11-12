using UnityEngine;

// Özel blok türü: kırılınca kendisine çarpan topu ikiye böler.
// Gelen topun yönü bir yana, yeni topun yönü öbür yana döndürülür; masada iki top olur.
public class BolunenBlok : CamBlok
{
    [SerializeField] MasaTopu topPrefabi;          // Bölünmede üretilecek yeni top
    [SerializeField] float ayrilmaAcisi = 25f;     // İki topun ilk yönden ne kadar ayrılacağı (derece)

    // Kırılma anında: eski topu bir yana çevirir, öbür yana yeni bir top fırlatır.
    protected override void Kirilinca(MasaTopu top)
    {
        Vector2 hiz = top.Govde.linearVelocity;
        top.Govde.linearVelocity = Quaternion.Euler(0f, 0f, ayrilmaAcisi) * hiz;

        MasaTopu yeni = Instantiate(topPrefabi, top.transform.position, Quaternion.identity);
        yeni.Govde.linearVelocity = Quaternion.Euler(0f, 0f, -ayrilmaAcisi) * hiz;
    }
}
