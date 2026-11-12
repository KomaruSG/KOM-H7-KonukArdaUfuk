# KOM Hafta 7 — Üç Mekanik, Dört Oyun

**Arda Ufuk Konuk** · Karmaşık Oyun Mekanikleri · İstanbul Nişantaşı Üniversitesi

Tek Unity projesinde dört oyun: üç mekaniği ayrı ayrı gösteren A, B, C ve üçünü birbirine bağlayan **D · Prizma**.

## Açma

1. Unity Hub → **Add** → bu klasörü seç. Sürüm: **Unity 6000.1.6f1** (Universal 2D, yeni Input System).
2. `Assets/Scenes` içinden bir sahne aç ve **Play**'e bas.

D'nin hazır Windows build'i: `Build/D_Birlesik/Prizma.exe` (Unity gerekmez, çift tıkla).

## Oyunlar ve kontroller

### A · Renk Kapısı — `A_TekTus`
Sağdan kırmızı ve mavi kapılar gelir; karakter kapıyla aynı renkteyse geçer.

| Girdi | Ne yapar |
|---|---|
| Boşluk / sol tık | Renk değiştir |
| Hızlı çift bas | Kalkan (bir yanlış kapıyı affeder, 5 sn bekleme) |
| Boşluk (kaybedince) | Yeniden başla |

### B · Hareket Hissi — `B_HareketHissi`
İvme, fren, değişken zıplama, coyote time ve zıplama tamponuyla kısa bir parkur. İki profil: **Kızak** (hızlı, kayar, uzun zıplar) ve **Kertenkele** (anında durur, kısa zıplar).

| Girdi | Ne yapar |
|---|---|
| A / D, ← / → | Yürü |
| W / ↑ / Boşluk | Zıpla (basılı tut = daha yüksek) |
| Q | Profil değiştir |
| R | Baştan |

### C · Sekme Masası — `C_Carpisma`
Üstten bakışlı masada 5 atışla bütün camları kır. Turuncu tamponlar topu çarpma noktasına göre açıyla ve hızlandırarak geri atar; mor bloklar kırılınca topu ikiye böler; siyah delik bir tetikleyicidir.

| Girdi | Ne yapar |
|---|---|
| Fare | Nişan al |
| Sol tık basılı tut / bırak | Güç topla / fırlat |
| R | Baştan |

### D · Prizma — `D_Birlesik`
Oyuncu kırmızı ya da mavi fazdadır. Kendi rengindeki bloklar katı, öteki renktekiler hayalettir. Faz değiştirmek enerji harcar; kristaller (ancak onların rengindeyken, üstten düşerek kırılır) enerji verir. Beyaz prizma kristali fazı bedavaya ama zorla değiştirir ve tuşu 2 sn kilitler. Hayalet bloğun içindeyken faz değiştiren ezilir.

| Girdi | Ne yapar |
|---|---|
| A / D, ← / → | Yürü |
| W / ↑ | Zıpla |
| Boşluk | Faz değiştir |
| R | Baştan |
| F1 / F2 / F3 | Bağlantı testi: tek tuşu / hareket hissini / çarpışma tasarımını aç-kapat |

## Klasörler

```
Assets/
  Scenes/       A_TekTus, B_HareketHissi, C_Carpisma, D_Birlesik
  Scripts/      A/ B/ C/ D/ ve Editor/ (sahne kurucular, build)
  Ayarlar/      Hareket profilleri (ScriptableObject) ve fizik malzemeleri
  Prefabs/      Kapı, top, cam, tampon ve kristal prefabları
  Sprites/      Kare ve daire
Build/D_Birlesik/   D'nin Windows build'i
Belge/              Tasarım belgesi ve oynanış videosu
```

## Editör menüsü (KOM)

- **KOM → A/B/C/D Sahnesini Kur:** Sahneyi baştan kurar. Inspector'da yapılan sahne ayarlarını siler; önce sorar. Hareket profilleri (`Assets/Ayarlar`) korunur.
- **KOM → D Build Al:** D'nin Windows build'ini `Build/D_Birlesik/Prizma.exe` olarak alır.
