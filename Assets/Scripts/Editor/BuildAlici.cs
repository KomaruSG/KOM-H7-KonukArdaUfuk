using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// Oyun D'nin Windows build'ini alır.
// Build süresince Player Settings'teki şirket adı (Company Name) geçici olarak değiştirilir,
// build bitince (başarılı da olsa başarısız da olsa) eski hâline geri getirilir.
// Menü: KOM > D Build Al.
public static class BuildAlici
{
    const string BuildSirketAdi = "Arda Ufuk Konuk";                 // Build'e yazılacak şirket adı
    const string SahneYolu = "Assets/Scenes/D_Birlesik.unity";       // Build'e giren tek sahne
    const string CiktiYolu = "Build/D_Birlesik/Prizma.exe";          // Çıktı dosyası

    // Menüden çağrılır: kaydedilmemiş sahne varsa önce sorar, sonra build alır.
    [MenuItem("KOM/D Build Al")]
    public static void BuildAl()
    {
        // Kaydedilmemiş değişiklik varsa build "Scene(s) Have Been Modified" penceresinde takılır; önce sor.
        if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        string eskiSirketAdi = PlayerSettings.companyName;
        try
        {
            PlayerSettings.companyName = BuildSirketAdi;

            BuildPlayerOptions secenekler = new BuildPlayerOptions
            {
                scenes = new[] { SahneYolu },
                locationPathName = CiktiYolu,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None,
            };
            BuildReport rapor = BuildPipeline.BuildPlayer(secenekler);
            BuildSummary ozet = rapor.summary;

            if (ozet.result == BuildResult.Succeeded)
                Debug.Log($"Build tamam: {CiktiYolu} ({ozet.totalSize / (1024f * 1024f):0.0} MB, {ozet.totalTime.TotalSeconds:0} sn)");
            else
                Debug.LogError($"Build başarısız: {ozet.result}, {ozet.totalErrors} hata");
        }
        finally
        {
            // Ne olursa olsun şirket adını eski hâline getir ve proje ayarlarını kaydet.
            PlayerSettings.companyName = eskiSirketAdi;
            AssetDatabase.SaveAssets();
            Debug.Log($"Şirket adı geri alındı: {eskiSirketAdi}");
        }
    }

    // Unity'yi pencere açmadan (batchmode) çalıştırırken çağrılan giriş noktası.
    public static void TopluBuild() => BuildAl();
}
