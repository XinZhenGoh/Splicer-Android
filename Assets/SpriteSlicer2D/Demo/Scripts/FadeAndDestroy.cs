using UnityEngine;
using System.Collections;

/// <summary>
/// Helper component to fade a gameobject out and then 
/// destroy it once invisible
/// </summary>
[RequireComponent(typeof(SlicedSprite))]
public class FadeAndDestroy : MonoBehaviour 
{
	public float m_FadeDelay = 1.0f;
	public float m_FadeTime = 1.0f;
    public bool m_WaitUntilStationary = false;
    public float m_StationaryVelocity = 0.2f;

	SlicedSprite m_SlicedSprite;
    Rigidbody2D m_RigidBody;
    Material m_Material;
    Color m_InitialColor;    
	float m_Timer;    
		
    /// <summary>
    /// Called on script construction
    /// </summary>
	void Awake()
	{        
		m_SlicedSprite = GetComponent<SlicedSprite>();
        m_RigidBody = m_SlicedSprite.GetComponent<Rigidbody2D>();
        m_Material = m_SlicedSprite.GetComponent<Renderer>().material;
		m_InitialColor = m_Material.color;
	}
		
    /// <summary>
    /// Update this instance
    /// </summary>
	void Update () 
	{
        if (!m_WaitUntilStationary || m_RigidBody.velocity.sqrMagnitude < (m_StationaryVelocity * m_StationaryVelocity))
        {
            m_Timer += Time.deltaTime;

            if (m_FadeTime > 0)
            {
                Color newColor = m_InitialColor;
                newColor.a = 1.0f - Mathf.Clamp01((m_Timer - m_FadeDelay) / m_FadeTime);
                m_SlicedSprite.GetComponent<Renderer>().material.color = newColor;
            }

            if ((m_Timer - m_FadeDelay) >= m_FadeTime)
            {
                Destroy(this.gameObject);
            }
        }		
	}
}
