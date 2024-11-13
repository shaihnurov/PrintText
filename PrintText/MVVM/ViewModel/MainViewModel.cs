using CommunityToolkit.Mvvm.ComponentModel;

namespace PrintText.MVVM.ViewModel;

public class MainViewModel : ObservableObject
{
    private object _currentView;
    public object CurrentView
    {
        get => _currentView;
        private set => SetProperty(ref _currentView, value);
    }
    
    public MainViewModel()
    {
        CurrentView = new PrintViewModel();
    }
}