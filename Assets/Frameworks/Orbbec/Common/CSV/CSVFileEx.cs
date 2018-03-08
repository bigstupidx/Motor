using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public enum CSVType
{
	INT		= 0,
	BOOL	= 1,
	FLOAT	= 2,
	STRING	= 3,
	VECTOR3 = 4,
	VECTOR2 = 5,
}

/* 文件结构如下
 * ColName,  文件中第一行 每一列的数据名称
 * Descript, 文件中第二行 每一列数据的描述 仅为增加可读性
 * Type,	 文件中第三行 每一列数据的类型 可用于自动生成配置文件 
 * DATE...,
 * DATE...,
 * DATE...,
 */

public class CSVFileEx
{
	CSVFileEx()
	{

	}

	Dictionary<string, int> NameToCol = new Dictionary<string, int>();

	string m_Path = "";

	// CSV 完整内存结构 注意前三行为 特殊行 从第四行才是数据开始
	public string[][] EditorElement
	{
		set
		{
			Elem = value;
		}
		get
		{
			return Elem;
		}
	}

	CSVType[] TypeToCol;

	string[][] Elem = null;

	public static CSVFileEx CreateCSVFile(string strPath)
	{
		UnityEngine.Object obj = Resources.Load(strPath);
		TextAsset textAsset = obj as TextAsset;
		if (textAsset == null)
			return null;

		CSVFileEx newCSVFile = CreateCSVFileFromMemory(textAsset.text);

		newCSVFile.m_Path = strPath;

		return newCSVFile;
	}

	public static CSVFileEx CreateCSVFileByStream(string strPath)
	{
// 		FileStream file = null;
// 		try
// 		{
// 			file = new FileStream(strPath, FileMode.Open);
// 		}
// 		catch (Exception )
// 		{
// 			return null;
// 		}
// 
// 		long FileSize = file.Length;
// 		if (FileSize == 0)
// 		{
// 			return null;
// 		}
// 
// 		byte[] byData = new byte[FileSize];
// 
// 		//file.Seek(0, SeekOrigin.Begin);
// 		file.Read(byData, 0, (int)FileSize);
// 
// 		file.Close();

		string textAsset = "";
		
#if !UNITY_WEBPLAYER
		try
		{
			textAsset = File.ReadAllText(strPath, System.Text.Encoding.UTF8);
		}
		catch(Exception e)
		{
			Log.Print(Log.Level.Error, "Read File Path:" + strPath + " Error, Exception:" + e.Message + " Stack:" + e.StackTrace);
			return null;
		}
#else
		Log.Print( Log.Level.Error, "This function is not support on webplayer");
#endif

		CSVFileEx newCSVFile = CreateCSVFileFromMemory(textAsset);

		newCSVFile.m_Path = strPath;

		return newCSVFile;
	}

	public static CSVFileEx CreateCSVFileFromMemory(string textData)
	{
		string[] Split = { "\r\n" };
		string[] lineArray = textData.Split(Split, StringSplitOptions.RemoveEmptyEntries);

		if (lineArray.Length <= 1)
			return null;

		CSVFileEx newCSVFile = new CSVFileEx();

		newCSVFile.m_Path = "";

		newCSVFile.Elem = new string[lineArray.Length][];

		for (int i = 0; i < lineArray.Length; i++)
		{
			newCSVFile.Elem[i] = lineArray[i].Split(new char[] { ',', '\t' });
		}

		for (int i = 0; i < newCSVFile.Elem[0].Length; ++i)
		{
			newCSVFile.NameToCol.Add(newCSVFile.Elem[0][i], i);
		}

		newCSVFile.TypeToCol = new CSVType[newCSVFile.Elem[2].Length];
		for (int i = 0; i < newCSVFile.Elem[2].Length; ++i)
		{
			newCSVFile.TypeToCol[i] = (CSVType)Enum.Parse(typeof(CSVType), newCSVFile.Elem[2][i].ToUpper());
		}

		return newCSVFile;
	}

	public static CSVFileEx CreateCSVFileFromMemory(byte[] byteData)
	{
		string textData = System.Text.Encoding.UTF8.GetString(byteData);
		return CreateCSVFileFromMemory(textData);
	}

#if UNITY_EDITOR
	public void Save(string strPath)
	{
		string FileData = "";

		for (int r = 0; r < 3; ++r )
		{
			for (int col = 0; col < ColNum(); ++col)
			{
				FileData += Elem[r][col];
				FileData += col == ColNum() - 1 ? "\r\n" : ",";
			}
		}
			
		for (int row = 0; row < RowNum(); ++row )
		{
			for (int col = 0; col < ColNum(); ++col)
			{
				FileData += GetData(row, col);
				FileData += col ==  ColNum() - 1? "\r\n": ",";
			}
		}

#if !UNITY_WEBPLAYER
		File.WriteAllText(strPath, FileData);
#else
		Log.Print( Log.Level.Error, "This function is not support on webplayer");
#endif

	}
#endif

	public string GetPath()
	{
		return m_Path;
	}

	public CSVType GetColType(int Col)
	{
		return TypeToCol[Col];
	}

	public string GetColName(int Col)
	{
		return Elem[0][Col];
	}

	public int RowNum()
	{
		return Elem.Length - 3;
	}

	public int ColNum()
	{
		return NameToCol.Count;
	}


	public int GetColNumByName(string strColName)
	{
		int Col;
		if (!NameToCol.TryGetValue(strColName, out Col))
		{
			return -1;
		}

		return Col;
	}

	public int GetIntData(int nRow, string strColName)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return 0;

		int nCol = 0;
		if (!NameToCol.TryGetValue(strColName, out nCol))
		{
			return 0;
		}

		if (nCol >= Elem[RealRow].Length)
			return 0;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return 0;

		return int.Parse(Elem[RealRow][nCol]);
	}

	public bool GetBoolData(int nRow, string strColName)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return false;

		int nCol = 0;
		if (!NameToCol.TryGetValue(strColName, out nCol))
		{
			return false;
		}

		if (nCol >= Elem[RealRow].Length)
			return false;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return false;

		return int.Parse(Elem[RealRow][nCol]) != 0;
	}

	public float GetFloatData(int nRow, string strColName)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return 0.0f;

		int nCol = 0;
		if (!NameToCol.TryGetValue(strColName, out nCol))
		{
			return 0.0f;
		}

		if (nCol >= Elem[RealRow].Length)
			return 0.0f;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return 0.0f;

		return float.Parse(Elem[RealRow][nCol]);
	}

	public Vector3 GetVector3Data(int nRow, string strColName)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return Vector3.zero;

		int nCol = 0;
		if (!NameToCol.TryGetValue(strColName, out nCol))
		{
			return Vector3.zero;
		}

		if (nCol >= Elem[RealRow].Length)
			return Vector3.zero;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return Vector3.zero;

		string[] Split = { " " };
		string[] data = Elem[RealRow][nCol].Split(Split, 3, StringSplitOptions.RemoveEmptyEntries);

		Vector3 rt = Vector3.zero;
		if (data.Length != 3)
			return Vector3.zero;
		rt.x = float.Parse(data[0]);
		rt.y = float.Parse(data[1]);
		rt.z = float.Parse(data[2]);

		return rt;
	}

	public Vector2 GetVector2Data(int nRow, string strColName)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return Vector2.zero;

		int nCol = 0;
		if (!NameToCol.TryGetValue(strColName, out nCol))
		{
			return Vector2.zero;
		}

		if (nCol >= Elem[RealRow].Length)
			return Vector2.zero;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return Vector2.zero;

		string[] Split = { " " };
		string[] data = Elem[RealRow][nCol].Split(Split, 2, StringSplitOptions.RemoveEmptyEntries);

		Vector2 rt = Vector2.zero;
		if (data.Length != 2)
			return Vector2.zero;
		rt.x = float.Parse(data[0]);
		rt.y = float.Parse(data[1]);

		return rt;
	}

	public string GetData(int nRow, string strColName)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return "";

		int nCol = 0;
		if (!NameToCol.TryGetValue(strColName, out nCol))
		{
			return "";
		}

		if (nCol >= Elem[RealRow].Length)
			return "";

		return Elem[RealRow][nCol];
	}

	public int GetIntData(int nRow, int nCol)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return 0;

		if (nCol >= Elem[RealRow].Length)
			return 0;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return 0;

		return int.Parse(Elem[RealRow][nCol]);
	}

	public bool GetBoolData(int nRow, int nCol)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return false;

		if (nCol >= Elem[RealRow].Length)
			return false;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return false;

		return int.Parse(Elem[RealRow][nCol]) != 0;
	}

	public float GetFloatData(int nRow, int nCol)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return 0.0f;

		if (nCol >= Elem[RealRow].Length)
			return 0.0f;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return 0.0f;

		return float.Parse(Elem[RealRow][nCol]);
	}

	public Vector3 GetVector3Data(int nRow, int nCol)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return Vector3.zero;

		if (nCol >= Elem[RealRow].Length)
			return Vector3.zero;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return Vector3.zero;

		string[] Split = { " " };
		string[] data = Elem[RealRow][nCol].Split(Split, 3, StringSplitOptions.RemoveEmptyEntries);

		Vector3 rt = Vector3.zero;
		if (data.Length != 3)
			return Vector3.zero;
		rt.x = float.Parse(data[0]);
		rt.y = float.Parse(data[1]);
		rt.z = float.Parse(data[2]);

		return rt;
	}

	public Vector2 GetVector2Data(int nRow, int nCol)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return Vector2.zero;

		if (nCol >= Elem[RealRow].Length)
			return Vector2.zero;

		if (string.IsNullOrEmpty(Elem[RealRow][nCol]))
			return Vector2.zero;

		string[] Split = { " " };
		string[] data = Elem[RealRow][nCol].Split(Split, 2, StringSplitOptions.RemoveEmptyEntries);

		Vector2 rt = Vector2.zero;
		if (data.Length != 2)
			return Vector2.zero;
		rt.x = float.Parse(data[0]);
		rt.y = float.Parse(data[1]);

		return rt;
	}

	public string GetData(int nRow, int nCol)
	{
		int RealRow = nRow + 3;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return "";

		if (nCol >= Elem[RealRow].Length)
			return "";

		return Elem[RealRow][nCol];
	}
}
