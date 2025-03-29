namespace Api.Services
{
    public class OtpService
    {
        public string GenerateOTP()
        {
            var optCode = new Random().Next(100000, 999999).ToString();
            return optCode;
        }
    }

    public class SmsService(ILogger<SmsService> logger)
    {
        public async Task SendOtpAsync(string moblieNumber, string opt)
        {
            logger.LogDebug("sent OTP {} to {}",opt,moblieNumber);
            await Task.CompletedTask;
        }
    }
}
