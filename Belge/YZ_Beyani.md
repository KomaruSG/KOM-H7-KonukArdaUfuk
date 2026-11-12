# Yapay Zekâ Kullanım Notu

Tasarım belgesinin 6. bölümü bu nottan doldurulur.

**Araç:** Claude (Anthropic), Claude Code masaüstü uygulaması üzerinden.
**Tarih:** 10–12 Kasım 2026

## Yapay zekâya yaptırılanlar

| Ne | Dosyalar |
|---|---|
| Ders yönergesinin özetlenmesi ve D için üç oyun fikri önerisi (Prizma, Sonar, Ağır Çekim). Prizma'yı ben seçtim. | — |
| A, B, C, D oyunlarının bütün betiklerinin yazılması (Türkçe açıklama satırlarıyla) | `Assets/Scripts/A/`, `B/`, `C/`, `D/` |
| Sahneleri, prefabları, hareket profillerini ve fizik malzemelerini kodla kuran Editor betikleri; D'nin bölüm haritası | `Assets/Scripts/Editor/SahneKurucuA–D.cs`, `KurucuOrtak.cs` |
| Windows build betiği | `Assets/Scripts/Editor/BuildAlici.cs` |
| README ve bu tasarım belgesinin metni, PDF'i üreten betik | `README.md`, `Belge/kaynak/belge_uret.py` |
| Hata ayıklama: B ve D'de karakterin hareket etmemesi (profil bağlantılarının boş kaydedilmesi) | `SahneKurucuB.cs`, `SahneKurucuD.cs` |
| Oyun testlerimden sonra istediğim ayar değişikliklerinin uygulanması (C'de top sürtünmesi, cam eşiği, durma hızı; D'de yerçekimi, köprü uzunluğu, kristal eşiği) | `Assets/Ayarlar/`, `Assets/Prefabs/`, ilgili betikler |

## Benim yaptıklarım

- D için fikir seçimi (Prizma) ve A/B/C'nin buna göre planlanmasının onayı.
- Dört oyunu Unity'de oynayarak test etmek; hissettiğim sorunları belirleyip ayar değişikliklerine karar vermek
  (B/D'de hareket edememe, C'de topun çok çabuk yavaşlaması sonra hiç durmaması, D'de çok hızlı düşme).
- Build'i çalıştırıp denemek, oynanış videosu, sınıftaki eşli test ve savunma.
