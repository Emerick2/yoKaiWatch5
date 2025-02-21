using System.Collections;
using UnityEngine;
using System.Linq;
 
public class tousEstDéplasable : MonoBehaviour{ 
    [Header("Paramètres spécifique")]
    [Tooltip("image Si Doit Flip.")]
    public SpriteRenderer imageSiDoitFlip;
    public bool uneFoisArrivéJeNeParPlus;
    private bool STOP;
    public bool pauseQuandJArriveSurLaPosition;
    public float tempsDAttante=0f;
    private bool coroutineDéjaActivé = false;
    [Space]
    [Header("Pour que le personnage soit autonome")]
    [Tooltip("avence et tombe si il y à un troue si il y à un mur il font demis tour")]
    public bool goumba;
    [Tooltip("si il y à un troue ou un mur il font demis tour")]
    public bool koopa;
    public bool murBD;
    public bool murBG;
    public LayerMask solLayer;
    public Transform BD_BH;
    public Transform BD_BB;
    public Transform BG_BH;
    public Transform BG_BB;
    public bool destroyQuandIlToucheUnMur;
    public bool flipAutoriser;
    public bool parEtNeRevientPas;
    public bool droitePourParEtNeRevientPas;
    public bool doitFaireDemiTour = true;
    [Space]
    [Header("Obligatoire")]
    [Range(0,50)]
    public float vitesse;
    public Transform[] pointDePassage;
    [Range(0,100)]

    private Transform cible;
    private int pointDeDestination = 0;
    public bool leJoueurEstAsserProcheDeMoi=true;//25
    private Transform positionDuJoueur;

    public bool nousSommeEnCombat;

    void Awake(){
        if (pointDePassage != null && pointDePassage.Length > 0){
            cible = pointDePassage[0];
        }
        if (koopa){
            goumba = true;
        }
        if (nousSommeEnCombat){
            positionDuJoueur = GameObject.FindWithTag("Player").transform;
        }
        StartCoroutine(regardeSiIlVaAGaucheOuADroite());
    }

    IEnumerator regardeSiIlVaAGaucheOuADroite(){
        float positionActuelAntérieur = transform.position.x;
        while (true){
            yield return new WaitForSeconds(0.03f);
            if (imageSiDoitFlip != null && doitFaireDemiTour && !parEtNeRevientPas){
                if (transform.position.x > positionActuelAntérieur){
                    //va à droite
                    imageSiDoitFlip.flipX = false;
                }
                else if (transform.position.x < positionActuelAntérieur){
                    //va à gauche
                    imageSiDoitFlip.flipX = true;
                }
                positionActuelAntérieur = transform.position.x;
            }
        }
    }


    void Update(){
        if (!nousSommeEnCombat &&(positionDuJoueur.position.x-25 < transform.position.x && positionDuJoueur.position.x+25 > transform.position.x) && (positionDuJoueur.position.y-15 < transform.position.y && positionDuJoueur.position.y+15 > transform.position.y)){
            leJoueurEstAsserProcheDeMoi=true;
        }
        if (leJoueurEstAsserProcheDeMoi){
            if (BD_BB != null && BD_BH != null && BG_BB != null && BG_BH != null){ 
                murBD = Physics2D.OverlapArea(BD_BB.position, BD_BH.position, solLayer) != null;
                murBG = Physics2D.OverlapArea(BG_BB.position, BG_BH.position, solLayer) != null;
            }
            if(!parEtNeRevientPas && pointDePassage != null || STOP == true){
                Vector3 dir = cible.position - transform.position;
                transform.Translate(dir.normalized * vitesse * Time.deltaTime, Space.World);

                if(Vector3.Distance(transform.position, cible.position) < 0.3f && doitFaireDemiTour){
                    pointDeDestination = (pointDeDestination + 1) % pointDePassage.Length;
                    cible = pointDePassage[pointDeDestination];/*
                    if (imageSiDoitFlip != null && flipAutoriser){
                        imageSiDoitFlip.flipX = !imageSiDoitFlip.flipX;
                    }*/
                    if (uneFoisArrivéJeNeParPlus){
                        STOP=true;
                    }
                    if (pauseQuandJArriveSurLaPosition && coroutineDéjaActivé== false){
                        coroutineDéjaActivé=true;
                        StartCoroutine(pausePosition());
                    }
                }
            }

            if (koopa){
                if (!murBD && droitePourParEtNeRevientPas){
                    droitePourParEtNeRevientPas = false;
                    if (imageSiDoitFlip!=null){
                        imageSiDoitFlip.flipX = !imageSiDoitFlip.flipX;
                    }
                    Vector2 direction = droitePourParEtNeRevientPas ? Vector2.right : Vector2.left;
                    transform.Translate(direction * vitesse * Time.deltaTime);
                }

                if (!murBG && !droitePourParEtNeRevientPas){
                    droitePourParEtNeRevientPas = true;
                    if (imageSiDoitFlip != null){
                        imageSiDoitFlip.flipX = !imageSiDoitFlip.flipX;
                    }
                    Vector2 direction = droitePourParEtNeRevientPas ? Vector2.right : Vector2.left;
                    transform.Translate(direction * vitesse * Time.deltaTime);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision){
        if (leJoueurEstAsserProcheDeMoi){
            if(destroyQuandIlToucheUnMur){
                Destroy(gameObject);
            }
        }
    }
    
    IEnumerator pausePosition(){
        yield return new WaitForSeconds(tempsDAttante);
        STOP=false;
    }

    void OnCollisionStay2D(Collision2D maCollision){
        if (leJoueurEstAsserProcheDeMoi){
            foreach (ContactPoint2D contact in maCollision.contacts) {
	            Vector2 normal = contact.normal;
                if (normal.x>0){
                    droitePourParEtNeRevientPas = true;
                    if (imageSiDoitFlip != null){imageSiDoitFlip.flipX = false; print("retourn image");}
                }
                if (normal.x<0){
                    droitePourParEtNeRevientPas = false;
                    if (imageSiDoitFlip != null){imageSiDoitFlip.flipX = true;}
                }
            }
            Vector2 direction = droitePourParEtNeRevientPas ? Vector2.right : Vector2.left;
            transform.Translate(direction * vitesse * Time.deltaTime);
        }
    }
}