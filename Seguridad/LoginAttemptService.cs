using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seguridad
{
    public class LoginAttemptService
    {
        private static int _failedAttempts = 0;
        private static DateTime _lockoutEndTime;

        public static bool IsLockedOut()
        {
            if (_failedAttempts >= 5 && DateTime.Now < _lockoutEndTime)
            {
                return true;
            }
            return false;
        }

        public static void RecordFailedAttempt()
        {
            _failedAttempts++;
            if (_failedAttempts >= 5)
            {
                _lockoutEndTime = DateTime.Now.AddMinutes(15); // Bloqueo por 15 minutos
            }
        }

        public static void ResetFailedAttempts()
        {
            _failedAttempts = 0;
        }
    }
}