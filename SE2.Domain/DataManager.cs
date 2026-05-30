using System;
using System.Collections.Generic;
using System.Dynamic;
using SE2.Data;

namespace SE2.Domain;

/// <summary>
/// This is the DataManage. 
/// Here is the place where data get selected and transfered to the places where its needed.
/// 
/// Add Methods if its needed to get specific stuff from AM RDM SDM or more.
/// </summary>

public static class DM
{
    public static AM AM { get; } = new();
    public static RDM RDM { get; } = new();
    public static SDM SDM { get; } = new();

    private static Dictionary<string, IPeriod> scenarioPeriods = new();
    private static IPeriod currentPeriod = new Winter();
    private static readonly List<Asset> selectedAssets = [];
    private static string scenarioName = "1";

    private static readonly Optimizer optimizer = new();

    private static bool runEnabled = true;
    static Exception? _threadException;

    public static void Init()
    {
        SDM.Load(currentPeriod);
        AM.Load();
        AM.LoadScenario(scenarioName);
        Load();
    }

    public static void Load()
    {
        selectedAssets.Clear();
        for (int i = 0; i < AM.ScenarioData.AvailableUnits.Count; i++)
        {
            selectedAssets.Add(AM.GetAssetByName(AM.ScenarioData.AvailableUnits[i]) ??
                throw new Exception("Selected Assets don't exist any more"));
        }
    }

    public static async Task RunOptimization(CancellationToken ct, IProgress<double> progress)
    {
        if (!runEnabled)
        {
            return;
        }

        runEnabled = false;
        SemaphoreSlim signal = new SemaphoreSlim(0, 1);

        _threadException = null;
        Thread thread = new Thread(() => Thread_RunOptimizer(signal, ct, progress));
        thread.Start();
        await signal.WaitAsync();
        runEnabled = true;

        if (_threadException != null)
        {
            throw _threadException;
        }
    }

    private static void Thread_RunOptimizer(SemaphoreSlim signal, CancellationToken ct, IProgress<double> progress)
    {
        try
        {
            StartOptimizer(ct, progress);
        }
        catch (Exception e)
        {
            _threadException = e;
        }
        finally
        {
            signal.Release();
        }
    }

    public static void StartOptimizer(CancellationToken ct, IProgress<double> progress)
    {
        Load();

        optimizer.Sources = SDM.Sources;
        optimizer.Assets = selectedAssets;
        optimizer.MaintainableAssets = AM.GetMaintainableAssets();
        optimizer.OptimizerInit();

        RDM.SetCurrentScenarioResultingData(optimizer.CalculateSchedule(ct, progress));
    }

    public static void SetScenario(int index)
    {
        // temporal for switching from scenario 1 and 2.
        scenarioName = "" + (index + 1);
        if (scenarioPeriods.ContainsKey(scenarioName))
        {
            UpdatePeriod(scenarioPeriods[scenarioName]);
        }
        else
        {
            UpdatePeriod(new Winter());
        }
        AM.LoadScenario(scenarioName);
        Console.WriteLine("SetScenario");
        UpdateRDMCurrentScenario();
        Load();
    }

    public static void UpdatePeriod(IPeriod period)
    {
        Console.WriteLine("UpdatePeriod");
        currentPeriod = period;
        scenarioPeriods[scenarioName] = period;
        SDM.Load(currentPeriod);
        UpdateRDMCurrentScenario();
    }

    private static void UpdateRDMCurrentScenario()
    {
        RDM.SetCurrentScenario($"{scenarioName}_{currentPeriod.Period()}");
    }

    public static void Export()
    {
        RDM.Save(currentPeriod);
    }
}