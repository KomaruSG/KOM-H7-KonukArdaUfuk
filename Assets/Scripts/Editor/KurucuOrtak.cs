using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Sahne kurucularının ortak yardımcıları: sprite üretme, kamera, ışık, sahne kaydetme, build listesi.
// Sadece Editor'da çalışır, oyuna (build'e) girmez.
public static class KurucuOrtak
{
    public const string SpriteKlasoru = "Assets/Sprites";   // Üretilen sprite'ların klasörü
    public const string PrefabKlasoru = "Assets/Prefabs";   // Üretilen prefabların klasörü
    public const string SahneKlasoru = "Assets/Scenes";     // Sahnelerin klasörü

    // 64x64 beyaz kare sprite'ı döndürür; yoksa üretir.
    public static Sprite Kare() => SpriteGetir("Kare", false);

    // 64x64 beyaz daire sprite'ı döndürür; yoksa üretir.
    public static Sprite Daire() => SpriteGetir("Daire", true);

    // İstenen sprite'ı diskte yoksa PNG olarak yazar, Sprite olarak içe aktarır ve döndürür.
    static Sprite SpriteGetir(string ad, bool daire)
    {
        string yol = $"{SpriteKlasoru}/{ad}.png";
        if (!File.Exists(yol))
        {
            Directory.CreateDirectory(SpriteKlasoru);
            const int boyut = 64;
            Texture2D doku = new Texture2D(boyut, boyut, TextureFormat.RGBA32, false);
            Vector2 merkez = new Vector2(boyut / 2f - 0.5f, boyut / 2f - 0.5f);
            for (int y = 0; y < boyut; y++)
                for (int x = 0; x < boyut; x++)
                {
                    bool dolu = !daire || Vector2.Distance(new Vector2(x, y), merkez) <= boyut / 2f - 0.5f;
                    doku.SetPixel(x, y, dolu ? Color.white : Color.clear);
                }
            File.WriteAllBytes(yol, doku.EncodeToPNG());
            Object.DestroyImmediate(doku);
            AssetDatabase.ImportAsset(yol);

            TextureImporter ice = (TextureImporter)AssetImporter.GetAtPath(yol);
            ice.textureType = TextureImporterType.Sprite;
            ice.spriteImportMode = SpriteImportMode.Single;
            ice.spritePixelsPerUnit = boyut; // 1 sprite = 1 dünya birimi
            ice.filterMode = FilterMode.Bilinear;
            ice.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(yol);
    }

    // Boş yeni bir sahne açar, ortografik kamera ve 2D global ışık ekler.
    public static void YeniSahne(Color arkaPlan, float kameraBoyutu)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject kameraNesnesi = new GameObject("Main Camera") { tag = "MainCamera" };
        Camera kamera = kameraNesnesi.AddComponent<Camera>();
        kamera.orthographic = true;
        kamera.orthographicSize = kameraBoyutu;
        kamera.clearFlags = CameraClearFlags.SolidColor;
        kamera.backgroundColor = arkaPlan;
        kameraNesnesi.transform.position = new Vector3(0f, 0f, -10f);
        kameraNesnesi.AddComponent<UniversalAdditionalCameraData>();

        GameObject isik = new GameObject("Global Light 2D");
        Light2D l = isik.AddComponent<Light2D>();
        l.lightType = Light2D.LightType.Global;
        l.intensity = 1f;
    }

    // Sprite'lı bir nesne oluşturur.
    public static GameObject SpriteNesnesi(string ad, Sprite sprite, Color renk, Vector3 konum, Vector3 olcek, int sira = 0)
    {
        GameObject go = new GameObject(ad);
        go.transform.position = konum;
        go.transform.localScale = olcek;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = renk;
        sr.sortingOrder = sira;
        return go;
    }

    // Bir bileşenin [SerializeField] alanına değer yazar (Inspector'dan atamakla aynı iş).
    public static void AlanAta(Object hedef, string alanAdi, Object deger)
    {
        SerializedObject so = new SerializedObject(hedef);
        SerializedProperty p = so.FindProperty(alanAdi);
        if (p == null) { Debug.LogError($"Alan bulunamadı: {hedef.GetType().Name}.{alanAdi}"); return; }
        p.objectReferenceValue = deger;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    // Nesneyi prefab olarak kaydeder, sahnedeki kopyayı siler ve prefabı döndürür.
    public static GameObject PrefabKaydet(GameObject go, string ad)
    {
        Directory.CreateDirectory(PrefabKlasoru);
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{PrefabKlasoru}/{ad}.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    // Açık sahneyi kaydeder ve build listesine (yoksa) ekler.
    public static void SahneyiKaydet(string ad)
    {
        Directory.CreateDirectory(SahneKlasoru);
        string yol = $"{SahneKlasoru}/{ad}.unity";
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), yol);

        List<EditorBuildSettingsScene> liste = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        if (!liste.Exists(s => s.path == yol)) liste.Add(new EditorBuildSettingsScene(yol, true));
        liste.Sort((a, b) => string.CompareOrdinal(a.path, b.path)); // A, B, C, D sırası
        EditorBuildSettings.scenes = liste.ToArray();
        AssetDatabase.SaveAssets();
    }

    // Sahne zaten varsa üzerine yazmadan önce sorar (ayarlar silinmesin diye). Toplu modda sormaz.
    public static bool UzerineYazilsinMi(string ad)
    {
        if (Application.isBatchMode) return true;
        if (!File.Exists($"{SahneKlasoru}/{ad}.unity")) return true;
        return EditorUtility.DisplayDialog("Sahne zaten var",
            $"{ad} sahnesi yeniden kurulursa Inspector'da yaptığın ayarlar silinir. Devam edilsin mi?",
            "Evet, yeniden kur", "Vazgeç");
    }
}
