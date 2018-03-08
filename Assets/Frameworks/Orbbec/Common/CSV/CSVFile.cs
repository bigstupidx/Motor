using UnityEngine;
using System;
using System.Collections.Generic;

public class CSVFile
{
	CSVFile()
	{

	}

	Dictionary<string, int> NameToCol = new Dictionary<string, int>();

	string m_Path = "";

	string[][] Elem = null;

	public static CSVFile CreateCSVFile(string strPath)
	{
		UnityEngine.Object obj = Resources.Load(strPath);
		TextAsset textAsset = obj as TextAsset;
		if (textAsset == null)
			return null;

		string[] Split = {"\r\n"};
		string[] lineArray = textAsset.text.Split(Split,StringSplitOptions.RemoveEmptyEntries);
	//	UnityEngine.Object.DestroyImmediate( textAsset, true);

		if (lineArray.Length <= 1)
			return null;

		CSVFile newCSVFile = new CSVFile();

		newCSVFile.m_Path = strPath;

		newCSVFile.Elem = new string[lineArray.Length][];

		for (int i = 0; i < lineArray.Length; i++)
		{
			newCSVFile.Elem[i] = lineArray[i].Split(new char[]{',','\t'});
		}

		for (int i = 0; i < newCSVFile.Elem[0].Length; ++i )
		{
			newCSVFile.NameToCol.Add(newCSVFile.Elem[0][i], i);
		}

		return newCSVFile;
	}

	public string GetPath()
	{
		return m_Path;
	}

	public int RowNum()
	{
		return Elem.Length - 2;
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

	public string GetData(int nRow, string strColName)
	{
		int RealRow = nRow + 2;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return "";

		int nColNum = 0;
		if (!NameToCol.TryGetValue( strColName, out nColNum))
		{
			return "";
		}

		if (nColNum >= Elem[RealRow].Length)
			return "";

		return Elem[RealRow][nColNum];
	}

	public string GetData(int nRow, int nCol)
	{
		int RealRow = nRow + 2;
		if (Elem.Length <= 0 || RealRow >= Elem.Length)
			return "";

		if (nCol >= Elem[RealRow].Length)
			return "";

		return Elem[RealRow][nCol];
	}
}