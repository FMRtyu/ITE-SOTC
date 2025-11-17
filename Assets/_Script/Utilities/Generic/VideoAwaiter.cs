using System.Threading.Tasks;
using UnityEngine.Video;

public static class VideoAwaiter
{
    public static Task WaitForEnd(VideoPlayer vp)
    {
        var tcs = new TaskCompletionSource<bool>();

        void finished(VideoPlayer _)
        {
            vp.loopPointReached -= finished;
            tcs.TrySetResult(true);
        }

        vp.loopPointReached += finished;

        return tcs.Task;
    }
}
