namespace PM2_ExamenFinal.Controllers
{
    public class Configuracion
    {
        private string Url;
        
        public Configuracion()
        {
            Url = "https://nailbar-e4252-default-rtdb.firebaseio.com/Examen/";
        }
        public string GetUrlMain()
        {
            return Url;
        }

    }

}
