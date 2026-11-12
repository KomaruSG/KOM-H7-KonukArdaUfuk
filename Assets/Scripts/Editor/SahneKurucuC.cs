using System.IO;
using UnityEditor;
using UnityEngine;

// Oyun C (Sekme Masası) sahnesini kodla kurar: duvarlar, top/cam/bölünen/tampon prefabları,
// prefabdan çoğaltılan cam ızgarası, tamponlar, hedef delik, fırlatıcı ve yönetici.
// Menü: KOM > C Sahnesini Kur.
public static class SahneKurucuC
{
    const string SahneAdi = "C_Carpisma";         // Kaydedilecek sahnenin adı
    const string AyarKlasoru = "Assets/Ayarlar";  // Fizik malzemesinin klasörü

    // Menüden çağrılır: sahneyi kurar (varsa önce sorar).
    [MenuItem("KOM/C Sahnesini Kur")]
    public static void Kur()
    {
        if (!KurucuOrtak.UzerineYazilsinMi(SahneAdi)) return;

        Sprite kare = KurucuOrtak.Kare();
        Sprite daire = KurucuOrtak.Daire();
        PhysicsMaterial2D sekmeli = SekmeliMalzeme();
        KurucuOrtak.YeniSahne(new Color(0.08f, 0.2f, 0.16f), 6f); // Yeşil masa örtüsü

        // --- Prefablar ---
        // Top: dinamik, yerçekimsiz, sürtünmeyle yavaşlayan, sekmeli.
        GameObject top = KurucuOrtak.SpriteNesnesi("MasaTopu", daire, Color.white, Vector3.zero, Vector3.one * 0.4f, 5);
        Rigidbody2D topGovde = top.AddComponent<Rigidbody2D>();
        topGovde.gravityScale = 0f;
        topGovde.linearDamping = 0.25f; // 0.35: çok çabuk yavaşlıyordu; 0.12: neredeyse hiç durmuyordu; 0.25 ortası
        topGovde.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        topGovde.interpolation = RigidbodyInterpolation2D.Interpolate;
        top.AddComponent<CircleCollider2D>().sharedMaterial = sekmeli;
        top.AddComponent<MasaTopu>();
        MasaTopu topPrefabi = KurucuOrtak.PrefabKaydet(top, "C_MasaTopu").GetComponent<MasaTopu>();

        // Cam blok: kırılabilir.
        GameObject cam = KurucuOrtak.SpriteNesnesi("CamBlok", kare, new Color(0.6f, 0.9f, 1f, 0.75f), Vector3.zero, new Vector3(1.2f, 0.5f, 1f), 2);
        cam.AddComponent<BoxCollider2D>();
        cam.AddComponent<CamBlok>();
        GameObject camPrefabi = KurucuOrtak.PrefabKaydet(cam, "C_CamBlok");

        // Bölünen blok: özel tür.
        GameObject bolunen = KurucuOrtak.SpriteNesnesi("BolunenBlok", kare, new Color(0.75f, 0.45f, 1f), Vector3.zero, new Vector3(1.2f, 0.5f, 1f), 2);
        bolunen.AddComponent<BoxCollider2D>();
        BolunenBlok bolunenBetik = bolunen.AddComponent<BolunenBlok>();
        KurucuOrtak.AlanAta(bolunenBetik, "topPrefabi", topPrefabi);
        GameObject bolunenPrefabi = KurucuOrtak.PrefabKaydet(bolunen, "C_BolunenBlok");

        // Tampon: tasarlanmış sekme.
        GameObject tampon = KurucuOrtak.SpriteNesnesi("Tampon", daire, new Color(1f, 0.6f, 0.2f), Vector3.zero, Vector3.one, 2);
        tampon.AddComponent<CircleCollider2D>();
        tampon.AddComponent<Tampon>();
        GameObject tamponPrefabi = KurucuOrtak.PrefabKaydet(tampon, "C_Tampon");

        // --- Masa ---
        Color duvarRengi = new Color(0.35f, 0.22f, 0.12f);
        Duvar("UstDuvar", kare, duvarRengi, new Vector3(0f, 5.75f, 0f), new Vector3(15f, 0.5f, 1f));
        Duvar("AltDuvar", kare, duvarRengi, new Vector3(0f, -5.75f, 0f), new Vector3(15f, 0.5f, 1f));
        Duvar("SolDuvar", kare, duvarRengi, new Vector3(-7.25f, 0f, 0f), new Vector3(0.5f, 12f, 1f));
        Duvar("SagDuvar", kare, duvarRengi, new Vector3(7.25f, 0f, 0f), new Vector3(0.5f, 12f, 1f));

        // Cam ızgarası: prefabdan çoğaltılır. İki hücre bölünen bloktur.
        GameObject izgara = new GameObject("CamIzgarasi");
        float[] satirlar = { 2.2f, 2.9f, 3.6f };
        for (int s = 0; s < satirlar.Length; s++)
            for (int k = 0; k < 7; k++)
            {
                float x = -4.5f + k * 1.5f;
                bool ozel = s == 1 && (k == 1 || k == 5);
                GameObject blok = (GameObject)PrefabUtility.InstantiatePrefab(ozel ? bolunenPrefabi : camPrefabi);
                blok.transform.position = new Vector3(x, satirlar[s], 0f);
                blok.transform.SetParent(izgara.transform, true);
            }

        // Tamponlar.
        Vector3[] tamponYerleri = { new Vector3(-3.5f, -0.5f, 0f), new Vector3(3.5f, -0.5f, 0f), new Vector3(0f, 0.8f, 0f) };
        foreach (Vector3 yer in tamponYerleri)
        {
            GameObject t = (GameObject)PrefabUtility.InstantiatePrefab(tamponPrefabi);
            t.transform.position = yer;
        }

        // Hedef delik: tetikleyici, camların arkasında.
        GameObject delik = KurucuOrtak.SpriteNesnesi("HedefDelik", daire, new Color(0f, 0f, 0f, 0.85f), new Vector3(0f, 4.7f, 0f), Vector3.one * 0.9f, 1);
        delik.AddComponent<CircleCollider2D>().isTrigger = true;
        delik.AddComponent<HedefDelik>();

        // Fırlatıcı ve nişan oku.
        GameObject firlatici = KurucuOrtak.SpriteNesnesi("Firlatici", kare, new Color(0.7f, 0.7f, 0.7f), new Vector3(0f, -4.8f, 0f), Vector3.one * 0.6f, 3);
        Firlatici firlaticiBetik = firlatici.AddComponent<Firlatici>();
        GameObject ok = KurucuOrtak.SpriteNesnesi("NisanOku", kare, new Color(1f, 1f, 1f, 0.7f), new Vector3(0f, -3.8f, 0f), new Vector3(0.12f, 0.8f, 1f), 4);
        KurucuOrtak.AlanAta(firlaticiBetik, "topPrefabi", topPrefabi);
        KurucuOrtak.AlanAta(firlaticiBetik, "nisanOku", ok.transform);

        // Yönetici.
        new GameObject("Oyun").AddComponent<MasaOyun>();

        KurucuOrtak.SahneyiKaydet(SahneAdi);
        Debug.Log("C_Carpisma sahnesi kuruldu.");
    }

    // Unity'yi pencere açmadan (batchmode) çalıştırırken çağrılan giriş noktası.
    public static void TopluKur() => Kur();

    // Masa kenarı: düz çarpıştırıcı. Buradan sekme fizik motorunun hesabıdır (simüle).
    static void Duvar(string ad, Sprite kare, Color renk, Vector3 konum, Vector3 olcek)
    {
        GameObject go = KurucuOrtak.SpriteNesnesi(ad, kare, renk, konum, olcek);
        go.AddComponent<BoxCollider2D>();
    }

    // Topun sekmesi için sürtünmesiz, yüksek esneklikli malzemeyi yükler ya da oluşturur.
    static PhysicsMaterial2D SekmeliMalzeme()
    {
        string yol = $"{AyarKlasoru}/Sekmeli.physicsMaterial2D";
        PhysicsMaterial2D mevcut = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(yol);
        if (mevcut != null) return mevcut;

        Directory.CreateDirectory(AyarKlasoru);
        PhysicsMaterial2D yeni = new PhysicsMaterial2D("Sekmeli") { friction = 0f, bounciness = 0.85f };
        AssetDatabase.CreateAsset(yeni, yol);
        AssetDatabase.SaveAssets();
        return yeni;
    }
}
