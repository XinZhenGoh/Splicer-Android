using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SpriteSlicer2DDemoManager : MonoBehaviour
{
    public static int sliceCount = 0;
    public int slicenumber;
    public static int addCount = 0;
    List<SpriteSlicer2DSliceInfo> m_SlicedSpriteInfo = new List<SpriteSlicer2DSliceInfo>();
    TrailRenderer m_TrailRenderer;
    


    struct MousePosition
    {
        public Vector3 m_WorldPosition;
        public float m_Time;
    }

    public float m_MouseRecordInterval = 0.05f;
    public int m_MaxMousePositions = 5;
    public bool addScore = true;

    List<MousePosition> m_MousePositions = new List<MousePosition>();
    float m_MouseRecordTimer = 0.0f;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        m_TrailRenderer = GetComponentInChildren<TrailRenderer>();
    }

    /// <summary>
    /// Update this instance.
    /// </summary>
    void Update()
    {


        // Right mouse button - explode any sprite the we click on
        if (Input.GetMouseButtonDown(0) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftAlt)))
        {




            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = Camera.main.transform.position.z;
            RaycastHit2D rayCastResult = Physics2D.Raycast(mouseWorldPosition, new Vector3(0, 0, 0), 0.0f);


            if (rayCastResult.rigidbody)
            {

                if (Input.GetKey(KeyCode.LeftControl))
                {
                    

                    if (m_SlicedSpriteInfo.Count == 0)
                    {
                        // Couldn't cut for whatever reason, add some force anyway
                        rayCastResult.rigidbody.AddForce(new Vector2(0.0f, 400.0f));

                    }
                }
            }
        }

        // Left mouse button - hold and swipe to cut objects
        else if (Input.GetMouseButton(0))
        {


            bool mousePositionAdded = false;
            m_MouseRecordTimer -= Time.deltaTime;

            // Record the world position of the mouse every x seconds
            if (m_MouseRecordTimer <= 0.0f)
            {
                MousePosition newMousePosition = new MousePosition();
                newMousePosition.m_WorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newMousePosition.m_Time = Time.time;

                m_MousePositions.Add(newMousePosition);
                m_MouseRecordTimer = m_MouseRecordInterval;
                mousePositionAdded = true;

                // Remove the first recorded point if we've recorded too many
                if (m_MousePositions.Count > m_MaxMousePositions)
                {
                    m_MousePositions.RemoveAt(0);
                }
            }

            // Forget any positions that are too old to care about
            if (m_MousePositions.Count > 0 && (Time.time - m_MousePositions[0].m_Time) > m_MouseRecordInterval * m_MaxMousePositions)
            {
                m_MousePositions.RemoveAt(0);
            }

            // Go through all our recorded positions and slice any sprites that intersect them
            if (mousePositionAdded)
            {
                if (sliceCount < 3)
                {
                    for (int loop = 0; loop < m_MousePositions.Count - 1; loop++)
                    {
                        SpriteSlicer2D.SliceAllSprites(m_MousePositions[loop].m_WorldPosition, m_MousePositions[m_MousePositions.Count - 1].m_WorldPosition, true, ref m_SlicedSpriteInfo);

                        if (m_SlicedSpriteInfo.Count > 0)
                        {
                            // Add some force in the direction of the swipe so that stuff topples over rather than just being
                            // sliced but remaining stationary
                            for (int spriteIndex = 0; spriteIndex < m_SlicedSpriteInfo.Count; spriteIndex++)
                            {
                                for (int childSprite = 0; childSprite < m_SlicedSpriteInfo[spriteIndex].ChildObjects.Count; childSprite++)
                                {
                                    Vector2 sliceDirection = m_MousePositions[m_MousePositions.Count - 1].m_WorldPosition - m_MousePositions[loop].m_WorldPosition;
                                    sliceDirection.Normalize();
                                    //used to be > 500f
                                    m_SlicedSpriteInfo[spriteIndex].ChildObjects[childSprite].GetComponent<Rigidbody2D>().AddForce(sliceDirection * 100.0f);
                                    GetComponent<AudioSource>().Play();

                                }

                            }

                            m_MousePositions.Clear();
                            sliceCount += 1;
                            ScoreManager.count += 1;
                            Debug.Log("slicecount is " + sliceCount);
                            Debug.Log("count is " + ScoreManager.count);
                            if (sliceCount == 1)
                            {
                                nextLevelButton.played = 0;
                            }
                            break;

                        }
                    }
                    
                }
                if(sliceCount >= 3)
                {
                    m_MousePositions.Clear();
                    addCount += 1;
                }
                             
            }

            if (m_TrailRenderer)
            {
                Vector3 trailPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                trailPosition.z = -9.0f;
                m_TrailRenderer.transform.position = trailPosition;
            }
        }
        else
        {
            m_MousePositions.Clear();

        }

        // Sliced sprites sharing the same layer as standard Unity sprites could increase the draw call count as
        // the engine will have to keep swapping between rendering SlicedSprites and Unity Sprites.To avoid this, 
        // move the newly sliced sprites either forward or back along the z-axis after they are created
        for (int spriteIndex = 0; spriteIndex < m_SlicedSpriteInfo.Count; spriteIndex++)
        {
            for (int childSprite = 0; childSprite < m_SlicedSpriteInfo[spriteIndex].ChildObjects.Count; childSprite++)
            {
                Vector3 spritePosition = m_SlicedSpriteInfo[spriteIndex].ChildObjects[childSprite].transform.position;
                spritePosition.z = -1.0f;
                m_SlicedSpriteInfo[spriteIndex].ChildObjects[childSprite].transform.position = spritePosition;
            }
        }

       
        if (addScore)
        {
            for (int spriteIndex = 0; spriteIndex < m_SlicedSpriteInfo.Count; spriteIndex++)
            {
                for (int childSprite = 0; childSprite < m_SlicedSpriteInfo[spriteIndex].ChildObjects.Count; childSprite++)
                {
                    if (!m_SlicedSpriteInfo[spriteIndex].ChildObjects[childSprite].GetComponent<Rigidbody2D>().isKinematic)
                    {
                        m_SlicedSpriteInfo[spriteIndex].ChildObjects[childSprite].AddComponent<OutOfCamera>();

                    }
                }
            }

        }


        m_SlicedSpriteInfo.Clear();

    }


}
