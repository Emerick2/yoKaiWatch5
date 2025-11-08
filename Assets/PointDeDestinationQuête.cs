using UnityEngine;
using UnityEngine.UI;

public class PointDeDestinationQuête : MonoBehaviour{
    public GameObject laFlèche;
    public GameObject laCible;
    public Camera cameraMiniMap;
    private Renderer flècheRenderer;
    private Renderer cibleRenderer;
    public float marge = 0.05f;
    public Vector3[] coinsVisibles = new Vector3[4];
    public RectTransform rondDeZoom;
    public RectTransform zoomHaut;
    public RectTransform zoomBas;
    private int zoomMin = 5;
    private int zoomMax = 30;
    private float saut = 0.1f;


    void Start(){
        flècheRenderer = laFlèche.GetComponent<Renderer>();
        cibleRenderer = laCible.GetComponent<Renderer>();
        FaireLeZoom();
    }

    void Update(){
        if (!EstVisibleParLaCamera(cibleRenderer)){
            //BonneRotationDeLaFlèche();
            PositionFlèchePourLaCamera();
            laFlèche.SetActive(true);
        } else {
            laFlèche.SetActive(false);
        }

        
        if ((Input.GetButton("+") || Input.GetButton("R")) && cameraMiniMap.orthographicSize < zoomMax){
            cameraMiniMap.orthographicSize+=saut;
            FaireLeZoom();
        } else if ((Input.GetButton("-") || Input.GetButton("L"))&& cameraMiniMap.orthographicSize > zoomMin){
            cameraMiniMap.orthographicSize-=saut;
            FaireLeZoom();
        }
    }

    void FaireLeZoom(){
        float hauteurMax = zoomHaut.position.y - zoomBas.position.y;
        float plageZoom = zoomMax - zoomMin;
        float valeurNormalisée = (cameraMiniMap.orthographicSize - zoomMin) / plageZoom;
        float nouvellePosY = (valeurNormalisée * hauteurMax) + zoomBas.position.y;

        Vector3 pos = rondDeZoom.position;
        pos.y = nouvellePosY;
        rondDeZoom.position = pos;
    }

    public bool EstVisibleParLaCamera(Renderer cible){
        Plane[] plansFrustum = GeometryUtility.CalculateFrustumPlanes(cameraMiniMap);
        bool estDansFrustum = GeometryUtility.TestPlanesAABB(plansFrustum, cible.bounds);
        return estDansFrustum;
    }
    void PositionFlèchePourLaCamera(){
        Vector3 posCam = cameraMiniMap.transform.position;
        Vector3 posCible = laCible.transform.position;

        posCam.y = 0;
        posCible.y = 0;

        Vector3 direction = (posCible - posCam).normalized;

        float demiLargeur = cameraMiniMap.orthographicSize * cameraMiniMap.aspect - marge;
        float demiHauteur = cameraMiniMap.orthographicSize - marge;

        float tX = Mathf.Abs(direction.x) > 0.0001f ? demiLargeur / Mathf.Abs(direction.x) : float.MaxValue;
        float tZ = Mathf.Abs(direction.z) > 0.0001f ? demiHauteur / Mathf.Abs(direction.z) : float.MaxValue;
        float t = Mathf.Min(tX, tZ);

        Vector3 nouvellePosition = posCam + direction * t;
        nouvellePosition.y = laFlèche.transform.position.y;

        laFlèche.transform.position = nouvellePosition;

        Vector3 dirXZ = posCible - laFlèche.transform.position;
        dirXZ.y = 0;
        if (dirXZ.sqrMagnitude > 0.001f){
            laFlèche.transform.rotation = Quaternion.LookRotation(dirXZ, Vector3.up) * Quaternion.Euler(90, 0, 0);
        }
    }
}
