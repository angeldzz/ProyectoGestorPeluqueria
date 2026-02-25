namespace MvcCoreCryptography.Helpers
{
    public class HelperTools
    {
        public static string GenerateSalt()
        {
            //IMPORTANTE, NUESTRO SALT DEBE SER EXACTO AL TAMAÑO
            //DEL CAMPO DE LA BASE DE DATOS
            Random random = new Random();
            string salt = "";
            for (int i = 0; i < 50; i++)
            {
                int num = random.Next(0, 255);
                char letra = Convert.ToChar(num);
                salt += letra;
            }
            return salt;
        }
        public static bool CompareArrays(byte[] a, byte[] b)
        {
            bool iguales = true;
            if (a.Length != b.Length)
            {
                
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i].Equals(b[i]) == false)
                    {
                        iguales = false;
                        break;
                    }
                }
            }
            return iguales;
        }
    }
}
