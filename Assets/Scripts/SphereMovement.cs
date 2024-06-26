using UnityEngine;
using MethFunctions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class SphereMovement : MonoBehaviour
{
    // The speed of the sphere
    public float speed = 8f;
     // The Rigidbody component of the sphere
    private Rigidbody rb;
     // The speed of the sphere after being modified by the MethFunction
    // public float pepsiSpeed;
     // A boolean to check if the sphere is jumping
    public bool isJumping = true;
     // The GameObject that the sphere is jumping on
    private GameObject jumpTile;
    private GameObject normalTile;
    private GameObject glassTile;
    public List<GameObject> glassTiles = new List<GameObject>();
    public List<GameObject> fallingObstacles = new List<GameObject>();
    public List<GameObject> glassGroup1 = new List<GameObject>();
    public List<GameObject> fallingObstaclesGroup1 = new List<GameObject>();
    private bool hitGroup1 = false;
    public List<GameObject> glassGroup2 = new List<GameObject>();
    public List<GameObject> fallingObstaclesGroup2 = new List<GameObject>();
    private bool hitGroup2 = false;
    public List<GameObject> glassGroup3 = new List<GameObject>();
    public List<GameObject> fallingObstaclesGroup3 = new List<GameObject>();
    private bool hitGroup3 = false;
    public bool isNotFalling = true;
    private GameManager manager;
     // The z-position of the sphere when it collides with the jumpTile
    public float collisionZ = 0f;
    public List<GameObject> moverGroup1 = new List<GameObject>();
    public List<GameObject> movingObstaclesGroup1 = new List<GameObject>();
    public List<GameObject> moverGroup2 = new List<GameObject>();
    public List<GameObject> movingObstaclesGroup2 = new List<GameObject>();
    public List<GameObject> moverGroup3 = new List<GameObject>();
    public List<GameObject> movingObstaclesGroup3 = new List<GameObject>();
    private void Start()
    {
        // Get the Rigidbody component of the sphere
        rb = GetComponent<Rigidbody>();
        manager = GetComponent<GameManager>();
         // Create a new instance of the MethFunction
        MethFunction meth = new MethFunction();
         // Modify the speed of the sphere using the MethFunction
        // pepsiSpeed = meth.piEpsilon(speed);
        //rb.MovePosition(rb.position + Vector3.back * speed * 0.025f);
        rb.useGravity = false;
         // Freeze the rotation of the sphere
        //rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    IEnumerator waitBeforeLoading()
    {
        Debug.Log("Waiting...");
        yield return new WaitForSeconds(1f);
        Debug.Log("Done!");
        enabled = true;
    }

    IEnumerator WaitFor(float time)
    {
        yield return new WaitForSeconds(time);
        collisionZ = transform.position.z;
        isNotFalling = false;
    }

    public void SetSpeed(float newSpeed) {
        speed = newSpeed;
    }

    private void FixedUpdate()
    {
        //Debug.Log(manager.isGamePaused);
        //Debug.Log(manager.isGameOver);
        if (!manager.isGamePaused && !manager.isGameOver) {
            //Debug.Log("Running");
            // Create a Vector3 for the movement of the sphere
            Vector3 direction = Vector3.forward;
            Vector3 movement = direction.normalized * speed;
            // Move the sphere according to the movement Vector3
            rb.MovePosition(transform.position + movement * Time.fixedDeltaTime);
            if (!isNotFalling && !manager.isDeathDisabled && !manager.isGameOver) {
                rb.useGravity = true;
            }
            // If the sphere is jumping, call the Jump() method
            if (jumpTile != null && isJumping)
            {
                Jump(3.8f, jumpTile);
                rb.useGravity = false;
                FallingGlass();
            }
            else if (CheckIfObjectIsNotAboveAnyOtherObject() && !isJumping && !isNotFalling && !manager.isDeathDisabled && !manager.isGameOver)
            {
                exponential_falus();
            } else if (glassTiles.Count > 0) {
                FallingGlass();
            } else if (manager.levelConfig.startPortal && isJumping && rb.position.z < manager.levelConfig.startPos + speed) {
                rb.useGravity = false;
                isNotFalling = true;
                Jump(4f, new Vector3(0f, 0f, manager.levelConfig.startPos));
                FallingGlass();
            }
        } else {
            rb.velocity = Vector3.zero;
            //rb.position = new Vector3(0f, 0.5f, 0f);
        }
    }

    private void ActivateGlassTilesG1(GameObject currentGlassTile) {
        if (hitGroup1) return;
        GameObject currentGlassTileParent = currentGlassTile.transform.parent.gameObject;
        GameObject currentGlassTileNormal = currentGlassTileParent.transform.GetChild(1).gameObject;
        GameObject currentGlassTileActive = currentGlassTileParent.transform.GetChild(2).gameObject;
        currentGlassTileActive.SetActive(true);
        currentGlassTileNormal.SetActive(false);
        glassGroup1.Add(currentGlassTile);
        normalTile = null;
        glassTile = null;
        collisionZ = 0f;
        isJumping = false;
        jumpTile = null;
        List<GameObject> c_risers = GameObject.FindGameObjectsWithTag("Riser").ToList();
        if (c_risers.Any(c_riser => c_riser.transform.position.z == currentGlassTileParent.transform.position.z && c_riser.transform.position.x == currentGlassTileParent.transform.position.x)) {
        foreach (GameObject c_riser in c_risers) {
            if (c_riser.transform.position.z == currentGlassTileParent.transform.position.z && c_riser.transform.position.x == currentGlassTileParent.transform.position.x) {
                fallingObstaclesGroup1.Add(c_riser);
                break;
            }
        }
        }
        List<GameObject> c_glassTiles = GameObject.FindGameObjectsWithTag("GlassCollisionGroup1").ToList();
        foreach (GameObject c_glassTile in c_glassTiles) {
            if (!glassGroup1.Contains(c_glassTile)) {
                if (Mathf.Abs(c_glassTile.transform.position.z - currentGlassTileParent.transform.position.z) == 1f && Mathf.Abs(c_glassTile.transform.position.x - currentGlassTileParent.transform.position.x) == 1f) {
                    continue;
                }
                if (Mathf.Abs(c_glassTile.transform.position.z - currentGlassTileParent.transform.position.z) <= 1f && Mathf.Abs(c_glassTile.transform.position.x - currentGlassTileParent.transform.position.x) <= 1f) {
                    ActivateGlassTilesG1(c_glassTile);
                }
            }
        }
    }

    private void ActivateGlassTilesG2(GameObject currentGlassTile) {
        if (hitGroup2) return;
        GameObject currentGlassTileParent = currentGlassTile.transform.parent.gameObject;
        GameObject currentGlassTileNormal = currentGlassTileParent.transform.GetChild(1).gameObject;
        GameObject currentGlassTileActive = currentGlassTileParent.transform.GetChild(2).gameObject;
        currentGlassTileActive.SetActive(true);
        currentGlassTileNormal.SetActive(false);
        glassGroup2.Add(currentGlassTile);
        normalTile = null;
        glassTile = null;
        collisionZ = 0f;
        isJumping = false;
        jumpTile = null;
        List<GameObject> c_risers = GameObject.FindGameObjectsWithTag("Riser").ToList();
        if (c_risers.Any(c_riser => c_riser.transform.position.z == currentGlassTileParent.transform.position.z && c_riser.transform.position.x == currentGlassTileParent.transform.position.x)) {
        foreach (GameObject c_riser in c_risers) {
            if (c_riser.transform.position.z == currentGlassTileParent.transform.position.z && c_riser.transform.position.x == currentGlassTileParent.transform.position.x) {
                fallingObstaclesGroup2.Add(c_riser);
                break;
            }
        }
        }
        List<GameObject> c_glassTiles = GameObject.FindGameObjectsWithTag("GlassCollisionGroup2").ToList();
        foreach (GameObject c_glassTile in c_glassTiles) {
            if (!glassGroup2.Contains(c_glassTile)) {
                if (Mathf.Abs(c_glassTile.transform.position.z - currentGlassTileParent.transform.position.z) == 1f && Mathf.Abs(c_glassTile.transform.position.x - currentGlassTileParent.transform.position.x) == 1f) {
                    continue;
                }
                if (Mathf.Abs(c_glassTile.transform.position.z - currentGlassTileParent.transform.position.z) <= 1f && Mathf.Abs(c_glassTile.transform.position.x - currentGlassTileParent.transform.position.x) <= 1f) {
                    ActivateGlassTilesG2(c_glassTile);
                }
            }
        }
    }

    private void ActivateGlassTilesG3(GameObject currentGlassTile) {
        if (hitGroup3) return;
        GameObject currentGlassTileParent = currentGlassTile.transform.parent.gameObject;
        GameObject currentGlassTileNormal = currentGlassTileParent.transform.GetChild(1).gameObject;
        GameObject currentGlassTileActive = currentGlassTileParent.transform.GetChild(2).gameObject;
        currentGlassTileActive.SetActive(true);
        currentGlassTileNormal.SetActive(false);
        glassGroup3.Add(currentGlassTile);
        normalTile = null;
        glassTile = null;
        collisionZ = 0f;
        isJumping = false;
        jumpTile = null;
        List<GameObject> c_risers = GameObject.FindGameObjectsWithTag("Riser").ToList();
        if (c_risers.Any(c_riser => c_riser.transform.position.z == currentGlassTileParent.transform.position.z && c_riser.transform.position.x == currentGlassTileParent.transform.position.x)) {
        foreach (GameObject c_riser in c_risers) {
            if (c_riser.transform.position.z == currentGlassTileParent.transform.position.z && c_riser.transform.position.x == currentGlassTileParent.transform.position.x) {
                fallingObstaclesGroup3.Add(c_riser);
                break;
            }
        }
        }
        List<GameObject> c_glassTiles = GameObject.FindGameObjectsWithTag("GlassCollisionGroup3").ToList();
        foreach (GameObject c_glassTile in c_glassTiles) {
            if (!glassGroup3.Contains(c_glassTile)) {
                if (Mathf.Abs(c_glassTile.transform.position.z - currentGlassTileParent.transform.position.z) == 1f && Mathf.Abs(c_glassTile.transform.position.x - currentGlassTileParent.transform.position.x) == 1f) {
                    continue;
                }
                if (Mathf.Abs(c_glassTile.transform.position.z - currentGlassTileParent.transform.position.z) <= 1f && Mathf.Abs(c_glassTile.transform.position.x - currentGlassTileParent.transform.position.x) <= 1f) {
                    ActivateGlassTilesG3(c_glassTile);
                }
            }
        }
    }

    public void PrepareMoverTilesG1(GameObject currentMoverTile) {
        if (hitGroup1) return;
        GameObject currentMoverTileParent = currentMoverTile.transform.parent.gameObject;
        moverGroup1.Add(currentMoverTile);
        List<GameObject> c_risers = GameObject.FindGameObjectsWithTag("Riser").ToList();
        if (c_risers.Any(c_riser => c_riser.transform.position.z == currentMoverTileParent.transform.position.z && c_riser.transform.position.x == currentMoverTileParent.transform.position.x)) {
        foreach (GameObject c_riser in c_risers) {
            if (c_riser.transform.position.z == currentMoverTileParent.transform.position.z && c_riser.transform.position.x == currentMoverTileParent.transform.position.x) {
                movingObstaclesGroup1.Add(c_riser);
                break;
            }
        }
        }
        List<GameObject> c_moverTiles = GameObject.FindGameObjectsWithTag("MoverCollisionGroup1").ToList();
        foreach (GameObject c_moverTile in c_moverTiles) {
            if (!moverGroup1.Contains(c_moverTile)) {
                if (Mathf.Abs(c_moverTile.transform.position.z - currentMoverTileParent.transform.position.z) == 1f && Mathf.Abs(c_moverTile.transform.position.x - currentMoverTileParent.transform.position.x) == 1f) {
                    continue;
                }
                if (Mathf.Abs(c_moverTile.transform.position.z - currentMoverTileParent.transform.position.z) <= 1f && Mathf.Abs(c_moverTile.transform.position.x - currentMoverTileParent.transform.position.x) <= 1f) {
                    PrepareMoverTilesG1(c_moverTile);
                }
            }
        }
    }

    public void PrepareMoverTilesG2(GameObject currentMoverTile) {
        if (hitGroup2) return;
        GameObject currentMoverTileParent = currentMoverTile.transform.parent.gameObject;
        moverGroup2.Add(currentMoverTile);
        List<GameObject> c_risers = GameObject.FindGameObjectsWithTag("Riser").ToList();
        if (c_risers.Any(c_riser => c_riser.transform.position.z == currentMoverTileParent.transform.position.z && c_riser.transform.position.x == currentMoverTileParent.transform.position.x)) {
        foreach (GameObject c_riser in c_risers) {
            if (c_riser.transform.position.z == currentMoverTileParent.transform.position.z && c_riser.transform.position.x == currentMoverTileParent.transform.position.x) {
                movingObstaclesGroup2.Add(c_riser);
                break;
            }
        }
        }
        List<GameObject> c_moverTiles = GameObject.FindGameObjectsWithTag("MoverCollisionGroup2").ToList();
        foreach (GameObject c_moverTile in c_moverTiles) {
            if (!moverGroup2.Contains(c_moverTile)) {
                if (Mathf.Abs(c_moverTile.transform.position.z - currentMoverTileParent.transform.position.z) == 1f && Mathf.Abs(c_moverTile.transform.position.x - currentMoverTileParent.transform.position.x) == 1f) {
                    continue;
                }
                if (Mathf.Abs(c_moverTile.transform.position.z - currentMoverTileParent.transform.position.z) <= 1f && Mathf.Abs(c_moverTile.transform.position.x - currentMoverTileParent.transform.position.x) <= 1f) {
                    PrepareMoverTilesG2(c_moverTile);
                }
            }
        }
    }

    public void PrepareMoverTilesG3(GameObject currentMoverTile) {
        if (hitGroup3) return;
        GameObject currentMoverTileParent = currentMoverTile.transform.parent.gameObject;
        moverGroup3.Add(currentMoverTile);
        List<GameObject> c_risers = GameObject.FindGameObjectsWithTag("Riser").ToList();
        if (c_risers.Any(c_riser => c_riser.transform.position.z == currentMoverTileParent.transform.position.z && c_riser.transform.position.x == currentMoverTileParent.transform.position.x)) {
        foreach (GameObject c_riser in c_risers) {
            if (c_riser.transform.position.z == currentMoverTileParent.transform.position.z && c_riser.transform.position.x == currentMoverTileParent.transform.position.x) {
                movingObstaclesGroup3.Add(c_riser);
                break;
            }
        }
        }
        List<GameObject> c_moverTiles = GameObject.FindGameObjectsWithTag("MoverCollisionGroup3").ToList();
        foreach (GameObject c_moverTile in c_moverTiles) {
            if (!moverGroup3.Contains(c_moverTile)) {
                if (Mathf.Abs(c_moverTile.transform.position.z - currentMoverTileParent.transform.position.z) == 1f && Mathf.Abs(c_moverTile.transform.position.x - currentMoverTileParent.transform.position.x) == 1f) {
                    continue;
                }
                if (Mathf.Abs(c_moverTile.transform.position.z - currentMoverTileParent.transform.position.z) <= 1f && Mathf.Abs(c_moverTile.transform.position.x - currentMoverTileParent.transform.position.x) <= 1f) {
                    PrepareMoverTilesG3(c_moverTile);
                }
            }
        }
    }

    public void ActivateMoverTilesG1(Vector3 direction) {
        foreach (GameObject c_moverTile in moverGroup1) {
            if (c_moverTile == null) continue;
            GameObject currentMoverTileParent = c_moverTile.transform.parent.gameObject;
            currentMoverTileParent.transform.Translate(direction);
        }
        foreach (GameObject c_Riser in movingObstaclesGroup1) {
            if (c_Riser == null) continue;
            c_Riser.transform.Translate(direction);
            if (c_Riser.TryGetComponent<RiserAnim>(out RiserAnim riserAnim)) {
                foreach (Frame frame in riserAnim.animator.frames) {
                    frame.position = new Vector3(frame.position.x, frame.position.y, frame.position.z) + direction;
                }
                foreach (Frame frame in riserAnim.animator2.frames) {
                    frame.position = new Vector3(frame.position.x, frame.position.y, frame.position.z) + direction;
                }
            }
        }
        moverGroup1.Clear();
        movingObstaclesGroup1.Clear();
    }

    public void ActivateMoverTilesG2(Vector3 direction) {
        foreach (GameObject c_moverTile in moverGroup2) {
            if (c_moverTile == null) continue;
            GameObject currentMoverTileParent = c_moverTile.transform.parent.gameObject;
            currentMoverTileParent.transform.Translate(direction);
        }
        foreach (GameObject c_Riser in movingObstaclesGroup2) {
            if (c_Riser == null) continue;
            c_Riser.transform.Translate(direction);
            if (c_Riser.TryGetComponent<RiserAnim>(out RiserAnim riserAnim)) {
                foreach (Frame frame in riserAnim.animator.frames) {
                    frame.position = new Vector3(frame.position.x, frame.position.y, frame.position.z) + direction;
                }
                foreach (Frame frame in riserAnim.animator2.frames) {
                    frame.position = new Vector3(frame.position.x, frame.position.y, frame.position.z) + direction;
                }
            }
        }
        moverGroup2.Clear();
        movingObstaclesGroup2.Clear();
    }

    public void ActivateMoverTilesG3(Vector3 direction) {
        foreach (GameObject c_moverTile in moverGroup3) {
            if (c_moverTile == null) continue;
            GameObject currentMoverTileParent = c_moverTile.transform.parent.gameObject;
            currentMoverTileParent.transform.Translate(direction);
        }
        foreach (GameObject c_Riser in movingObstaclesGroup3) {
            if (c_Riser == null) continue;
            c_Riser.transform.Translate(direction);
            if (c_Riser.TryGetComponent<RiserAnim>(out RiserAnim riserAnim)) {
                foreach (Frame frame in riserAnim.animator.frames) {
                    frame.position = new Vector3(frame.position.x, frame.position.y, frame.position.z) + direction;
                }
                foreach (Frame frame in riserAnim.animator2.frames) {
                    frame.position = new Vector3(frame.position.x, frame.position.y, frame.position.z) + direction;
                }
            }
        }
        moverGroup3.Clear();
        movingObstaclesGroup3.Clear();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null && gameObject != null && manager != null) {
            isNotFalling = true;
            if (manager.levelConfig.startPortal && rb.position.z < manager.levelConfig.startPos + 4f) return;
            // Check if the sphere has collided with a jumpTile
            if (collision.gameObject.tag == "JumpCollision")
            {
                // Set isJumping to true to indicate that the sphere is jumping 
                isJumping = true; 
                // Disable gravity for the sphere 
                rb.useGravity = false; 
                // Set the jumpTile to the GameObject the sphere has collided with 
                jumpTile = collision.gameObject; 
                // Set the collisionZ to the z-position of the sphere 
                collisionZ = transform.position.z;
                hitGroup1 = false;
                hitGroup2 = false;
                hitGroup3 = false;
            }
            else if (collision.gameObject.tag == "NormalCollision" || collision.gameObject.tag == "NormalEndCollision")
            {
                normalTile = null;
                glassTile = null;
                collisionZ = 0f;
                isJumping = false;
                jumpTile = null;
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(0f, 0f, 0f);
                }
                hitGroup1 = false;
                hitGroup2 = false;
                hitGroup3 = false;
            } else if (collision.gameObject.tag == "GlassCollision") {
                GameObject glassTileParent = collision.gameObject.transform.parent.gameObject;
                GameObject glassTileNormal = glassTileParent.transform.GetChild(1).gameObject;
                GameObject glassTileActive = glassTileParent.transform.GetChild(2).gameObject;
                glassTileActive.SetActive(true);
                glassTileNormal.SetActive(false);
                normalTile = null;
                glassTile = null;
                collisionZ = 0f;
                isJumping = false;
                jumpTile = null;
                GameObject[] risers = GameObject.FindGameObjectsWithTag("Riser");
                foreach (GameObject riser in risers) {
                    if (riser.transform.position.z == glassTileParent.transform.position.z && riser.transform.position.x == glassTileParent.transform.position.x) {
                        fallingObstacles.Add(riser);
                    }
                }
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(0f, 0f, 0f);
                }
                hitGroup1 = false;
                hitGroup2 = false;
                hitGroup3 = false;
            } else if (collision.gameObject.tag == "GlassCollisionGroup1" && !hitGroup1) {
                ActivateGlassTilesG1(collision.gameObject);
                hitGroup1 = true;
                isJumping = false;
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                }
                hitGroup2 = false;
                hitGroup3 = false;
            } else if (collision.gameObject.tag == "GlassCollisionGroup2" && !hitGroup2) {
                ActivateGlassTilesG2(collision.gameObject);
                hitGroup2 = true;
                isJumping = false;
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                }
                hitGroup1 = false;
                hitGroup3 = false;
            } else if (collision.gameObject.tag == "GlassCollisionGroup3" && !hitGroup3) {
                ActivateGlassTilesG3(collision.gameObject);
                hitGroup3 = true;
                isJumping = false;
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                }
                hitGroup1 = false;
                hitGroup2 = false;
            } else if (collision.gameObject.tag == "MoverCollisionGroup1") {
                normalTile = null;
                glassTile = null;
                collisionZ = 0f;
                jumpTile = null;
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(0f, 0f, 0f);
                }
                PrepareMoverTilesG1(collision.gameObject);
                hitGroup1 = true;
                hitGroup2 = false;
                hitGroup3 = false;
            } else if (collision.gameObject.tag == "MoverCollisionGroup2") {
                normalTile = null;
                glassTile = null;
                collisionZ = 0f;
                jumpTile = null;
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(0f, 0f, 0f);
                }
                PrepareMoverTilesG2(collision.gameObject);
                hitGroup1 = false;
                hitGroup2 = true;
                hitGroup3 = false;
            } else if (collision.gameObject.tag == "MoverCollisionGroup3") {
                normalTile = null;
                glassTile = null;
                collisionZ = 0f;
                jumpTile = null;
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(0f, 0f, 0f);
                }
                PrepareMoverTilesG3(collision.gameObject);
                hitGroup1 = false;
                hitGroup2 = false;
                hitGroup3 = true;
            } else if (collision.gameObject.tag == "NormalEndCollision") {
                normalTile = null;
                glassTile = null;
                collisionZ = 0f;
                isNotFalling = true;
                jumpTile = null;
                if (!isJumping && rb != null) {
                    rb.velocity = new Vector3(0f, 0f, 0f);
                }
                hitGroup1 = false;
                hitGroup2 = false;
                hitGroup3 = false;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "NormalCollision")
        {
            normalTile = null;
            glassTile = null;
            collisionZ = transform.position.z;
        } else if (collision.gameObject.tag == "GlassCollision") {
            normalTile = null;
            glassTile = null;
            collisionZ = transform.position.z;
        } else {
            normalTile = null;
            glassTile = null;
            collisionZ = transform.position.z;
        }
        if (collision.gameObject.tag == "DiamondCollision") {
            rb.MovePosition(rb.position + Vector3.down * 5f * Time.fixedDeltaTime);
        }
        isNotFalling = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (manager.levelConfig.startPortal && rb.position.z < manager.levelConfig.startPos + 4f) {
            return;
        }
        if (collision.gameObject.tag == "NormalCollision")
        {
            normalTile = collision.gameObject;
            glassTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z;
            isNotFalling = false;
        } else if (collision.gameObject.tag == "JumpCollision") {
            isJumping = true;
            normalTile = null;
            glassTile = null;
        } else if (collision.gameObject.tag == "GlassCollision") {
            glassTile = collision.gameObject;
            glassTiles.Add(collision.gameObject);
            normalTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z;
            isNotFalling = false;
        } else if (collision.gameObject.tag == "MoverArrowCollision") {
            glassTile = collision.gameObject;
            normalTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z;
            isNotFalling = true;
        } else if (collision.gameObject.tag == "GlassCollisionGroup1") {
            //hitGroup1 = true;
            glassTile = collision.gameObject;
            if (transform.position.z - collision.gameObject.transform.position.z > 0.01f) {
                foreach (GameObject m_GlassTile in glassGroup1) {
                    if (m_GlassTile == null) continue;
                    if (!glassTiles.Contains(m_GlassTile)) {
                        glassTiles.Add(m_GlassTile);
                    }
                }
                foreach (GameObject m_Riser in fallingObstaclesGroup1) {
                    if (m_Riser == null) continue;
                    fallingObstacles.Add(m_Riser);
                }
            }
            normalTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z + 0.2f;
            hitGroup1 = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            isNotFalling = false;
        } else if (collision.gameObject.tag == "GlassCollisionGroup2") {
            //hitGroup2 = true;
            glassTile = collision.gameObject;
            if (transform.position.z - collision.gameObject.transform.position.z > 0.01f) {
                foreach (GameObject m_GlassTile in glassGroup2) {
                    if (m_GlassTile == null) continue;
                    if (!glassTiles.Contains(m_GlassTile)) {
                        glassTiles.Add(m_GlassTile);
                    }
                }
                foreach (GameObject m_Riser in fallingObstaclesGroup2) {
                    if (m_Riser == null) continue;
                    fallingObstacles.Add(m_Riser);
                }
            }
            normalTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z + 0.2f;
            hitGroup2 = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            isNotFalling = false;
        } else if (collision.gameObject.tag == "GlassCollisionGroup3") {
            //hitGroup3 = true;
            glassTile = collision.gameObject;
            if (transform.position.z - collision.gameObject.transform.position.z > 0.01f) {
                foreach (GameObject m_GlassTile in glassGroup3) {
                    if (m_GlassTile == null) continue;
                    if (!glassTiles.Contains(m_GlassTile)) {
                        glassTiles.Add(m_GlassTile);
                    }
                }
                foreach (GameObject m_Riser in fallingObstaclesGroup3) {
                    if (m_Riser == null) continue;
                    fallingObstacles.Add(m_Riser);
                }
            }
            normalTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z + 0.2f;
            hitGroup3 = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            isNotFalling = false;
        } else if (collision.gameObject.tag == "MoverCollisionGroup1") {
            glassTile = collision.gameObject;
            normalTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z + 0.2f;
            //hitGroup1 = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            isNotFalling = false;
        } else if (collision.gameObject.tag == "MoverCollisionGroup2") {
            glassTile = collision.gameObject;
            normalTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z + 0.2f;
            //hitGroup2 = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            isNotFalling = false;
        } else if (collision.gameObject.tag == "MoverCollisionGroup3") {
            glassTile = collision.gameObject;
            normalTile = collision.gameObject;
            rb.useGravity = false;
            collisionZ = collision.gameObject.transform.position.z + 0.2f;
            //hitGroup3 = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            isNotFalling = false;
        } else {
            isNotFalling = true;
            /*if (!isJumping && !(manager.levelConfig.startPortal && rb.position.z < manager.levelConfig.startPos + 4f)) {
                rb.MovePosition(rb.position + Vector3.down * 0.25f);
            } */
        }
    }

    bool CheckIfObjectIsNotAboveAnyOtherObject()
    {
        // Cast a ray downward from the object's position
        Ray ray = new Ray(transform.position, Vector3.down);

        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // If the raycast hits any collider, the object is above another object
            if ((hit.collider.gameObject.tag == "GlassCollisionGroup1" || hit.collider.gameObject.tag == "GlassCollisionGroup2" || hit.collider.gameObject.tag == "GlassCollisionGroup3") && hit.collider.gameObject.transform.position.y < -0.1f) {
                return true;
            }
            return false;
        }
        else
        {
            // If the raycast does not hit any collider, the object is not above any other object
            return true;
        }
    }

    public void FallingGlass() {
        foreach (GameObject m_glassTile in glassTiles) {
            if (m_glassTile == null) continue;
            GameObject glassTileParent = m_glassTile.transform.parent.gameObject;
            GameObject glassTileNormal = glassTileParent.transform.GetChild(1).gameObject;
            GameObject glassTileActive = glassTileParent.transform.GetChild(2).gameObject;
            GlassObject glassObject = glassTileParent.GetComponent<GlassObject>();
            if (glassObject != null) {
                glassTileParent.transform.position = new Vector3(glassTileParent.transform.position.x, glassTileParent.transform.position.y - glassObject.fallCoefficient, glassTileParent.transform.position.z);
                glassObject.fallCoefficient += 0.025f;
            }
        }
        foreach(GameObject riser in fallingObstacles) {
            if (riser == null) continue;
            GlassObject obstacleGlassObject = riser.GetComponent<GlassObject>();
            if (obstacleGlassObject != null) {
                float fallCoefficient = obstacleGlassObject.fallCoefficient / 2f;
                if (riser.TryGetComponent<RiserAnim>(out RiserAnim riserAnim)) {
                    foreach (Frame frame in riserAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in riserAnim.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<CrusherAnim>(out CrusherAnim crusherAnim)) {
                    foreach (Frame frame in crusherAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in crusherAnim.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<LeftHammerAnim>(out LeftHammerAnim hammerAnim)) {
                    foreach (Frame frame in hammerAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in hammerAnim.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<RightHammerAnim>(out RightHammerAnim hammerAnim2)) {
                    foreach (Frame frame in hammerAnim2.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in hammerAnim2.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<LeftHammerLargeAnim>(out LeftHammerLargeAnim hammerLargeAnim)) {
                    foreach (Frame frame in hammerLargeAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<RightHammerLargeAnim>(out RightHammerLargeAnim hammerLargeAnim2)) {
                    foreach (Frame frame in hammerLargeAnim2.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<LargeTreeAnim>(out LargeTreeAnim largeTreeAnim)) {
                    foreach (Frame frame in largeTreeAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in largeTreeAnim.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in largeTreeAnim.animator3.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in largeTreeAnim.animator4.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in largeTreeAnim.animator5.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in largeTreeAnim.animator6.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in largeTreeAnim.animator7.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in largeTreeAnim.animator8.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<MediumTreeAnim>(out MediumTreeAnim mediumTreeAnim)) {
                    foreach (Frame frame in mediumTreeAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in mediumTreeAnim.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in mediumTreeAnim.animator3.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in mediumTreeAnim.animator4.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in mediumTreeAnim.animator5.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<SmallTreeAnim>(out SmallTreeAnim smallTreeAnim)) {
                    foreach (Frame frame in smallTreeAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in smallTreeAnim.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in smallTreeAnim.animator3.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in smallTreeAnim.animator4.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in smallTreeAnim.animator5.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in smallTreeAnim.animator6.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in smallTreeAnim.animator7.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                } else if (riser.TryGetComponent<LaserAnim>(out LaserAnim laserAnim)) {
                    foreach (Frame frame in laserAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in laserAnim.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in laserAnim.animator3.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    GameObject laserBaseObject = riser.transform.Find("DeceBalus_Laser_Base").gameObject;
                    laserBaseObject.transform.position = new Vector3(laserBaseObject.transform.position.x, laserBaseObject.transform.position.y - fallCoefficient, laserBaseObject.transform.position.z);
                } else if (riser.TryGetComponent<FloaterAnim>(out FloaterAnim floaterAnim)) {
                    foreach (Frame frame in floaterAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    foreach (Frame frame in floaterAnim.animator2.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                    
                } else if (riser.TryGetComponent<SpotlightAnim>(out SpotlightAnim spotlightAnim)) {
                    foreach (Frame frame in spotlightAnim.animator.frames) {
                        frame.position = new Vector3(frame.position.x, frame.position.y - fallCoefficient, frame.position.z);
                    }
                }
                riser.transform.position = new Vector3(riser.transform.position.x, riser.transform.position.y - fallCoefficient, riser.transform.position.z);
                obstacleGlassObject.fallCoefficient += 0.025f;
            }
        }
    }

    private float jumpFunction(float x, float div_coeff) {
        return Mathf.Sqrt(2f) * speed * -x / div_coeff;
    }

    public void Jump(float distance, GameObject jumpTile) 
    { 
        if (jumpTile == null) return;
        Vector3 newPosition = new Vector3(jumpTile.transform.position.x, jumpTile.transform.position.y + 0.1f, jumpTile.transform.position.z);
        GameObject jumpTileParent = jumpTile.transform.parent.parent.gameObject;
        GameObject jumpTileNormal = jumpTileParent.transform.GetChild(0).gameObject;
        GameObject jumpTileActive = jumpTileParent.transform.GetChild(1).gameObject;
        jumpTile = jumpTileParent;
        /* float p = distance / 3.025f * (-1f); */
        float z = transform.position.z - jumpTile.transform.position.z;
        /*float h = 0f;
        float k = 6.12f;
        float jumpCalc = p * ((float)Math.Pow(z - h, 2)) + k; */
        float jumpCalc = jumpFunction(z - (distance / (2f)), distance / 4f);
        if (z < 0f) {
            jumpCalc = 0f;
        }
        isJumping = true;
        if (!jumpTileActive.activeSelf) {
            jumpTileActive.SetActive(true);
            jumpTileNormal.SetActive(false);
        }
        // Create a Vector3 for the upward movement of the sphere 
        Vector3 movement2 = Vector3.up * jumpCalc;
        try 
        { 
            if (z > 0.25f && z < 1.25f && isJumping) {
                newPosition = new Vector3(jumpTile.transform.position.x, jumpTileActive.transform.position.y + 0.1f, jumpTile.transform.position.z);
                jumpTileActive.transform.position = newPosition;
            }
            if (z > 2.25f && z < 3.25f && isJumping) {
                newPosition = new Vector3(jumpTile.transform.position.x, jumpTileActive.transform.position.y - 0.1f, jumpTile.transform.position.z);
                jumpTileActive.transform.position = newPosition;
            }
            if (isJumping)
            {
                rb.MovePosition(rb.position + movement2 * Time.fixedDeltaTime);
                //collisionZ += 0.075f;
            }
            // If the zDiff is greater than the distance, set the jumpTile to null and enable gravity for the sphere 
            if (z >= distance) 
            {
                newPosition = new Vector3(jumpTile.transform.position.x, 0f, jumpTile.transform.position.z);
                jumpTileActive.transform.position = newPosition;
                jumpTile = null;
                jumpCalc = 0f;
                isJumping = false;
                collisionZ = transform.position.z;
            } 
        } 
        catch (Exception e) 
        { 
            Debug.LogError("Error in Jump: " + e.Message); 
        } 
    }

    public void Jump(float distance, Vector3 startPosition) 
    { 
        /* float p = distance / 3.025f * (-1f); */
        float z = transform.position.z - startPosition.z;
        /*float h = 0f;
        float k = 6.12f;
        float jumpCalc = p * ((float)Math.Pow(z - h, 2)) + k; */
        float jumpCalc = jumpFunction(z - (distance / (2f)), distance / 4f);
        if (z < 0f) {
            jumpCalc = 0f;
        }
        isJumping = true;
        // Create a Vector3 for the upward movement of the sphere 
        Vector3 movement2 = Vector3.up * jumpCalc;
        try 
        {
            if (isJumping)
            {
                rb.MovePosition(rb.position + movement2 * Time.fixedDeltaTime);
                //collisionZ += 0.075f;
            }
            // If the zDiff is greater than the distance, set the jumpTile to null and enable gravity for the sphere 
            if (z >= distance) 
            {
                //jumpCalc = 0f;
                isJumping = false;
                collisionZ = transform.position.z;
            } 
        } 
        catch (Exception e) 
        { 
            Debug.LogError("Error in Jump: " + e.Message); 
        } 
    }
    public void exponential_falus() 
    {
        if (!manager.isGameOver) {
            float z = transform.position.z - collisionZ;
            float downY = jumpFunction(z + 2f, 1f) * -1f;
            Vector3 downVector = Vector3.down * downY;
            rb.MovePosition(rb.position + downVector * Time.fixedDeltaTime);
            hitGroup1 = false;
            hitGroup2 = false;
            hitGroup3 = false;
        }
    }

    public void ClearFallingObstacles() {
        glassTiles.Clear();
        fallingObstacles.Clear();
        glassGroup1.Clear();
        fallingObstaclesGroup1.Clear();
        glassGroup2.Clear();
        fallingObstaclesGroup2.Clear();
        glassGroup3.Clear();
        fallingObstaclesGroup3.Clear();
        moverGroup1.Clear();
        movingObstaclesGroup1.Clear();
        moverGroup2.Clear();
        movingObstaclesGroup2.Clear();
        moverGroup3.Clear();
        movingObstaclesGroup3.Clear();
    }
}