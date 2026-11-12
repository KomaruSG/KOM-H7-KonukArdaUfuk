using UnityEditor;
using UnityEngine;

// Oyun A (Renk Kapısı) sahnesini kodla kurar: kamera, karakter, kalkan halkası, kapı prefabı, üretici, yönetici.
// Menü: KOM > A Sahnesini Kur. Sadece sahne ilk kez kurulurken kullanılır; sonra ayarlar Inspector'dan yapılır.
public static class SahneKurucuA
{
    const string SahneAdi = "A_TekTus"; // Kaydedilecek sahnenin adı

    // Menüden çağrılır: sahneyi kurar (varsa önce sorar).
    [MenuItem("KOM/A Sahnesini Kur")]
    public static void Kur()
    {
        if (!KurucuOrtak.UzerineYazilsinMi(SahneAdi)) return;

        Sprite kare = KurucuOrtak.Kare();
        Sprite daire = KurucuOrtak.Daire();
        KurucuOrtak.YeniSahne(new Color(0.11f, 0.12f, 0.15f), 5f);

        // Zemin çizgisi: sadece görsel, karakterin nerede durduğunu belli eder.
        KurucuOrtak.SpriteNesnesi("Zemin", kare, new Color(1f, 1f, 1f, 0.08f), new Vector3(0f, -0.75f, 0f), new Vector3(30f, 0.05f, 1f), -5);

        // Kapı prefabı: ince uzun dikey çubuk, tetikleyici, kinematik gövde.
        GameObject kapi = KurucuOrtak.SpriteNesnesi("RenkKapisi", kare, Color.white, Vector3.zero, new Vector3(0.5f, 12f, 1f));
        Rigidbody2D kapiGovde = kapi.AddComponent<Rigidbody2D>();
        kapiGovde.bodyType = RigidbodyType2D.Kinematic;
        kapiGovde.interpolation = RigidbodyInterpolation2D.Interpolate;
        BoxCollider2D kapiKutu = kapi.AddComponent<BoxCollider2D>();
        kapiKutu.isTrigger = true;
        kapi.AddComponent<RenkKapisi>();
        GameObject kapiPrefabi = KurucuOrtak.PrefabKaydet(kapi, "A_RenkKapisi");

        // Karakter: sabit kare. Dinamik gövde ama tüm hareketi kilitli; sadece tetikleyici olaylarını almak için.
        GameObject karakter = KurucuOrtak.SpriteNesnesi("Karakter", kare, KapiRenkleri.Kirmizi, new Vector3(-5f, 0f, 0f), Vector3.one, 2);
        Rigidbody2D govde = karakter.AddComponent<Rigidbody2D>();
        govde.gravityScale = 0f;
        govde.constraints = RigidbodyConstraints2D.FreezeAll;
        karakter.AddComponent<BoxCollider2D>();
        RenkKarakter renkKarakter = karakter.AddComponent<RenkKarakter>();

        // Kalkan halkası: karakterin çocuğu, açıkken görünür.
        GameObject kalkan = KurucuOrtak.SpriteNesnesi("Kalkan", daire, new Color(1f, 1f, 1f, 0.3f), karakter.transform.position, Vector3.one * 1.8f, 1);
        kalkan.transform.SetParent(karakter.transform, true);
        KurucuOrtak.AlanAta(renkKarakter, "kalkanGorseli", kalkan);

        // Yönetici ve üretici.
        GameObject yonetici = new GameObject("Oyun");
        RenkKapisiOyun oyun = yonetici.AddComponent<RenkKapisiOyun>();
        KurucuOrtak.AlanAta(oyun, "karakter", renkKarakter);
        KapiUretici uretici = yonetici.AddComponent<KapiUretici>();
        KurucuOrtak.AlanAta(uretici, "kapiPrefabi", kapiPrefabi.GetComponent<RenkKapisi>());

        KurucuOrtak.SahneyiKaydet(SahneAdi);
        Debug.Log("A_TekTus sahnesi kuruldu.");
    }

    // Unity'yi pencere açmadan (batchmode) çalıştırırken çağrılan giriş noktası.
    public static void TopluKur()
    {
        Kur();
    }
}
