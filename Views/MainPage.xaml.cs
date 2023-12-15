using PM2_ExamenFinal.Models;
using PM2_ExamenFinal.ViewModels;
using System.Diagnostics;

namespace PM2_ExamenFinal.Views
{
    public partial class MainPage : ContentPage
    {
        private ViewModelNotas _viewModel;

        public MainPage()
        {
            InitializeComponent();
            cargarDatos();
        }

        private async void AddNote(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AddNotePage(null));
            }catch(Exception ex)
            {
                await DisplayAlert("Error","Error: "+ex.Message,"Cerrar");
            }
        }


        private async void cargarDatos()
        {
            await Task.Run(() => getDatos());
        }

        private async Task getDatos()
        {
            _viewModel = new ViewModelNotas("", true);
            BindingContext =_viewModel;
        }


        private async void Frame_Tapped(object sender, EventArgs e)
        {
            if (sender is Frame frame)
            {
                if (frame.BindingContext is Nota selectedItem)
                {
                    list.SelectedItem = selectedItem;
                }
            }
        }

        private async void list_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Nota selectedNota)
            {
                if (selectedNota != null)
                {
                    string? resp = await DisplayActionSheet("Que desea hacer", "Cancelar", null, "Modificar nota", "Eliminar nota");

                    if (resp == "Modificar nota")
                    {
                        Modificar(selectedNota);
                    }
                    else if (resp == "Utilizar c�mara")
                    {
                        Eliminar(selectedNota);
                    }
                    Modificar(selectedNota);
                }
            }
        }

        private async void Modificar(Nota notaSeleccionada)
        {
            await Navigation.PushAsync(new AddNotePage(notaSeleccionada));
        }

        private async void Eliminar(Nota notaSeleccionada)
        {
            await _viewModel.DeleteData(notaSeleccionada.Id_nota);
        }
    }
}