using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ClothesTagger.Models;

namespace ClothesTagger.ViewModels;

// Represents a selectable tag with change notification for when UI is changed
public class TagOption : INotifyPropertyChanged
{
    private bool _isSelected;

    // Display text of the tag
    public string Name { get; }

    // Whether the tag is currently selected in the UI
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value) return; 
            _isSelected = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
        }
    }

    public TagOption(string name) => Name = name;

    public event PropertyChangedEventHandler? PropertyChanged;
}

// Main screen view model: manages picking an image, selecting tags, and saving clothing items
public class MainViewModel : ViewModelBase
{
    private string? _selectedImagePath;

    // The items the user has saved (image + tags). Lists it in the UI
    public ObservableCollection<ClothingItem> Clothes { get; } = new();

    // All tags the user can toggle
    public ObservableCollection<TagOption> AvailableTags { get; } = new()
    {
        // Style/occasion
        new("Casual"), new("Formal"), new("Athletic"),
        // Seasonality
        new("Winter"), new("Summer"),
        // Sleeve type
        new("Long Sleeve"), new("Short Sleeve"),
        // Materials
        new("Denim"), new("Cotton"), new("Wool"),
        // Colors
        new("Black"), new("White"), new("Blue"), new("Red"), new("Green")
    };

    // Full file path of the picked image. If image is invalid, it won't save 
    public string? SelectedImagePath
    {
        get => _selectedImagePath;
        set => SetProperty(ref _selectedImagePath, value); 
    }

    // Saving and Picking images
    public ICommand PickImageCommand { get; }
    public ICommand SaveItemCommand { get; }

    // Lists selected tags
    public string PendingTagsDisplay =>
        string.Join(", ", AvailableTags.Where(t => t.IsSelected).Select(t => t.Name));

    public MainViewModel()
    {
        // Updates selecting/deselecting tags 
        foreach (var t in AvailableTags)
        {
            t.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(TagOption.IsSelected))
                {
                    OnPropertyChanged(nameof(PendingTagsDisplay));
                }
            };
        }

        // Asks the user to choose an image
        PickImageCommand = new Command(async () => await PickImageAsync());

        // Saves only when a valid image is entered
        SaveItemCommand = new Command(
            SaveItem,
            () => !string.IsNullOrWhiteSpace(SelectedImagePath)
        );
    }

    // Opens system files so user can choose image. Resets tags when new image is uploaded
    private async Task PickImageAsync()
    {
        var result = await FilePicker.Default.PickAsync(new PickOptions
        {
            PickerTitle = "Select clothing image",
            FileTypes = FilePickerFileType.Images
        });

        if (result != null)
        {
            SelectedImagePath = result.FullPath; 
            ResetTagSelections();              
            RaiseCanExec();                     
        }
    }

    private void ResetTagSelections()
    {
        foreach (var t in AvailableTags)
        {
            t.IsSelected = false;
        }
    
    }

    // Creates and stores a ClothingItem from current selections, then resets
    private void SaveItem()
    {
        var selectedTags = AvailableTags
            .Where(t => t.IsSelected)
            .Select(t => t.Name)
            .ToList();

        Clothes.Add(new ClothingItem
        {
            ImagePath = SelectedImagePath!, 
            Tags = selectedTags
        });

        // Resets UI for the next item
        SelectedImagePath = null;
        ResetTagSelections();

        // Save becomes disabled again because there is no selected image
        RaiseCanExec();
    }

    // Updates save button after image clears
    private void RaiseCanExec()
    {
        ((Command)SaveItemCommand).ChangeCanExecute();
    }
}