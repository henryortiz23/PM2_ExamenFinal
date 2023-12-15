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
                await Navigation.PushAsync(new AddNotePage());
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
                    await DisplayAlert("Informacion",selectedNota.Descripcion,"Cerrar");
                    //bool resp;
                    /*if (int.Parse(selectedCita.Calificacion) > 0)
                    {
                        resp = await DisplayAlert("Confirmar", "�Desea volver a calificar esta cita?", "S�", "No");
                    }
                    else
                    {
                        resp = await DisplayAlert("Confirmar", "�Desea calificar esta cita?", "S�", "No");
                    }

                    if (resp)
                    {
                        Calificar(selectedCita);
                    }
                    */
                }
            }
        }
    }
}