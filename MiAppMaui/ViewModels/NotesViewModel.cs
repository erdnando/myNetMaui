using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MiAppMaui.Models;
using MiAppMaui.Services;
using System.Collections.ObjectModel;

namespace MiAppMaui.ViewModels;

public partial class NotesViewModel : BaseViewModel
{
    private readonly DatabaseService _database;

    [ObservableProperty]
    private ObservableCollection<Note> notes = new();

    [ObservableProperty]
    private Note? selectedNote;

    [ObservableProperty]
    private string noteTitle = string.Empty;

    [ObservableProperty]
    private string noteContent = string.Empty;

    [ObservableProperty]
    private string noteCategory = "General";

    [ObservableProperty]
    private string noteColor = "#667eea";

    [ObservableProperty]
    private bool isEditing = false;

    [ObservableProperty]
    private bool showForm = false;

    [ObservableProperty]
    private string searchText = string.Empty;

    private int editingNoteId = 0;

    public NotesViewModel(DatabaseService database)
    {
        _database = database;
        Title = "Mis Notas";
        
        // Inicializar tabla y cargar notas
        Task.Run(async () =>
        {
            await _database.CreateTableAsync<Note>();
            await LoadNotes();
        });
    }

    [RelayCommand]
    private async Task LoadNotes()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var allNotes = await _database.GetAllAsync<Note>();
            
            // Aplicar filtro de búsqueda si existe
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                allNotes = allNotes.Where(n => 
                    n.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    n.Content.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            Notes.Clear();
            foreach (var note in allNotes.OrderByDescending(n => n.UpdatedAt))
            {
                Notes.Add(note);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al cargar notas: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ShowAddForm()
    {
        ClearForm();
        ShowForm = true;
        IsEditing = false;
    }

    [RelayCommand]
    private void ShowEditForm(Note note)
    {
        if (note == null) return;

        editingNoteId = note.Id;
        NoteTitle = note.Title;
        NoteContent = note.Content;
        NoteCategory = note.Category;
        NoteColor = note.Color;
        IsEditing = true;
        ShowForm = true;
    }

    [RelayCommand]
    private void CancelForm()
    {
        ClearForm();
        ShowForm = false;
    }

    [RelayCommand]
    private async Task SaveNote()
    {
        if (string.IsNullOrWhiteSpace(NoteTitle))
        {
            await Shell.Current.DisplayAlert("Validación", "El título es requerido", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            if (IsEditing)
            {
                // Actualizar nota existente
                var note = await _database.GetByIdAsync<Note>(editingNoteId);
                if (note != null)
                {
                    note.Title = NoteTitle;
                    note.Content = NoteContent;
                    note.Category = NoteCategory;
                    note.Color = NoteColor;
                    note.UpdatedAt = DateTime.Now;

                    await _database.SaveAsync(note);
                    await Shell.Current.DisplayAlert("Éxito", "Nota actualizada", "OK");
                }
            }
            else
            {
                // Crear nueva nota
                var newNote = new Note
                {
                    Title = NoteTitle,
                    Content = NoteContent,
                    Category = NoteCategory,
                    Color = NoteColor,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _database.SaveAsync(newNote);
                await Shell.Current.DisplayAlert("Éxito", "Nota creada", "OK");
            }

            ClearForm();
            ShowForm = false;
            await LoadNotes();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al guardar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteNote(Note note)
    {
        if (note == null) return;

        var confirm = await Shell.Current.DisplayAlert(
            "Confirmar", 
            $"¿Eliminar '{note.Title}'?", 
            "Eliminar", 
            "Cancelar");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            await _database.DeleteAsync(note);
            await LoadNotes();
            await Shell.Current.DisplayAlert("Éxito", "Nota eliminada", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al eliminar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ToggleFavorite(Note note)
    {
        if (note == null) return;

        try
        {
            note.IsFavorite = !note.IsFavorite;
            note.UpdatedAt = DateTime.Now;
            await _database.SaveAsync(note);
            await LoadNotes();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private void SelectColor(string color)
    {
        NoteColor = color;
    }

    partial void OnSearchTextChanged(string value)
    {
        // Recargar con filtro cuando cambia el texto de búsqueda
        _ = LoadNotes();
    }

    private void ClearForm()
    {
        NoteTitle = string.Empty;
        NoteContent = string.Empty;
        NoteCategory = "General";
        NoteColor = "#667eea";
        IsEditing = false;
        editingNoteId = 0;
    }
}
