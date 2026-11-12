using UnityEngine;

// Oyun A'da kullanılan iki renk. Karakter ve kapılar bu iki renkten birindedir.
public enum KapiRengi
{
    Kirmizi, // Birinci renk
    Mavi     // İkinci renk
}

// Renk adını ekranda görünen gerçek renge çeviren yardımcı sınıf.
public static class KapiRenkleri
{
    public static readonly Color Kirmizi = new Color(0.93f, 0.30f, 0.30f); // Kırmızı kapı/karakter rengi
    public static readonly Color Mavi = new Color(0.27f, 0.55f, 0.95f);    // Mavi kapı/karakter rengi

    // Verilen renk adının ekrandaki rengini döndürür.
    public static Color Renk(KapiRengi renk)
    {
        return renk == KapiRengi.Kirmizi ? Kirmizi : Mavi;
    }

    // Verilen rengin tersini döndürür (kırmızı ↔ mavi).
    public static KapiRengi Ters(KapiRengi renk)
    {
        return renk == KapiRengi.Kirmizi ? KapiRengi.Mavi : KapiRengi.Kirmizi;
    }
}
