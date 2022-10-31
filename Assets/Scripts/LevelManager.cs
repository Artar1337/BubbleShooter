using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using System;
using System.Text;

/// <summary>
/// Контроллер содержимого уровня (статика)
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Singleton
    public static LevelManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        try
        {
            ParseAllLevels();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        DontDestroyOnLoad(gameObject);
        foreach(var e in levels)
        {
            Debug.LogWarning(e.Key + "\r\n" + e.Value);
        }
    }
    #endregion

    private const int RANDOMINDEX = -1;

    private int currentLevelIndex;

    [SerializeField] private TextAsset levelsXml;
    /// <summary>
    /// Словарь с уровнями, ключ - индекс уровня, значение - 
    /// Сам уровень (некоторое кол-во разделенных новой строкой цифр)
    /// </summary>
    private Dictionary<int, string> levels;
    
    /// <summary>
    /// Парсинг XML файлика с уровнями в словарь levels
    /// </summary>
    private void ParseAllLevels()
    {
        levels = new Dictionary<int, string>();
        XmlDocument document = new XmlDocument();
        document.Load(new StringReader(levelsXml.text));
        //получаем корневой элемент (обозначен в файле как root)
        XmlElement root = document.DocumentElement;
        if (root != null)
        {
            foreach (XmlElement mainNode in root)
            {
                //для каждого levels
                foreach (XmlElement node in mainNode)
                {
                    //получаем атрибут "index" в "level"
                    XmlNode attr = node.Attributes.GetNamedItem("index");
                    if (attr == null)
                    {
                        throw new Exception("Error with parsing level parameter 'index' in XML!");
                    }
                    int levelIndex = Convert.ToInt32(attr?.Value);
                    if (levels.ContainsKey(Convert.ToInt32(levelIndex)))
                    {
                        throw new Exception("Two identical names in XML!");
                    }
                    StringBuilder builder = new StringBuilder();
                    // теперь уже идём по line элементам внутри
                    foreach (XmlNode child in node.ChildNodes)
                    {
                        if (child.Name == "line")
                        {
                            builder.Append(child.InnerText);
                            builder.Append(Environment.NewLine);
                            continue;
                        }
                        throw new Exception("Unknown name in XML: " + child.Name);
                    }
                    levels.Add(levelIndex, builder.ToString().Substring(0, builder.Length - Environment.NewLine.Length));
                }
            }
            //Успешное завершение парсинга!
            return;
        }
        throw new Exception("Root element not found in XML!");
    }

    /// <summary>
    /// Возвращает строковое представление уровня по индексу
    /// </summary>
    /// <param name="index">Индекс уровня</param>
    /// <returns>Строковое представление уровня</returns>
    public string GetLevelByIndex(int index)
    {
        if (levels.ContainsKey(index))
        {
            return levels[index];
        }
        return null;
    }

    /// <summary>
    /// Возвращает строковое представление уровня по текущему индексу уровня (для рестарта)
    /// </summary>
    /// <returns>Строковое представление уровня</returns>
    public string GetCurrentLevel()
    {
        return levels[currentLevelIndex];
    }

    /// <summary>
    /// Устанавливает текущий уровень для загрузки
    /// </summary>
    /// <param name="index">Индекс уровня</param>
    public void SetCurrentLevel(int index = RANDOMINDEX)
    {
        if (levels.ContainsKey(index))
        {
            currentLevelIndex = index;
            return;
        }
        currentLevelIndex = RANDOMINDEX;
    }
}
