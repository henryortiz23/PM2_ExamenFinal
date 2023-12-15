using Firebase.Database;
using Firebase.Database.Query;
using PM2_ExamenFinal.Models;
using System.Collections.ObjectModel;

namespace PM2_ExamenFinal.ViewModels
{
    public class ViewModelNotas
    {
        private FirebaseClient _firebase;
        private string childString;

        public ObservableCollection<Nota> DataItems { get; } = new ObservableCollection<Nota>();

        public ViewModelNotas(string child, bool cargar)
        {
            childString = child;
            _firebase = new FirebaseClient(new Controllers.Configuracion().GetUrlMain());
            
            if (cargar)
                ListenToChanges();
            
        }

        private void ListenToChanges()
        {
            _firebase
                .Child(childString)
                .AsObservable<Nota>()
                .Subscribe(args =>
                {
                    if (args.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                    {
                        var newItem = args.Object;
                        newItem.Id_nota = args.Key;
                        var existingItem = DataItems.FirstOrDefault(x => x.Id_nota == newItem.Id_nota);

                        if (existingItem != null)
                        {
                            int index = GetIndexId(newItem.Id_nota);
                            
                            DataItems.RemoveAt(index);
                            DataItems.Insert(index, newItem);
                        }
                        else
                        {
                            DataItems.Add(newItem);
                        }
                    }
                    else if (args.EventType == Firebase.Database.Streaming.FirebaseEventType.Delete)
                    {
                        var itemToRemove = DataItems.FirstOrDefault(x => x.Id_nota == args.Key);
                        if (itemToRemove != null)
                        {
                            DataItems.Remove(itemToRemove);
                        }
                    }
                });
        }

        public async Task InsertData(Nota newItem)
        {
            await _firebase
            .Child(childString)
            .PostAsync(newItem);
        }

        public async Task UpdateData(Nota updatedItem)
        {
            await _firebase
                .Child(childString)
                .Child(updatedItem.Id_nota)
                .PutAsync(updatedItem);
        }

        public async Task DeleteData(string itemId)
        {
            await _firebase
                .Child(childString)
                .Child(itemId)
                .DeleteAsync();
        }

        public int GetIndexId(string itemId)
        {
            for (int i = 0; i < DataItems.Count; i++)
            {
                if (DataItems[i].Id_nota == itemId)
                {
                    return i; // Devuelve el índice si se encuentra el elemento
                }
            }
            return -1; // Devuelve -1 si no se encuentra el elemento con ese Id
        }

        
    }
}
