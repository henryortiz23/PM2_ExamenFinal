using Plugin.Maui.Audio;
using Plugin.Media;
using Plugin.Media.Abstractions;
using PM2_ExamenFinal.Models;
using PM2_ExamenFinal.ViewModels;
using System.Diagnostics;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace PM2_ExamenFinal.Views;

public partial class AddNotePage : ContentPage
{
    private Nota notaActual;
    private Plugin.Media.Abstractions.MediaFile image = null;
    private bool updating;

    IAudioManager audioManager;
    readonly IDispatcher dispatcher;
    IAudioRecorder audioRecorder;
    AsyncAudioPlayer audioPlayer;
    IAudioSource audioSource = null;
    bool isPlaying;
    bool isRecorder;
    public AddNotePage()
    {
        InitializeComponent();
    }

    private void OnCerrarClicked(object sender, EventArgs e)
    {
        //
    }

    private async void CrearActualizarClicked(object sender, EventArgs e)
    {
        /*
        if (!entNombre.Text.Trim().Equals(usuarioActual.Nombre) || !entTelefono.Text.Trim().Equals(usuarioActual.Telefono) || image != null && !updating)
        {
            contActualizar.WidthRequest = 300;
            updating = true;
            entTelefono.IsEnabled = false;
            entNombre.IsEnabled = false;

            await Task.Run(() => Updating());
        }
        */
        contActualizar.WidthRequest = 300;
        updating = true;
        await Task.Run(() => CreatedUpdating());
        contActualizar.WidthRequest = 0;
        updating = false;
        await Navigation.PopAsync();
    }

    private async Task CreatedUpdating()
    {
        ViewModelNotas viewModel = new ViewModelNotas("", false);


        try
        {
            string nFecha = datFecha.Date.ToShortDateString();
            Nota newNota = new Nota
            {
                Descripcion = entDescripcion.Text,
                Photo_Record = ImageToBase64(),
                Audio_Record = AudioToBase64(),
                Fecha = nFecha
            };

            if (notaActual == null)
            {
                await viewModel.InsertData(newNota);
            }
            else
            {
                newNota.Id_nota = notaActual.Id_nota;
                await viewModel.UpdateData(newNota);

            }

        }
        catch (Exception ex)
        {
            
        }

    }


    public string ImageToBase64()
    {
        if (image != null)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Stream stream = image.GetStream();
                stream.CopyTo(memory);
                byte[] data = memory.ToArray();
                string base64 = Convert.ToBase64String(data);

                return base64;
            }
        }

        return null;
    }


    public string AudioToBase64()
    {
        if (audioSource != null)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                Stream stream = audioSource.GetAudioStream();
                stream.CopyTo(memory);
                byte[] data = memory.ToArray();
                string base64 = Convert.ToBase64String(data);

                return base64;
            }
        }

        return null;
    }

    private async void obtenerFoto1()
    {
        await Task.Run(() => obtenerFoto());
    }


    public async void hacerFoto(object sender, EventArgs e)
    {
        image = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
        {
            PhotoSize=PhotoSize.Small,
            Directory = "MIALBUM",
            Name = "Foto.jpg",
            SaveToAlbum = true
        });

        if (image != null)
        {

            foto.Source = ImageSource.FromStream(() =>
            {
                return image.GetStream();
            });

        }
    }

    
    private async Task obtenerFoto()
    {
        string base64String = notaActual.Photo_Record;
        if (base64String != null && !string.IsNullOrEmpty(base64String))
        {
            byte[] bytes = System.Convert.FromBase64String(base64String);
            var stream = new MemoryStream(bytes);

            Device.BeginInvokeOnMainThread(() =>
            {
                foto.Source = ImageSource.FromStream(() => stream);
            });
        }
    }



    private async void StartAudio(object sender, EventArgs e)
    {
        if (await ComprobarPermisos<Microphone>())
        {
            if (audioManager == null)
            {
                audioManager = Plugin.Maui.Audio.AudioManager.Current;
            }
            if (!isRecorder)
            {
                isRecorder = true;
                audioRecorder = audioManager.CreateRecorder();

                await audioRecorder.StartAsync();
                btnStart.Text = "Detener grabación";
            }
            else
            {
                isRecorder = false;
                audioSource = await audioRecorder.StopAsync();
                btnStart.Text = "Regrabar audio";
                btnPlay.IsEnabled = true;
                btnPlay.ImageSource = "play.svg";
            }
        }
    }


    private async void Play(object sender, EventArgs e)
    {
        if (audioSource != null)
        {
            audioPlayer = this.audioManager.CreateAsyncPlayer(((FileAudioSource)audioSource).GetAudioStream());

            btnPlay.ImageSource = "stop.svg";
            isPlaying = true;
            await audioPlayer.PlayAsync(CancellationToken.None);
            isPlaying = false;
            btnPlay.ImageSource = "play.svg";
        }
    }

    public static async Task<bool> ComprobarPermisos<TPermission>() where TPermission : BasePermission, new()
    {
        PermissionStatus status = await Permissions.CheckStatusAsync<TPermission>();

        if (status == PermissionStatus.Granted)
        {
            return true;
        }

        if (Permissions.ShouldShowRationale<TPermission>())
        {

        }

        status = await Permissions.RequestAsync<TPermission>();

        return status == PermissionStatus.Granted;
    }


}