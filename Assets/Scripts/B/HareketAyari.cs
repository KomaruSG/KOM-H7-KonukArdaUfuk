using UnityEngine;

// Bir hareket profilinin bütün sayıları. Sahnede değil, ayrı bir varlık (ScriptableObject) olarak durur.
// Böylece aynı karakter iki farklı profil arasında geçiş yapabilir ve değerler Play modunda değiştirilip kalıcı olur.
[CreateAssetMenu(menuName = "KOM/Hareket Ayarı", fileName = "HareketAyari")]
public class HareketAyari : ScriptableObject
{
    public string ad = "Profil";        // Ekranda görünen profil adı
    public Color renk = Color.white;    // Bu profil seçiliyken karakterin rengi

    [Header("Yatay hareket")]
    public float azamiHiz = 8f;         // Ulaşılabilecek en yüksek yatay hız (birim/sn)
    public float ivme = 40f;            // Tuşa basınca hızın artış oranı (birim/sn²): büyük = anında, küçük = ağır kalkış
    public float fren = 50f;            // Tuş bırakılınca ya da ters yöne basınca hızın azalma oranı: küçük = kayar
    [Range(0f, 1f)] public float havaKontrolu = 0.8f; // Havadayken ivme ve frenin ne kadarı geçerli (1 = yerdeki gibi)

    [Header("Zıplama")]
    public float ziplamaYuksekligi = 3f;   // Tuş basılı tutulursa çıkılan yükseklik (birim)
    public float yercekimiOlcegi = 3f;     // Yerçekimi çarpanı: büyük = daha hızlı ve sert zıplama
    public float dususCarpani = 1.6f;      // Düşerken yerçekimine eklenen çarpan: iniş çıkıştan hızlı olur, zıplama "ağırlaşır"
    [Range(0f, 1f)] public float kisaZiplamaCarpani = 0.5f; // Tuş erken bırakılınca yukarı hızın çarpıldığı değer (değişken yükseklik)
    public float azamiDususHizi = 20f;     // Düşüş hızının üst sınırı

    [Header("Bağışlayıcılık")]
    public float coyoteSuresi = 0.1f;   // Kenardan düştükten sonra hâlâ zıplanabilen süre (sn)
    public float tamponSuresi = 0.12f;  // Yere değmeden önce basılan zıplamanın hatırlandığı süre (sn)
}
