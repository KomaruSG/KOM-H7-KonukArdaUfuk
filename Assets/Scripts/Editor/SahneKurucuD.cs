using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

// Oyun D (Prizma) sahnesini kodla kurar. Bölüm, aşağıdaki yazı haritasından okunur:
//   #  nötr zemin (her fazda katı)        K / M  kırmızı / mavi blok (fazına göre katı ya da geçirgen)
//   k / m  kırmızı / mavi kristal          P  prizma kristali (renksiz, her fazda katı)
//   ^  diken (ölüm)                        S  başlangıç    C  kontrol noktası    F  varış
// Harita 12 satır yüksekliğinde, yedi bölümün yan yana eklenmesiyle oluşur. Her karakter 1x1 birimdir.
// Menü: KOM > D Sahnesini Kur.
public static class SahneKurucuD
{
    const string SahneAdi = "D_Birlesik";         // Kaydedilecek sahnenin adı
    const string AyarKlasoru = "Assets/Ayarlar";  // Profil varlıklarının klasörü
    const int Yukseklik = 12;                     // Harita satır sayısı

    // Menüden çağrılır: sahneyi kurar (varsa önce sorar).
    [MenuItem("KOM/D Sahnesini Kur")]
    public static void Kur()
    {
        if (!KurucuOrtak.UzerineYazilsinMi(SahneAdi)) return;

        Sprite kare = KurucuOrtak.Kare();
        Sprite daire = KurucuOrtak.Daire();
        PhysicsMaterial2D surtunmesiz = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>($"{AyarKlasoru}/Surtunmesiz.physicsMaterial2D");
        KurucuOrtak.YeniSahne(new Color(0.09f, 0.09f, 0.13f), 7f);

        // Profiller yeni sahne açıldıktan sonra yüklenir (açılışta bellekten boşaltılıp boş kalmasınlar diye).
        HareketAyari kirmizi = KirmiziProfil();
        HareketAyari mavi = MaviProfil();
        HareketAyari notr = NotrProfil();

        // --- Kristal prefabları ---
        GameObject kirmiziKristal = KristalPrefabi("D_KirmiziKristal", daire, FazRengi.Kirmizi, false);
        GameObject maviKristal = KristalPrefabi("D_MaviKristal", daire, FazRengi.Mavi, false);
        GameObject prizmaKristal = KristalPrefabi("D_PrizmaKristali", daire, FazRengi.Kirmizi, true);

        // --- Harita ---
        string[] harita = HaritayiBirlestir();
        int genislik = harita[0].Length;
        Transform kok = new GameObject("Bolum").transform;
        Vector3 baslangic = Vector3.zero;
        Vector3 varis = Vector3.zero;
        Color zeminRengi = new Color(0.42f, 0.43f, 0.48f);

        for (int r = 0; r < Yukseklik; r++)
        {
            float y = Yukseklik - 1 - r;
            int c = 0;
            while (c < genislik)
            {
                char h = harita[r][c];
                if (h == '#' || h == 'K' || h == 'M' || h == '^')
                {
                    // Aynı karakterden oluşan yatay diziyi tek nesne yap (daha az nesne, daha az dikiş).
                    int bas = c;
                    while (c < genislik && harita[r][c] == h) c++;
                    Seri(h, bas, c - 1, y, kare, zeminRengi, kok);
                    continue;
                }

                Vector3 konum = new Vector3(c, y, 0f);
                switch (h)
                {
                    case 'k': Yerlestir(kirmiziKristal, konum, kok); break;
                    case 'm': Yerlestir(maviKristal, konum, kok); break;
                    case 'P': Yerlestir(prizmaKristal, konum, kok); break;
                    case 'S': baslangic = konum; break;
                    case 'F': varis = konum; break;
                    case 'C':
                        GameObject kn = KurucuOrtak.SpriteNesnesi("KontrolNoktasi", kare, new Color(0.7f, 0.7f, 0.7f, 0.6f), konum + Vector3.up * 0.5f, new Vector3(0.3f, 2f, 1f), 1);
                        kn.AddComponent<BoxCollider2D>().isTrigger = true;
                        kn.AddComponent<PrizmaKontrolNoktasi>();
                        kn.transform.SetParent(kok, true);
                        break;
                }
                c++;
            }
        }

        // Sol sınır duvarı ve varış.
        GameObject sol = KurucuOrtak.SpriteNesnesi("SolDuvar", kare, zeminRengi, new Vector3(-1f, 6f, 0f), new Vector3(1f, 13f, 1f));
        sol.AddComponent<BoxCollider2D>();
        GameObject varisNesnesi = KurucuOrtak.SpriteNesnesi("Varis", kare, new Color(1f, 0.85f, 0.2f), varis + Vector3.up * 0.5f, new Vector3(0.6f, 3f, 1f), 1);
        varisNesnesi.AddComponent<BoxCollider2D>().isTrigger = true;
        varisNesnesi.AddComponent<PrizmaVaris>();

        // Çukurun dibi: geniş, görünmez ölüm alanı.
        GameObject cukur = new GameObject("CukurDibi");
        cukur.transform.position = new Vector3(genislik / 2f, -4f, 0f);
        BoxCollider2D cukurKutu = cukur.AddComponent<BoxCollider2D>();
        cukurKutu.isTrigger = true;
        cukurKutu.size = new Vector2(genislik + 40f, 2f);
        cukur.AddComponent<OlumAlani>();

        // --- Oyuncu ---
        GameObject oyuncu = KurucuOrtak.SpriteNesnesi("Oyuncu", kare, FazRenkleri.Kirmizi, baslangic, new Vector3(0.8f, 0.8f, 1f), 5);
        Rigidbody2D govde = oyuncu.AddComponent<Rigidbody2D>();
        govde.freezeRotation = true;
        govde.interpolation = RigidbodyInterpolation2D.Interpolate;
        govde.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        BoxCollider2D kutu = oyuncu.AddComponent<BoxCollider2D>();
        kutu.sharedMaterial = surtunmesiz;
        Gezgin gezgin = oyuncu.AddComponent<Gezgin>();
        KurucuOrtak.AlanAta(gezgin, "ayar", kirmizi);
        BoolAta(gezgin, "boslukZiplatir", false); // Boşluk faz değiştirmeye ayrıldı
        FazKontrol faz = oyuncu.AddComponent<FazKontrol>();
        KurucuOrtak.AlanAta(faz, "gezgin", gezgin);
        KurucuOrtak.AlanAta(faz, "kirmiziAyar", kirmizi);
        KurucuOrtak.AlanAta(faz, "maviAyar", mavi);
        KurucuOrtak.AlanAta(faz, "notrAyar", notr);

        // Kamera takibi.
        TakipKamerasi takip = Camera.main.gameObject.AddComponent<TakipKamerasi>();
        KurucuOrtak.AlanAta(takip, "hedef", oyuncu.transform);
        SerializedObject kso = new SerializedObject(takip);
        kso.FindProperty("kayma").vector2Value = new Vector2(4f, 2f);
        kso.FindProperty("enAltY").floatValue = 5f;
        kso.ApplyModifiedPropertiesWithoutUndo();
        Camera.main.transform.position = new Vector3(baslangic.x + 4f, 5f, -10f);

        // Yönetici.
        PrizmaOyun oyun = new GameObject("Oyun").AddComponent<PrizmaOyun>();
        KurucuOrtak.AlanAta(oyun, "gezgin", gezgin);
        KurucuOrtak.AlanAta(oyun, "faz", faz);
        SerializedObject oso = new SerializedObject(oyun);
        oso.FindProperty("toplamMesafe").floatValue = varis.x - baslangic.x;
        oso.ApplyModifiedPropertiesWithoutUndo();

        KurucuOrtak.SahneyiKaydet(SahneAdi);
        Debug.Log($"D_Birlesik sahnesi kuruldu. Harita genişliği: {genislik}, varış mesafesi: {varis.x - baslangic.x}");
    }

    // Unity'yi pencere açmadan (batchmode) çalıştırırken çağrılan giriş noktası.
    public static void TopluKur() => Kur();

    // ---------------- Harita ----------------

    // Yedi bölümü yan yana ekleyip tek harita yapar. Her bölümün satırları kendi genişliğine '.' ile tamamlanır.
    static string[] HaritayiBirlestir()
    {
        List<(int genislik, string[] satirlar)> bolumler = new List<(int, string[])>
        {
            (30, Bolum1_Tanisma()),
            (30, Bolum2_Merdiven()),
            (30, Bolum3_UzunAtlayis()),
            (36, Bolum4_Ezilme()),
            (30, Bolum5_Prizma()),
            (40, Bolum6_SiraSira()),
            (25, Bolum7_Final()),
        };

        StringBuilder[] satirlar = new StringBuilder[Yukseklik];
        for (int r = 0; r < Yukseklik; r++) satirlar[r] = new StringBuilder();
        foreach (var (genislik, bolum) in bolumler)
            for (int r = 0; r < Yukseklik; r++)
            {
                string s = r < bolum.Length && bolum[r] != null ? bolum[r] : "";
                if (s.Length > genislik) Debug.LogError($"Harita satırı bölüm genişliğinden uzun: '{s}'");
                satirlar[r].Append(s.PadRight(genislik, '.'));
            }

        string[] sonuc = new string[Yukseklik];
        for (int r = 0; r < Yukseklik; r++) sonuc[r] = satirlar[r].ToString();
        return sonuc;
    }

    static string N(int n) => new string('.', n); // n tane boşluk
    static string Z(int n) => new string('#', n); // n tane nötr zemin
    static string T(char c, int n) => new string(c, n); // n tane aynı karakter

    // Bölüm 1 — Tanışma: kırmızı duvar (maviye geç), mavi duvar (kırmızıya geç), kırmızı köprü. Bir kırmızı kristal.
    static string[] Bolum1_Tanisma()
    {
        string duvar = N(8) + "K" + N(6) + "M";
        return new[]
        {
            null, null, null, null, null,
            duvar, duvar, duvar, duvar,
            N(2) + "S" + N(2) + "k" + N(2) + "K" + N(6) + "M" + N(11) + "C",
            Z(20) + T('K', 6) + Z(4),
            Z(20) + N(6) + Z(4),
        };
    }

    // Bölüm 2 — Merdiven: yüksek çıkıntıya sadece mavi basamaklarla çıkılır. Altta kırmızı, üstte mavi kristal.
    static string[] Bolum2_Merdiven()
    {
        return new[]
        {
            null, null, null, null, null,
            N(23) + "m" + N(3) + "C",
            N(16) + "MMM" + Z(11),
            N(19) + Z(11),
            N(11) + "MMM" + N(5) + Z(11),
            N(6) + "k" + N(12) + Z(11),
            Z(30),
            Z(30),
        };
    }

    // Bölüm 3 — Uzun atlayış: 6 birimlik boşluk. Kırmızı tek hamlede atlar; mavi ortadaki mavi basamağı kullanır.
    // Sonra dikenler ve kırmızı duvar.
    static string[] Bolum3_UzunAtlayis()
    {
        string duvar = N(22) + "K";
        return new[]
        {
            null, null, null, null, null,
            duvar, duvar, duvar,
            N(12) + "MM" + N(8) + "K",
            N(18) + "^^" + N(2) + "K" + N(4) + "C",
            Z(10) + N(6) + Z(14),
            Z(10) + N(6) + Z(14),
        };
    }

    // Bölüm 4 — Ezilme: büyük mavi bloğun içinden kırmızıyla geçilir. İçindeyken maviye geçen ezilir.
    // Çıkınca, köprüye basmadan önce maviye geçilmeli (kırmızı ~8.9 birim zıplar, 11 birimlik köprüyü atlayamaz).
    // Köprüde bir mavi kristal.
    static string[] Bolum4_Ezilme()
    {
        string blok = N(8) + T('M', 12);
        return new[]
        {
            null, null, null, null, null,
            blok, blok, blok, blok,
            blok + N(8) + "m" + N(5) + "C",
            Z(23) + T('M', 11) + Z(2),
            Z(23) + N(11) + Z(2),
        };
    }

    // Bölüm 5 — Prizma: boşluğun kenarındaki prizma kristali kırılınca faz bedavaya kırmızıya döner ve 2 sn kilitlenir;
    // kırılma sıçraması ve kırmızının uzun zıplamasıyla boşluk geçilir. Ya da enerji ödenip kırmızıya geçilir.
    static string[] Bolum5_Prizma()
    {
        string duvar = N(20) + "K";
        return new[]
        {
            null, null, null, null, null,
            duvar, duvar, duvar, duvar,
            N(3) + "m" + N(2) + "P" + N(13) + "K" + N(6) + "C",
            Z(8) + N(6) + Z(16),
            Z(8) + N(6) + Z(16),
        };
    }

    // Bölüm 6 — Sıra sıra: büyük çukurun üstünde ikişer kırmızı ve mavi platform, havada kristaller.
    // Havada faz değiştirmek ve kristallerden enerji toplamak gerekir.
    static string[] Bolum6_SiraSira()
    {
        return new[]
        {
            null, null, null, null, null, null, null,
            N(23) + "m",
            N(3) + "KK" + N(2) + "KK" + N(2) + "MM" + N(2) + "MM" + N(2) + "k" + N(6) + "KK" + N(2) + "KK",
            N(37) + "C",
            Z(2) + N(34) + Z(4),
            Z(2) + N(34) + Z(4),
        };
    }

    // Bölüm 7 — Final: mavi duvar, kırmızı duvar, varış.
    static string[] Bolum7_Final()
    {
        string duvarlar = N(6) + "M" + N(5) + "K";
        return new[]
        {
            null, null, null, null, null,
            duvarlar, duvarlar, duvarlar, duvarlar,
            duvarlar + N(7) + "F",
            Z(25),
            Z(25),
        };
    }

    // ---------------- Nesne üretimi ----------------

    // Yatay bir diziyi (zemin, renkli blok ya da diken) tek nesne olarak oluşturur.
    static void Seri(char h, int bas, int son, float y, Sprite kare, Color zeminRengi, Transform kok)
    {
        int uzunluk = son - bas + 1;
        float x = (bas + son) / 2f;
        GameObject go;
        switch (h)
        {
            case 'K':
            case 'M':
                FazRengi renk = h == 'K' ? FazRengi.Kirmizi : FazRengi.Mavi;
                go = KurucuOrtak.SpriteNesnesi(h == 'K' ? "KirmiziBlok" : "MaviBlok", kare, FazRenkleri.Renk(renk), new Vector3(x, y, 0f), new Vector3(uzunluk, 1f, 1f), 1);
                go.AddComponent<BoxCollider2D>();
                FazNesnesi n = go.AddComponent<FazNesnesi>();
                EnumAta(n, "renk", (int)renk);
                break;
            case '^':
                go = KurucuOrtak.SpriteNesnesi("Diken", kare, new Color(0.95f, 0.9f, 0.95f), new Vector3(x, y - 0.25f, 0f), new Vector3(uzunluk, 0.5f, 1f), 1);
                go.AddComponent<BoxCollider2D>().isTrigger = true;
                OlumAlani o = go.AddComponent<OlumAlani>();
                SerializedObject so = new SerializedObject(o);
                so.FindProperty("sebep").stringValue = "Dikenlere bastın!";
                so.ApplyModifiedPropertiesWithoutUndo();
                break;
            default:
                go = KurucuOrtak.SpriteNesnesi("Zemin", kare, zeminRengi, new Vector3(x, y, 0f), new Vector3(uzunluk, 1f, 1f));
                go.AddComponent<BoxCollider2D>();
                break;
        }
        go.transform.SetParent(kok, true);
    }

    // Prefabı sahneye bağlantılı olarak yerleştirir.
    static void Yerlestir(GameObject prefab, Vector3 konum, Transform kok)
    {
        GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.position = konum;
        go.transform.SetParent(kok, true);
    }

    // Kristal prefabı oluşturur: renkli olanlara FazNesnesi eklenir, prizma kristali renksiz ve her fazda katıdır.
    static GameObject KristalPrefabi(string ad, Sprite daire, FazRengi renk, bool prizma)
    {
        Color gorunus = prizma ? Color.white : FazRenkleri.Renk(renk);
        GameObject go = KurucuOrtak.SpriteNesnesi(ad, daire, gorunus, Vector3.zero, Vector3.one * 0.9f, 2);
        go.AddComponent<BoxCollider2D>();
        if (prizma)
        {
            PrizmaKristali p = go.AddComponent<PrizmaKristali>();
            SerializedObject so = new SerializedObject(p);
            so.FindProperty("enerjiKazanci").floatValue = 0f; // Faydası bedava faz değişimi; enerji vermez
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        else
        {
            go.AddComponent<Kristal>();
            FazNesnesi n = go.AddComponent<FazNesnesi>();
            EnumAta(n, "renk", (int)renk);
        }
        return KurucuOrtak.PrefabKaydet(go, ad);
    }

    // [SerializeField] enum alanına değer yazar.
    static void EnumAta(Object hedef, string alan, int deger)
    {
        SerializedObject so = new SerializedObject(hedef);
        so.FindProperty(alan).enumValueIndex = deger;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // [SerializeField] bool alanına değer yazar.
    static void BoolAta(Object hedef, string alan, bool deger)
    {
        SerializedObject so = new SerializedObject(hedef);
        so.FindProperty(alan).boolValue = deger;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // ---------------- Profiller ----------------

    // Kırmızı faz: hızlı, kayan, uzun zıplayan. Uzun boşluklar için.
    static HareketAyari KirmiziProfil() => ProfilGetir("D_Kirmizi", p =>
    {
        p.ad = "Kırmızı faz"; p.renk = FazRenkleri.Kirmizi;
        p.azamiHiz = 9f; p.ivme = 45f; p.fren = 12f; p.havaKontrolu = 0.75f;
        p.ziplamaYuksekligi = 3.2f; p.yercekimiOlcegi = 2.4f; p.dususCarpani = 1.25f; // önce 3.2 / 1.5: çok çabuk düşüyordu p.kisaZiplamaCarpani = 0.5f; p.azamiDususHizi = 18f;
        p.coyoteSuresi = 0.1f; p.tamponSuresi = 0.12f;
    });

    // Mavi faz: yavaş, anında duran, kısa ve sert inen (kristali daha kolay kırar), uzun coyote.
    static HareketAyari MaviProfil() => ProfilGetir("D_Mavi", p =>
    {
        p.ad = "Mavi faz"; p.renk = FazRenkleri.Mavi;
        p.azamiHiz = 6.5f; p.ivme = 60f; p.fren = 70f; p.havaKontrolu = 1f;
        p.ziplamaYuksekligi = 2.4f; p.yercekimiOlcegi = 3.2f; p.dususCarpani = 1.6f; // önce 4.5 / 2.4: çok çabuk düşüyordu p.kisaZiplamaCarpani = 0.45f; p.azamiDususHizi = 22f;
        p.coyoteSuresi = 0.18f; p.tamponSuresi = 0.15f;
    });

    // Bağlantı testi profili: hareket hissi kapalı. Anında hız, sabit yerçekimi, coyote ve tampon yok.
    static HareketAyari NotrProfil() => ProfilGetir("D_Notr", p =>
    {
        p.ad = "Düz (test)"; p.renk = Color.gray;
        p.azamiHiz = 7f; p.ivme = 500f; p.fren = 500f; p.havaKontrolu = 1f;
        p.ziplamaYuksekligi = 2.8f; p.yercekimiOlcegi = 3.5f; p.dususCarpani = 1f; p.kisaZiplamaCarpani = 1f; p.azamiDususHizi = 20f;
        p.coyoteSuresi = 0f; p.tamponSuresi = 0f;
    });

    // Profil varlığını yükler; yoksa oluşturup başlangıç değerlerini yazar (var olan ayarların üzerine yazmaz).
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
}
