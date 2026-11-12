using UnityEngine;
using UnityEngine.InputSystem;

// Masanın altındaki fırlatıcı: fareyle nişan alınır, sol tık basılı tutuldukça güç dolar, bırakınca top fırlar.
// Nişan oku, yönü ve gücü gösterir.
public class Firlatici : MonoBehaviour
{
    [SerializeField] MasaTopu topPrefabi;        // Fırlatılacak top
    [SerializeField] Transform nisanOku;         // Yönü ve gücü gösteren ok
    [SerializeField] float enAzHiz = 5f;         // Güç sıfırken fırlatma hızı
    [SerializeField] float azamiHiz = 16f;       // Güç doluyken fırlatma hızı
    [SerializeField] float dolmaSuresi = 1f;     // Gücün sıfırdan dolmasına kadar geçen süre (sn)
    [SerializeField] float enAzYukariEgim = 0.15f; // Nişanın en az bu kadar yukarı bakması gerekir (aşağı atış olmasın)

    float guc;          // 0 ile 1 arasında toplanan güç
    bool doluyor;       // Sol tık basılı ve güç doluyor mu
    Vector2 yon = Vector2.up; // Nişan yönü

    // Girdi Update'te okunur: nişan, güç doldurma ve bırakınca fırlatma.
    void Update()
    {
        MasaOyun oyun = MasaOyun.Ornek;
        bool hazir = oyun != null && oyun.AtisYapabilirMi();
        if (nisanOku != null) nisanOku.gameObject.SetActive(hazir);
        if (!hazir || Mouse.current == null || Camera.main == null) { doluyor = false; guc = 0f; return; }

        // Nişan: fırlatıcıdan farenin olduğu yere doğru.
        Vector2 fare = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 fark = fare - (Vector2)transform.position;
        if (fark.sqrMagnitude > 0.01f)
        {
            fark.Normalize();
            fark.y = Mathf.Max(fark.y, enAzYukariEgim);
            yon = fark.normalized;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame) { doluyor = true; guc = 0f; }
        if (doluyor && Mouse.current.leftButton.isPressed) guc = Mathf.Min(1f, guc + Time.deltaTime / dolmaSuresi);
        if (doluyor && Mouse.current.leftButton.wasReleasedThisFrame)
        {
            Firlat();
            doluyor = false;
            guc = 0f;
        }

        OkuGuncelle();
    }

    // Topu güç oranına göre hızla nişan yönünde fırlatır.
    void Firlat()
    {
        MasaOyun.Ornek.AtisYapildi();
        MasaTopu top = Instantiate(topPrefabi, transform.position + (Vector3)(yon * 0.6f), Quaternion.identity);
        top.Govde.linearVelocity = yon * Mathf.Lerp(enAzHiz, azamiHiz, guc);
    }

    // Oku nişan yönüne çevirir; boyu gücü gösterir.
    void OkuGuncelle()
    {
        if (nisanOku == null) return;
        float boy = 0.8f + 2.2f * guc;
        nisanOku.position = transform.position + (Vector3)(yon * (0.5f + boy / 2f));
        nisanOku.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(yon.y, yon.x) * Mathf.Rad2Deg - 90f);
        nisanOku.localScale = new Vector3(0.12f, boy, 1f);
    }
}
