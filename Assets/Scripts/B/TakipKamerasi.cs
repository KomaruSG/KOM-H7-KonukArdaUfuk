using UnityEngine;

// Kamerayı hedefin arkasından yumuşakça takip ettirir.
// LateUpdate'te çalışır: karakter o karede hareketini bitirdikten sonra kamera yetişir, titreme olmaz.
public class TakipKamerasi : MonoBehaviour
{
    [SerializeField] Transform hedef;                         // Takip edilecek nesne
    [SerializeField] Vector2 kayma = new Vector2(3f, 1.5f);  // Hedefe göre kameranın duracağı yer (ileriyi daha çok göster)
    [SerializeField] float yumusaklik = 0.18f;                // Takip gecikmesi (sn): büyük = daha yumuşak ama geç
    [SerializeField] float enAltY = -1f;                      // Kameranın inebileceği en alt y (düşerken boşluğu göstermesin)

    Vector3 hiz; // SmoothDamp'in kullandığı iç hız

    // Hedefe doğru yumuşak hareket.
    void LateUpdate()
    {
        if (hedef == null) return;
        Vector3 istenen = new Vector3(hedef.position.x + kayma.x, Mathf.Max(enAltY, hedef.position.y + kayma.y), transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, istenen, ref hiz, yumusaklik);
    }
}
