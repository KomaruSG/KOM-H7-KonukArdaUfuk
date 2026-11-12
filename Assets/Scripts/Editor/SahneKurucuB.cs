using System.IO;
using UnityEditor;
using UnityEngine;

// Oyun B (Hareket Hissi) sahnesini kodla kurar: iki hareket profili, sürtünmesiz malzeme, parkur, karakter,
// kamera takibi, kontrol noktası, düşme alanı ve varış noktası.
// Menü: KOM > B Sahnesini Kur. Profil varlıkları zaten varsa üzerine yazılmaz (ayarlar korunur).
public static class SahneKurucuB
{
    const string SahneAdi = "B_HareketHissi";     // Kaydedilecek sahnenin adı
    const string AyarKlasoru = "Assets/Ayarlar";  // Profil ve malzeme varlıklarının klasörü

    // Menüden çağrılır: sahneyi kurar (varsa önce sorar).
    [MenuItem("KOM/B Sahnesini Kur")]
    public static void Kur()
    {
        if (!KurucuOrtak.UzerineYazilsinMi(SahneAdi)) return;

        Sprite kare = KurucuOrtak.Kare();
        PhysicsMaterial2D surtunmesiz = SurtunmesizMalzeme();

        KurucuOrtak.YeniSahne(new Color(0.13f, 0.14f, 0.18f), 6f);

        // Profiller YENİ SAHNE AÇILDIKTAN SONRA yüklenir: yeni sahne açılırken Unity kullanılmayan varlıkları
        // bellekten boşaltır ve önceden yüklenen profil boş (null) kalır; sahneye boş bağlantı yazılırdı.
        HareketAyari kizak = KizakProfili();
        HareketAyari kertenkele = KertenkeleProfili();
        Color zeminRengi = new Color(0.45f, 0.47f, 0.52f);

        // Parkur: (sol x, sağ x, üst y). Aralarındaki boşluklar bilerek seçildi.
        Platform("Baslangic", -12f, 6f, -3f, kare, zeminRengi);
        Platform("Basamak", 6f, 12f, -1.5f, kare, zeminRengi);
        Platform("KenarSonrasi", 16f, 19f, -1.5f, kare, zeminRengi);   // 4 birimlik boşluk: kenar zıplaması
        Platform("DarAdim", 22f, 24f, -1.5f, kare, zeminRengi);        // dar platform: kayan profil tutunamaz
        Platform("Pist", 27f, 40f, -1.5f, kare, zeminRengi);           // hızlanma pisti
        Platform("Varis", 47f, 60f, -1.5f, kare, zeminRengi);          // 7 birimlik uzun boşluktan sonra
        Platform("SonDuvar", 60f, 61f, 4f, kare, zeminRengi);          // dünyanın sonu

        // Karakter.
        GameObject karakter = KurucuOrtak.SpriteNesnesi("Gezgin", kare, kizak.renk, new Vector3(-8f, -2.4f, 0f), new Vector3(0.8f, 0.8f, 1f), 5);
        Rigidbody2D govde = karakter.AddComponent<Rigidbody2D>();
        govde.freezeRotation = true;
        govde.interpolation = RigidbodyInterpolation2D.Interpolate;
        govde.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        govde.gravityScale = kizak.yercekimiOlcegi;
        BoxCollider2D kutu = karakter.AddComponent<BoxCollider2D>();
        kutu.sharedMaterial = surtunmesiz; // Duvara yapışmayı önler
        Gezgin gezgin = karakter.AddComponent<Gezgin>();
        KurucuOrtak.AlanAta(gezgin, "ayar", kizak);

        // Kamera takibi.
        Camera kamera = Camera.main;
        TakipKamerasi takip = kamera.gameObject.AddComponent<TakipKamerasi>();
        KurucuOrtak.AlanAta(takip, "hedef", karakter.transform);
        kamera.transform.position = new Vector3(-5f, -1f, -10f);

        // Kontrol noktası (pistin başında) ve varış noktası.
        Tetikleyici("KontrolNoktasi", kare, new Color(0.7f, 0.7f, 0.7f, 0.6f), new Vector3(28.5f, 0f, 0f), new Vector3(0.3f, 3f, 1f)).AddComponent<KontrolNoktasi>();
        Tetikleyici("VarisNoktasi", kare, new Color(1f, 0.85f, 0.2f), new Vector3(57f, 0f, 0f), new Vector3(0.6f, 3f, 1f)).AddComponent<VarisNoktasi>();

        // Düşme alanı: görünmez, geniş tetikleyici.
        GameObject dusme = new GameObject("DusmeAlani");
        dusme.transform.position = new Vector3(25f, -10f, 0f);
        BoxCollider2D dusmeKutu = dusme.AddComponent<BoxCollider2D>();
        dusmeKutu.isTrigger = true;
        dusmeKutu.size = new Vector2(200f, 2f);
        dusme.AddComponent<DusmeAlani>();

        // Yönetici ve profil değiştirici.
        GameObject yonetici = new GameObject("Oyun");
        ParkurSayaci sayac = yonetici.AddComponent<ParkurSayaci>();
        KurucuOrtak.AlanAta(sayac, "gezgin", gezgin);
        ProfilDegistirici degistirici = yonetici.AddComponent<ProfilDegistirici>();
        KurucuOrtak.AlanAta(degistirici, "gezgin", gezgin);
        SerializedObject so = new SerializedObject(degistirici);
        SerializedProperty liste = so.FindProperty("profiller");
        liste.arraySize = 2;
        liste.GetArrayElementAtIndex(0).objectReferenceValue = kizak;
        liste.GetArrayElementAtIndex(1).objectReferenceValue = kertenkele;
        so.ApplyModifiedPropertiesWithoutUndo();

        KurucuOrtak.SahneyiKaydet(SahneAdi);
        Debug.Log("B_HareketHissi sahnesi kuruldu.");
    }

    // Unity'yi pencere açmadan (batchmode) çalıştırırken çağrılan giriş noktası.
    public static void TopluKur() => Kur();

    // Üst yüzeyi verilen yükseklikte, aşağı doğru uzanan katı bir platform oluşturur.
    static void Platform(string ad, float solX, float sagX, float ustY, Sprite kare, Color renk)
    {
        const float altY = -7f;
        Vector3 merkez = new Vector3((solX + sagX) / 2f, (ustY + altY) / 2f, 0f);
        Vector3 olcek = new Vector3(sagX - solX, ustY - altY, 1f);
        GameObject go = KurucuOrtak.SpriteNesnesi(ad, kare, renk, merkez, olcek);
        go.AddComponent<BoxCollider2D>();
    }

    // Tetikleyici çarpıştırıcılı görünür bir nesne oluşturur.
    static GameObject Tetikleyici(string ad, Sprite kare, Color renk, Vector3 konum, Vector3 olcek)
    {
        GameObject go = KurucuOrtak.SpriteNesnesi(ad, kare, renk, konum, olcek, 1);
        go.AddComponent<BoxCollider2D>().isTrigger = true;
        return go;
    }

    // "Kızak" profili: yüksek azami hız, yavaş fren (kayar), uzun ve süzülen zıplama. Uzun boşluk için.
    static HareketAyari KizakProfili()
    {
        return ProfilGetir("B_Kizak", p =>
        {
            p.ad = "Kızak";
            p.renk = new Color(0.55f, 0.85f, 1f);
            p.azamiHiz = 11f; p.ivme = 18f; p.fren = 6f; p.havaKontrolu = 0.6f;
            p.ziplamaYuksekligi = 3.2f; p.yercekimiOlcegi = 3f; p.dususCarpani = 1.6f; p.kisaZiplamaCarpani = 0.5f; p.azamiDususHizi = 18f;
            p.coyoteSuresi = 0.12f; p.tamponSuresi = 0.12f;
        });
    }

    // "Kertenkele" profili: anında kalkış, sert fren, kısa ve hızlı zıplama. Dar platformlar için.
    static HareketAyari KertenkeleProfili()
    {
        return ProfilGetir("B_Kertenkele", p =>
        {
            p.ad = "Kertenkele";
            p.renk = new Color(0.55f, 0.9f, 0.45f);
            p.azamiHiz = 7.5f; p.ivme = 70f; p.fren = 80f; p.havaKontrolu = 1f;
            p.ziplamaYuksekligi = 2.8f; p.yercekimiOlcegi = 4f; p.dususCarpani = 2.2f; p.kisaZiplamaCarpani = 0.45f; p.azamiDususHizi = 22f;
            p.coyoteSuresi = 0.1f; p.tamponSuresi = 0.15f;
        });
    }

    // Profil varlığını yükler; yoksa oluşturup başlangıç değerlerini yazar.
    static HareketAyari ProfilGetir(string ad, System.Action<HareketAyari> doldur)
    {
        string yol = $"{AyarKlasoru}/{ad}.asset";
        HareketAyari mevcut = AssetDatabase.LoadAssetAtPath<HareketAyari>(yol);
        if (mevcut != null) return mevcut;

        Directory.CreateDirectory(AyarKlasoru);
        HareketAyari yeni = ScriptableObject.CreateInstance<HareketAyari>();
        doldur(yeni);
        AssetDatabase.CreateAsset(yeni, yol);
        AssetDatabase.SaveAssets();
        return yeni;
    }

    // Sürtünmesi sıfır fizik malzemesini yükler ya da oluşturur.
    static PhysicsMaterial2D SurtunmesizMalzeme()
    {
        string yol = $"{AyarKlasoru}/Surtunmesiz.physicsMaterial2D";
        PhysicsMaterial2D mevcut = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(yol);
        if (mevcut != null) return mevcut;

        Directory.CreateDirectory(AyarKlasoru);
        PhysicsMaterial2D yeni = new PhysicsMaterial2D("Surtunmesiz") { friction = 0f, bounciness = 0f };
        AssetDatabase.CreateAsset(yeni, yol);
        AssetDatabase.SaveAssets();
        return yeni;
    }
}
