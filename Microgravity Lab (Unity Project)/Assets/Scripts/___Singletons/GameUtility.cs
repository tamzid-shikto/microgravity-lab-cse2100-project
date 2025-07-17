using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class GameUtility
{
    static long COUNT = 0;
    public static string GenerateObjectSpawnID(string prefix)
    {
        COUNT++;
        return prefix + "_" + COUNT;
    }
    public static string DecodeBase64(string b64)
    {
        byte[] bytes = Convert.FromBase64String(b64);
        return Encoding.UTF8.GetString(bytes).ToString();
    }
    public static long GetMillisecondsFromUTC()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
    public static void ForceLayout(RectTransform rect)
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }
    public static string GetTimeinHH_MM()
    {
        return DateTime.Now.ToString("HH':'mm");
    }
    public static string ComputeSha256Hash(string input)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = sha256.ComputeHash(bytes);
            StringBuilder builder = new StringBuilder();

            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2")); // Convert to hexadecimal
            }

            return builder.ToString();
        }
    }

    public static Sprite ImageDataB64ToSprite(string b64Data)
    {
        byte[] imageBytes = Convert.FromBase64String(b64Data);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(imageBytes);
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    public static void DoAtNextFrame(Action callback)
    {
        if(AppHost.instance != null) AppHost.instance.StartCoroutine(IDoAtNextFrame(callback));
        else if(LabHost.instance != null) LabHost.instance.StartCoroutine(IDoAtNextFrame(callback));
        else if(MainMenuHost.instance != null) MainMenuHost.instance.StartCoroutine(IDoAtNextFrame(callback));
    }

    static IEnumerator IDoAtNextFrame(Action callback)
    {
        yield return null;
        callback?.Invoke();
    }

    public static void DoAfterMilliseconds(int mills, Action callback)
    {
        AppHost.instance.StartCoroutine(IDoAfterMilliseconds(mills, callback));
    }

    static IEnumerator IDoAfterMilliseconds(int mills, Action callback)
    {
        yield return new WaitForSeconds(mills / 1000f);
        callback?.Invoke(); 
    }

    public static void HTTP_GET(string uri, Action<string> callback, Action failCallback)
    {
        //Debug.Log(uri);
        AppHost.instance.StartCoroutine(IHTTP_GET(uri, callback, failCallback));
    }

    static IEnumerator IHTTP_GET(string uri, Action<string> callback, Action failCallback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Error: " + webRequest.error);
                failCallback?.Invoke();
            }
            else
            {
                string text = webRequest.downloadHandler.text;
                //Debug.Log("IHTTP_GET :: " + text);
                callback?.Invoke(text);
            }
        }
    }
}

public class IChangableData<T>
{
    T data;
    public Action<T> OnChange;

    public IChangableData() { }
    public IChangableData(T initData)
    {
        data = initData;
    }
    public void SetData(T newData)
    {
        data = newData;
        OnChange?.Invoke(data);
    }
    public void TriggerCallback()
    {
        OnChange?.Invoke(data);
    }
    public T GetData() { return data; }
}
public class IChangableList<T>
{
    List<T> dataList = new List<T>();
    public Action<T> OnAdd;
    public Action<T> OnRemove;
    public void AddToList(T newData)
    {
        dataList.Add(newData);
        OnAdd?.Invoke(newData);
    }
    public void RemoveFromList(T oldData)
    {
        dataList.Remove(oldData);
        OnRemove?.Invoke(oldData);
    }
    public void ClearList()
    {
        foreach (var item in dataList) OnRemove?.Invoke(item);
        dataList.Clear();
    }
    public List<T> GetList() { return dataList; }
}

public class FixedSizeList<T>
{
    private readonly int _capacity;
    private readonly Queue<T> _queue;
    private T last;
    public FixedSizeList(int capacity)
    {
        _capacity = capacity;
        _queue = new Queue<T>(capacity);
    }

    public void Add(T item)
    {
        if (_queue.Count >= _capacity)
        {
            _queue.Dequeue(); // Remove the oldest item
        }
        _queue.Enqueue(item); // Add the new item
        last = item;
    }

    public void RemoveNFromLast(int n)
    {
        for(int i = 0; i < n; i++)
        {
            _queue.Dequeue();
        }
    }

    public T GetLast()
    {
        return last;
    }

    public int Count => _queue.Count;

    public T[] ToArray() => _queue.ToArray();

    public void Clear() => _queue.Clear();
}