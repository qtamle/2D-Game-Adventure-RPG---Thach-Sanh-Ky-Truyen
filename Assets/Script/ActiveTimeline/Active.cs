using UnityEngine;
using UnityEngine.Playables;

public class Active : MonoBehaviour
{
    public int timelineIndexToPlay; // Chỉ mục của timeline cần kiểm tra và chạy
    private TimelineManager timelineManager; // Tham chiếu đến TimelineManager

    private void Start()
    {
        timelineManager = FindObjectOfType<TimelineManager>();

        if (timelineManager == null)
        {
            Debug.LogError("Không tìm thấy TimelineManager trong scene!");
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
}
