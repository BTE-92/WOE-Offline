using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class ToolBox
{
	public static float lastOutDistLine1;

	public static float lastOutDistLine2;

	public static Vector2 lastLineIntersectionPoint;

	public static Vector3 outVector3;

	public static bool IsZero(float f)
	{
		return (double)Mathf.Abs(f) <= 0.0001;
	}

	public static bool IsLess(float f1, float f2)
	{
		return f1 < f2 && !IsZero(f1 - f2);
	}

	public static bool IsGreater(float f1, float f2)
	{
		return f1 > f2 && !IsZero(f1 - f2);
	}

	public static string GetTimestamp(DateTime value)
	{
		return value.ToString("yyyyMMddHHmmssffff");
	}

	public static bool linesIntersect(Vector2 p11, Vector2 p12, Vector2 p21, Vector2 p22)
	{
		lastOutDistLine1 = float.MaxValue;
		lastOutDistLine2 = float.MaxValue;
		float num = (p22.y - p21.y) * (p12.x - p11.x) - (p22.x - p21.x) * (p12.y - p11.y);
		if (IsZero(num))
		{
			return false;
		}
		float num2 = (p22.x - p21.x) * (p11.y - p21.y) - (p22.y - p21.y) * (p11.x - p21.x);
		float num3 = (p12.x - p11.x) * (p11.y - p21.y) - (p12.y - p11.y) * (p11.x - p21.x);
		lastOutDistLine1 = num2 / num;
		lastOutDistLine2 = num3 / num;
		return true;
	}

	public static bool lineSegmentsIntersect(Vector2 p11, Vector2 p12, Vector2 p21, Vector2 p22, bool getIntersectionPoint)
	{
		if (linesIntersect(p11, p12, p21, p22))
		{
			bool flag = IsLess(lastOutDistLine1, 1f) && IsLess(lastOutDistLine2, 1f) && IsGreater(lastOutDistLine1, 0f) && IsGreater(lastOutDistLine2, 0f);
			if (flag && getIntersectionPoint)
			{
				float num = p11.x - p12.x;
				float num2 = p11.y - p12.y;
				num *= lastOutDistLine1;
				num2 *= lastOutDistLine1;
				lastLineIntersectionPoint.x = p11.x - num;
				lastLineIntersectionPoint.y = p11.y - num2;
			}
			return flag;
		}
		return false;
	}

	public static bool DoLinesIntersect(Vector2 _l1a, Vector2 _l1b, Vector2 _l2a, Vector2 _l2b, ref Vector2 _pos)
	{
		float num = (_l2b.y - _l2a.y) * (_l1b.x - _l1a.x) - (_l2b.x - _l2a.x) * (_l1b.y - _l1a.y);
		if (num == 0f)
		{
			return false;
		}
		float num2 = (_l2b.x - _l2a.x) * (_l1a.y - _l2a.y) - (_l2b.y - _l2a.y) * (_l1a.x - _l2a.x);
		float num3 = (_l1b.x - _l1a.x) * (_l1a.y - _l2a.y) - (_l1b.y - _l1a.y) * (_l1a.x - _l2a.x);
		float num4 = num2 / num;
		float num5 = num3 / num;
		if (num4 >= 0f && num4 <= 1f && num5 >= 0f && num5 <= 1f)
		{
			_pos.x = _l1a.x + num4 * (_l1b.x - _l1a.x);
			_pos.y = _l1a.y + num4 * (_l1b.y - _l1a.y);
			return true;
		}
		return false;
	}

	public static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
	}

	public static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
	{
		bool flag = Sign(pt, v1, v2) < 0f;
		bool flag2 = Sign(pt, v2, v3) < 0f;
		bool flag3 = Sign(pt, v3, v1) < 0f;
		return flag == flag2 && flag2 == flag3;
	}

	public static float getPositionBetween(float val, float min, float max)
	{
		return limitBetween((val - min) / (max - min), 0f, 1f);
	}

	public static float limitBetween(float val, float min, float max)
	{
		if (val < min)
		{
			val = min;
		}
		if (val > max)
		{
			val = max;
		}
		return val;
	}

	public static int limitBetween(int val, int min, int max)
	{
		if (val < min)
		{
			val = min;
		}
		if (val > max)
		{
			val = max;
		}
		return val;
	}

	public static float getCappedAngle(float angle)
	{
		return getRolledValue(angle, 0f, 360f);
	}

	public static float getRolledValue(float val, float minValue, float maxValue)
	{
		if (maxValue == minValue)
		{
			val = minValue;
		}
		float num = Mathf.Floor((val - minValue) / (maxValue - minValue));
		val -= num * (maxValue - minValue);
		return val;
	}

	public static int getRolledValue(int val, int minValue, int maxValue)
	{
		maxValue++;
		if (maxValue == minValue)
		{
			val = minValue;
		}
		int num = (int)Mathf.Floor((float)(val - minValue) / (float)(maxValue - minValue));
		val -= num * (maxValue - minValue);
		return val;
	}

	public static Vector3 interpolateVector3(Vector3 fromVector, Vector3 toVector, float pos)
	{
		Vector3 vector = toVector - fromVector;
		return fromVector + vector * pos;
	}

	public static float getAngleFromVector2(Vector2 vec)
	{
		return Mathf.Atan2(vec.y, vec.x);
	}

	public static float interpolateAngles(float angCur, float angNew, float pos)
	{
		float num = angNew - angCur;
		if (Mathf.Abs(num) > 180f)
		{
			num -= Mathf.Sign(num) * 360f;
		}
		return getCappedAngle(angCur + num * pos);
	}

	public static int[] sortTable(int[] table, float[] keys)
	{
		int num = table.Length;
		for (int i = 1; i < num; i++)
		{
			int num2 = table[i];
			float num3 = keys[i];
			int num4 = i;
			while (num4 > 0 && keys[num4 - 1] > num3)
			{
				table[num4] = table[num4 - 1];
				keys[num4] = keys[num4 - 1];
				num4--;
			}
			table[num4] = num2;
			keys[num4] = num3;
		}
		return table;
	}

	public static void sortMeshOnZAxis(Mesh mesh)
	{
		int[] triangles = mesh.GetTriangles(0);
		float[] array = new float[triangles.Length / 3];
		for (int i = 0; i < array.Length; i++)
		{
			int num = i * 3;
			int num2 = triangles[num];
			int num3 = triangles[num + 1];
			int num4 = triangles[num + 2];
			float num5 = (mesh.vertices[num2].z + mesh.vertices[num3].z + mesh.vertices[num4].z) / 3f;
			array[i] = 0f - num5;
		}
		triangles = sortTriangles(triangles, array);
		mesh.SetTriangles(triangles, 0);
	}

	public static Vector2 calculateNormal(Vector2 point1, Vector2 point2)
	{
		Vector2 vector = point2 - point1;
		return new Vector2(0f - vector.y, vector.x).normalized;
	}

	public static float getDistanceToCamera(Vector3 pos, Camera cam, bool useZ)
	{
		Vector3 position = cam.transform.position;
		if (!useZ)
		{
			pos.z = 0f;
			position.z = 0f;
		}
		return (pos - position).magnitude;
	}

	public static int[] sortTriangles(int[] triangles, float[] keys)
	{
		int num = triangles.Length / 3;
		Triangle[] array = new Triangle[num];
		for (int i = 0; i < num; i++)
		{
			int num2 = i * 3;
			array[i].v1 = triangles[num2];
			array[i].v2 = triangles[num2 + 1];
			array[i].v3 = triangles[num2 + 2];
		}
		int num3 = array.Length;
		for (int j = 1; j < num3; j++)
		{
			Triangle triangle = array[j];
			float num4 = keys[j];
			int num5 = j;
			while (num5 > 0 && keys[num5 - 1] > num4)
			{
				array[num5] = array[num5 - 1];
				keys[num5] = keys[num5 - 1];
				num5--;
			}
			array[num5] = triangle;
			keys[num5] = num4;
		}
		for (int k = 0; k < num; k++)
		{
			int num6 = k * 3;
			triangles[num6] = array[k].v1;
			triangles[num6 + 1] = array[k].v2;
			triangles[num6 + 2] = array[k].v3;
		}
		return triangles;
	}

	public static Vector2[] shuffleArray(Vector2[] array)
	{
		for (int num = array.Length; num > 1; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num);
			Vector2 vector = array[num2];
			array[num2] = array[num - 1];
			array[num - 1] = vector;
		}
		return array;
	}

	public static char[] shuffleArray(char[] array)
	{
		for (int num = array.Length; num > 1; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num);
			char c = array[num2];
			array[num2] = array[num - 1];
			array[num - 1] = c;
		}
		return array;
	}

	public static string Md5Sum(string strToEncrypt)
	{
		UTF8Encoding uTF8Encoding = new UTF8Encoding();
		byte[] bytes = uTF8Encoding.GetBytes(strToEncrypt);
		MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
		byte[] array = mD5CryptoServiceProvider.ComputeHash(bytes);
		string text = string.Empty;
		for (int i = 0; i < array.Length; i++)
		{
			text += Convert.ToString(array[i], 16).PadLeft(2, '0');
		}
		return text.PadLeft(32, '0');
	}

	public static string getTimeStringFromSeconds(float seconds)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
		if (timeSpan.Minutes > 0)
		{
			return string.Format("{0:D2}:{1:D2}.{2:D3}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
		}
		return string.Format("{0:D2}.{1:D3}", timeSpan.Seconds, timeSpan.Milliseconds);
	}

	public static bool CircleSegmentIntersect(Vector3 circlePos, float radius, Vector3 segmentP1, Vector3 segmentP2)
	{
		Vector3 lhs = circlePos - segmentP1;
		Vector3 vector = segmentP2 - segmentP1;
		float num = Vector3.Dot(vector, vector);
		float num2 = Vector3.Dot(lhs, vector);
		float num3 = num2 / num;
		if (num3 < 0f)
		{
			num3 = 0f;
		}
		else if (num3 > 1f)
		{
			num3 = 1f;
		}
		outVector3 = segmentP1 + num3 * vector;
		Vector3 vector2 = outVector3 - circlePos;
		float num4 = Vector3.Dot(vector2, vector2);
		float num5 = radius * radius;
		if (num4 > num5)
		{
			return false;
		}
		return true;
	}

	public static Vector2 PointOnBezierSpline(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		Vector2 result = default(Vector2);
		float num = t * t;
		float num2 = num * t;
		result.x = 0.5f * (2f * p1.x + (0f - p0.x + p2.x) * t + (2f * p0.x - 5f * p1.x + 4f * p2.x - p3.x) * num + (0f - p0.x + 3f * p1.x - 3f * p2.x + p3.x) * num2);
		result.y = 0.5f * (2f * p1.y + (0f - p0.y + p2.y) * t + (2f * p0.y - 5f * p1.y + 4f * p2.y - p3.y) * num + (0f - p0.y + 3f * p1.y - 3f * p2.y + p3.y) * num2);
		return result;
	}

	public static Vector2 TangentOnBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		float num = t * t;
		return -3f * Mathf.Pow(1f - t, 2f) * p0 + 3f * Mathf.Pow(1f - t, 2f) * p1 - 6f * t * (1f - t) * p1 - 3f * num * p2 + 6f * t * (1f - t) * p2 + 3f * num * p3;
	}

	public static float LengthOfBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		Vector2 vector = default(Vector2);
		vector.x = p0.x - 2f * p1.x + p2.x;
		vector.y = p0.y - 2f * p1.y + p2.y;
		Vector2 vector2 = default(Vector2);
		vector2.x = 2f * p1.x - 2f * p0.x;
		vector2.y = 2f * p1.y - 2f * p0.y;
		float num = 4f * (vector.x * vector.x + vector.y * vector.y);
		float num2 = 4f * (vector.x * vector2.x + vector.y * vector2.y);
		float num3 = vector2.x * vector2.x + vector2.y * vector2.y;
		float num4 = 2f * Mathf.Sqrt(num + num2 + num3);
		float num5 = Mathf.Sqrt(num);
		float num6 = 2f * num * num5;
		float num7 = 2f * Mathf.Sqrt(num3);
		float num8 = num2 / num5;
		return (num6 * num4 + num5 * num2 * (num4 - num7) + (4f * num3 * num - num2 * num2) * Mathf.Log((2f * num5 + num8 + num4) / (num8 + num7))) / (4f * num6);
	}

	public static Vector2 GetCenterOfMass(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		float num = Mathf.Pow(1f - t, 3f);
		float num2 = 3f * t * Mathf.Pow(1f - t, 2f);
		float num3 = 3f * (t * t) * (1f - t);
		float num4 = t * t * t;
		return (p0 * num + p1 * num2 + p2 * num3 + p3 * num4) / (num + num2 + num3 + num4);
	}

	public static void calculateMeshTangents(Mesh mesh, bool _calculateNormals)
	{
		if (_calculateNormals)
		{
			mesh.RecalculateNormals();
		}
		int[] triangles = mesh.triangles;
		Vector3[] vertices = mesh.vertices;
		Vector2[] uv = mesh.uv;
		Vector3[] normals = mesh.normals;
		int num = triangles.Length;
		int num2 = vertices.Length;
		Vector3[] array = new Vector3[num2];
		Vector3[] array2 = new Vector3[num2];
		Vector4[] array3 = new Vector4[num2];
		for (long num3 = 0L; num3 < num; num3 += 3)
		{
			long num4 = triangles[num3];
			long num5 = triangles[num3 + 1];
			long num6 = triangles[num3 + 2];
			Vector3 vector = vertices[num4];
			Vector3 vector2 = vertices[num5];
			Vector3 vector3 = vertices[num6];
			Vector2 vector4 = uv[num4];
			Vector2 vector5 = uv[num5];
			Vector2 vector6 = uv[num6];
			float num7 = vector2.x - vector.x;
			float num8 = vector3.x - vector.x;
			float num9 = vector2.y - vector.y;
			float num10 = vector3.y - vector.y;
			float num11 = vector2.z - vector.z;
			float num12 = vector3.z - vector.z;
			float num13 = vector5.x - vector4.x;
			float num14 = vector6.x - vector4.x;
			float num15 = vector5.y - vector4.y;
			float num16 = vector6.y - vector4.y;
			float num17 = 1f / (num13 * num16 - num14 * num15);
			Vector3 vector7 = new Vector3((num16 * num7 - num15 * num8) * num17, (num16 * num9 - num15 * num10) * num17, (num16 * num11 - num15 * num12) * num17);
			Vector3 vector8 = new Vector3((num13 * num8 - num14 * num7) * num17, (num13 * num10 - num14 * num9) * num17, (num13 * num12 - num14 * num11) * num17);
			array[num4] += vector7;
			array[num5] += vector7;
			array[num6] += vector7;
			array2[num4] += vector8;
			array2[num5] += vector8;
			array2[num6] += vector8;
		}
		for (int i = 0; i < num2; i++)
		{
			Vector3 normal = normals[i];
			Vector3 tangent = array[i];
			Vector3.OrthoNormalize(ref normal, ref tangent);
			array3[i].x = tangent.x;
			array3[i].y = tangent.y;
			array3[i].z = tangent.z;
			array3[i].w = ((!(Vector3.Dot(Vector3.Cross(normal, tangent), array2[i]) < 0f)) ? 1f : (-1f));
		}
		mesh.tangents = array3;
	}
}
