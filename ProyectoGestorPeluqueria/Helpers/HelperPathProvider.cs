namespace MvcNetCoreUtilidades.Helpers
{
    //Enumeracion con las carpetas que deseemos subir ficheros
    public enum Folders { Uploads, Images, Facturas,Productos    }
    public class HelperPathProvider
    {
        private IWebHostEnvironment hostEnvironment;
        private IHttpContextAccessor httpContextAccessor;
        
        public HelperPathProvider(IWebHostEnvironment hostEnvironment, 
            IHttpContextAccessor httpContextAccessor)
        {
            this.hostEnvironment = hostEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }
        public string MapPath(string fileName, Folders folder)
        {
            string carpeta = "";
            if (folder == Folders.Images)
            {
                carpeta = "images";
            }
            else if (folder == Folders.Uploads)
            {
                carpeta = "uploads";
            }
            else if (folder == Folders.Facturas)
            {
                carpeta = "facturas";
            }
            else if (folder == Folders.Productos)
            {
                //Esta si va a cambiar por que ese sistema de ficheros necesita web
                carpeta = "images/productos";
            }
            string rootPath = this.hostEnvironment.WebRootPath;
            string path = Path.Combine(rootPath, carpeta, fileName);
            return path;
        }
        
        public string MapUrlPath(string fileName, Folders folder)
        {
            string carpeta = "";
            var request = this.httpContextAccessor.HttpContext.Request;
            if (folder == Folders.Images)
            {
                carpeta = "images";
            }
            else if (folder == Folders.Uploads)
            {
                carpeta = "uploads";
            }
            else if (folder == Folders.Facturas)
            {
                carpeta = "facturas";
            }
            else if (folder == Folders.Productos)
            {
                carpeta = "images/productos";
            }
            
            string serverUrl = $"{request.Scheme}://{request.Host}";
            string urlPath = $"{serverUrl}/{carpeta}/{fileName}";
            return urlPath;
        }
    }
}

