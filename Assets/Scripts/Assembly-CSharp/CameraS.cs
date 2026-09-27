using System;
using System.Collections.Generic;
using UnityEngine;

public static class CameraS
{
	public static DynamicArray<CameraLayer> m_cameraLayers = new DynamicArray<CameraLayer>(16, 0f, 0f, 1f);

	public static List<Camera> m_cameras;

	private static bool m_debugDraw = false;

	public static Camera m_mainCamera;

	public static Camera m_uiCamera;

	public static float m_mainCameraDistanceMultipler;

	public static float m_mainCameraPositionSlop = 0.25f;

	public static float m_mainCameraMaxVelocity = 15f;

	public static Vector2 m_mainCameraMaxAngle = Vector2.one * 10f;

	public static float m_mainCameraFov = 40f;

	public static float m_mainCameraDistance = 500f;

	public static float m_mainCameraTargetX = 0f;

	public static float m_mainCameraTargetY = 0f;

	public static Vector3 m_mainCameraAngle = Vector3.zero;

	public static Vector3 m_mainCameraPosition = Vector3.zero;

	public static float m_mainCameraNearClip = 10f;

	public static float m_mainCameraFarClip = 50000f;

	public static float m_uiCameraNearClip = 1f;

	public static float m_uiCameraFarClip = 1000f;

	public static cpBB m_cameraBB;

	public static int m_mainCameraLayer = 8;

	public static int m_mainCameraCullingMask = 1792;

	public static int m_uiCameraLayer = 15;

	public static int m_otherCamerasStartLayer = 16;

	public static DynamicArray<CameraTargetC> m_cameraTargetComponents;

	public static DynamicArray<CameraEffectC> m_cameraEffectComponents;

	public static DynamicArray<CameraBorderC> m_cameraBorderComponents;

	public static TransformC m_mainCameraTC;

	public static TransformC m_mainCameraRotateTC;

	public static TransformC m_debugTC;

	public static TransformC m_debugTC2;

	public static int m_screenWidth = 0;

	public static int m_screenHeight = 0;

	public static bool m_updateComponents = true;

	public static void Initialize()
	{
		m_screenWidth = Screen.width;
		m_screenHeight = Screen.height;
		Entity entity = EntityManager.AddEntity();
		entity.m_persistent = true;
		if (m_debugDraw)
		{
			m_debugTC = TransformS.AddComponent(entity, "Main Camera Debug Transform");
			m_debugTC2 = TransformS.AddComponent(entity, "Main Camera Debug Transform");
		}
		m_mainCameraTC = TransformS.AddComponent(entity, "Main Camera Transform");
		m_mainCameraRotateTC = TransformS.AddComponent(entity, "Main Camera Rotate Transform");
		m_mainCamera = Camera.main;
		m_mainCamera.cullingMask = m_mainCameraCullingMask;
		m_mainCamera.gameObject.layer = m_mainCameraLayer;
		m_mainCamera.fieldOfView = m_mainCameraFov;
		m_mainCamera.farClipPlane = m_mainCameraFarClip;
		m_mainCamera.nearClipPlane = m_mainCameraNearClip;
		m_mainCamera.fieldOfView = m_mainCameraFov;
		m_mainCamera.backgroundColor = Color.black;
		m_mainCamera.gameObject.AddComponent<Screenshot>();
		for (int i = 0; i < m_mainCamera.transform.childCount; i++)
		{
			Transform child = m_mainCamera.transform.GetChild(i);
			child.gameObject.layer = m_mainCamera.gameObject.layer;
		}
		m_mainCamera.transform.parent = m_mainCameraTC.transform;
		m_mainCameraTC.transform.parent = m_mainCameraRotateTC.transform;
		m_mainCamera.transform.localPosition = Vector3.forward * (0f - m_mainCameraDistance);
		m_mainCamera.transform.localRotation = Quaternion.identity;
		m_mainCameraRotateTC.transform.localPosition = Vector3.zero;
		m_mainCameraRotateTC.transform.localRotation = Quaternion.Euler(m_mainCameraAngle);
		GameObject gameObject = new GameObject("UI Camera");
		m_uiCamera = gameObject.AddComponent("Camera") as Camera;
		m_uiCamera.orthographic = true;
		m_uiCamera.orthographicSize = (float)Screen.height * 0.5f;
		m_uiCamera.depth = 1f;
		m_uiCamera.cullingMask = 1 << m_uiCameraLayer;
		m_uiCamera.gameObject.layer = m_uiCameraLayer;
		m_uiCamera.nearClipPlane = m_uiCameraNearClip;
		m_uiCamera.farClipPlane = m_uiCameraFarClip;
		m_uiCamera.gameObject.transform.position = new Vector3(0f, 0f, -500f);
		m_uiCamera.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		m_uiCamera.clearFlags = CameraClearFlags.Nothing;
		m_cameraTargetComponents = new DynamicArray<CameraTargetC>();
		m_cameraEffectComponents = new DynamicArray<CameraEffectC>();
		m_cameraBorderComponents = new DynamicArray<CameraBorderC>();
		m_cameraBB = ChipmunkProWrapper.ucpBBNew((float)Screen.width * -0.5f, (float)Screen.height * -0.5f, (float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		m_cameras = new List<Camera>();
		m_cameras.Add(m_mainCamera);
		m_cameras.Add(m_uiCamera);
	}

	public static void ResetMainCamera(Vector2 _position, float _zoom = 500f)
	{
		m_mainCameraDistance = _zoom;
		m_mainCameraTargetX = _position.x;
		m_mainCameraTargetY = _position.y;
		m_mainCameraAngle = Vector3.zero;
		m_mainCameraPosition = _position;
		m_cameraBB = ChipmunkProWrapper.ucpBBNew((float)Screen.width * -0.5f, (float)Screen.height * -0.5f, (float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		m_mainCameraRotateTC.transform.position = m_mainCameraPosition;
		m_mainCameraRotateTC.transform.position = m_mainCameraPosition;
		m_mainCameraTC.transform.localPosition = Vector3.zero;
	}

	public static Camera AddCamera(string _name, bool _ortographic)
	{
		GameObject gameObject = new GameObject(_name);
		Camera camera = gameObject.AddComponent("Camera") as Camera;
		camera.depth = 1f;
		camera.gameObject.transform.position = new Vector3(0f, 0f, -500f);
		camera.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		camera.clearFlags = CameraClearFlags.Nothing;
		CameraLayer cameraLayer = m_cameraLayers.AddItem();
		int num = m_otherCamerasStartLayer + cameraLayer.m_index;
		if (_ortographic)
		{
			camera.orthographic = _ortographic;
			camera.orthographicSize = (float)Screen.height * 0.5f;
			camera.cullingMask = 1 << num;
			camera.gameObject.layer = num;
			camera.nearClipPlane = m_uiCameraNearClip;
			camera.farClipPlane = m_uiCameraFarClip;
			m_cameras.Add(camera);
		}
		else
		{
			camera.orthographic = false;
			camera.cullingMask = 1 << num;
			camera.gameObject.layer = num;
			camera.nearClipPlane = m_mainCameraNearClip;
			camera.farClipPlane = m_mainCameraFarClip;
			for (int i = 0; i < m_cameras.Count; i++)
			{
				if (m_cameras[i] == m_uiCamera)
				{
					m_cameras.Insert(i, camera);
					break;
				}
			}
		}
		return camera;
	}

	public static void RemoveCamera(Camera _camera)
	{
		if (_camera != null && _camera != m_uiCamera && _camera != m_mainCamera)
		{
			m_cameraLayers.RemoveItem(_camera.gameObject.layer - m_otherCamerasStartLayer);
			m_cameras.Remove(_camera);
			UnityEngine.Object.DestroyImmediate(_camera.gameObject);
		}
	}

	public static CameraTargetC AddTargetComponent(TransformC _tc, float _width, float _height)
	{
		CameraTargetC cameraTargetC = m_cameraTargetComponents.AddItem();
		cameraTargetC.TC = TransformS.AddComponent(_tc.p_entity, "Camera Target Transform");
		cameraTargetC.TC.forceRotation = true;
		cameraTargetC.TC.forceScale = true;
		TransformS.ParentComponent(cameraTargetC.TC, _tc, Vector3.zero);
		cameraTargetC.bb = ChipmunkProWrapper.ucpBBNew(_width * -0.5f, _height * -0.5f, _width * 0.5f, _height * 0.5f);
		EntityManager.AddComponentToEntity(_tc.p_entity, cameraTargetC);
		return cameraTargetC;
	}

	public static void SetTargetBB(CameraTargetC _ct, float _width, float _height)
	{
		_ct.bb = ChipmunkProWrapper.ucpBBNew(_width * -0.5f, _height * -0.5f, _width * 0.5f, _height * 0.5f);
	}

	public static void RemoveAllTargetComponents()
	{
		while (m_cameraTargetComponents.m_aliveCount > 0)
		{
			CameraTargetC c = m_cameraTargetComponents.m_array[m_cameraTargetComponents.m_aliveIndices[0]];
			RemoveTargetComponent(c);
		}
	}

	public static void RemoveTargetComponent(CameraTargetC _c)
	{
		EntityManager.RemoveComponentFromEntity(_c);
		m_cameraTargetComponents.RemoveItem(_c);
	}

	public static CameraBorderC AddBorderComponent(TransformC _tc, CameraBorderType _border, float _offset)
	{
		CameraBorderC cameraBorderC = m_cameraBorderComponents.AddItem();
		cameraBorderC.TC = _tc;
		switch (_border)
		{
		case CameraBorderType.Left:
			cameraBorderC.hasLeft = true;
			cameraBorderC.leftOffset = _offset;
			break;
		case CameraBorderType.Right:
			cameraBorderC.hasRight = true;
			cameraBorderC.rightOffset = _offset;
			break;
		case CameraBorderType.Top:
			cameraBorderC.hasTop = true;
			cameraBorderC.topOffset = _offset;
			break;
		case CameraBorderType.Bottom:
			cameraBorderC.hasBottom = true;
			cameraBorderC.bottomOffset = _offset;
			break;
		}
		EntityManager.AddComponentToEntity(_tc.p_entity, cameraBorderC);
		return cameraBorderC;
	}

	public static void RemoveBorderComponent(CameraBorderC _c)
	{
		EntityManager.RemoveComponentFromEntity(_c);
		m_cameraBorderComponents.RemoveItem(_c);
	}

	public static CameraEffectC AddEffectComponent(Camera _camera, Entity _e)
	{
		CameraEffectC cameraEffectC = m_cameraEffectComponents.AddItem();
		EntityManager.AddComponentToEntity(_e, cameraEffectC);
		return cameraEffectC;
	}

	public static void RemoveEffectComponent(CameraEffectC _c)
	{
		EntityManager.RemoveComponentFromEntity(_c);
		m_cameraEffectComponents.RemoveItem(_c);
	}

	public static cpBB TransformBB(cpBB _bb, Vector3 _pos, float _scale)
	{
		_bb.l = _pos.x + _bb.l * _scale;
		_bb.r = _pos.x + _bb.r * _scale;
		_bb.b = _pos.y + _bb.b * _scale;
		_bb.t = _pos.y + _bb.t * _scale;
		return _bb;
	}

	public static void Update()
	{
		if (Screen.width != m_screenWidth || Screen.height != m_screenHeight)
		{
			m_uiCamera.orthographicSize = (float)Screen.height * 0.5f;
			m_screenWidth = Screen.width;
			m_screenHeight = Screen.height;
			UIManager.ScreenSizeChanged();
		}
		Vector3 vector = -m_mainCamera.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f + 1f, (float)Screen.height * 0.5f, m_mainCamera.transform.position.z));
		m_mainCameraDistanceMultipler = 1f / (vector + m_mainCamera.transform.position).x;
		if (m_updateComponents)
		{
			if (m_debugDraw)
			{
				DebugDraw.Clear(m_mainCamera, m_debugTC);
				DebugDraw.Clear(m_mainCamera, m_debugTC2);
			}
			float num = 999999f;
			float num2 = -999999f;
			float num3 = -999999f;
			float num4 = 999999f;
			int aliveCount = m_cameraBorderComponents.m_aliveCount;
			for (int i = 0; i < aliveCount; i++)
			{
				CameraBorderC cameraBorderC = m_cameraBorderComponents.m_array[m_cameraBorderComponents.m_aliveIndices[i]];
				if (!cameraBorderC.m_active)
				{
					continue;
				}
				if (cameraBorderC.hasLeft)
				{
					float num5 = cameraBorderC.TC.transform.position.x + cameraBorderC.leftOffset;
					if (num5 < num)
					{
						num = num5;
					}
				}
				if (cameraBorderC.hasRight)
				{
					float num6 = cameraBorderC.TC.transform.position.x + cameraBorderC.rightOffset;
					if (num6 > num2)
					{
						num2 = num6;
					}
				}
				if (cameraBorderC.hasTop)
				{
					float num7 = cameraBorderC.TC.transform.position.y + cameraBorderC.topOffset;
					if (num7 > num3)
					{
						num3 = num7;
					}
				}
				if (cameraBorderC.hasBottom)
				{
					float num8 = cameraBorderC.TC.transform.position.y + cameraBorderC.bottomOffset;
					if (num8 < num4)
					{
						num4 = num8;
					}
				}
			}
			if (m_debugDraw)
			{
				DebugDraw.CreateLine(m_mainCamera, m_debugTC, new Vector2(num, num4), new Vector2(num, num3));
				DebugDraw.CreateLine(m_mainCamera, m_debugTC, new Vector2(num2, num4), new Vector2(num2, num3));
				DebugDraw.CreateLine(m_mainCamera, m_debugTC, new Vector2(num, num3), new Vector2(num2, num3));
				DebugDraw.CreateLine(m_mainCamera, m_debugTC, new Vector2(num, num4), new Vector2(num2, num4));
				SpriteS.SetColorByTransformComponent(m_debugTC, Color.red);
			}
			int num9 = 0;
			Vector3 zero = Vector3.zero;
			aliveCount = m_cameraTargetComponents.m_aliveCount;
			for (int j = 0; j < aliveCount; j++)
			{
				CameraTargetC cameraTargetC = m_cameraTargetComponents.m_array[m_cameraTargetComponents.m_aliveIndices[j]];
				if (cameraTargetC.m_active)
				{
					float num10 = cameraTargetC.bb.r - cameraTargetC.bb.l;
					float num11 = cameraTargetC.bb.t - cameraTargetC.bb.b;
					Vector2 vector2 = cameraTargetC.TC.transform.position - cameraTargetC.prevPos;
					Vector2 normalized = vector2.normalized;
					normalized.Scale(new Vector2(num10, num11) * 0.5f);
					float num12 = Mathf.Abs(vector2.x);
					float num13 = Mathf.Abs(vector2.y);
					Vector2 zero2 = Vector2.zero;
					if (num12 > cameraTargetC.lowVelocity.x)
					{
						zero2.x = Mathf.Min(1f - cameraTargetC.safeFrame, (num12 - cameraTargetC.lowVelocity.x) / (cameraTargetC.highVelocity.x - cameraTargetC.lowVelocity.x));
					}
					if (num13 > cameraTargetC.lowVelocity.y)
					{
						zero2.y = Mathf.Min(1f - cameraTargetC.safeFrame, (num13 - cameraTargetC.lowVelocity.y) / (cameraTargetC.highVelocity.y - cameraTargetC.lowVelocity.y));
					}
					Vector2 zero3 = Vector2.zero;
					if (num12 > cameraTargetC.lowScaleVelocity.x)
					{
						zero3.x = Mathf.Min(1f - cameraTargetC.safeFrame, (num12 - cameraTargetC.lowScaleVelocity.x) / (cameraTargetC.highScaleVelocity.x - cameraTargetC.lowScaleVelocity.x));
					}
					if (num13 > cameraTargetC.lowScaleVelocity.y)
					{
						zero3.y = Mathf.Min(1f - cameraTargetC.safeFrame, (num13 - cameraTargetC.lowScaleVelocity.y) / (cameraTargetC.highScaleVelocity.y - cameraTargetC.lowScaleVelocity.y));
					}
					float num14 = 1f + zero3.magnitude * cameraTargetC.velocityScale;
					float num15 = (num14 - cameraTargetC.scale) * 0.1f;
					if (num15 > cameraTargetC.maxScaleChange)
					{
						num15 = cameraTargetC.maxScaleChange;
					}
					else if (num15 < 0f - cameraTargetC.maxScaleChange)
					{
						num15 = 0f - cameraTargetC.maxScaleChange;
					}
					cameraTargetC.scale += num15;
					num10 = cameraTargetC.scale * num10;
					num11 = cameraTargetC.scale * num11;
					Vector3 vector3 = new Vector3(num10 * -0.5f * cameraTargetC.horizontalOffset, num11 * -0.5f * cameraTargetC.verticalOffset);
					Vector3 vector4 = new Vector3(normalized.x * zero2.x, normalized.y * zero2.y, 0f);
					Vector3 vector5 = (vector3 + vector4 - cameraTargetC.offset) * 0.05f;
					if (vector5.x > cameraTargetC.maxOffsetChange)
					{
						vector5.x = cameraTargetC.maxOffsetChange;
					}
					else if (vector5.x < 0f - cameraTargetC.maxOffsetChange)
					{
						vector5.x = 0f - cameraTargetC.maxOffsetChange;
					}
					if (vector5.y > cameraTargetC.maxOffsetChange)
					{
						vector5.y = cameraTargetC.maxOffsetChange;
					}
					else if (vector5.y < 0f - cameraTargetC.maxOffsetChange)
					{
						vector5.y = 0f - cameraTargetC.maxOffsetChange;
					}
					cameraTargetC.offset += vector5;
					if (m_debugDraw)
					{
						DebugDraw.Clear(m_mainCamera, cameraTargetC.TC);
						DebugDraw.CreateBox(m_mainCamera, cameraTargetC.TC, cameraTargetC.offset, num10, num11);
						SpriteS.SetColorByTransformComponent(cameraTargetC.TC, Color.cyan);
					}
					cpBB cpBB2 = TransformBB(cameraTargetC.bb, cameraTargetC.TC.transform.position + cameraTargetC.offset, cameraTargetC.scale);
					if (num < 999999f && num > cpBB2.l)
					{
						cpBB2.l = num;
					}
					if (num2 > -999999f && num2 < cpBB2.r)
					{
						cpBB2.r = num2;
					}
					if (num4 < 999999f && num4 > cpBB2.b)
					{
						cpBB2.b = num4;
					}
					if (num3 > -999999f && num3 < cpBB2.t)
					{
						cpBB2.t = num3;
					}
					if (num9 == 0)
					{
						m_cameraBB = cpBB2;
					}
					else
					{
						m_cameraBB = ChipmunkProWrapper.ucpBBMerge(m_cameraBB, cpBB2);
					}
					Vector3 vector6 = new Vector3(cameraTargetC.verticalAngle, cameraTargetC.horizontalAngle, 0f);
					Vector3 vector7 = new Vector3(-20f * zero3.y * normalized.y, 20f * zero3.x * normalized.x, 0f);
					Vector3 vector8 = (vector6 + vector7 - m_mainCameraAngle) * 0.05f;
					if (vector8.x > cameraTargetC.maxAngleChange.x)
					{
						vector8.x = cameraTargetC.maxAngleChange.x;
					}
					else if (vector8.x < 0f - cameraTargetC.maxAngleChange.x)
					{
						vector8.x = 0f - cameraTargetC.maxAngleChange.x;
					}
					if (vector8.y > cameraTargetC.maxAngleChange.y)
					{
						vector8.y = cameraTargetC.maxAngleChange.y;
					}
					else if (vector8.y < 0f - cameraTargetC.maxAngleChange.y)
					{
						vector8.y = 0f - cameraTargetC.maxAngleChange.y;
					}
					zero += vector8;
					cameraTargetC.prevPos = cameraTargetC.TC.transform.position;
					num9++;
				}
			}
			Vector2 mainCameraMaxAngle = m_mainCameraMaxAngle;
			if (num9 > 1)
			{
				zero /= (float)num9;
				mainCameraMaxAngle /= (float)num9;
			}
			m_mainCameraAngle += zero;
			if (m_mainCameraAngle.x > mainCameraMaxAngle.x)
			{
				m_mainCameraAngle.x = mainCameraMaxAngle.x;
			}
			else if (m_mainCameraAngle.x < 0f - mainCameraMaxAngle.x)
			{
				m_mainCameraAngle.x = 0f - mainCameraMaxAngle.x;
			}
			if (m_mainCameraAngle.y > mainCameraMaxAngle.y)
			{
				m_mainCameraAngle.y = mainCameraMaxAngle.y;
			}
			else if (m_mainCameraAngle.y < 0f - mainCameraMaxAngle.y)
			{
				m_mainCameraAngle.y = 0f - mainCameraMaxAngle.y;
			}
			float num16 = (m_cameraBB.r - m_cameraBB.l) * 0.5f;
			float num17 = (m_cameraBB.t - m_cameraBB.b) * 0.5f;
			m_mainCameraPosition.x = m_cameraBB.l + num16;
			m_mainCameraPosition.y = m_cameraBB.b + num17;
			if (m_debugDraw)
			{
				DebugDraw.CreateBox(m_mainCamera, m_debugTC2, m_mainCameraPosition, num16 * 2f, num17 * 2f);
				SpriteS.SetColorByTransformComponent(m_debugTC2, Color.green);
			}
			float num18 = (float)Screen.width / (float)Screen.height;
			float a = (0f - 1f / Mathf.Tan(m_mainCamera.fieldOfView * 0.5f * ((float)Math.PI / 180f))) * num17;
			float b = (0f - 1f / Mathf.Tan(m_mainCamera.fieldOfView * num18 * 0.5f * ((float)Math.PI / 180f))) * num16;
			Vector3 position = m_mainCameraRotateTC.transform.position;
			Vector3 vector9 = (m_mainCameraPosition - position) * m_mainCameraPositionSlop;
			if (vector9.magnitude > m_mainCameraMaxVelocity)
			{
				vector9 = vector9.normalized * m_mainCameraMaxVelocity;
			}
			m_mainCameraPosition = position + vector9;
			TransformS.SetPosition(m_mainCameraRotateTC, m_mainCameraPosition);
			TransformS.SetRotation(m_mainCameraRotateTC, m_mainCameraAngle);
			m_mainCamera.transform.localPosition = Vector3.forward * Mathf.Min(a, b);
		}
		m_cameraBorderComponents.Update();
		m_cameraEffectComponents.Update();
		m_cameraTargetComponents.Update();
	}
}
