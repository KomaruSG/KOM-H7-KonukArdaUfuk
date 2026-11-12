using System.Collections.Generic;
using UnityEngine;

// Renkli blok ya da kristal. Oyuncu kendi rengindeyse ÇARPIŞTIRICI (katı, üstüne basılır, duvar olur),
// öteki renkteyse TETİKLEYİCİ (içinden geçilir) olur. Bağ 1'in kalbi: tek tuş, çarpışmanın türünü değiştirir.
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class FazNesnesi : MonoBehaviour
{
    public static readonly List<FazNesnesi> Hepsi = new List<FazNesnesi>(); // Sahnedeki bütün faz nesneleri

    [SerializeField] FazRengi renk;                       // Bu nesnenin rengi
    [SerializeField, Range(0f, 1f)] float hayaletSaydamligi = 0.2f; // Geçirgenken ne kadar görünür olduğu

    public FazRengi Renk => renk; // Diğer betiklerin rengi okuması için

    Collider2D kutu;     // Katı/geçirgen yapılacak çarpıştırıcı
    SpriteRenderer sr;   // Saydamlığı değişecek görsel

    // Bileşenleri bulur.
    void Awake()
    {
        kutu = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Etkinleşince listeye girer.
    void OnEnable() => Hepsi.Add(this);

    // Kapanınca (ya da yok olunca) listeden çıkar.
    void OnDisable() => Hepsi.Remove(this);

    // Oyuncunun fazına göre katı ya da geçirgen olur ve görünüşünü buna göre ayarlar.
    public void FazaGoreAyarla(FazRengi oyuncuFazi)
    {
        bool kati = oyuncuFazi == renk;
        kutu.isTrigger = !kati;
        Color c = FazRenkleri.Renk(renk);
        c.a = kati ? 1f : hayaletSaydamligi;
        sr.color = c;
    }
}
