using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetoScript : MonoBehaviour
{
    [SerializeField]
    GameObject Boat;

    [SerializeField]
    GameObject FishingRod;

    [SerializeField]
    GameObject Hitbox;

    [SerializeField]
    GameObject HitboxPivotPoint;

    [SerializeField]
    GameObject FishingRodTip;

    [SerializeField]
    GameObject StringEnd;

    float LastClick = 0.0f;
    private LineRenderer liner;
    private Vector3 GrapplePoint;
    private float TransitionToGrapple;
    public LayerMask GrappleSites;
    public bool SceneTransition = false;
    public bool IsInteractive = true;

    [Header("Sprite Costumes")]
    public Sprite DefaultSprite;
    public Sprite SakabamSprite;

    public bool ScreenShowing = false;

    void Awake() {
        if (IsInteractive) {
            liner = FishingRod.GetComponent<LineRenderer>();
        }
    }

    void Start() {
    }

    // Update is called once per frame
    void Update() {
        string BoatName = Boat.GetComponent<SpriteRenderer>().sprite.name;
        Vector3 Offset = new Vector3(0, 0, 0);
        if (BoatName == "Ship-Sailboat") {
            Offset.x = 0.9f;
            Offset.y = 0.9f;
        }
        else if (BoatName == "Ship-Baguette") {
            Offset.y = 0.1f;
        }
        else if (BoatName == "Ship-Yacht") {
            Offset.y = 0.1f;
        }
        else if (BoatName == "Ship-sakabambabis") {
            Offset.y = 0.55f;
        }
        else if (BoatName == "Ship-AircraftCarrier") {
            Offset.x = 0.52f;
            Offset.y = 0.36f;
        }

        HitboxPivotPoint.transform.position = Boat.transform.position + new Vector3(0, 0.2f, 0);
        Hitbox.transform.localPosition = new Vector3(
            Offset.x,
            Offset.y,
            0
        );
        HitboxPivotPoint.transform.eulerAngles = Boat.transform.eulerAngles;

        if (IsInteractive) {
            FishingRod.transform.position = Hitbox.transform.position;
            FishingRod.transform.eulerAngles = Hitbox.transform.eulerAngles;

            float TimeSinceLastClick = Time.time - LastClick;
            DrawRope();

            if (Input.GetMouseButtonDown(0) && TimeSinceLastClick > 0.5f && !ScreenShowing) {
                if (StringEnd.GetComponent<RopeEnd>().Launching) {
                    Retract();
                } else {
                    GoFish();
                }
            }
            if (LastClick == 0.0f) {
                StopGrapple();
            }

            // Ensure Boat isn't Out of Bounds
            if (!SceneTransition) {
                if (Boat.transform.position.x > 1f) {
                    Vector2 Force = new Vector2(-20f, 0);
                    Boat.GetComponent<Rigidbody2D>().AddForce(Force);
                }
                else if (Boat.transform.position.x < -1.25f) {
                    Vector2 Force = new Vector2(20f, 0);
                    Boat.GetComponent<Rigidbody2D>().AddForce(Force);
                }
            }
        }
        
        SpriteRenderer SpriteRender = GetComponent<SpriteRenderer>();
        if (Boat.GetComponent<SpriteRenderer>().sprite.name == "Ship-sakabambabis") {
            SpriteRender.sprite = SakabamSprite;
        } else {
            SpriteRender.sprite = DefaultSprite;
        }
    }

    // fishing mechanisms
    void GoFish() {
        Debug.Log("Launching ...");
        Vector3 OriginalPos = StringEnd.transform.position;
        StringEnd.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        StringEnd.transform.position = Camera.main.ScreenToWorldPoint(StringEnd.transform.position);
        if (StringEnd.transform.position.x < 1.25f) {
            StringEnd.transform.position = OriginalPos;
            return;
        }
        StartGrapple();
        LastClick = Time.time;
        TransitionToGrapple = 0;

        // Boat Fysiks
        Vector2 Force = new Vector2(0, 140f);
        Boat.GetComponent<Rigidbody2D>().AddForce(Force);

        StringEnd.GetComponent<RopeEnd>().GenerateFish();
    }

    void Retract() {
        StringEnd.GetComponent<RopeEnd>().Launching = false;
    }

    // grappler rahhhhhh
    void StartGrapple() {
        liner.positionCount = 2;
    }

    void DrawRope() {
        if (StringEnd.transform.position.z == 2763) return;
        if (liner.positionCount == 0) return;
        liner.SetPosition(0, FishingRodTip.transform.position);
        Vector3 TransitionPoint = new Vector3(
            Mathf.Lerp(FishingRodTip.transform.position.x, StringEnd.transform.position.x, TransitionToGrapple),
            Mathf.Lerp(FishingRodTip.transform.position.y, StringEnd.transform.position.y, TransitionToGrapple),
            0
        );
        liner.SetPosition(1, TransitionPoint);
        if (StringEnd.GetComponent<RopeEnd>().Launching) {
            TransitionToGrapple += (1f-TransitionToGrapple)/25f;
        } else {
            TransitionToGrapple -= TransitionToGrapple/15f;
        }
    }

    void StopGrapple() {
        liner.positionCount = 0;
    }
}
