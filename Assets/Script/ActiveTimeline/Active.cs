using UnityEngine;
using UnityEngine.Playables;

public class Active : MonoBehaviour
{
    public int timelineIndexToPlay; // Chỉ mục của timeline cần kiểm tra và chạy
    private TimelineManager timelineManager; // Tham chiếu đến TimelineManager

    private void Start()
    {
        InitializeTimelineManager();
        TryPlayTimeline();
    }

    private void InitializeTimelineManager()
    {
        timelineManager = FindObjectOfType<TimelineManager>();

        if (timelineManager == null)
        {
            Debug.LogError("Không tìm thấy TimelineManager trong scene!");
        }
    }

    private void TryPlayTimeline()
    {
        if (timelineManager == null || timelineIndexToPlay < 0 || timelineIndexToPlay >= timelineManager.timelines.Count)
        {
            Debug.LogError("Thông tin TimelineManager hoặc chỉ mục không hợp lệ.");
            return;
        }

        // Kiểm tra xem Timeline đã chạy chưa
        if (timelineManager.HasTimelinePlayed(timelineManager.timelines[timelineIndexToPlay].name))
        {
            Debug.Log($"Timeline {timelineManager.timelines[timelineIndexToPlay].name} đã chạy trước đó, không chạy lại.");
            // Nếu đã chạy rồi, set PlayableDirector thành inactive
            timelineManager.directors[timelineIndexToPlay].gameObject.SetActive(false);
        }
        else
        {
            // Nếu chưa chạy, tiến hành chạy
            timelineManager.PlayTimeline(timelineIndexToPlay);
        }
    }

    // Hàm công khai để thiết lập và kích hoạt timeline từ script khác
    public void SetAndPlayTimeline(int index)
    {
        timelineIndexToPlay = index;
        TryPlayTimeline();
    }
}
