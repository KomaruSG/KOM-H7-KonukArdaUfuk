using UnityEngine;

// Prizma'daki iki faz. Oyuncu her an bu ikisinden birindedir; nesnelerin katı ya da geçirgen olması buna bağlıdır.
public enum FazRengi
{
    Kirmizi, // Kırmızı faz
    Mavi     // Mavi faz
}

// Faz adını ekrandaki renge çeviren ve tersini bulan yardımcı sınıf.
public static class FazRenkleri
{
    public static readonly Color Kirmizi = new Color(0.95f, 0.32f, 0.32f); // Kırmızı faz rengi
    public static readonly Color Mavi = new Color(0.3f, 0.58f, 1f);        // Mavi faz rengi

    // Fazın ekrandaki rengini döndürür.
    public static Color Renk(FazRengi faz) => faz == FazRengi.Kirmizi ? Kirmizi : Mavi;

    // Öteki fazı döndürür.
    public static FazRengi Ters(FazRengi faz) => faz == FazRengi.Kirmizi ? FazRengi.Mavi : FazRengi.Kirmizi;

    // Fazın Türkçe adı (arayüz için).
    public static string Ad(FazRengi faz) => faz == FazRengi.Kirmizi ? "Kırmızı" : "Mavi";
}
