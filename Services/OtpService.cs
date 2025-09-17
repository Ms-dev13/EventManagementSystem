using Microsoft.Extensions.Caching.Memory;

namespace EventManagementSystem.Services
{
    public class OtpService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _otpExpiry = TimeSpan.FromMinutes(10);
        public OtpService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }
            
       public string GenerateOtp(string email) 

        { 
            string otp = new Random().Next(100000, 999999).ToString();
            _cache.Set("otp"+email, otp, _otpExpiry);
            return otp;
        }
        
        public bool ValidateOtp(string email,string Enteredotp)
        {
            if(_cache.TryGetValue("otp"+email,out string cachedotp))
            {
                if(cachedotp == Enteredotp)
                {
                    _cache.Remove("otp"+email);
                    return true;
                }
            }
            return false;
        }


    }
}
