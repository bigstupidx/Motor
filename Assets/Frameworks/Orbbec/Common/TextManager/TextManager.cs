using System;
using System.Collections.Generic;

class TextManager
{
	public static TextManager Instance;

	public int 		CurLanguageIndex = 0;
	public string[]	LanguageName;
	Dictionary< int, string>[] m_TextConfigTable;

	public void SetCurLanguage(int Index)
	{
		if (LanguageName.Length <= Index)
			return;

		CurLanguageIndex = Index;
	}

	public static TextManager Create(string PublicControllerFilePath)
	{
		if (Instance != null)
			return Instance;

		CSVFile configFile = CSVFile.CreateCSVFile(PublicControllerFilePath);
		if (configFile == null)
		{
			Log.Print(Log.Level.Error, "CreateCSVFile Failed, can't open " + PublicControllerFilePath);
			return null;
		}

		Instance = new TextManager();
		Instance.Init(configFile);
		return Instance;
	}

	public string GetContext(int id)
	{
		string context;
		if (!m_TextConfigTable[CurLanguageIndex].TryGetValue(id, out context))
		{
			return "";
		}

		return context;
	}

	protected void Init(CSVFile configFile)
	{
		int rowNum = configFile.RowNum();
		if (rowNum == 0)
			return;

		int typeCol = configFile.GetColNumByName("type");
		int filePathCol = configFile.GetColNumByName("FileName");

		if (rowNum == -1 || filePathCol == -1)
		{
			Log.Print(Log.Level.Error, configFile.GetPath() + "is not a public controller file.");
			return;
		}

		LanguageName = new string[rowNum];
		m_TextConfigTable	= new Dictionary<int, string>[rowNum];

		for (int i = 0; i < rowNum; ++i)
		{
			LanguageName[i] = configFile.GetData(i, typeCol);

			CSVFile refFile = CSVFile.CreateCSVFile(configFile.GetData(i, filePathCol));
			if (refFile == null)
			{
				Log.Print(Log.Level.Error, "CreateCSVFile Failed, can't open " + refFile.GetPath());
				continue;
			}

			int idCol = refFile.GetColNumByName("id");
			int contextCol = refFile.GetColNumByName("context");

			int rowTotal = refFile.RowNum();

			m_TextConfigTable[i] =  new Dictionary< int, string>();
			for (int row = 0; row < rowTotal; ++row)
			{
				int id = int.Parse(refFile.GetData(row, idCol));
				if (m_TextConfigTable[i].ContainsKey(id))
				{
					Log.Print(Log.Level.Warning, refFile.GetPath() + "Has the same id :" + id);
					continue;
				}
				string context = refFile.GetData(row, contextCol);

				m_TextConfigTable[i].Add(id, context);
			}
		}
	}
}

