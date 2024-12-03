using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    [System.Serializable]
    public class TimelineData
    {
        public string timelineName;
        public bool hasPlayed;
    }

    [System.Serializable]
    public class TimelineSaveData
    {
        public List<TimelineData> timelines = new List<TimelineData>();
    }

    public List<PlayableAsset> timelines;  // Mảng chứa các Timeline
    public List<PlayableDirector> directors; // Mảng chứa các PlayableDirector tương ứng với Timeline
    private TimelineSaveData saveData;
    private string saveFolderPath = "Assets/saveTimeline/";
    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(saveFolderPath, "timelineData.json");

        // Tạo folder lưu nếu chưa có
        if (!Directory.Exists(saveFolderPath))
        {
            Directory.CreateDirectory(saveFolderPath);
        }

        // Load trạng thái đã lưu
        LoadTimelineData();
    }

    /// <summary>
    /// Chạy một Timeline nếu nó chưa được chạy trước đó.
    /// </summary>
    public void PlayTimeline(int timelineIndex)
    {
        if (timelineIndex < 0 || timelineIndex >= timelines.Count || timelineIndex >= directors.Count)
        {
            Debug.LogError("Chỉ mục timeline hoặc PlayableDirector không hợp lệ.");
            return;
        }

        PlayableAsset timeline = timelines[timelineIndex];
        PlayableDirector director = directors[timelineIndex];
        string timelineName = timeline.name;

        // Kiểm tra nếu timeline đã chạy
        if (HasTimelinePlayed(timelineName))
        {
            Debug.Log($"Timeline {timelineName} đã chạy trước đó, không chạy lại.");
            // Nếu timeline đã chạy, tắt PlayableDirector
            director.gameObject.SetActive(false);
            return;
        }

        // Gán PlayableAsset và chạy
        director.playableAsset = timeline;
        director.Play();

        // Đăng ký sự kiện khi Timeline kết thúc
        director.stopped += (PlayableDirector d) =>
        {
            OnTimelineComplete(d); // Chỉ truyền PlayableDirector vào
        };
    }

    /// <summary>
    /// Hàm xử lý khi Timeline chạy xong.
    /// </summary>
    private void OnTimelineComplete(PlayableDirector director)
    {
        // Lấy tên Timeline từ PlayableAsset
        string timelineName = director.playableAsset.name;
        Debug.Log($"Timeline {timelineName} đã chạy xong.");

        // Cập nhật trạng thái
        var timelineData = saveData.timelines.Find(t => t.timelineName == timelineName);
        if (timelineData == null)
        {
            saveData.timelines.Add(new TimelineData { timelineName = timelineName, hasPlayed = true });
        }
        else
        {
            timelineData.hasPlayed = true;
        }

        SaveTimelineData();
    }

    /// <summary>
    /// Kiểm tra xem Timeline đã chạy chưa.
    /// </summary>
    public bool HasTimelinePlayed(string timelineName)
    {
        return saveData.timelines.Exists(t => t.timelineName == timelineName && t.hasPlayed);
    }

    /// <summary>
    /// Lưu trạng thái Timeline vào file JSON.
    /// </summary>
    private void SaveTimelineData()
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log("Lưu trạng thái Timeline vào: " + saveFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Lỗi khi lưu trạng thái Timeline: " + e.Message);
        }
    }

    /// <summary>
    /// Load trạng thái Timeline từ file JSON.
    /// </summary>
    private void LoadTimelineData()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<TimelineSaveData>(json);
            Debug.Log("Đã load trạng thái Timeline từ file.");
        }
        else
        {
            saveData = new TimelineSaveData();
            Debug.Log("Không tìm thấy file lưu, khởi tạo mới.");
        }
    }
}
