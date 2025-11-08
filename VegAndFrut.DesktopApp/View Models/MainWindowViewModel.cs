using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VegAndFrut.BLL;
using VegAndFrut.Models;

namespace VegAndFrut;

public class MainWindowViewModel : ViewModelBase
{
    private decimal _max;
    private decimal _min;
    private decimal _avg;
    private decimal _fromValue;
    private decimal _toValue;

    private readonly Service _service = new();

    public decimal Max
    {
        get => _max;
        set => SetField(ref _max, value);
    }
    public decimal Min
    {
        get => _min;
        set => SetField(ref _min, value);
    }
    public decimal Avg
    {
        get => _avg;
        set => SetField(ref _avg, value);
    }

    public decimal FromValue
    {
        get => _fromValue;
        set => SetField(ref _fromValue, value);
    }

    public decimal ToValue
    {
        get => _toValue;
        set => SetField(ref _toValue, value);
    }
    
    public ObservableCollection<string> ShowList { get; private set; } = [];
    public ObservableCollection<Product> Products { get; } = [];
    public ICommand CommandLoad { get; private set; }
    public ICommand ComandShowAllInfo { get; private set; }
    public ICommand ComandShowNameAndColor { get; private set; }
    public ICommand ComandShowColors { get; private set; }

    public MainWindowViewModel()
    {
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        CommandLoad = new LambdaCommand(
            execute: _ => LoadProducts(),
            canExecute: _ => true);

        ComandShowAllInfo = new LambdaCommand(
            execute: _ => ShowAllInfo(),
            canExecute: _ => true);

        ComandShowNameAndColor = new LambdaCommand(
            execute: _ => ShowNameAndColor(),
            canExecute: _ => true);

        ComandShowColors = new LambdaCommand(
            execute: _ => ShowColors(),
            canExecute: _ => true);
    }

    private void LoadProducts(object? parameter = null)
    {
        var products = _service.GetAll();

        Products.Clear();
        foreach (var product in products)
        {
            Products.Add(product);
        }

        ShowAllInfo();
        Max = _service.GetMax(products);
        Min = _service.GetMin(products);
        Avg = _service.GetAvg(products);
        FromValue = Min;
        ToValue = Max;
    }

    private void ShowAllInfo(object? parameter = null)
    {
        if (Products.Count > 0)
        {
            ShowList.Clear();
            foreach (var p in Products)
                ShowList.Add($"{p.Id}. {p.Name}, {p.Type}, {p.Color}, ({p.Calories} ккал.)");
        }
        else MessageBox.Show("No products found");
    }

    private void ShowNameAndColor(object? parameter = null)
    {
        if (Products.Count > 0)
        {
            ShowList.Clear();
            foreach (var p in Products)
                ShowList.Add($"{p.Name}, {p.Color}");
        }
        else MessageBox.Show("No products found");
    }

    private void ShowColors(object? parameter = null)
    {
        if (Products.Count > 0)
        {
            ShowList.Clear();
            var colors = Products.Select(p => p.Color).Distinct().ToArray();
            foreach (var c in colors) ShowList.Add(c);
        }
        else MessageBox.Show("No products found");
    }
}