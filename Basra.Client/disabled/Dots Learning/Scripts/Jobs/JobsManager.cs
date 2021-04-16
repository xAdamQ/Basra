using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;

public class JobsManager : MonoBehaviour
{
    [SerializeField] bool UseJobs;

    public static int ConcurretnOperations = 10;
    public static int Iterations = 99999;

    private void Update()
    {
        if (UseJobs)
        {
            var nativeArr = new NativeArray<JobHandle>(ConcurretnOperations, Allocator.Temp);
            for (var i = 0; i < ConcurretnOperations; i++)
            {
                var jHandle = ToughTaskJob();//to make multithreaded jobs, start all jobs like this at once and complete all of them at once (rather than make then complete cycle) 
                nativeArr[i] = jHandle;
            }
            JobHandle.CompleteAll(nativeArr);//"excutes the job on main thread"(code monkey), "insure the job is done"(function summary)
            nativeArr.Dispose();//this will work after all jobs are done, because despite of being asyn internally, in my scope it's sync
        }
        else
        {
            for (var i = 0; i < ConcurretnOperations; i++)
            {
                ToughTask();
            }
        }
    }

    public static void ToughTask()
    {
        for (var i = 0; i < Iterations; i++)
        {
            Unity.Mathematics.math.sqrt(55);
        }
    }

    private JobHandle ToughTaskJob()
    {
        var job = new ToughJob();
        return job.Schedule();
    }

}

[BurstCompile]
public struct ToughJob : IJob
//jobs doen't work on the main thread and we can't use UnityEngine code on it
//when you want to
{
    // [ReadOnly]//works on job struct props
    public void Execute()
    {
        for (var i = 0; i < 99999; i++)
        {
            Unity.Mathematics.math.sqrt(55);
        }
    }
}//jobs are taking twice the time in some durations, and give inconsistent framerate.. why? 
