//#define TK2D_SLICING_ENABLED

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper class to pass information about sliced sprites to the calling method 
/// </summary>
public class SpriteSlicer2DSliceInfo
{
	public GameObject SlicedObject 				{ get; set; }
	public Vector2 SliceEnterWorldPosition 		{ get; set; }
	public Vector2 SliceExitWorldPosition 		{ get; set; }
	public List<GameObject> ChildObjects 		{ get { return m_ChildObjects; } set { m_ChildObjects = value; } }

    List<GameObject> m_ChildObjects = new List<GameObject>();
}


/// <summary>
/// Main sprite slicer class, provides static functions to slice sprites
/// </summary>
public static class SpriteSlicer2D 
{
	public static bool DebugLoggingEnabled { get { return s_DebugLoggingEnabled; } set { s_DebugLoggingEnabled = value; } }

	// Enable or disable debug logging
	static bool s_DebugLoggingEnabled = false;

	// Child sprite vertices may not be centered around the pivot point of the Unity gameobject, causing unexpected
	// behaviour if you wish to manually rotate or move them after slicing. At the cost of some extra processing, you
	// can enable this flag in order to correctly position them. Set to false by default, which assumes that the user will
	// just let the physics system handle movement.
	public static bool s_CentreChildSprites = false;

	// Helper to allow ExplodeSprite to slice sub parts that violate the same sprite id or game object rules
	static List<SpriteSlicer2DSliceInfo> s_SubSlicesCont = new List<SpriteSlicer2DSliceInfo>();

	// Static lists to help concave polygon slicing
	static List<LinkedPolygonPoint> s_ConcavePolygonPoints = new List<LinkedPolygonPoint>();
	static List<LinkedPolygonPoint> s_ConcavePolygonIntersectionPoints = new List<LinkedPolygonPoint>();
	static List<Polygon> s_ConcaveSlicePolygonResults = new List<Polygon>();
	
	// Vector sorting function
	static VectorComparer s_VectorComparer = new VectorComparer();


	/// <summary>
	/// Slices any sprites that are intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint)
	{
		LayerMask layerMask = -1;
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, true, -1, ref slicedObjectInfo, layerMask, null);
	}
	
	/// <summary>
	/// Slices any sprites that are intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="layerMask">Layermask to use in raycast operations.</param>
	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, LayerMask layerMask)
	{
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, true, -1, ref slicedObjectInfo, layerMask, null);
	}

	/// <summary>
	/// Slices any sprites that are intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="layerMask">Layermask to use in raycast operations.</param>
	/// <param name="tag">Only sprites with the given tag can be cut.</param>
	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, string tag)
	{
		LayerMask layerMask = -1;
		List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, true, -1, ref slicedObjectInfo, layerMask, tag);
	}
	
	/// <summary>
	/// Slices any sprites that are intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="destroySlicedObjects">Controls whether the parent objects are automatically destroyed. Set to false if you need to perform additional processing on them after slicing</param>
	/// <param name="slicedObjectInfo">A list of SpriteSlicer2DSliceInfo that will be fill out with details about slice locations, slcied objects, and created child objects.</param>
	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
	}

	/// <summary>
	/// Slices any sprites that are intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="destroySlicedObjects">Controls whether the parent objects are automatically destroyed. Set to false if you need to perform additional processing on them after slicing</param>
	/// <param name="slicedObjectInfo">A list of SpriteSlicer2DSliceInfo that will be fill out with details about slice locations, slcied objects, and created child objects.</param>
	/// <param name="tag">Only sprites with the given tag can be cut.</param>
	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, string tag)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, tag);
	}
	
	/// <summary>
	/// Slices any sprites that are intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="destroySlicedObjects">Controls whether the parent objects are automatically destroyed. Set to false if you need to perform additional processing on them after slicing</param>
	/// <param name="slicedObjectInfo">A list of SpriteSlicer2DSliceInfo that will be fill out with details about slice locations, slcied objects, and created child objects.</param>
	/// <param name="layerMask">Layermask to use in raycast operations.</param>
	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask)
	{
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
	}
	
	/// <summary>
	/// Slices any sprites that are intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="destroySlicedObjects">Controls whether the parent objects are automatically destroyed. Set to false if you need to perform additional processing on them after slicing</param>
	/// <param name="maxCutDepth">The maximum number of times that any sprite can be subdivided</param>
	/// <param name="slicedObjectInfo">A list of SpriteSlicer2DSliceInfo that will be fill out with details about slice locations, slcied objects, and created child objects.</param>
	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		LayerMask layerMask = -1;
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
	}
	
	/// <summary>
	/// Slices any sprites that are intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="destroySlicedObjects">Controls whether the parent objects are automatically destroyed. Set to false if you need to perform additional processing on them after slicing</param>
	/// <param name="maxCutDepth">The maximum number of times that any sprite can be subdivided</param>
	/// <param name="slicedObjectInfo">A list of SpriteSlicer2DSliceInfo that will be fill out with details about slice locations, slcied objects, and created child objects.</param>
	/// <param name="layerMask">Layermask to use in raycast operations.</param>
	public static void SliceAllSprites(Vector3 worldStartPoint, Vector3 worldEndPoint, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask)
	{
		SliceSpritesInternal(worldStartPoint, worldEndPoint, null, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
	}

	/// <summary>
	/// Slices a specific sprite if it is intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="sprite">The sprite to cut</param>
	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite)
	{
		if(sprite)
		{
			LayerMask layerMask = -1;
			List<SpriteSlicer2DSliceInfo> slicedObjectInfo = null;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, true, -1, ref slicedObjectInfo, layerMask, null);
		}
	}

	/// <summary>
	/// Slices a specific sprite if it is intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="sprite">The sprite to cut</param>
	/// <param name="destroySlicedObjects">Controls whether the parent objects are automatically destroyed. Set to false if you need to perform additional processing on them after slicing</param>
	/// <param name="slicedObjectInfo">A list of SpriteSlicer2DSliceInfo that will be fill out with details about slice locations, slcied objects, and created child objects.</param>
	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite, bool destroySlicedObjects, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		if(sprite)
		{
			LayerMask layerMask = -1;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, destroySlicedObjects, -1, ref slicedObjectInfo, layerMask, null);
		}
	}

	/// <summary>
	/// Slices a specific sprite if it is intersected by the given vector
	/// </summary>
	/// <param name="worldStartPoint">Slice start point in world coordinates.</param>
	/// <param name="worldEndPoint">Slice end point in world coordinates.</param>
	/// <param name="sprite">The sprite to cut</param>
	/// <param name="destroySlicedObjects">Controls whether the parent objects are automatically destroyed. Set to false if you need to perform additional processing on them after slicing</param>
	/// <param name="maxCutDepth">The maximum number of times that any sprite can be subdivided</param>
	/// <param name="slicedObjectInfo">A list of SpriteSlicer2DSliceInfo that will be fill out with details about slice locations, slcied objects, and created child objects.</param>
	public static void SliceSprite(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject sprite, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo)
	{
		if(sprite)
		{
			LayerMask layerMask = -1;
			SliceSpritesInternal(worldStartPoint, worldEndPoint, sprite, 0, destroySlicedObjects, maxCutDepth, ref slicedObjectInfo, layerMask, null);
		}
	}

	
	
	/// <summary>
	/// Slice any sprite with an attached PolygonCollider2D that intersects the given ray
	/// </summary>
	/// <param name="worldStartPoint">Cut world start point.</param>
	/// <param name="worldEndPoint">Cut world end point.</param>
	/// <param name="spriteObject">The specific sprite to cut - pass null to cut any sprite</param>
	/// <param name="spriteInstanceID">The specific sprite unique ID to cut - pass 0 to cut any sprite</param>
	/// <param name="destroySlicedObjects">Whether to automatically destroy the parent object - if false, the calling code must be responsible</param>
	/// <param name="maxCutDepth">Max cut depth - prevents a sprite from being subdivided too many times. Pass -1 to divide infinitely.</param>
	/// <param name="slicedObjectInfo">A list of information regarding the sliced objects, cut locations etc.</param>
	static void SliceSpritesInternal(Vector3 worldStartPoint, Vector3 worldEndPoint, GameObject spriteObject, int spriteInstanceID, bool destroySlicedObjects, int maxCutDepth, ref List<SpriteSlicer2DSliceInfo> slicedObjectInfo, LayerMask layerMask, string tag)
	{

		Vector3 direction = Vector3.Normalize(worldEndPoint - worldStartPoint);
		float length = Vector3.Distance(worldStartPoint, worldEndPoint);
		RaycastHit2D[] cutStartResults = Physics2D.RaycastAll(worldStartPoint, direction, length, layerMask.value);
		RaycastHit2D[] cutEndResults = Physics2D.RaycastAll(worldEndPoint, -direction, length, layerMask.value);

		if(cutStartResults.Length == cutEndResults.Length)
		{
			for(int cutResultIndex = 0; cutResultIndex < cutStartResults.Length && cutResultIndex < cutEndResults.Length; cutResultIndex++)
			{
				RaycastHit2D cutEnter = cutStartResults[cutResultIndex];

				int cutExitIndex = -1;
				
				// Find the matching cut end point in the cut end results
				for(int endResultIndex = 0; endResultIndex < cutEndResults.Length; endResultIndex++)
				{
					if(cutEndResults[endResultIndex].collider == cutEnter.collider)
					{
						cutExitIndex = endResultIndex;
						break;
					}
				}
				
				if(cutExitIndex == -1)
				{
					continue;
				}
				
				RaycastHit2D cutExit = cutEndResults[cutExitIndex];

				if(cutEnter.rigidbody == cutExit.rigidbody)
				{
					Rigidbody2D parentRigidBody = cutEnter.rigidbody;
					Transform parentTransform = cutEnter.transform;

					if(!parentRigidBody)
					{
						continue;
					}

					if(parentRigidBody.gameObject.isStatic)
					{
						continue;
					}

					if(spriteObject != null && parentRigidBody.gameObject != spriteObject)
					{
						continue;
					}

					if(tag != null && parentRigidBody.tag != tag)
					{
						continue;
					}

#if TK2D_SLICING_ENABLED
					tk2dSprite parenttk2dSprite = parentRigidBody.GetComponent<tk2dSprite>();
#endif

					SlicedSprite parentSlicedSprite = null;
					SpriteRenderer parentUnitySprite = null;

					// The object we're cutting must either be a unity sprite, a tk2D sprite, or a previously sliced sprite
#if TK2D_SLICING_ENABLED
					if(parenttk2dSprite == null)
#endif
					{
						parentUnitySprite = parentRigidBody.GetComponent<SpriteRenderer>();
						
						if(parentUnitySprite == null)
						{
							parentSlicedSprite = parentRigidBody.GetComponent<SlicedSprite>();
							
							if(parentSlicedSprite == null || (maxCutDepth >= 0 && parentSlicedSprite.CutsSinceParentObject >= maxCutDepth))
							{
								continue;	
							}
						}
					}

					// If we've passed in a specific spriteInstanceID, then only that specific object or sliced
					// objects derived from it can be cut
					if(spriteInstanceID != 0 && parentRigidBody.gameObject.GetInstanceID() != spriteInstanceID)
					{
						if(parentSlicedSprite == null || parentSlicedSprite.ParentInstanceID != spriteInstanceID)
						{
							continue;
						}
					}

					Vector3 cutStartLocalPoint = parentTransform.InverseTransformPoint(worldStartPoint);
					Vector3 cutEndLocalPoint = parentTransform.InverseTransformPoint(worldEndPoint);

					Vector3 cutEnterLocalPoint = parentTransform.InverseTransformPoint(cutEnter.point);
					Vector3 cutExitLocalPoint = parentTransform.InverseTransformPoint(cutExit.point);

					List<Vector2> polygonPoints = null;
					PolygonCollider2D polygonCollider = parentRigidBody.GetComponent<PolygonCollider2D>();
					PhysicsMaterial2D physicsMaterial = null;

					if(polygonCollider)
					{
						polygonPoints = new List<Vector2>(polygonCollider.points);
						physicsMaterial = polygonCollider.sharedMaterial;
					}
					else
					{
						BoxCollider2D boxCollider = parentRigidBody.GetComponent<BoxCollider2D>();

						if(boxCollider)
						{
							polygonPoints = new List<Vector2>(4);

#if UNITY_5
                            Vector2 colliderCenter = boxCollider.offset;
#else
                            Vector2 colliderCenter = boxCollider.offset;
#endif

                            polygonPoints.Add(colliderCenter + new Vector2(-boxCollider.size.x * 0.5f, -boxCollider.size.y * 0.5f));
                            polygonPoints.Add(colliderCenter + new Vector2(boxCollider.size.x * 0.5f, -boxCollider.size.y * 0.5f));
                            polygonPoints.Add(colliderCenter + new Vector2(boxCollider.size.x * 0.5f, boxCollider.size.y * 0.5f));
                            polygonPoints.Add(colliderCenter + new Vector2(-boxCollider.size.x * 0.5f, boxCollider.size.y * 0.5f));
							physicsMaterial = boxCollider.sharedMaterial;
						}
						else
						{
							CircleCollider2D circleCollider = parentRigidBody.GetComponent<CircleCollider2D>();

							if(circleCollider)
							{
								int numSteps = 32;
								float angleStepRate = (Mathf.PI * 2)/numSteps;
								polygonPoints = new List<Vector2>(numSteps);

#if UNITY_5
                                Vector2 colliderCenter = circleCollider.offset;
#else
                                Vector2 colliderCenter = circleCollider.offset;
#endif

								for(int loop = 0; loop < numSteps; loop++)
								{
									float angle = angleStepRate * loop;
                                    polygonPoints.Add(colliderCenter + new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * circleCollider.radius);
								}

								physicsMaterial = circleCollider.sharedMaterial;
							}
						}
					}

					if(polygonPoints != null)
					{
						// Collision rays must travel through the whole object - if the ray either starts or
						// ends inside the object then it is considered invalid
						if(IsPointInsidePolygon(cutStartLocalPoint, ref polygonPoints) ||
						   IsPointInsidePolygon(cutEndLocalPoint, ref polygonPoints))
						{
							if(s_DebugLoggingEnabled && spriteObject != null)
							{
								Debug.LogWarning("Failed to slice " + parentRigidBody.gameObject.name + " - start or end cut point is inside the collision mesh");
							}

							continue;
						}

						SpriteSlicer2DSliceInfo sliceInfo = new SpriteSlicer2DSliceInfo();
						sliceInfo.SlicedObject = parentRigidBody.gameObject;
						sliceInfo.SliceEnterWorldPosition = cutEnter.point;
						sliceInfo.SliceExitWorldPosition = cutExit.point;
						float parentArea = Mathf.Abs(Area(ref polygonPoints));
                        Debug.Log("parentarea" + parentArea);

                        if (IsConvex(ref polygonPoints))
						{
							int numPoints = polygonPoints.Count;
							List<Vector2> childSprite1VerticesUnsorted = new List<Vector2>(numPoints);
							List<Vector2> childSprite2VerticesUnsorted = new List<Vector2>(numPoints);
							
							childSprite1VerticesUnsorted.Add(cutEnterLocalPoint);
							childSprite1VerticesUnsorted.Add(cutExitLocalPoint);
							
							childSprite2VerticesUnsorted.Add(cutEnterLocalPoint);
							childSprite2VerticesUnsorted.Add(cutExitLocalPoint);
							
							for(int vertex = 0; vertex < numPoints; vertex++)
							{
								Vector2 point = polygonPoints[vertex];
								float determinant = CalculateDeterminant2x3(cutStartLocalPoint, cutEndLocalPoint, point);
								
								if (determinant > 0)
								{
									childSprite1VerticesUnsorted.Add(point);
								}
								else
								{
									childSprite2VerticesUnsorted.Add(point);
								}
							}
							
							Vector2[] childSprite1Vertices = ArrangeVertices(ref childSprite1VerticesUnsorted);
							Vector2[] childSprite2Vertices = ArrangeVertices(ref childSprite2VerticesUnsorted);
							
							float child1Area = GetArea(ref childSprite1Vertices);
							float child2Area = GetArea(ref childSprite2Vertices);
							
							if(!AreVerticesAcceptable(ref childSprite1Vertices, child1Area, true) || !AreVerticesAcceptable(ref childSprite2Vertices, child2Area, true))
							{
								continue;
							}
							else
							{
                                    SlicedSprite childSprite1, childSprite2;
								PolygonCollider2D child1Collider, child2Collider;
								CreateChildSprite(parentRigidBody, physicsMaterial, ref childSprite1Vertices, parentArea, child1Area, out childSprite1, out child1Collider);
                                if (parentRigidBody.gameObject.layer == LayerMask.NameToLayer("nonslice"))
                                {
                                    childSprite1.gameObject.layer = LayerMask.NameToLayer("nonslice");
                                }
                                childSprite1.gameObject.name = parentRigidBody.gameObject.name + "_child1";
								
								CreateChildSprite(parentRigidBody, physicsMaterial, ref childSprite2Vertices, parentArea, child2Area, out childSprite2, out child2Collider);
                                if (parentRigidBody.gameObject.layer == LayerMask.NameToLayer("nonslice"))
                                {
                                    childSprite2.gameObject.layer = LayerMask.NameToLayer("nonslice");
                                }
                                childSprite2.gameObject.name = parentRigidBody.gameObject.name + "_child2";
								
#if TK2D_SLICING_ENABLED
								if(parenttk2dSprite)
								{
									childSprite1.InitFromTK2DSprite(parenttk2dSprite, ref child1Collider, ref childSprite1Vertices, false);
									childSprite2.InitFromTK2DSprite(parenttk2dSprite, ref child2Collider, ref childSprite2Vertices, false);
								}
								else
#endif
									if(parentSlicedSprite)
								{
									childSprite1.InitFromSlicedSprite(parentSlicedSprite, ref child1Collider, ref childSprite1Vertices, false);
									childSprite2.InitFromSlicedSprite(parentSlicedSprite, ref child2Collider, ref childSprite2Vertices, false);
								}
								else if(parentUnitySprite)
								{
									childSprite1.InitFromUnitySprite(parentUnitySprite, ref child1Collider, ref childSprite1Vertices, false);
									childSprite2.InitFromUnitySprite(parentUnitySprite, ref child2Collider, ref childSprite2Vertices, false);
								}
								
								sliceInfo.ChildObjects.Add(childSprite1.gameObject);
								sliceInfo.ChildObjects.Add(childSprite2.gameObject);
							}
						}
						else // Use concave slicing method
						{
							Polygon polygon = new Polygon();
							SpriteSlicerLine line = new SpriteSlicerLine(cutStartLocalPoint, cutEndLocalPoint);
							polygon.points = polygonPoints;
                            SliceConcave(polygon, line);

							for(int childIndex = 0; childIndex < s_ConcaveSlicePolygonResults.Count; childIndex++)
							{
								Vector2[] childSpriteVertices = s_ConcaveSlicePolygonResults[childIndex].points.ToArray();
								float childArea = GetArea(ref childSpriteVertices);

								if(AreVerticesAcceptable(ref childSpriteVertices, childArea, false))
								{
									SlicedSprite childSprite;
									PolygonCollider2D childCollider;
									
									CreateChildSprite(parentRigidBody, physicsMaterial, ref childSpriteVertices, parentArea, childArea, out childSprite, out childCollider);
									childSprite.gameObject.name = parentRigidBody.gameObject.name + "_child" + childIndex;

#if TK2D_SLICING_ENABLED
									if(parenttk2dSprite)
									{
										childSprite.InitFromTK2DSprite(parenttk2dSprite, ref childCollider, ref childSpriteVertices, true);
									}
									else
#endif
										if(parentSlicedSprite)
									{
										childSprite.InitFromSlicedSprite(parentSlicedSprite, ref childCollider, ref childSpriteVertices, true);
									}
									else if(parentUnitySprite)
									{
										childSprite.InitFromUnitySprite(parentUnitySprite, ref childCollider, ref childSpriteVertices, true);
									}
									
									sliceInfo.ChildObjects.Add(childSprite.gameObject);
								}
							}
						}

						if(sliceInfo.ChildObjects.Count > 0)
						{
							// Send an OnSpriteSliced message to the sliced object
							parentRigidBody.gameObject.SendMessage("OnSpriteSliced", sliceInfo, SendMessageOptions.DontRequireReceiver);
							
							if(slicedObjectInfo != null)
							{
								// Need to null the parent object as we're about to destroy it
								if(destroySlicedObjects)
								{
									sliceInfo.SlicedObject = null;
								}
								
								slicedObjectInfo.Add(sliceInfo);
							}
							
							if(destroySlicedObjects)
							{
                                parentRigidBody.gameObject.layer = LayerMask.NameToLayer("trans");
                                GameObject.Destroy(parentRigidBody.gameObject);
                                if (ScoreManager.count >= 1)
                                {
                                  
                                }
                            }
							else
							{
								parentRigidBody.gameObject.SetActive(false);
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Create a child sprite from the given parent sprite, using the provided vertices
	/// </summary>
	static void CreateChildSprite(Rigidbody2D parentRigidBody, PhysicsMaterial2D physicsMaterial, ref Vector2[] spriteVertices, float parentArea, float childArea, out SlicedSprite slicedSprite, out PolygonCollider2D polygonCollider)
	{
		GameObject childObject = new GameObject();
		slicedSprite = childObject.AddComponent<SlicedSprite>();

		// Child sprites should inherit the rigid body behaviour of their parents
		Rigidbody2D childRigidBody = slicedSprite.GetComponent<Rigidbody2D>();
		childRigidBody.mass = parentRigidBody.mass * (childArea/parentArea);
		childRigidBody.drag = parentRigidBody.drag;
		childRigidBody.angularDrag = parentRigidBody.angularDrag;
		childRigidBody.gravityScale = parentRigidBody.gravityScale;
        ScoreManager.score += 0;


#if UNITY_5
        childRigidBody.constraints = parentRigidBody.constraints;
#else
        childRigidBody.fixedAngle = parentRigidBody.fixedAngle;
#endif

		childRigidBody.isKinematic = parentRigidBody.isKinematic;
		childRigidBody.interpolation = parentRigidBody.interpolation;
		childRigidBody.sleepMode = parentRigidBody.sleepMode;
		childRigidBody.collisionDetectionMode = parentRigidBody.collisionDetectionMode;
		childRigidBody.velocity = parentRigidBody.velocity;
		childRigidBody.angularVelocity = parentRigidBody.angularVelocity;

		polygonCollider = slicedSprite.GetComponent<PolygonCollider2D>();
		polygonCollider.points = spriteVertices;
		polygonCollider.sharedMaterial = physicsMaterial;
	}

	#region "HELPER_FUNCTIONS"
	enum LineSide
	{
		Left,
		Right,
		On
	};

	struct SpriteSlicerLine
	{
		public SpriteSlicerLine(Vector2 a, Vector2 b)
		{
			p1 = a;
			p2 = b;
		}
		
		public Vector2 p1;
		public Vector2 p2;
	};

	public class Polygon
	{
		public List<Vector2> points = new List<Vector2>();
	}

	class LinkedPolygonPoint
	{
		public Vector2 Position;
		public LineSide SliceSide;
		public LinkedPolygonPoint Next;
		public LinkedPolygonPoint Prev;
		public float DistOnLine = 0.0f;
		public bool Visited = false;
		
		public LinkedPolygonPoint(Vector2 startPos, LineSide side)
		{
			Position = startPos;
			SliceSide = side;
		}
	}

	static LineSide GetSideOfLine(SpriteSlicerLine line, Vector2 pt)
	{
		float d = (pt.x-line.p1.x)*(line.p2.y-line.p1.y)-(pt.y-line.p1.y)*(line.p2.x-line.p1.x);
		return (d > float.Epsilon ? LineSide.Right : (d < -float.Epsilon ? LineSide.Left : LineSide.On));
	}

	static float CalculateDeterminant2x3(Vector2 start, Vector2 end, Vector2 point) 
	{
		return start.x * end.y + end.x * point.y + point.x * start.y - start.y * end.x - end.y * point.x - point.y * start.x;
	}
	
	public static float CalculateDeterminant2x2(Vector2 vectorA, Vector2 vectorB)
	{
		return vectorA.x * vectorB.y - vectorA.y * vectorB.x;
	}

	public static float DotProduct(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
	{
		return (point.x - lineStart.x) * (lineEnd.x - lineStart.x) + (point.y - lineStart.y) * (lineEnd.y - lineStart.y);
	}

	// Trianglulate the given polygon
	public static int[] Triangulate(ref List<Vector2> points) 
	{
		List<int> indices = new List<int>();
		
		int n = points.Count;
		if (n < 3)
			return indices.ToArray();
		
		int[] V = new int[n];
		
		if (Area(ref points) > 0)
		{
			for (int v = 0; v < n; v++)
				V[v] = v;
		}
		else 
		{
			for (int v = 0; v < n; v++)
				V[v] = (n - 1) - v;
		}
		
		int nv = n;
		int count = 2 * nv;
		for (int m = 0, v = nv - 1; nv > 2; ) {
			if ((count--) <= 0)
				return indices.ToArray();
			
			int u = v;
			if (nv <= u)
				u = 0;
			v = u + 1;
			if (nv <= v)
				v = 0;
			int w = v + 1;
			if (nv <= w)
				w = 0;
			
			if (Snip(ref points, u, v, w, nv, V)) 
			{
				int a, b, c, s, t;
				a = V[u];
				b = V[v];
				c = V[w];
				indices.Add(a);
				indices.Add(b);
				indices.Add(c);
				m++;
				for (s = v, t = v + 1; t < nv; s++, t++)
					V[s] = V[t];
				nv--;
				count = 2 * nv;
			}
		}
		
		indices.Reverse();
		return indices.ToArray();
	}

	// Get the area of the given polygon
	static float Area (ref List<Vector2> points) 
	{
		int n = points.Count;
		float A = 0.0f;
		
		for (int p = n - 1, q = 0; q < n; p = q++) 
		{
			Vector2 pval = points[p];
			Vector2 qval = points[q];
			A += pval.x * qval.y - qval.x * pval.y;
		}
		
		return (A * 0.5f);
	}	
	
	static bool Snip (ref List<Vector2> points, int u, int v, int w, int n, int[] V) 
	{
		int p;
		Vector2 A = points[V[u]];
		Vector2 B = points[V[v]];
		Vector2 C = points[V[w]];
		
		if (Mathf.Epsilon > (((B.x - A.x) * (C.y - A.y)) - ((B.y - A.y) * (C.x - A.x))))
		{
			return false;
		}
		
		for (p = 0; p < n; p++) 
		{
			if ((p == u) || (p == v) || (p == w))
			{
				continue;
			}
			
			Vector2 P = points[V[p]];
			
			if (InsideTriangle(A, B, C, P))
			{
				return false;
			}
		}
		
		return true;
	}

	// Check if a point is inside a given triangle
	static bool InsideTriangle (Vector2 A, Vector2 B, Vector2 C, Vector2 P) 
	{
		float ax, ay, bx, by, cx, cy, apx, apy, bpx, bpy, cpx, cpy;
		float cCROSSap, bCROSScp, aCROSSbp;
		
		ax = C.x - B.x; ay = C.y - B.y;
		bx = A.x - C.x; by = A.y - C.y;
		cx = B.x - A.x; cy = B.y - A.y;
		apx = P.x - A.x; apy = P.y - A.y;
		bpx = P.x - B.x; bpy = P.y - B.y;
		cpx = P.x - C.x; cpy = P.y - C.y;
		
		aCROSSbp = ax * bpy - ay * bpx;
		cCROSSap = cx * apy - cy * apx;
		bCROSScp = bx * cpy - by * cpx;
		
		return ((aCROSSbp >= 0.0f) && (bCROSScp >= 0.0f) && (cCROSSap >= 0.0f));
	}
	
	// Helper class to sort vertices in ascending X coordinate order
	public class VectorComparer : IComparer<Vector2>
	{
		public int Compare(Vector2 vectorA, Vector2 vectorB)
		{
			if (vectorA.x > vectorB.x) 
			{
				return 1;
			} 
			else if (vectorA.x < vectorB.x) 
			{
				return -1;
			}
			
			return 0; 
		}
	}

	// Helper class to sort polygon edges
	class EdgeComparer : IComparer<LinkedPolygonPoint>
	{
		SpriteSlicerLine line;
		
		public EdgeComparer(SpriteSlicerLine ln)
		{
			line = ln;
		}
		
		public int Compare(LinkedPolygonPoint edgeA, LinkedPolygonPoint edgeB)
		{
			float dotA = SpriteSlicer2D.DotProduct(line.p1, line.p2, edgeA.Position);
			float dotB = SpriteSlicer2D.DotProduct(line.p1, line.p2, edgeB.Position);
			
			if(dotA < dotB)
			{
				return -1;
			}
			else
			{
				return 1;
			}
		}
	}
	
	/// <summary>
	/// Sort the vertices into a counter clockwise order
	/// </summary>
	static Vector2[] ArrangeVertices(ref List<Vector2> vertices)
	{
		float determinant;
		int numVertices = vertices.Count;
		int counterClockWiseIndex = 1;
		int clockWiseIndex = numVertices - 1;			
		vertices.Sort(s_VectorComparer);
		List<Vector2> sortedVertices = new List<Vector2>(numVertices);
		
		for(int vertex = 0; vertex < numVertices; vertex++)
		{
			sortedVertices.Add(Vector2.zero);
		}

		Vector2 startPoint = vertices[0];
		Vector2 endPoint = vertices[numVertices - 1];
		sortedVertices[0] = startPoint;
		
		for(int vertex = 1; vertex < numVertices - 1; vertex++)
		{
			determinant = CalculateDeterminant2x3(startPoint, endPoint, vertices[vertex]);
			
			if (determinant < 0)
			{
				sortedVertices[counterClockWiseIndex++] = vertices[vertex];
			}
			else 
			{
				sortedVertices[clockWiseIndex--] = vertices[vertex];
			}
		}
		
		sortedVertices[counterClockWiseIndex] = endPoint;
		return sortedVertices.ToArray();
	}
	
	/// <summary>
	/// Work out the area defined by the vertices
	/// </summary>
	static float GetArea(ref Vector2[] vertices)
	{
		// Check that the total area isn't stupidly small
		int numVertices = vertices.Length;
		float area = vertices[0].y * (vertices[numVertices-1].x- vertices[1].x);
		
		for(int i = 1; i < numVertices; i++)
		{
			area += vertices[i].y * (vertices[i-1].x - vertices[(i+1)% numVertices].x);
		}
		
		return Mathf.Abs(area * 0.5f);
	}
	
	/// <summary>
	/// Check if this list of points defines a convex shape
	/// </summary>
	public static bool IsConvex(ref Vector2[] vertices)
	{
		int numVertices = vertices.Length;
		float determinant;
		Vector3 v1 = vertices[0] - vertices[numVertices-1];
		Vector3 v2 = vertices[1] - vertices[0];
		float referenceDeterminant = CalculateDeterminant2x2(v1, v2);
		
		for (int i=1; i< numVertices - 1; i++)
		{
			v1 = v2;
			v2 = vertices[i+1] - vertices[i];
			determinant = CalculateDeterminant2x2(v1, v2);
			
			if (referenceDeterminant * determinant < 0.0f)
			{
				return false;
			}
		}
		
		v1 = v2;
		v2 = vertices[0] - vertices[numVertices-1];
		determinant = CalculateDeterminant2x2(v1, v2);
		
		if (referenceDeterminant * determinant < 0.0f)
		{
			return false;
		}
		
		return true;
	}

	/// <summary>
	/// Check if this list of points defines a convex shape
	/// </summary>
	public static bool IsConvex(ref List<Vector2> vertices)
	{
		int numVertices = vertices.Count;
		float determinant;
		Vector3 v1 = vertices[0] - vertices[numVertices-1];
		Vector3 v2 = vertices[1] - vertices[0];
		float referenceDeterminant = CalculateDeterminant2x2(v1, v2);
		
		for (int i=1; i< numVertices - 1; i++)
		{
			v1 = v2;
			v2 = vertices[i+1] - vertices[i];
			determinant = CalculateDeterminant2x2(v1, v2);
			
			if (referenceDeterminant * determinant < 0.0f)
			{
				return false;
			}
		}
		
		v1 = v2;
		v2 = vertices[0] - vertices[numVertices-1];
		determinant = CalculateDeterminant2x2(v1, v2);
		
		if (referenceDeterminant * determinant < 0.0f)
		{
			return false;
		}
		
		return true;
	}
	
	/// <summary>
	/// Verify if the list of vertices are suitable to create a new 2D collider shape
	/// 
	static bool AreVerticesAcceptable(ref Vector2[] vertices, float area, bool failOnConcave)
	{
		// Polygons need to at least have 3 vertices, not be convex, and have a vaguely sensible total area
		if (vertices.Length < 3)
		{
			if(s_DebugLoggingEnabled)
			{
				Debug.LogWarning("Vertices rejected - insufficient vertices");
			}

			return false;
		}
		    
		if(area < 0.0001f)
		{
			if(s_DebugLoggingEnabled)
			{
				Debug.LogWarning("Vertices rejected - below minimum area");
			}

			return false;
		}
		
		if(failOnConcave && !IsConvex(ref vertices))
		{
			if(s_DebugLoggingEnabled)
			{
				Debug.LogWarning("Vertices rejected - shape is not convex");
			}

			return false;
		}
		
		return true;
	}

	static void SliceConcave(Polygon poly, SpriteSlicerLine line)
	{
		SplitEdges(poly, line);
		SortEdges(line);
		SplitPolygon();
		CollectPolys();
	}
	
	static Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
	{
		float A1 = pe1.y-ps1.y;
		float B1 = ps1.x-pe1.x;
		float C1 = A1*ps1.x+B1*ps1.y;
		float A2 = pe2.y-ps2.y;
		float B2 = ps2.x-pe2.x;
		float C2 = A2*ps2.x+B2*ps2.y;
		float delta = A1*B2 - A2*B1;
		
		if(delta == 0)
		{
			Debug.LogError("Lines are parallel!");
		}
		
		float inverseDelta = 1.0f / delta;
		return new Vector2((B2*C1 - B1*C2) * inverseDelta, (A1*C2 - A2*C1) * inverseDelta);
	}
	
    /// <summary>
    /// Generate polygon edge points based on the slice line
    /// </summary>
    static void SplitEdges(Polygon poly, SpriteSlicerLine line)
	{
		s_ConcavePolygonPoints.Clear();
		s_ConcavePolygonIntersectionPoints.Clear();
		
		for (int i=0; i < poly.points.Count; i++)
		{
			SpriteSlicerLine edge = new SpriteSlicerLine(poly.points[i], poly.points[(i+1) % poly.points.Count]);
			LineSide edgeStartSide = GetSideOfLine(line, edge.p1);
			LineSide edgeEndSide = GetSideOfLine(line, edge.p2);
			s_ConcavePolygonPoints.Add(new LinkedPolygonPoint(poly.points[i], edgeStartSide));
			
			if (edgeStartSide == LineSide.On)
			{
				s_ConcavePolygonIntersectionPoints.Add(s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1]);
			}
			else if (edgeStartSide != edgeEndSide && edgeEndSide != LineSide.On)
			{
				Vector2 ip = LineIntersectionPoint(edge.p1, edge.p2, line.p1, line.p2);
				s_ConcavePolygonPoints.Add(new LinkedPolygonPoint(ip, LineSide.On));
				s_ConcavePolygonIntersectionPoints.Add(s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1]);
			}
		}
		
		for(int loop = 0; loop < s_ConcavePolygonPoints.Count - 1; loop++)
		{
			int nextIndex = (loop + 1) % s_ConcavePolygonPoints.Count;
			LinkedPolygonPoint current = s_ConcavePolygonPoints[loop];
			LinkedPolygonPoint next = s_ConcavePolygonPoints[nextIndex];
			current.Next = next;
			next.Prev = current;
		}
		
		s_ConcavePolygonPoints [s_ConcavePolygonPoints.Count - 1].Next = s_ConcavePolygonPoints [0];
		s_ConcavePolygonPoints [0].Prev = s_ConcavePolygonPoints [s_ConcavePolygonPoints.Count - 1];
	}
	
	static void SortEdges(SpriteSlicerLine line)
	{
		EdgeComparer edgeComparer = new EdgeComparer (line);
		s_ConcavePolygonIntersectionPoints.Sort (edgeComparer);
		
		for(int i = 1; i < s_ConcavePolygonIntersectionPoints.Count; i++)
		{
			s_ConcavePolygonIntersectionPoints[i].DistOnLine = (s_ConcavePolygonIntersectionPoints[i].Position - s_ConcavePolygonIntersectionPoints[0].Position).magnitude;
		}
	}
	
    /// <summary>
    /// Splits the polygon along the cut line
    /// </summary>
	static void SplitPolygon()
	{
		LinkedPolygonPoint useSrc = null;
		
		for(int i = 0; i < s_ConcavePolygonIntersectionPoints.Count; i++)
		{
			LinkedPolygonPoint srcEdge = useSrc;
			useSrc = null;
			
			for (; srcEdge == null && i < s_ConcavePolygonIntersectionPoints.Count; i++)
			{
				LinkedPolygonPoint curEdge = s_ConcavePolygonIntersectionPoints[i];
				LineSide curSide = curEdge.SliceSide;
				LineSide prevSide = curEdge.Prev.SliceSide;
				LineSide nextSide = curEdge.Next.SliceSide;
				
				if(curSide != LineSide.On)
				{
					Debug.LogError("Current side should be ON");
				}
				
				if ((prevSide == LineSide.Left && nextSide == LineSide.Right) ||
				    (prevSide == LineSide.Left && nextSide == LineSide.On && curEdge.Next.DistOnLine < curEdge.DistOnLine) ||
				    (prevSide == LineSide.On && nextSide == LineSide.Right && curEdge.Prev.DistOnLine < curEdge.DistOnLine))
				{
					srcEdge = curEdge;
				}
			}
			
			// find destination
			LinkedPolygonPoint dstEdge = null;
			
			for (; dstEdge == null && i<s_ConcavePolygonIntersectionPoints.Count; )
			{
				LinkedPolygonPoint curEdge = s_ConcavePolygonIntersectionPoints[i];
				LineSide curSide = curEdge.SliceSide;
				LineSide prevSide = curEdge.Prev.SliceSide;
				LineSide nextSide = curEdge.Next.SliceSide;
				
				if(curSide != LineSide.On)
				{
					Debug.LogError("Current side should be ON");
				}
				
				if ((prevSide == LineSide.Right && nextSide == LineSide.Left)  ||
				    (prevSide == LineSide.On && nextSide == LineSide.Left)     ||
				    (prevSide == LineSide.Right && nextSide == LineSide.On)    ||
				    (prevSide == LineSide.Right && nextSide == LineSide.Right) ||
				    (prevSide == LineSide.Left && nextSide == LineSide.Left))
				{
					dstEdge = curEdge;
				}
				else
                {
					i++;
                }
			}
			
			if (srcEdge != null && dstEdge != null)
			{
				LinkPoints(srcEdge, dstEdge);
                VerifyPolygons();
				
				if (srcEdge.Prev.Prev.SliceSide == LineSide.Left)
				{
					useSrc = srcEdge.Prev;
				}
				else if (dstEdge.Next.SliceSide == LineSide.Right)
				{
					useSrc = dstEdge;
				}
			}
		}
	}
	
    /// <summary>
    /// Sort polygon points into their appropriate polygons
    /// </summary>
	static void CollectPolys()
	{
		s_ConcaveSlicePolygonResults.Clear ();
		
		foreach(LinkedPolygonPoint e in s_ConcavePolygonPoints)
		{
			if (!e.Visited)
			{
				Polygon splitPoly = new Polygon();
				LinkedPolygonPoint curSide = e;
				
				do
				{
					curSide.Visited = true;
					splitPoly.points.Add(curSide.Position);
					curSide = curSide.Next;
				}
				while (curSide != e);
				
				s_ConcaveSlicePolygonResults.Add(splitPoly);
			}
		}
	}

    /// <summary>
    /// Verifies the polygons are correct and correctly wrap around
    /// </summary>
	static void VerifyPolygons()
	{
		foreach (LinkedPolygonPoint edge in s_ConcavePolygonPoints)
		{
			LinkedPolygonPoint curSide = edge;
			int count = 0;
			
			do
			{
				if(count >= s_ConcavePolygonPoints.Count)
				{
					Debug.LogError("Invalid polygon cycle detected");
					break;
				}
				
				curSide = curSide.Next;
				count++;
			}
			while (curSide != edge);
		}
	}
	
    /// <summary>
    /// Link two points on the polygon by creating a link between them in each direction
    /// </summary>
    static void LinkPoints(LinkedPolygonPoint srcEdge, LinkedPolygonPoint dstEdge)
	{
		s_ConcavePolygonPoints.Add(new LinkedPolygonPoint(srcEdge.Position, srcEdge.SliceSide));
		LinkedPolygonPoint a = s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1];
		s_ConcavePolygonPoints.Add(new LinkedPolygonPoint(dstEdge.Position, dstEdge.SliceSide));
		LinkedPolygonPoint b = s_ConcavePolygonPoints[s_ConcavePolygonPoints.Count - 1];
		a.Next = dstEdge;
		a.Prev = srcEdge.Prev;
		b.Next = srcEdge;
		b.Prev = dstEdge.Prev;
		srcEdge.Prev.Next = a;
		srcEdge.Prev = b;
		dstEdge.Prev.Next = b;
		dstEdge.Prev = a;
	}

	/// <summary>
	/// Use the polygon winding algorithm to check whether a point is inside the given polygon
	/// </summary>
	static bool IsPointInsidePolygon(Vector2 pos, ref List<Vector2> polygonPoints)
	{
		int winding = 0;
		int numPoints = polygonPoints.Count;
		
		for(int vertexIndex = 0; vertexIndex < numPoints; vertexIndex++)
		{
			int nextIndex = vertexIndex + 1;
			
			if(nextIndex >= numPoints)
			{
				nextIndex = 0;
			}
			
			Vector2 thisPoint = polygonPoints[vertexIndex];
			Vector2 nextPoint = polygonPoints[nextIndex];
			
			if(thisPoint.y <= pos.y)
			{
				if(nextPoint.y > pos.y)
				{
					float isLeft = ((nextPoint.x - thisPoint.x) * (pos.y - thisPoint.y) - (pos.x - thisPoint.x) * (nextPoint.y - thisPoint.y));
					
					if(isLeft > 0)
					{
						winding++;
					}
				}
			}
			else
			{
				if(nextPoint.y <= pos.y)
				{
					float isLeft = ((nextPoint.x - thisPoint.x) * (pos.y - thisPoint.y) - (pos.x - thisPoint.x) * (nextPoint.y - thisPoint.y));
					
					if(isLeft < 0)
					{
						winding--;
					}
				}
			}
		}
		
		return winding != 0;
	}
	#endregion
}

#if UNITY_EDITOR
public static class SpriteSlicerConvexHelper
{
    [MenuItem("Tools/Sprite Slicer 2D/Add Sliced Sprite Component")]
    static void AddSlicedSpriteComponent()
    {
        for (int loop = 0; loop < Selection.gameObjects.Length; loop++)
        {
            Selection.gameObjects[loop].AddComponent<SlicedSprite>();
        }
    }

	[MenuItem("Tools/Sprite Slicer 2D/Make Convex")]
	static void MakeConvex()
	{
		for(int loop = 0; loop < Selection.gameObjects.Length; loop++)
		{
			PolygonCollider2D polyCollider = Selection.gameObjects[loop].GetComponent<PolygonCollider2D>();

			if(polyCollider)
			{
				List<Vector2> vertices = new List<Vector2>(polyCollider.points);
				int originalNumVertices = vertices.Count;
				int iterations = 0;

				if(SpriteSlicer2D.IsConvex(ref vertices))
				{
					Debug.Log(Selection.gameObjects[loop].name + " is already convex - no work to do");
				}
				else
				{
					do
					{
						float determinant;
						Vector3 v1 = vertices[0] - vertices[vertices.Count-1];
						Vector3 v2 = vertices[1] - vertices[0];
						float referenceDeterminant = SpriteSlicer2D.CalculateDeterminant2x2(v1, v2);
						
						for (int i=1; i< vertices.Count - 1;)
						{
							v1 = v2;
							v2 = vertices[i+1] - vertices[i];
							determinant = SpriteSlicer2D.CalculateDeterminant2x2(v1, v2);
							
							if (referenceDeterminant * determinant < 0.0f)
							{
								vertices.RemoveAt(i);
							}
							else
							{
								i++;
							}
						}
						
						v1 = v2;
						v2 = vertices[0] - vertices[vertices.Count-1];
						determinant = SpriteSlicer2D.CalculateDeterminant2x2(v1, v2);
						
						if (referenceDeterminant * determinant < 0.0f)
						{
							vertices.RemoveAt(vertices.Count - 1);
						}

						iterations++;
					} while(!SpriteSlicer2D.IsConvex(ref vertices) && iterations < 25);
				}
				
				if(SpriteSlicer2D.IsConvex(ref vertices))
				{
					polyCollider.points = vertices.ToArray();
					Debug.Log(Selection.gameObjects[loop].name + " points reduced to " + vertices.Count.ToString() + " from " + originalNumVertices.ToString() );
				}
				else
				{
					Debug.Log(Selection.gameObjects[loop].name + " could not be made convex, please adjust shape manually");
				}
			}
		}
	}
}
#endif

/// <summary>
/// Simple class that takes a sprite and a polygon collider, and creates
/// a render mesh that exactly fits the collider.
/// </summary>
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody2D), typeof(PolygonCollider2D))]
public class SlicedSprite : MonoBehaviour
{
    public MeshRenderer MeshRenderer { get { return m_MeshRenderer; } }
    public Vector2 MinCoords { get { return m_MinCoords; } }
    public Vector2 MaxCoords { get { return m_MaxCoords; } }
    public Bounds SpriteBounds { get { return m_SpriteBounds; } }
    public int ParentInstanceID { get { return m_ParentInstanceID; } }
    public int CutsSinceParentObject { get { return m_CutsSinceParentObject; } }
    public bool Rotated { get { return m_Rotated; } }
    public bool HFlipped { get { return m_HFlipped; } }
    public bool VFlipped { get { return m_VFlipped; } }

    MeshRenderer m_MeshRenderer;
    MeshFilter m_MeshFilter;
    Transform m_Transform;

    Vector2 m_MinCoords;
    Vector2 m_MaxCoords;
    Vector2 m_Centroid;
    Vector2 m_UVOffset;
    Bounds m_SpriteBounds;
    int m_ParentInstanceID;
    int m_CutsSinceParentObject;
    bool m_Rotated;
    bool m_VFlipped;
    bool m_HFlipped;

    /// <summary>
    /// Called when the object is created
    /// </summary>
    void Awake()
    {
        m_Transform = this.transform;
        m_MeshFilter = GetComponent<MeshFilter>();
        m_MeshRenderer = GetComponent<MeshRenderer>();
        m_ParentInstanceID = this.gameObject.GetInstanceID();
        m_MinCoords = new Vector2(0.0f, 0.0f);
        m_MaxCoords = new Vector2(1.0f, 1.0f);

        if (m_MeshFilter.mesh)
        {
            m_SpriteBounds = m_MeshFilter.mesh.bounds;
        }
    }

#if TK2D_SLICING_ENABLED
	/// <summary>
	/// Initialise the sliced sprite using an existing tk2dsprite
	/// </summary>
	public void InitFromTK2DSprite(tk2dSprite sprite, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, bool isConcave)
	{
        MeshRenderer meshRenderer = sprite.GetComponent<MeshRenderer>();
        Bounds bounds = meshRenderer.bounds;
        Vector2 position = sprite.transform.position;
        Vector2 min = bounds.min;
        Vector2 max = bounds.max;
        Vector2 size = bounds.size;

        Vector3 lossyScale = sprite.transform.lossyScale;
        Vector2 offsetOfAbsolutePositionRelativelyToMinOfBounds = Vector2.zero;

        // Adjust pivot position calculation if sprite has been flipped on the x axis
        if(Mathf.Sign(lossyScale.x) < 0.0f)
        {
            offsetOfAbsolutePositionRelativelyToMinOfBounds.x = max.x - position.x;
        }
        else 
        {
            offsetOfAbsolutePositionRelativelyToMinOfBounds.x = position.x - min.x;
        }
        
        // Adjust pivot position calculation if sprite has been flipped on the y axis
        if(Mathf.Sign(lossyScale.y) < 0.0f)
        {
            offsetOfAbsolutePositionRelativelyToMinOfBounds.y = max.y - position.y;
        }
        else
        {
            offsetOfAbsolutePositionRelativelyToMinOfBounds.y = position.y - min.y;
        }
        
        Vector2 pivotVector = new Vector2(
            offsetOfAbsolutePositionRelativelyToMinOfBounds.x / size.x,
            offsetOfAbsolutePositionRelativelyToMinOfBounds.y / size.y
            );
        
        pivotVector -= new Vector2(0.5f, 0.5f);

		tk2dSpriteDefinition spriteDefinition = sprite.GetCurrentSpriteDef();
		bool isRotated = spriteDefinition.flipped != tk2dSpriteDefinition.FlipMode.None;
		bool isHFlipped = sprite.FlipX;
		bool isVFlipped = sprite.FlipY;
        InitSprite(sprite.gameObject, sprite.GetComponent<MeshRenderer>(), ref polygon, ref polygonPoints, spriteDefinition.uvs[0], spriteDefinition.uvs[spriteDefinition.uvs.Length - 1], spriteDefinition.GetBounds(), meshRenderer.sharedMaterial, isRotated, isHFlipped, isVFlipped, Vector2.zero, pivotVector, isConcave);
		m_ParentInstanceID = sprite.gameObject.GetInstanceID();
	}
#endif

    /// <summary>
    /// Initialise this sliced sprite using an existing SlicedSprite
    /// </summary>
    public void InitFromSlicedSprite(SlicedSprite slicedSprite, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, bool isConcave)
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock ();
        slicedSprite.m_MeshRenderer.GetPropertyBlock(block);
        m_MeshRenderer.SetPropertyBlock(block);

		InitSprite(slicedSprite.gameObject, slicedSprite.MeshRenderer, ref polygon, ref polygonPoints, slicedSprite.MinCoords, slicedSprite.MaxCoords, slicedSprite.SpriteBounds, slicedSprite.m_MeshRenderer.sharedMaterial, slicedSprite.Rotated, slicedSprite.HFlipped, slicedSprite.VFlipped, slicedSprite.m_Centroid, slicedSprite.m_UVOffset, isConcave);
        m_ParentInstanceID = slicedSprite.GetInstanceID();
        m_CutsSinceParentObject = slicedSprite.CutsSinceParentObject + 1;
    }

    /// <summary>
    /// Initialise using a unity sprite
    /// </summary>
	public void InitFromUnitySprite(SpriteRenderer unitySprite, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, bool isConcave)
    {
        Sprite sprite = unitySprite.sprite;
                
		Bounds bounds = unitySprite.bounds;
		Vector2 position = unitySprite.transform.position;
		Vector2 min = bounds.min;
        Vector2 max = bounds.max;
		Vector2 size = bounds.size;
		Vector2 offsetOfAbsolutePositionRelativelyToMinOfBounds = Vector2.zero;
        Vector3 lossyScale = unitySprite.transform.lossyScale;

        // Adjust pivot position calculation if sprite has been flipped on the x axis
        if(Mathf.Sign(lossyScale.x) < 0.0f)
        {
            offsetOfAbsolutePositionRelativelyToMinOfBounds.x = max.x - position.x;
        }
        else 
        {
            offsetOfAbsolutePositionRelativelyToMinOfBounds.x = position.x - min.x;
        }
        
        // Adjust pivot position calculation if sprite has been flipped on the y axis
        if(Mathf.Sign(lossyScale.y) < 0.0f)
        {
            offsetOfAbsolutePositionRelativelyToMinOfBounds.y = max.y - position.y;
        }
        else
        {
            offsetOfAbsolutePositionRelativelyToMinOfBounds.y = position.y - min.y;
        }

		Vector2 pivotVector = new Vector2(
				offsetOfAbsolutePositionRelativelyToMinOfBounds.x/size.x,
				offsetOfAbsolutePositionRelativelyToMinOfBounds.y/size.y
				);    		
				
		pivotVector -= new Vector2(0.5f, 0.5f);
        								
        Texture2D spriteTexture = sprite.texture;
        Vector2 textureSize = new Vector2(spriteTexture.width, spriteTexture.height);

        Material material = unitySprite.sharedMaterial;
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        block.SetTexture("_MainTex", spriteTexture);
        m_MeshRenderer.SetPropertyBlock(block);

        Rect textureRect = sprite.textureRect;		
        Vector2 minTextureCoords = new Vector2(textureRect.xMin / (float)textureSize.x, textureRect.yMin / (float)textureSize.y);
        Vector2 maxTextureCoords = new Vector2(textureRect.xMax / (float)textureSize.x, textureRect.yMax / (float)textureSize.y);

        InitSprite(unitySprite.gameObject, unitySprite.GetComponent<Renderer>(), ref polygon, ref polygonPoints, minTextureCoords, maxTextureCoords, unitySprite.sprite.bounds, material, false, false, false, Vector2.zero, pivotVector, isConcave);
        m_ParentInstanceID = unitySprite.gameObject.GetInstanceID();
    }

    /// <summary>
    /// Initialise this sprite using the given polygon definition
    /// </summary>
	void InitSprite(GameObject parentObject, Renderer parentRenderer, ref PolygonCollider2D polygon, ref Vector2[] polygonPoints, Vector3 minCoords, Vector3 maxCoords, Bounds spriteBounds, Material material, bool rotated, bool hFlipped, bool vFlipped, Vector2 parentCentroid, Vector2 uvOffset, bool isConcave)
    {
        m_MinCoords = minCoords;
        m_MaxCoords = maxCoords;
        m_SpriteBounds = spriteBounds;
        m_VFlipped = vFlipped;
        m_HFlipped = hFlipped;
        m_Rotated = rotated;
        m_SpriteBounds = spriteBounds;
		m_UVOffset = uvOffset;

        gameObject.tag = parentObject.tag;
        gameObject.layer = parentObject.layer;

        Mesh spriteMesh = new Mesh();
        spriteMesh.name = "SlicedSpriteMesh";
        m_MeshFilter.mesh = spriteMesh;

        int numVertices = polygonPoints.Length;
        Vector3[] vertices = new Vector3[numVertices];
        Color[] colors = new Color[numVertices];
        Vector2[] uvs = new Vector2[numVertices];
		int[] triangles;

        // Convert vector2 -> vector3
        for (int loop = 0; loop < vertices.Length; loop++)
        {
            vertices[loop] = polygonPoints[loop];
            colors[loop] = Color.white;
        }

        Vector2 uvWidth = maxCoords - minCoords;
        Vector3 boundsSize = spriteBounds.size;
        Vector2 invBoundsSize = new Vector2(1.0f / boundsSize.x, 1.0f / boundsSize.y);

        for (int vertexIndex = 0; vertexIndex < numVertices; vertexIndex++)
        {
			Vector2 vertex = polygonPoints[vertexIndex] + parentCentroid;
			float widthFraction = 0.5f + ((vertex.x * invBoundsSize.x) + (uvOffset.x));
			float heightFraction = 0.5f + ((vertex.y * invBoundsSize.y) + (uvOffset.y));            

            if (hFlipped)
            {
                widthFraction = 1.0f - widthFraction;
            }

            if (vFlipped)
            {
                heightFraction = 1.0f - heightFraction;
            }

            Vector2 texCoords = new Vector2();

            if (rotated)
            {
                texCoords.y = maxCoords.y - (uvWidth.y * (1.0f - widthFraction));
                texCoords.x = minCoords.x + (uvWidth.x * heightFraction);
            }
            else
            {
                texCoords.x = minCoords.x + (uvWidth.x * widthFraction);
                texCoords.y = minCoords.y + (uvWidth.y * heightFraction);
            }

            uvs[vertexIndex] = texCoords;
        }

		if (isConcave) 
		{
			List<Vector2> polyPointList = new List<Vector2>(polygonPoints);
			triangles = SpriteSlicer2D.Triangulate(ref polyPointList);
		}
		else
		{
			int triangleIndex = 0;
			triangles = new int[numVertices * 3];
			
			for (int vertexIndex = 1; vertexIndex < numVertices - 1; vertexIndex++)
			{
				triangles[triangleIndex++] = 0;
				triangles[triangleIndex++] = vertexIndex + 1;
				triangles[triangleIndex++] = vertexIndex;
			}
		}
		
        spriteMesh.Clear();
        spriteMesh.vertices = vertices;
        spriteMesh.uv = uvs;
        spriteMesh.triangles = triangles;
        spriteMesh.colors = colors;
        spriteMesh.RecalculateBounds();
        spriteMesh.RecalculateNormals();
        ;

        Vector2 localCentroid = Vector3.zero;

        if (SpriteSlicer2D.s_CentreChildSprites)
        {
            localCentroid = spriteMesh.bounds.center;

            // Finally, fix up our mesh, collider, and object position to at the same position as the pivot point
            for (int vertexIndex = 0; vertexIndex < numVertices; vertexIndex++)
            {
                vertices[vertexIndex] -= (Vector3)localCentroid;
            }

            for (int vertexIndex = 0; vertexIndex < numVertices; vertexIndex++)
            {
                polygonPoints[vertexIndex] -= localCentroid;
            }

            m_Centroid = localCentroid + parentCentroid;
            polygon.points = polygonPoints;
            spriteMesh.vertices = vertices;
            spriteMesh.RecalculateBounds();
        }

        Transform parentTransform = parentObject.transform;
        m_Transform.parent = parentTransform.parent;
        m_Transform.position = parentTransform.position + (parentTransform.rotation * (Vector3)localCentroid);
        m_Transform.rotation = parentTransform.rotation;
        m_Transform.localScale = parentTransform.localScale;
        m_MeshRenderer.material = material;

        m_MeshRenderer.sortingLayerID = parentRenderer.sortingLayerID;
        m_MeshRenderer.sortingOrder = parentRenderer.sortingOrder;
    }
}