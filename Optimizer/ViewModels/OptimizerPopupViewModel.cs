using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DialogHostAvalonia;
using System.Threading;
using System.Threading.Tasks;
using SE2.Data;
using SE2.Domain;
using SE2.Models;
using SE2.Domain;

namespace SE2.ViewModels;

public partial class OptimizerPopupViewModel(DialogHost dialogHost) : ViewModelBase
{
    [ObservableProperty]
    private double _progress = 0.0f;

    [ObservableProperty]
    private string _error = "";

    [ObservableProperty]
    private bool _hasError = false;

    [ObservableProperty]
    private bool _hasNoError = true;

    private CancellationTokenSource cts = new();

    public async Task Run()
    {
        var progress = new Progress<double>(value => Progress = value * 100);
        try
        {
            await DM.RunOptimization(cts.Token, progress);
            Close();
        }
        catch (OperationCanceledException)
        {
            Close();
        }
        catch (Exception e)
        {
            Error = e.Message;
            HasNoError = false;
            HasError = true;
        }
    }

    [RelayCommand]
    public void OnClose()
    {
        Close();
    }

    [RelayCommand]
    public void OnCancel()
    {
        cts.Cancel();
    }

    void Close()
    {
        DialogSession dialogSession = dialogHost.CurrentSession!;
        dialogSession.Close();
    }
}
