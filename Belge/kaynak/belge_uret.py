# H7 tasarım belgesini (Belge/H7_TasarimBelgesi.pdf) üretir.
# Çalıştırma (proje kökünden):  py Belge/kaynak/belge_uret.py
# Eşli test notları sınıfta öğrenildikten sonra OYUN_TESTI sözlüğü doldurulup yeniden çalıştırılabilir.

import math
import os

from reportlab.graphics.shapes import Drawing, Line, Polygon, Rect, String
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.units import cm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import KeepTogether, Paragraph, SimpleDocTemplate, Spacer, Table, TableStyle

KOK = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))  # Belge/
CIKTI = os.path.join(KOK, "H7_TasarimBelgesi.pdf")

# Türkçe karakterler için Arial.
pdfmetrics.registerFont(TTFont("Arial", r"C:\Windows\Fonts\arial.ttf"))
pdfmetrics.registerFont(TTFont("Arial-Bold", r"C:\Windows\Fonts\arialbd.ttf"))
pdfmetrics.registerFont(TTFont("Arial-Italic", r"C:\Windows\Fonts\ariali.ttf"))
pdfmetrics.registerFontFamily("Arial", normal="Arial", bold="Arial-Bold", italic="Arial-Italic")

# Eşli oyun testi: sınıfta doldurulacak. Boş kalırsa belgede doldurma satırları çıkar.
OYUN_TESTI = {
    "test_eden": "",
    "gozlemler": ["", "", ""],
    "duzeltme": "",
}

KOYU = colors.HexColor("#1f2430")
CIZGI = colors.HexColor("#b9bfcc")
BASLIK_ZEMIN = colors.HexColor("#e6e9f0")
KIRMIZI = colors.HexColor("#e05252")
MAVI = colors.HexColor("#4a90e2")

baslik = ParagraphStyle("baslik", fontName="Arial-Bold", fontSize=15, leading=19, textColor=KOYU, spaceAfter=2)
alt_baslik = ParagraphStyle("alt", fontName="Arial", fontSize=8.5, leading=11, textColor=colors.HexColor("#555b66"))
bolum = ParagraphStyle("bolum", fontName="Arial-Bold", fontSize=11.5, leading=14, textColor=KOYU, spaceBefore=9, spaceAfter=4)
ara = ParagraphStyle("ara", fontName="Arial-Bold", fontSize=9.2, leading=12, textColor=KOYU, spaceBefore=5, spaceAfter=2)
govde = ParagraphStyle("govde", fontName="Arial", fontSize=8.8, leading=11.6, spaceAfter=3)
hucre = ParagraphStyle("hucre", fontName="Arial", fontSize=7.7, leading=9.4)
hucre_b = ParagraphStyle("hucre_b", parent=hucre, fontName="Arial-Bold")


def p(metin, stil=govde):
    return Paragraph(metin, stil)


def tablo(satirlar, genislikler, baslik_satiri=True):
    """Hücreleri paragrafa çevirip ince çizgili bir tablo döndürür."""
    veri = []
    for i, satir in enumerate(satirlar):
        stil = hucre_b if (baslik_satiri and i == 0) else hucre
        veri.append([Paragraph(str(h), stil) for h in satir])
    t = Table(veri, colWidths=[g * cm for g in genislikler], repeatRows=1 if baslik_satiri else 0)
    komutlar = [
        ("GRID", (0, 0), (-1, -1), 0.4, CIZGI),
        ("VALIGN", (0, 0), (-1, -1), "TOP"),
        ("LEFTPADDING", (0, 0), (-1, -1), 3),
        ("RIGHTPADDING", (0, 0), (-1, -1), 3),
        ("TOPPADDING", (0, 0), (-1, -1), 1.5),
        ("BOTTOMPADDING", (0, 0), (-1, -1), 1.5),
    ]
    if baslik_satiri:
        komutlar.append(("BACKGROUND", (0, 0), (-1, 0), BASLIK_ZEMIN))
    t.setStyle(TableStyle(komutlar))
    return t


# ---------------- Bağ şeması ----------------

def ok(d, x1, y1, x2, y2, renk=KOYU, kesikli=False):
    """(x1,y1)'den (x2,y2)'ye uçlu ok çizer."""
    cizgi = Line(x1, y1, x2, y2, strokeColor=renk, strokeWidth=1.2)
    if kesikli:
        cizgi.strokeDashArray = [3, 2]
    d.add(cizgi)
    aci = math.atan2(y2 - y1, x2 - x1)
    boy, gen = 7, 3.2
    sol = (x2 - boy * math.cos(aci) + gen * math.sin(aci), y2 - boy * math.sin(aci) - gen * math.cos(aci))
    sag = (x2 - boy * math.cos(aci) - gen * math.sin(aci), y2 - boy * math.sin(aci) + gen * math.cos(aci))
    d.add(Polygon([x2, y2, sol[0], sol[1], sag[0], sag[1]], fillColor=renk, strokeColor=renk))


def kutu(d, x, y, w, h, metin, alt):
    d.add(Rect(x, y, w, h, rx=5, ry=5, fillColor=colors.white, strokeColor=KOYU, strokeWidth=1.2))
    d.add(String(x + w / 2, y + h / 2 + 1, metin, fontName="Arial-Bold", fontSize=10, textAnchor="middle", fillColor=KOYU))
    d.add(String(x + w / 2, y + h / 2 - 10, alt, fontName="Arial", fontSize=7, textAnchor="middle", fillColor=colors.HexColor("#555b66")))


def etiket(d, x, y, metin, renk=KOYU, hiza="middle"):
    d.add(String(x, y, metin, fontName="Arial", fontSize=7.2, textAnchor=hiza, fillColor=renk))


def bag_semasi():
    d = Drawing(17 * cm, 5.4 * cm)
    w, h = 108, 36
    tx, ty = 205, 112   # Tek tuş (üst orta)
    cx, cy = 30, 12     # Çarpışma (sol alt)
    hx, hy = 342, 12    # Hareket (sağ alt)
    kutu(d, tx, ty, w, h, "TEK TUŞ", "Boşluk: faz değiştir")
    kutu(d, cx, cy, w, h, "ÇARPIŞMA", "renkli blok, kristal, prizma")
    kutu(d, hx, hy, w, h, "HAREKET", "kırmızı / mavi profil")

    # Bağ 1: Tek tuş → Çarpışma
    ok(d, tx + 8, ty, cx + w - 10, cy + h, renk=KIRMIZI)
    etiket(d, 118, 96, "1  faz → collider mı trigger mı", KIRMIZI, "end")
    # Bağ 2: Çarpışma → Tek tuş
    ok(d, cx + w + 6, cy + h - 6, tx + 26, ty, renk=MAVI)
    etiket(d, 178, 58, "2  kristal → +3 enerji", MAVI, "start")
    etiket(d, 178, 49, "    prizma → zorla faz + 2 sn kilit", MAVI, "start")
    # Bağ 3: Tek tuş → Hareket
    ok(d, tx + w - 8, ty, hx + 14, hy + h, renk=KOYU)
    etiket(d, 336, 96, "3  faz → hareket profili", KOYU, "start")
    # İkincil: Hareket → Çarpışma
    ok(d, hx, cy + h / 2, cx + w + 2, cy + h / 2, renk=colors.HexColor("#888888"), kesikli=True)
    etiket(d, 240, 22, "düşüş hızı → kristal kırılır mı (ikincil)", colors.HexColor("#666666"))
    return d


# ---------------- İçerik ----------------

def icerik():
    s = []
    s.append(p("Hafta 7 Tasarım Belgesi — Üç Mekanik, Dört Oyun", baslik))
    s.append(p("<b>Arda Ufuk Konuk</b> &nbsp;·&nbsp; Öğrenci no: 20246000034 &nbsp;·&nbsp; Karmaşık Oyun Mekanikleri "
               "&nbsp;·&nbsp; Dr. Öğr. Üyesi Ziyad Guliyev &nbsp;·&nbsp; İstanbul Nişantaşı Üniversitesi &nbsp;·&nbsp; Kasım 2026", alt_baslik))
    s.append(p("Unity 6000.1.6f1 · Universal 2D · yeni Input System. Sahneler: A_TekTus, B_HareketHissi, C_Carpisma, D_Birlesik. "
               "Girdi her oyunda <b>Update</b>'te okunur; Rigidbody2D'ye dokunan hareket <b>FixedUpdate</b>'te yapılır.", alt_baslik))

    # 1. Mekanik envanteri
    s.append(p("1. Mekanik envanteri", bolum))
    s.append(tablo([
        ["Oyun", "Fiil", "Girdi", "Geri bildirim", "Başarısızlık"],
        ["<b>A · Renk Kapısı</b>", "renk değiştir, kalkan aç",
         "Boşluk/tık: tek bas = renk; 0,25 sn içinde çift bas = kalkan (iki değişim birbirini götürür, renk aynı kalır)",
         "karakter rengi, kalkan halkası, geçilen kapı soluklaşır, kapı sayısı ve hız, kalkan sayacı",
         "yanlış renkte kapıya kalkansız değmek; aynı tuşla yeniden başlar"],
        ["<b>B · Hareket Hissi</b>", "koş, zıpla, profil değiştir",
         "A/D; W/↑/Boşluk (basılı tut = yüksek); Q = Kızak ↔ Kertenkele",
         "karakter rengi = profil, hız ve süre, kamera takibi, kontrol noktası yeşile döner",
         "çukura düşmek (kontrol noktasına dönüş, düşme sayılır); süre ölçülür"],
        ["<b>C · Sekme Masası</b>", "nişan al, güç topla, fırlat",
         "fare = yön; sol tık basılı tut = güç (1 sn'de dolar), bırak = fırlat",
         "ok boyu = güç, tampon büyüyüp parlar, cam kırılır, top ikiye bölünür, puan",
         "5 atış bitince masada cam kalması"],
        ["<b>D · Prizma</b>", "faz değiştir, koş, zıpla, kristal kır",
         "Boşluk = faz; A/D; W/↑ = zıpla; kristale üstten düşerek kırma",
         "blokların katı/saydam görünmesi, karakter rengi, enerji çubuğu ve maliyet, kristal sıçraması, kilit sayacı, uyarı yazıları",
         "çukur, diken, ezilme (hayalet bloğun içindeyken faz değiştirmek); 5 can, sonra baştan"],
    ], [2.3, 2.6, 4.4, 4.2, 4.3]))

    # 2. Ayar tabloları
    s.append(p("2. Ayar tabloları", bolum))
    s.append(p("Değerler oynayarak değiştirildi; değişenler “önce → şimdi” olarak yazıldı.", govde))

    s.append(p("A · Renk Kapısı", ara))
    s.append(tablo([
        ["Değişken", "Değer", "Oyuncunun hissettiği"],
        ["baslangicHizi / hizArtisi / azamiHiz", "4 / +0,12 her sn / 11", "<b>Artan zorluk sayısı.</b> İlk 10 sn rahat, ~1 dk sonra kapılar refleks sınırına gelir."],
        ["baslangicAraligi → asgariAralik", "7 → 3,5 birim (−0,05/sn)", "İkinci zorluk sayısı: kapılar sıklaşır, art arda farklı renkler gelir."],
        ["ciftBasmaPenceresi", "0,25 sn", "Daha kısası kalkanı açmayı zorlaştırıyor; daha uzunu iki ayrı renk değişimini kalkana çeviriyor."],
        ["kalkanSuresi / kalkanBeklemeSuresi", "2 sn / 5 sn", "Kalkan her zaman açık tutulamaz: “şimdi mi kalkan, sonra mı?” kararı."],
        ["renkDegismeOlasiligi", "0,55", "Kapıların yarıdan biraz fazlası renk değiştirir; ezber yerine dikkat ister."],
    ], [5.2, 3.6, 9.0]))

    s.append(p("B · Hareket Hissi (Q ile değişen iki profil)", ara))
    s.append(tablo([
        ["Değişken", "Kızak", "Kertenkele", "Hissedilen fark"],
        ["azamiHiz", "11", "7,5", "Kızak uzun boşluk için hız toplar."],
        ["ivme / fren", "18 / 6", "70 / 80", "Kızak geç kalkar, durmaz, kayar; Kertenkele anında başlar ve durur (dar platformlar)."],
        ["havaKontrolu", "0,6", "1,0", "Kızak havada yön değiştiremez; Kertenkele havada düzeltme yapar."],
        ["ziplamaYuksekligi / yerçekimi / düşüş", "3,2 / 3 / ×1,6", "2,8 / 4 / ×2,2", "Kızak süzülür (~9 birim atlar), Kertenkele kısa ve sert (~5 birim)."],
        ["kisaZiplamaCarpani", "0,5", "0,45", "Tuş erken bırakılınca yukarı hız kesilir: değişken yükseklik."],
        ["coyoteSuresi / tamponSuresi", "0,12 / 0,12 sn", "0,10 / 0,15 sn", "Kenardan geç zıplamak ve yere değmeden basmak affedilir."],
    ], [5.0, 2.6, 2.6, 7.6]))
    s.append(Spacer(1, 3))
    s.append(p("Parkur iki profili de ister: 4 birimlik <b>kenar zıplaması</b> ve dar platform Kertenkele'ye, 7 birimlik <b>uzun boşluk</b> Kızak'a göre ölçüldü.", govde))

    s.append(p("C · Sekme Masası", ara))
    s.append(tablo([
        ["Değişken", "Değer", "Oyuncunun hissettiği"],
        ["Top linearDamping", "0,35 → 0,12 → <b>0,25</b>", "0,35'te top çok çabuk yavaşlıyor, az cam kırılıyordu; 0,12'de neredeyse hiç durmuyordu. 0,25 ortası (~10 sn)."],
        ["CamBlok.kirilmaHizi", "5 → <b>3</b>", "Camlar daha kolay kırılıyor; yavaş top yine de sadece seker."],
        ["MasaTopu.durmaHizi", "0,4 → <b>1,5</b>", "Topun son yavaş süzülmesi bekletiyordu; artık yavaşlayınca hemen sıradaki atışa geçilir."],
        ["Tampon hizCarpani / en az / en çok", "×1,3 / 7 / 20", "Tampon her vuruşta topu hızlandırır ama kontrolden çıkarmaz."],
        ["Fırlatma hızı / dolma süresi", "5–16 / 1 sn", "Hafif dokunuş ile tam güç arasında belirgin fark."],
        ["BolunenBlok.ayrilmaAcisi", "25°", "İki top birbirinden açıkça ayrılır, ikisi de cama gider."],
    ], [5.2, 3.6, 9.0]))

    s.append(p("D · Prizma", ara))
    s.append(tablo([
        ["Değişken", "Kırmızı faz", "Mavi faz", "Neden böyle"],
        ["azamiHiz / ivme / fren", "9 / 45 / 12", "6,5 / 60 / 70", "Kırmızı hızlı ve kayar; mavi yavaş ama anında durur."],
        ["ziplamaYuksekligi", "3,2", "2,4", "Mavi merdiven basamakları 2 birim: mavi çıkar, ama mavi yüksek duvarı aşamaz."],
        ["yercekimiOlcegi / dususCarpani", "3,2 → <b>2,4</b> / 1,5 → <b>1,25</b>", "4,5 → <b>3,2</b> / 2,4 → <b>1,6</b>",
         "Zıplamadan sonra çok çabuk düşüyordu. Şimdi kırmızı ~8,9, mavi ~4,5 birim atlar."],
        ["coyoteSuresi / tamponSuresi", "0,10 / 0,12", "0,18 / 0,15", "Mavi daha bağışlayıcı: havada faz değiştirip inmeye zaman tanır."],
    ], [4.6, 3.6, 3.6, 6.0]))
    s.append(Spacer(1, 3))
    s.append(tablo([
        ["Değişken", "Değer", "Neden böyle"],
        ["Enerji: azami / başlangıç", "8 / 5", "Başta 5 değişime yeter; kristal toplamadan bölümün yarısına gelinemez."],
        ["Faz maliyeti", "1 → 2 (+0,005/m)", "<b>Zamanla artan zorluk sayısı:</b> ileride her değişim daha pahalı, kristal daha değerli."],
        ["Kristal: enerji / sıçrama / kırılma hızı", "+3 / 13 / 11 → <b>9</b>", "Kristal bir değişimden fazlasını geri verir. Yerçekimi düşünce düşüş hızı azaldı, eşik 9'a indi."],
        ["Prizma kristali kilit", "2 sn", "Bedava faz değişimi ama tuş kilitli: yanlış yerde kırılırsa oyuncu fazına mahkûm kalır."],
        ["Ezilme kutusu oranı", "0,8", "Hayalet blok kenarına sürtünmek affedilir, içindeyken değiştirmek affedilmez."],
        ["Bölüm 4 mavi köprü", "9 → <b>11</b> birim", "Yerçekimi düşünce kırmızı ~8,9 birim atlayabildi; köprü maviye geçmeyi zorunlu kılsın diye uzatıldı."],
        ["Can / doğunca enerji", "5 / en az 2 değişimlik", "Oyuncu kontrol noktasında hiç değiştiremeyeceği bir yerde sıkışmaz."],
    ], [5.0, 3.6, 9.2]))

    # 3. Bağ tablosu ve şeması
    s.append(p("3. Bağ tablosu ve şeması (Oyun D)", bolum))
    s.append(tablo([
        ["Kaynak mekanik", "Ne üretir?", "Hedef mekanik", "Neyi değiştirir?", "Kapatılınca ne olur?"],
        ["<b>Tek tuş</b>", "faz (kırmızı/mavi)", "Çarpışma",
         "Her renkli blok ve kristalin <b>collider mı trigger mı</b> olduğu; hangi kristalin kırılabileceği",
         "renkli duvarlar hep aynı kalır; ilk kırmızı duvarda yol biter"],
        ["<b>Çarpışma</b>", "kırılan kristal (+3 enerji); kırılan prizma (zorla faz + 2 sn kilit)", "Tek tuş",
         "tuşa kaç kez basılabileceği; tuşun bir süre kullanılamaması",
         "enerji başlangıçtaki 5 ile sınırlı kalır; ~4 değişimden sonra tuş kullanılamaz"],
        ["<b>Tek tuş</b>", "faz", "Hareket",
         "hız, fren, zıplama yüksekliği, yerçekimi, coyote ve tampon (iki profil)",
         "iki faz aynı hissettirir; “uzun boşluk için kırmızı, dar yer için mavi” kararı kaybolur"],
    ], [2.3, 3.8, 2.2, 5.0, 4.5]))
    s.append(Spacer(1, 4))
    s.append(bag_semasi())

    # 4. Bağlantı testi
    s.append(p("4. Bağlantı testi", bolum))
    s.append(p("Oyunda <b>F1, F2, F3</b> tuşları üç mekaniği tek tek kapatır (ekranın üstünde AÇIK/KAPALI yazar). Her biri kapatıldığında olanlar:", govde))
    s.append(p("<b>Tek tuş kapalı (F1).</b> Boşluk bir şey yapmaz, oyuncu kırmızı fazda kalır. Bölüm 1'deki beş birim yüksekliğindeki kırmızı duvar katıdır ve "
               "aşılamaz; oyun ~8 metrede biter. Mavi bloklar hep hayalet, mavi kristaller hiç kırılamaz. Oyun kolaylaşmaz, "
               "<b>anlamını kaybeder</b>: geriye bir duvarın önünde yürümek kalır. <b>Bağ güçlü.</b>", govde))
    s.append(p("<b>Hareket hissi kapalı (F2).</b> İki faz da aynı düz profili kullanır: anında hızlanma ve durma, sabit yerçekimi, coyote ve tampon sıfır. "
               "Bölüm hâlâ teknik olarak geçilebilir ama faz seçimi yalnızca “hangi blok katı” sorusuna iner; kırmızıyla uzun atlayıp maviyle dar yerde "
               "durma kararı ortadan kalkar. Coyote ve tampon olmadığı için kenar zıplamaları ve kristal üstü sıçramalar sık kaçar, oyun daha sinir bozucu olur. "
               "Bağ bu yüzden üçü içinde <b>en zayıfı</b>; onu güçlendiren şey bölüm tasarımı (6 birimlik boşluklar ve 11 birimlik köprü fazların atlayış farkına göre ölçüldü).", govde))
    s.append(p("<b>Çarpışma tasarımı kapalı (F3).</b> Kristaller kırılmaz, sıradan blok gibi durur; enerji ve sıçrama gelmez, prizma kristali bedava değişim vermez. "
               "Bölümün tamamı yaklaşık 14 faz değişimi ister; başlangıçtaki 5 enerji, artan maliyetle Bölüm 3 civarında biter. Bundan sonra enerji ancak ölüp "
               "kontrol noktasından doğunca (iki değişimlik) gelir: oyun <b>“öl ve enerjini doldur”</b> oyununa dönüşür. <b>Bağ güçlü.</b>", govde))

    # 5. Oyun testi notu
    s.append(KeepTogether([
        p("5. Oyun testi notu", bolum),
        p("Geliştirme sırasında kendi testlerimde gördüklerim ve yaptığım düzeltmeler:", govde),
        tablo([
        ["Gözlem", "Düzeltme"],
        ["B ve D'de karakter hiç hareket etmiyor, Q profil değiştirmiyor (ekranda “Profil: -”).",
         "Sahne kurucu, profilleri yeni sahne açılmadan önce yüklüyordu; Unity yeni sahnede bunları bellekten boşaltıyor, sahneye boş bağlantı yazılıyordu. Profiller sahne açıldıktan sonra yükleniyor."],
        ["C'de top çok çabuk yavaşlıyor, az cam kırılıyor; sonra da neredeyse hiç durmuyor.", "Sürtünme 0,35 → 0,12 → 0,25, cam eşiği 5 → 3, durma hızı 0,4 → 1,5."],
        ["D'de zıpladıktan sonra çok çabuk düşülüyor.", "Yerçekimi ve düşüş çarpanı iki fazda düşürüldü; köprü 11 birime, kristal eşiği 9'a ayarlandı."],
        ], [7.6, 10.2]),
    ]))
    s.append(Spacer(1, 4))
    test = OYUN_TESTI
    bos = "_" * 95
    satirlar = [["Sınıftaki eşli test", ""],
                ["Test eden", test["test_eden"] or "_" * 40]]
    for i, g in enumerate(test["gozlemler"], 1):
        satirlar.append([f"Gözlem {i}", g or bos])
    satirlar.append(["Yapılan düzeltme", test["duzeltme"] or bos])
    s.append(tablo(satirlar, [3.2, 14.6]))

    # 6. Yapay zekâ beyanı
    s.append(KeepTogether([
        p("6. Yapay zekâ beyanı", bolum),
        p("<b>Araç:</b> Claude (Anthropic), Claude Code masaüstü uygulaması. Ayrıntılı çalışma notu: <i>Belge/YZ_Beyani.md</i>.", govde),
        tablo([
            ["Hangi iş", "Hangi dosyalar"],
            ["Yönergenin özetlenmesi ve D için üç fikir önerisi (Prizma, Sonar, Ağır Çekim); Prizma'yı ben seçtim.", "—"],
            ["Dört oyunun bütün betikleri (Türkçe açıklama satırlarıyla)", "Assets/Scripts/A, B, C, D"],
            ["Sahneleri, prefabları, profilleri ve D'nin bölüm haritasını kuran Editor betikleri; build betiği", "Assets/Scripts/Editor"],
            ["README, bu belgenin metni ve PDF'i üreten betik", "README.md, Belge/kaynak/belge_uret.py"],
            ["Hata ayıklama ve benim oyun testlerimden sonra istediğim ayar değişikliklerinin uygulanması", "Assets/Ayarlar, Assets/Prefabs, ilgili betikler"],
        ], [11.0, 6.8]),
        Spacer(1, 3),
        p("<b>Benim yaptıklarım:</b> fikir seçimi; dört oyunu Unity'de oynayarak test etmek ve ayar kararlarını vermek; build'i denemek; "
          "oynanış videosu, sınıftaki eşli test ve savunma.", govde),
    ]))
    return s


def sayfa_numarasi(canvas, doc):
    canvas.saveState()
    canvas.setFont("Arial", 7.5)
    canvas.setFillColor(colors.HexColor("#777777"))
    canvas.drawRightString(A4[0] - 1.6 * cm, 1.0 * cm, f"Arda Ufuk Konuk · H7 Tasarım Belgesi · {doc.page}")
    canvas.restoreState()


def main():
    doc = SimpleDocTemplate(CIKTI, pagesize=A4, leftMargin=1.6 * cm, rightMargin=1.6 * cm,
                            topMargin=1.4 * cm, bottomMargin=1.5 * cm,
                            title="H7 Tasarım Belgesi — Üç Mekanik, Dört Oyun", author="Arda Ufuk Konuk")
    doc.build(icerik(), onFirstPage=sayfa_numarasi, onLaterPages=sayfa_numarasi)
    print("Yazıldı:", CIKTI)


if __name__ == "__main__":
    main()
