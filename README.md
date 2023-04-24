# 特点

（1）提供和PlayerPrefs一样的接口

（2）可以轻松的将原本由PlayerPrefs存储的数据同步到云端

（2）兼容新老用户，将PlayerPrefs替换成OnlinePrefs，老用户数据不会丢失

# 注意
OnlinePrefs依赖Newtonsoft.Json。(附Nuget地址：https://www.nuget.org/packages/Newtonsoft.Json)

如果不想引入该库，可以修改`OnlinePrefs.cs`文件中`LoadAsync`方法和`Save`方法中关于序列化和反序列化的实现。


# 如何使用

使用OnlinePrefs前，需要注册`OnLoadRequest`事件和`OnSaveRequest`（用来定义如何将数据存储到云端，如何从云端获取数据），并且执行`LoadAsync`方法同步云端数据。

完成这几个步骤就可以使用`SetInt`、`SetString`、`SetFloat`、`GetInt`、`GetString`、`GetFloat`等方法。

以下是使用示例



```C#
private async void Awake()
{
    OnlinePrefs.OnLoadRequest += OnLoadRequest;
    OnlinePrefs.OnSaveRequest += OnSaveRequest;
    OnlinePrefs.OnValueChanged += OnValueChanged;
    // 初始化OnlinePrefs
    await OnlinePrefs.Initialize();

    OnlinePrefs.SetInt("XXX", 0);
    OnlinePrefs.SetString("XXX", "XXX");
    OnlinePrefs.SetFloat("XXX", 0);

    _ = OnlinePrefs.GetInt("XXX");
    _ = OnlinePrefs.GetString("XXX");
    _ = OnlinePrefs.GetFloat("XXX");
}

private Task<byte[]> OnLoadRequest()
{
    string userID = "XXX";
    var request = UnityWebRequest.Get($"http://localhost:9005/record/{userID}");
    var completionSource = new TaskCompletionSource<byte[]>();
    request.SendWebRequest().completed += _ =>
    {
        var data = request.result == UnityWebRequest.Result.Success ? request.downloadHandler.data : null;
        completionSource.SetResult(data);
        request.Dispose();
    };
    return completionSource.Task;
}

// 将数据保存到云端的示例实现
private void OnSaveRequest(byte[] data)
{
    string userID = "XXX";
    var request = UnityWebRequest.Post($"http://localhost:9005/record/{userID}", "POST");
    request.uploadHandler = new UploadHandlerRaw(data);
    request.SendWebRequest().completed += _ =>
    {
        request.Dispose();
    };
}

private void OnValueChanged()
{
    // 数据上传到云端
    OnlinePrefs.Save();
}
```
