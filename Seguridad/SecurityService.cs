using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguridad
{
        public class SecurityService
        {
            public string CreateSessionToken(string userId)
            {
                // Crear un token simple basado en el nombre de usuario y la fecha actual
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(userId + ":" + DateTime.Now.Ticks));
            }

            public bool ValidateSessionToken(string token)
            {
                try
                {
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                    var parts = decoded.Split(':');
                    if (parts.Length == 2)
                    {
                        // Validar token aquí, por ejemplo, comprobando la expiración
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
                return false;
            }
        }
}

