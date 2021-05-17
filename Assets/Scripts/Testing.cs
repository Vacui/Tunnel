using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class Testing : MonoBehaviour {

    public bool startTest = false;
    private void Update() {
        if (startTest) {
            NativeList<JobHandle> jobHandleList = new NativeList<JobHandle>(Allocator.Temp);
            for (int i = 0; i < 5; i++) {
                ReallyThoughJob job = new ReallyThoughJob();
                jobHandleList.Add(job.Schedule());
            }
            JobHandle.CompleteAll(jobHandleList);
            startTest = false;
        }
    }
}

public struct ReallyThoughJob : IJob {

    public void Execute() {
        float value = 0f;
        for(int i = 0; i < 50000; i++) {
            value = math.exp10(math.sqrt(value));
        }
    }
}