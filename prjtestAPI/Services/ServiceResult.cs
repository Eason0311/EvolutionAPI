namespace prjEvolutionAPI.Services
{
    public class ServiceResult
    {
        /// <summary>此 Service 是否執行成功</summary>
        public bool IsSuccess { get; private set; }

        /// <summary>失敗時的錯誤訊息，成功時為 null</summary>
        public string? ErrorMessage { get; private set; }

        /// <summary>私有建構子，透過靜態方法建立</summary>
        private ServiceResult(bool isSuccess, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        /// <summary>建立一個代表「成功」的結果</summary>
        public static ServiceResult Success()
        {
            return new ServiceResult(isSuccess: true);
        }

        /// <summary>建立一個代表「失敗」的結果，並附上錯誤訊息</summary>
        public static ServiceResult Fail(string errorMessage)
        {
            return new ServiceResult(isSuccess: false, errorMessage: errorMessage);
        }
    }
}
